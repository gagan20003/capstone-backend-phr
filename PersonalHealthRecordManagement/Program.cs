using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PersonalHealthRecordManagement.Data;
using PersonalHealthRecordManagement.Middleware;
using PersonalHealthRecordManagement.Models;
using PersonalHealthRecordManagement.Repositories;
using PersonalHealthRecordManagement.Services;

namespace PersonalHealthRecordManagement
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configuration
            var configuration = builder.Configuration;

            // Add services to the container.

            // Configure/Register DbContext class for  Oracle EF Core
            builder.Services.AddDbContext<AppDbContext>(options => options.UseOracle(builder.Configuration.GetConnectionString("DefaultConnection")));
            // Identity
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 8;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();
            builder.Services.AddTransient<IUserProfileRepository, UserProfileRepository>();
            builder.Services.AddTransient<IUserProfileService, UserProfileService>();
            builder.Services.AddTransient<IMedicalRecordRepository, MedicalRecordRepository>();
            builder.Services.AddTransient<IMedicalRecordService, MedicalRecordService>();
            builder.Services.AddTransient<IAppointmentRepository, AppointmentRepository>();
            builder.Services.AddTransient<IAppointmentService, AppointmentService>();
            builder.Services.AddTransient<IAllergyRepository, AllergyRepository>();
            builder.Services.AddTransient<IAllergyService, AllergyService>();
            builder.Services.AddTransient<IMedicationRepository, MedicationRepository>();
            builder.Services.AddTransient<IMedicationService, MedicationService>();


            // JWT
            var jwtSection = configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSection["Key"]!);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtSection["Issuer"],
                    ValidateAudience = true,
                    ValidAudience = jwtSection["Audience"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(30)
                };
            });

            builder.Services.AddAuthorization();

            // DI
            builder.Services.AddScoped<JwtTokenService>();

            builder.Services.AddControllers();
            
            // CORS
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.WithOrigins(builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? new[] { "*" })
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials();
                });
            });

            // Health Checks
            builder.Services.AddHealthChecks();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
                });



            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Global Exception Handler
            app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

            app.UseHttpsRedirection();

            app.UseCors();

            app.UseAuthentication();
            app.UseAuthorization();

            // Health Check endpoint
            app.MapHealthChecks("/health");

            app.MapControllers();

            // Ensure DB & seed roles
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var db = services.GetRequiredService<AppDbContext>();
                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                var logger = services.GetRequiredService<ILogger<Program>>();
                
                // Apply migrations (recommended to use dotnet ef migrations add / update in development)
                try
                {
                    logger.LogInformation("Applying database migrations...");
                    db.Database.Migrate();
                    logger.LogInformation("Database migrations applied successfully.");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while applying database migrations");
                    // In production, you might want to fail fast or handle this differently
                    if (!app.Environment.IsDevelopment())
                    {
                        throw;
                    }
                }
                var roles = new[] { "Admin", "User" };
                foreach (var role in roles)
                {
                    if (roleManager.Roles.FirstOrDefault(r => r.Name == role) == null)
                    {
                        var result = roleManager.CreateAsync(new IdentityRole(role)).GetAwaiter().GetResult();
                        if (result.Succeeded)
                        {
                            logger.LogInformation("Role '{Role}' created successfully", role);
                        }
                        else
                        {
                            logger.LogWarning("Failed to create role '{Role}': {Errors}", role, string.Join(", ", result.Errors.Select(e => e.Description)));
                        }
                    }
                }
            }

            app.Run();
        }
    }
}
