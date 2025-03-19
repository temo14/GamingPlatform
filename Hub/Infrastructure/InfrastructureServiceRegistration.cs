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

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        AddConfiguration(services, configuration);
        AddAuthentication(services, configuration);
        ConfigureSwaggerAuthentication(services);
        AddDatabase(services, configuration);
        AddRedis(services, configuration);
        AddRepositories(services);
        AddMessaging(services);
        AddSignalR(services);
        AddBackgroundServices(services);
        AddCors(services);

        return services;
    }
    public static void InitializeDatabase(this WebApplication app)
    {
        using (var serviceScope = app.Services.CreateScope())
        {
            var context = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            FormattableString sqlQuery = $"SELECT COUNT(*) FROM sys.databases WHERE name = 'HubDb'";
            var dbExists = context.Database.CanConnect() && context.Database.SqlQuery<int>(sqlQuery).FirstOrDefault() > 0;

            if (!dbExists)
            {
                context.Database.EnsureCreated();
            }
        }
    }
    private static void AddConfiguration(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
        services.Configure<RabbitMQSettings>(configuration.GetSection("RabbitMQ"));
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
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;

                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    }
                };
            });

        services.AddSingleton<ITokenService, TokenService>();
    }
    private static void AddDatabase(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("Db")));
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
    private static void AddRedis(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IConnectionMultiplexer>(provider =>
        {
            var redisConnectionString = configuration.GetConnectionString("Redis") ?? "localhost:6379";
            return ConnectionMultiplexer.Connect(redisConnectionString);
        });
    }
    private static void AddRepositories(IServiceCollection services)
    {
        services.AddScoped<IPlayerRepository, PlayerRepository>();
        services.AddScoped<IBetRepository, BetRepository>();
    }
    private static void AddMessaging(IServiceCollection services)
    {
        services.AddSingleton<IMessagePublisher, BetPublisher>();
        services.AddSingleton<IPrizeNotificationService, PrizeNotificationService>();
    }
    private static void AddSignalR(IServiceCollection services)
    {
        services.AddSignalR();
        services.AddSingleton<IUserConnectionManager, UserConnectionManager>();
    }
    private static void AddBackgroundServices(IServiceCollection services)
    {
        services.AddHostedService<PrizeSubscriber>();
    }
    private static void AddCors(IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });
        });
    }
}