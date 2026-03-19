using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using WineryAPI.Data;
using WineryAPI.Repositories;
using WineryAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure JWT Authentication
var jwtSecret = builder.Configuration["JwtSettings:Secret"] 
    ?? throw new InvalidOperationException("JWT Secret nije konfigurisan");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
    };
});

builder.Services.AddAuthorization();

// Register Repositories
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IVinogradRepository, VinogradRepository>();
builder.Services.AddScoped<ISortagrozdjaRepository, SortagrozdjaRepository>();
builder.Services.AddScoped<IBerbaRepository, BerbaRepository>();
builder.Services.AddScoped<IRadoviRepository, RadoviRepository>();
builder.Services.AddScoped<IUbranasirovinaRepository, UbranasirovinaRepository>();
builder.Services.AddScoped<ISirovinazatretmanRepository, SirovinazatretmanRepository>();
builder.Services.AddScoped<ITretmanRepository, TretmanRepository>();
builder.Services.AddScoped<ISirovovinoRepository, SirovovinoRepository>();
builder.Services.AddScoped<IVinoRepository, VinoRepository>();
builder.Services.AddScoped<IDegustacijaRepository, DegustacijaRepository>();

// Register Services
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IVinogradService, VinogradService>();
builder.Services.AddScoped<ISortagrozdjaService, SortagrozdjaService>();
builder.Services.AddScoped<IBerbaService, BerbaService>();
builder.Services.AddScoped<IRadoviService, RadoviService>();
builder.Services.AddScoped<IUbranasirovinaService, UbranasirovinaService>();
builder.Services.AddScoped<ISirovinazatretmanService, SirovinazatretmanService>();
builder.Services.AddScoped<ITretmanService, TretmanService>();
builder.Services.AddScoped<ISirovovinoService, SirovovinoService>();
builder.Services.AddScoped<IVinoService, VinoService>();
builder.Services.AddScoped<IPodrumService, PodrumService>();
builder.Services.AddScoped<IBureService, BureService>();
builder.Services.AddScoped<IMagacinService, MagacinService>();
builder.Services.AddScoped<ISeLagerujeService, SeLagerujeService>();
builder.Services.AddScoped<IBocaService, BocaService>();
builder.Services.AddScoped<IDegustacijaService, DegustacijaService>();

// Configure Swagger with JWT support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Winery API", Version = "v1" });
    
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header koristeći Bearer šemu. Unesi: 'Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000") // Vue dev server
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Seed default menadzer
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await DbSeeder.SeedDefaultManagerAsync(context);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
