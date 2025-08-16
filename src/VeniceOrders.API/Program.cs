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

var builder = WebApplication.CreateBuilder(args);
var cfg = builder.Configuration;
var services = builder.Services;

// Controllers + Swagger
services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen(o =>
{
    o.SwaggerDoc("v1", new() { Title = "Venice Orders API", Version = "v1" });
    o.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Insira o token no formato: Bearer {seu_token}"
    });
    o.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
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

// Azure Service Bus
services.AddSingleton<ServiceBusClient>(_ => new ServiceBusClient(cfg.GetConnectionString("ServiceBus")));

// Repositórios e Serviços
services.AddScoped<IPedidoSqlRepository, PedidoSqlRepository>();
services.AddScoped<IPedidoItensMongoRepository, PedidoItensMongoRepository>();
services.AddScoped<IPedidoCache, PedidoCache>();
services.AddScoped<IEventPublisher, EventPublisher>();

// MediatR + FluentValidation
services.AddMediatR(cfgM => cfgM.RegisterServicesFromAssembly(typeof(CreatePedidoHandler).Assembly));
services.AddValidatorsFromAssembly(typeof(CreatePedidoRequestValidator).Assembly);

var app = builder.Build();

// Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapGet("/", () => Results.Redirect("/swagger"));
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
