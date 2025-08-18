using System.Text;
using Azure.Messaging.ServiceBus;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using VeniceOrders.Application.Commands.CreatePedido;
using VeniceOrders.Application.Validation;
using VeniceOrders.Domain.Repositories;
using VeniceOrders.Domain.Services;
using VeniceOrders.Infrastructure.Cache;
using VeniceOrders.Infrastructure.Data.Context;
using VeniceOrders.Infrastructure.Data.Repositories;
using VeniceOrders.Infrastructure.Messaging;
using VeniceOrders.Infrastructure.Mongo;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
var cfg = builder.Configuration;
var services = builder.Services;

// Controllers + Swagger
services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen(o =>
{
    o.SwaggerDoc("v1", new() { Title = "Venice Orders API", Version = "v1" });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token no formato: Bearer {seu_token}"
    };

    o.AddSecurityDefinition("Bearer", securityScheme);
    o.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, Array.Empty<string>() }
    });
});

// Auth JWT
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = cfg["Jwt:Issuer"],
            ValidAudience = cfg["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(cfg["Jwt:Key"]!))
        };
    });

// DB SQL Server
services.AddDbContext<OrdersDbContext>(o =>
    o.UseSqlServer(cfg.GetConnectionString("SqlServer")));

// MongoDB
services.AddSingleton<IMongoClient>(_ => new MongoClient(cfg.GetConnectionString("Mongo")));

// Redis
services.AddStackExchangeRedisCache(o =>
{
    o.Configuration = cfg.GetConnectionString("Redis");
});

// Azure Service Bus ou FakeEventPublisher
if (builder.Environment.IsDevelopment() && string.IsNullOrWhiteSpace(cfg.GetConnectionString("ServiceBus")))
{
    services.AddScoped<IEventPublisher, FakeEventPublisher>();
}
else
{
    services.AddSingleton<ServiceBusClient>(_ => new ServiceBusClient(cfg.GetConnectionString("ServiceBus")));
    services.AddScoped<IEventPublisher, EventPublisher>();
}

// Repositórios e Serviços
services.AddScoped<IPedidoSqlRepository, PedidoSqlRepository>();
services.AddScoped<IPedidoItensMongoRepository, PedidoItensMongoRepository>();
services.AddScoped<IPedidoCache, PedidoCache>();

// MediatR + FluentValidation
services.AddMediatR(cfgM => cfgM.RegisterServicesFromAssembly(typeof(CreatePedidoHandler).Assembly));
services.AddValidatorsFromAssembly(typeof(CreatePedidoRequestValidator).Assembly);

var app = builder.Build();

// Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // Redireciona / → /swagger mas NÃO aparece no Swagger
    app.MapGet("/", () => Results.Redirect("/swagger"))
        .ExcludeFromDescription();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
