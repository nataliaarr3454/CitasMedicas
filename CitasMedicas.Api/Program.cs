using AutoMapper;
using CitasMedicas.Core.DTOs;
using CitasMedicas.Core.Interfaces;
using CitasMedicas.Core.Services;
using CitasMedicas.Infrastructure.Data;
using CitasMedicas.Infrastructure.Mappings;
using CitasMedicas.Infrastructure.Repositories;
using CitasMedicas.Infrastructure.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using CitasMedicas.Infrastructure.Filters;
using Swashbuckle.AspNetCore.Annotations; 
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// ====================
// Registrar Servicios
// ====================
builder.Services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();
builder.Services.AddScoped<IDapperContext, DapperContext>();

var connectionString = builder.Configuration.GetConnectionString("ConnectionSqlServer");

builder.Services.AddDbContext<CitasMedicasContext>(options =>
    options.UseSqlServer(connectionString));

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Validaciones FluentValidation
builder.Services.AddScoped<IValidator<MedicoDto>, MedicoDtoValidator>();
builder.Services.AddScoped<IValidator<PacienteDto>, PacienteDtoValidator>();
builder.Services.AddScoped<IValidator<DisponibilidadDto>, DisponibilidadDtoValidator>();
builder.Services.AddScoped<IValidator<ReservaCitaDto>, ReservaCitaDtoValidator>();

// Servicios
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IMedicoService, MedicoService>();
builder.Services.AddScoped<IPacienteService, PacienteService>();
builder.Services.AddScoped<IDisponibilidadService, DisponibilidadService>();
builder.Services.AddScoped<ICitaService, CitaService>();

// Controladores y Filtros globales
builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalExceptionFilter>();
})
.AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
})
.ConfigureApiBehaviorOptions(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

// ====================
// Configurar Swagger
// ====================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "Citas Médicas API",
        Version = "v1",
        Description = "Documentación de la API para la gestión de citas médicas - .NET 9",
        Contact = new()
        {
            Name = "Equipo de Desarrollo UCB",
            Email = "desarrollo@ucb.edu.bo"
        }
    });

   options.EnableAnnotations();

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

// ====================
// Autorización
// ====================
builder.Services.AddAuthorization();

// ====================
// Construir aplicación
// ====================
var app = builder.Build();

// ====================
// Middleware de Swagger
// ====================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Citas Médicas API v1");
        options.RoutePrefix = string.Empty; 
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
