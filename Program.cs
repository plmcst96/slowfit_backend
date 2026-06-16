using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using slowfit.DBModels;
using slowfit.DTORequest;
using slowfit.JWT;
using slowfit.Services;
using System.Text;

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
            builder.Services.AddControllers(options =>
            {
                options.Filters.Add(new AuthorizeFilter());
            });
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddServer(new OpenApiServer { Url = "http://localhost:5051" });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Insert JWT token only. Swagger will add the Bearer prefix."
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
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

            builder.Services.Configure<JWTModel>(builder.Configuration.GetSection("JwtConfiguration"));
            var jwtKey = builder.Configuration["JwtConfiguration:Key"];
            var jwtIssuer = builder.Configuration["JwtConfiguration:Issuer"];
            var jwtAudience = builder.Configuration["JwtConfiguration:Audience"];

            if (string.IsNullOrWhiteSpace(jwtKey))
            {
                throw new InvalidOperationException("JwtConfiguration:Key is required.");
            }

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
                    ValidIssuer = jwtIssuer,
                    ValidateAudience = true,
                    ValidAudience = jwtAudience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

            builder.Services.AddAuthorization();
            builder.Services.AddHttpClient();
            builder.Services.AddScoped<IRoleService, RoleService>();
            builder.Services.AddScoped<ICrudService<TypeNutritionRes>, TypeNutritionService>();
            builder.Services.AddScoped<ICrudService<CategoryOfDayRes>, CategoryOfDayService>();
            builder.Services.AddScoped<ICrudService<DayWeekRes>, DayWeekService>();
            builder.Services.AddScoped<ICrudService<BodyPartRes>, BodyPartService>();
            builder.Services.AddScoped<ICrudService<LevelTrainingRes>, LevelTrainingService>();
            builder.Services.AddScoped<ICrudService<LocationTrainingRes>, LocationTrainingService>();
            builder.Services.AddScoped<ICrudService<InputTypeRes>, InputTypeService>();
            builder.Services.AddScoped<ICrudService<PaymentTypeRes>, PaymentTypeService>();
            builder.Services.AddScoped<ICrudService<TypePlanRes>, TypePlanService>();
            builder.Services.AddScoped<ICrudService<TypeTrainingRes>, TypeTrainingService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IUserRegistrationService, UserRegistrationService>();
            builder.Services.AddScoped<IIngredientService, IngredientService>();
            builder.Services.AddScoped<IExerciseService, ExerciseService>();
            builder.Services.AddScoped<ICrudService<QuestionRes>, QuestionService>();
            builder.Services.AddScoped<IAnswerService, AnswerService>();
            builder.Services.AddScoped<IResponseQuizService, ResponseQuizService>();
            builder.Services.AddScoped<ICrudService<ProductRes>, ProductService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IBillingService, BillingService>();
            builder.Services.AddScoped<IMeasureService, MeasureService>();
            builder.Services.AddScoped<IDetailExerciseService, DetailExerciseService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IQuizService, QuizService>();
            builder.Services.AddScoped<IAppointmentService, AppointmentService>();
            builder.Services.AddScoped<INotificationService, NotificationService>();
            builder.Services.AddScoped<IProgressTrainingService, ProgressTrainingService>();
            builder.Services.AddScoped<IProgressNutritionService, ProgressNutritionService>();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //app.UseHttpsRedirection();
            app.UseCors("slowFit_policy");

            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
