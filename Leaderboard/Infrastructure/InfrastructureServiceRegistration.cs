using Application.Services;
using Domain.Repositories;
using Infrastructure.BackgroundServices;
using Infrastructure.Configuration;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using System.Text;

namespace Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        AddConfiguration(services, configuration);
        AddRedis(services, configuration);
        AddApplicationServices(services);
        AddDatabase(services, configuration);
        AddRepositories(services);
        AddAuthentication(services, configuration);
        ConfigureSwaggerAuthentication(services);
        AddBackgroundServices(services);

        return services;
    }
    public static void InitializeDatabase(this WebApplication app)
    {
        using (var serviceScope = app.Services.CreateScope())
        {
            var context = serviceScope.ServiceProvider.GetRequiredService<LeaderboardDbContext>();
            FormattableString sqlQuery = $"SELECT COUNT(*) FROM sys.databases WHERE name = 'LeaderboardDb'";
            var dbExists = context.Database.CanConnect() && context.Database.SqlQuery<int>(sqlQuery).FirstOrDefault() > 0;

            if (!dbExists)
            {
                context.Database.EnsureCreated();
            }
        }
    }
    private static void AddConfiguration(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RabbitMQSettings>(configuration.GetSection("RabbitMQ"));
        services.Configure<LeaderboardSettings>(configuration.GetSection("Leaderboard"));
    }
    private static void AddRedis(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IConnectionMultiplexer>(provider =>
        {
            var redisConnectionString = configuration.GetConnectionString("Redis") ?? "localhost:6379";
            return ConnectionMultiplexer.Connect(redisConnectionString);
        });
    }
    private static void AddApplicationServices(IServiceCollection services)
    {
        services.AddSingleton<IBetEventProcessor, BetEventProcessor>();
        services.AddScoped<ILeaderboardAggregator, LeaderboardAggregator>();
        services.AddScoped<ILeaderboardService, LeaderboardService>();
        services.AddScoped<IMessagePublisher, MessagePublisher>();
    }
    private static void AddDatabase(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<LeaderboardDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("Db")));
    }
    private static void AddRepositories(IServiceCollection services)
    {
        services.AddScoped<ILeaderboardRepository, LeaderboardRepository>();
        services.AddScoped<IPrizeRepository, PrizeRepository>();
    }
    private static void AddBackgroundServices(IServiceCollection services)
    {
        services.AddHostedService<BetSubscriber>();
    }
    private static void AddAuthentication(IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["JwtSettings:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"]))
                };
            });
    }
    private static void ConfigureSwaggerAuthentication(IServiceCollection services)
    {
        services.AddSwaggerGen(opt =>
        {
            opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "bearer"
            });
            opt.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                    new string[]{}
                }
            });
        });
    }
}