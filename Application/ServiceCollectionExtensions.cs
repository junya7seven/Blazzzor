using Application.Service;
using Entities.Interfaces;
using Entities.Models;
using Infrasrtucture.Data;
using Infrasrtucture.Managers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices<TUser>(
         this IServiceCollection services,
         Action<DbContextOptionsBuilder> dbContextOptions) where TUser : User
        {
            services.AddDbContext<ApplicationDbContext<TUser>>(dbContextOptions);

            services.AddScoped(typeof(UserManager<>));
            services.AddScoped(typeof(RoleManager<>));
            return services;

        }
        public static IServiceCollection AddJwtAuthentication<TUser>(
            this IServiceCollection services,
            Action<JwtSettings> configureOptions) where TUser : User
        {
            services.AddSingleton<JwtSettings>();
            var jwtSettings = new JwtSettings();
            configureOptions(jwtSettings);


            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })  
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;

            options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
            };
        });


            services.AddScoped(typeof(IUserManager<TUser>), typeof(UserManager<TUser>));
            services.AddScoped(typeof(IRoleManager<TUser>), typeof(RoleManager<TUser>));
            services.AddScoped<IRefreshTokenManager, RefreshTokenManager<TUser>>();

            services.AddScoped<AccessControl<TUser>>(provider =>
            {
                var roleManager = provider.GetRequiredService<IRoleManager<TUser>>();
                var userManager = provider.GetRequiredService<IUserManager<TUser>>();
                var tokenManager = provider.GetRequiredService<IRefreshTokenManager>();

                return new AccessControl<TUser>(jwtSettings, roleManager, userManager, tokenManager);
            });
            return services;
        }
    }
}
