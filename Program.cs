using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using slowfit.DBModels;
using slowfit.JWT;

namespace slowfit
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            if (builder.Environment.IsDevelopment())
            {
                // ✅ Ambiente locale: porta custom, niente HTTPS obbligatorio
                builder.WebHost.ConfigureKestrel(o =>
                {
                    o.ListenAnyIP(5051); // HTTP
                });
            }
            else
            {
                // ✅ Produzione: HTTPS e certificati
                builder.WebHost.ConfigureKestrel(o =>
                {
                    o.ListenAnyIP(80); // redirect HTTP
                    o.ListenAnyIP(443, opt => opt.UseHttps("cert.pfx", "mypassword"));
                });
            }

            // ----------------- CONFIGURAZIONE SERVIZI -----------------
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.Configure<JWTModel>(builder.Configuration.GetSection("JwtConfiguration"));

            // CORS
            builder.Services.AddCors(options =>
                options.AddPolicy("slowFit_policy", policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod())
            );

            // DbContext SQL Server
            builder.Services.AddDbContext<SlowFitContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("dbConnectionString"));
                // ✅ Abilita logging dettagliato delle query EF Core
                options.LogTo(Console.WriteLine, LogLevel.Information) // oppure Debug per più dettagli
                       .EnableSensitiveDataLogging()                   // mostra i valori dei parametri nelle query
                       .EnableDetailedErrors();
            });

            // ----------------- ASP.NET Core Identity -----------------
            builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                // Password
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;

                // User
                options.User.RequireUniqueEmail = true;

                // Lockout
                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;

            })
            .AddEntityFrameworkStores<SlowFitContext>()  // EF Core DbContext
            .AddDefaultTokenProviders();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //app.UseHttpsRedirection();
            app.UseCors("slowFit_policy");

            app.UseAuthorization();

            app.UseMiddleware<JWTConfiguration>(); // JWT custom
            app.MapControllers();

            app.Run();
        }
    }
}
