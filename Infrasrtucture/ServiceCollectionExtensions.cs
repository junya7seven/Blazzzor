using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application;
using Application.Models;
using Application.Service;
using Entities.Interfaces;
using Entities.Models;
using Infrasrtucture.Data;
using Infrasrtucture.Managers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Quartz.Impl;
using Quartz.Spi;
using Quartz;
using Microsoft.Extensions.Hosting;
using Application.Interfaces;

namespace Infrasrtucture
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices<TUser>(
         this IServiceCollection services,
         Action<DbContextOptionsBuilder> dbContextOptions) where TUser : User
        {
            services.AddDbContext<ApplicationDbContext<TUser>>(dbContextOptions);
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
                IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                ClockSkew = TimeSpan.Zero,
            };
        });
            return services;
        }
        public static IServiceCollection AddManagers<TUser>(this IServiceCollection services) where TUser : User
        {
            services.AddScoped<IUserManager<TUser>, UserManager<TUser>>();

            // Регистрация RoleManager, который зависит от UserManager
            services.AddScoped<IRoleManager<TUser>, RoleManager<TUser>>();

            // Другие сервисы, которые зависят от UserManager или RoleManager
            services.AddScoped<IRefreshTokenManager, RefreshTokenManager<TUser>>();

            services.AddScoped<IUserRoleService, UserRoleService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<AccessControl>();

            return services;
        }
        public static IServiceCollection AddQuartz(this IServiceCollection services)
        {
            /*services.AddQuartz(q =>
            {
                q.UseMicrosoftDependencyInjectionJobFactory();
            });

            services.AddScoped<IJobScheduler, JobScheduler>();

            services.AddHostedService<QuartzHostedService>();*/


            /*services.AddQuartz(o =>
            {

                var jobKey = JobKey.Create("Unlock");
                o.AddJob<UnlockUserJob>(jobKey)
                .AddTrigger(t =>
                t.ForJob(jobKey)
                .WithSimpleSchedule(s => s.WithIntervalInSeconds(2).RepeatForever()));
            });
            services.AddQuartzHostedService(o =>
            {
                o.WaitForJobsToComplete = true;
            });*/




            return services;
        }

    }
}