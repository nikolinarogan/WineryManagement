using Microsoft.EntityFrameworkCore;
using WineryAPI.Models;

namespace WineryAPI.Data
{
    public static class DbSeeder
    {
        public static async Task SeedDefaultManagerAsync(ApplicationDbContext context)
        {
            // Proveri da li već postoji Menadžer
            var menadzeriPostoje = await context.Zaposlenis
                .AnyAsync(z => z.Kategorija == "Menadzer");

            if (menadzeriPostoje)
            {
                Console.WriteLine("✓ Menadžer već postoji u bazi.");
                return;
            }

            // Kreiraj default Menadžera
            var defaultManager = new Zaposleni
            {
                Ime = "Admin",
                Prez = "Menadžer",
                Jmbg = "0000000000000", // Promijeniti na realan JMBG
                Email = "admin@vinarija.rs",
                Kategorija = "Menadzer",
                // Default lozinka: "Admin123!"
                // BCrypt hash generisan sa 12 rounds
                Sifra = BCrypt.Net.BCrypt.HashPassword("Admin123!")
            };

            context.Zaposlenis.Add(defaultManager);
            await context.SaveChangesAsync();

            Console.WriteLine("✓ Default Menadžer uspješno kreiran!");
            Console.WriteLine("  Email: admin@vinarija.rs");
            Console.WriteLine("  Lozinka: Admin123!");
            Console.WriteLine("  ⚠️  PROMIJENITE LOZINKU nakon prvog logovanja!");
        }
    }
}

