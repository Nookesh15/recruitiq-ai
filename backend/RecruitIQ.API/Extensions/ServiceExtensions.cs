using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RecruitIQ.Application.Common.Interfaces;
using RecruitIQ.Domain.Interfaces;
using RecruitIQ.Infrastructure.Persistence;
using RecruitIQ.Infrastructure.Repositories;
using RecruitIQ.Infrastructure.Services;

namespace RecruitIQ.API.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<AppDbContext>());
        services.AddScoped<ICandidateRepository, CandidateRepository>();
        services.AddScoped<ILookupRepository, LookupRepository>();

        // AI Engine HTTP client — base URL defaults to localhost for dev
        var aiEngineUrl = config["AiEngineUrl"] ?? "http://localhost:8000";
        services.AddHttpClient<IAiEngineService, AiEngineService>(c =>
            c.BaseAddress = new Uri(aiEngineUrl));

        services.AddScoped<IJwtService, JwtService>();
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddSingleton<IEmailService, SmtpEmailService>();

        return services;
    }

    public static IServiceCollection AddJwtAuth(this IServiceCollection services, IConfiguration config)
    {
        var secret = config["Jwt:Secret"] ?? "default-dev-secret-min-32-chars!!";

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = config["Jwt:Issuer"] ?? "recruitiq-api",
                    ValidAudience = config["Jwt:Audience"] ?? "recruitiq-frontend",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))
                };
            });

        services.AddAuthorization();
        return services;
    }

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(
                typeof(Application.Candidates.Commands.CreateCandidate.CreateCandidateHandler).Assembly));

        return services;
    }
}
