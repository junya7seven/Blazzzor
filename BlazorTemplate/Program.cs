using BlazorTemplate;
using BlazorTemplate.Models;
using BlazorTemplateAPI;
using Application;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<MyDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("VillageContext")));

builder.Services.AddApplicationServices<ApplicationUser>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("VillageContext")));

builder.Services.AddJwtAuthentication<ApplicationUser>(options =>
{
    options.Issuer = "https://localhost:7234";
    options.Audience = "https://localhost:7234";
    options.SecretKey = "MySecretKey123456789asdasdasdas0";
    options.TokenValidityMinutes = 15;
    options.RefreshTokenValidityDays = 7;
});




builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("https://localhost:7234")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


var app = builder.Build();
app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
