using AutoMapper;
using CitasMedicas.Core.CustomEntities;
using CitasMedicas.Core.DTOs;
using CitasMedicas.Core.Entities;
using CitasMedicas.Core.Enums;
using CitasMedicas.Core.Interfaces;
using CitasMedicas.Core.Services;
using CitasMedicas.Infrastructure.Data;
using CitasMedicas.Infrastructure.Filters;
using CitasMedicas.Infrastructure.Mappings;
using CitasMedicas.Infrastructure.Repositories;
using CitasMedicas.Infrastructure.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configuración
builder.Configuration.Sources.Clear();
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
    Console.WriteLine("User Secrets habilitados para desarrollo");
}

// ============================================================
// CONFIGURACION DE BASE DE DATOS
// ============================================================
var connectionString = builder.Configuration.GetConnectionString("ConnectionSqlServer");
builder.Services.AddDbContext<CitasMedicasContext>(options =>
    options.UseSqlServer(connectionString));

// ============================================================
// CONFIGURACION AUTOMAPPER
// ============================================================
builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.Configure<PasswordOptions>(builder.Configuration.GetSection("PasswordOptions"));

// ============================================================
// INYECCION DE DEPENDENCIAS
// ============================================================
builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<IMedicoService, MedicoService>();
builder.Services.AddScoped<IPacienteService, PacienteService>();
builder.Services.AddScoped<IDisponibilidadService, DisponibilidadService>();
builder.Services.AddScoped<ICitaService, CitaService>();
builder.Services.AddScoped<ISecurityService, SecurityService>();
builder.Services.AddSingleton<IPasswordService, PasswordService>();


// ============================================================
// DAPPER
// ============================================================
builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
builder.Services.AddScoped<IDapperContext, DapperContext>();

// ============================================================
// VALIDACION GLOBAL
// ============================================================
builder.Services.AddValidatorsFromAssemblyContaining <ReservaCitaDtoValidator>();

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
    options.Filters.Add<GlobalExceptionFilter>();
})
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling =Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    })
.ConfigureApiBehaviorOptions(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

// ============================================================
// AUTENTICACION JWT 
// ============================================================

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Authentication:Issuer"],
        ValidAudience = builder.Configuration["Authentication:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
                builder.Configuration["Authentication:SecretKey"]
            )
        )
    };
});
// Repositorios base
builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));

// Repositorios específicos
builder.Services.AddScoped<IMedicoRepository, MedicoRepository>();
builder.Services.AddScoped<IPacienteRepository, PacienteRepository>();
builder.Services.AddScoped<IDisponibilidadRepository, DisponibilidadRepository>();
builder.Services.AddScoped<ICitaRepository, CitaRepository>();
builder.Services.AddScoped<IPagoRepository, PagoRepository>();
builder.Services.AddScoped<ISecurityRepository, SecurityRepository>();

// UnitOfWork
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
// ============================================================
// SWAGGER Y VERSIONAMIENTO
// ============================================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Citas Medicas API",
        Version = "v1",
        Description = "Documentación de la API para gestionar el sistema de citas medicas - Version 1.0",
        Contact = new OpenApiContact
        {
            Name = "Equipo de Desarrollo Citas Medicas",
            Email = "natalia.arrazola@ucb.edu.bo"
        }
    });

    options.SwaggerDoc("v2", new()
    {
        Title = "Citas Medicas API - v2",
        Version = "v2",
        Description = "API para gestion de citas medicas - Version 2.0 con mejoras",
        Contact = new()
        {
            Name = "Equipo de Desarrollo citas medicas",
            Email = "natalia.arrazola@ucb.edu.bo"
        }
    });


    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);

    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
        Console.WriteLine($"Documentacion XML cargada: {xmlFile}");
    }
    else
    {
        Console.WriteLine($"Advertencia: Archivo de documentacion XML no encontrado: {xmlFile}");
        Console.WriteLine("Asegurate de tener <GenerateDocumentationFile>true</GenerateDocumentationFile> en el .csproj");
    }

    options.DocInclusionPredicate((version, apiDescription) =>
    {
        if (!apiDescription.TryGetMethodInfo(out var methodInfo)) return false;

        var versions = methodInfo.DeclaringType?
            .GetCustomAttributes(true)
            .OfType<ApiVersionAttribute>()
            .SelectMany(attr => attr.Versions);

        var maps = methodInfo
            .GetCustomAttributes(true)
            .OfType<MapToApiVersionAttribute>()
            .SelectMany(attr => attr.Versions);

        return versions?.Any(v => v.ToString() == version) == true ||
               maps?.Any(v => v.ToString() == version) == true;
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Introduce el token JWT en el formato: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[] {}
        }
    });

});

// ----------------------------
// Versionamiento (API Versioning)
// ----------------------------
builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("x-api-version"),
        new QueryStringApiVersionReader("api-version")
    );
});

// ----------------------------
// Explorador de Versiones (Swagger)
// ----------------------------
builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

var app = builder.Build();

// ============================================================
// SWAGGER UI
// ============================================================
app.Environment.IsDevelopment();
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Citas Medicas  API v1");
    options.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();


