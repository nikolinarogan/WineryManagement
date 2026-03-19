using WineryAPI.DTOs;
using WineryAPI.Models;
using WineryAPI.Repositories;

namespace WineryAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IJwtService _jwtService;

        public AuthService(IAuthRepository authRepository, IJwtService jwtService)
        {
            _authRepository = authRepository;
            _jwtService = jwtService;
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto loginDto)
        {
            var zaposleni = await _authRepository.GetZaposleniByEmailAsync(loginDto.Email);

            if (zaposleni == null)
                return null;

            
            if (!BCrypt.Net.BCrypt.Verify(loginDto.Sifra, zaposleni.Sifra))
                return null;

            
            var token = _jwtService.GenerateToken(zaposleni);

            return new AuthResponseDto
            {
                Token = token,
                Email = zaposleni.Email,
                Ime = zaposleni.Ime,
                Prez = zaposleni.Prez,
                Kategorija = zaposleni.Kategorija,
                Idzap = zaposleni.Idzap,
                ExpiresAt = DateTime.UtcNow.AddHours(24)
            };
        }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
    {
        
        if (await _authRepository.EmailExistsAsync(registerDto.Email))
            throw new InvalidOperationException("Korisnik sa ovim email-om već postoji");

        if (await _authRepository.JmbgExistsAsync(registerDto.Jmbg))
            throw new InvalidOperationException("Korisnik sa ovim JMBG-om već postoji");

        
        var validneKategorije = new[] { "Enolog", "Somleijer", "Radnik" };
        if (!validneKategorije.Contains(registerDto.Kategorija))
            throw new InvalidOperationException("Neispravna kategorija zaposlenog");

        // hash
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerDto.PrivremenaLozinka);

        
        var noviZaposleni = new Zaposleni
        {
            Ime = registerDto.Ime,
            Prez = registerDto.Prez,
            Jmbg = registerDto.Jmbg,
            Email = registerDto.Email,
            Kategorija = registerDto.Kategorija,
            Sifra = hashedPassword,
            ZaposleniIdzap = registerDto.SuperiorId
        };

        noviZaposleni = await _authRepository.AddZaposleniAsync(noviZaposleni);

        switch (registerDto.Kategorija)
        {
            case "Enolog":
                if (string.IsNullOrEmpty(registerDto.Brsert))
                    throw new InvalidOperationException("Broj sertifikata je obavezan za Enologa");
                
                var enolog = new Enolog
                {
                    Idzap = noviZaposleni.Idzap,
                    Brsert = registerDto.Brsert
                };
                await _authRepository.AddEnologAsync(enolog);
                break;

            case "Radnik":
                if (string.IsNullOrEmpty(registerDto.Fizickaspremnost))
                    throw new InvalidOperationException("Fizička spremnost je obavezna za Radnika");
                
                var radnik = new Radnik
                {
                    Idzap = noviZaposleni.Idzap,
                    Fizickaspremnost = registerDto.Fizickaspremnost
                };
                await _authRepository.AddRadnikAsync(radnik);
                break;

            case "Somleijer":
                if (string.IsNullOrEmpty(registerDto.Specijalnost))
                    throw new InvalidOperationException("Specijalnost je obavezna za Somleijera");
                
                var somleijer = new Somleijer
                {
                    Idzap = noviZaposleni.Idzap,
                    Specijalnost = registerDto.Specijalnost
                };
                await _authRepository.AddSomleijerAsync(somleijer);
                break;
        }

      
        var token = _jwtService.GenerateToken(noviZaposleni);

        return new AuthResponseDto
        {
            Token = token,
            Email = noviZaposleni.Email,
            Ime = noviZaposleni.Ime,
            Prez = noviZaposleni.Prez,
            Kategorija = noviZaposleni.Kategorija,
            Idzap = noviZaposleni.Idzap,
            ExpiresAt = DateTime.UtcNow.AddHours(24)
        };
    }

        public async Task<bool> ChangePasswordAsync(int zaposleniId, ChangePasswordDto changePasswordDto)
        {
            var zaposleni = await _authRepository.GetZaposleniByIdAsync(zaposleniId);
            if (zaposleni == null)
                return false;

            if (!BCrypt.Net.BCrypt.Verify(changePasswordDto.StaraLozinka, zaposleni.Sifra))
                return false;

            zaposleni.Sifra = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.NovaLozinka);
            await _authRepository.UpdateZaposleniAsync(zaposleni);

            return true;
        }

        public async Task<ProfileDto?> GetProfileAsync(int zaposleniId)
        {
            var zaposleni = await _authRepository.GetZaposleniWithDetailsAsync(zaposleniId);

            if (zaposleni == null)
                return null;

            var profile = new ProfileDto
            {
                Idzap = zaposleni.Idzap,
                Ime = zaposleni.Ime,
                Prez = zaposleni.Prez,
                Jmbg = zaposleni.Jmbg,
                Email = zaposleni.Email,
                Kategorija = zaposleni.Kategorija
            };

            switch (zaposleni.Kategorija)
            {
                case "Enolog":
                    profile.Brsert = zaposleni.Enolog?.Brsert;
                    break;
                case "Menadzer":
                    profile.Bonusucinak = zaposleni.Menadzer?.Bonusucinak;
                    break;
                case "Radnik":
                    profile.Fizickaspremnost = zaposleni.Radnik?.Fizickaspremnost;
                    break;
                case "Somleijer":
                    profile.Specijalnost = zaposleni.Somleijer?.Specijalnost;
                    break;
            }

            return profile;
        }

        public async Task<List<EmployeeListDto>> GetAllEmployeesAsync(string? kategorija = null)
        {
            var zaposleni = await _authRepository.GetAllZaposleniAsync(kategorija);

            var employees = zaposleni
                .Select(z => new EmployeeListDto
                {
                    Idzap = z.Idzap,
                    Ime = z.Ime,
                    Prez = z.Prez,
                    Email = z.Email,
                    Jmbg = z.Jmbg,
                    Kategorija = z.Kategorija
                })
                .ToList();

            return employees;
        }
    }
}

