using BlazorTemplate;
using BlazorTemplateAPI;
using Application;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Application.Helpers;
using Infrasrtucture;
using Application.Models;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy.WithOrigins("https://localhost:7063", "https://localhost:7234")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Укажите токен авторизации",
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
            new string[] { }
        }
    });
});


builder.Services.AddDbContext<MyDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("VillageContext")));

builder.Services.AddApplicationServices<ApplicationUser>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("VillageContext")));
builder.Services.AddAutoMapper(typeof(UserMappingProfile));

builder.Services.AddJwtAuthentication<ApplicationUser>(options =>
{
    options.Issuer = "https://localhost:7063";
    options.Audience = "https://localhost:7063";
    options.SecretKey = "MySecretKey123456789asdasdasdas0";
    options.TokenValidityMinutes = 90;
    options.RefreshTokenValidityDays = 7;
});
builder.Services.AddManagers<ApplicationUser>();
builder.Services.AddQuartz();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    });
}


app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseRouting();

app.UseCors("AllowSpecificOrigin");

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();







