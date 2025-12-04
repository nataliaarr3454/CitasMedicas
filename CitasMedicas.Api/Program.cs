using AutoMapper;
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
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Text;
using CitasMedicas.Core.CustomEntities;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.Sources.Clear();
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
    Console.WriteLine("User Secrets habilitados para desarrollo");
}

builder.Configuration.AddEnvironmentVariables();

Console.WriteLine($"Ambiente: {builder.Environment.EnvironmentName}");
Console.WriteLine($"Aplicacion: {builder.Environment.ApplicationName}");

var secretKey = builder.Configuration["Authentication:SecretKey"];
var issuer = builder.Configuration["Authentication:Issuer"];
var audience = builder.Configuration["Authentication:Audience"];

if (string.IsNullOrEmpty(secretKey))
{
    Console.WriteLine("Advertencia: Authentication:SecretKey no configurada");
    Console.WriteLine("En desarrollo, use: dotnet user-secrets set \"Authentication:SecretKey\" \"tu_clave_aqui\"");
    Console.WriteLine("En produccion, configure la variable de entorno: Authentication:SecretKey");
}

if (string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
{
    Console.WriteLine("Advertencia: Issuer o Audience no configurados");
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer ?? "https://localhost:7137/",
        ValidAudience = audience ?? "https://localhost:7137/",
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(secretKey ?? "DefaultKeyForDevelopmentOnly12345!")
        ),
        ClockSkew = TimeSpan.Zero
    };

    Console.WriteLine("Autenticacion JWT configurada");
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole(RoleType.Administrador.ToString()));

    options.AddPolicy("MedicoOrAdmin", policy =>
        policy.RequireRole(RoleType.Medico.ToString(), RoleType.Administrador.ToString()));

    options.AddPolicy("PacienteOrAdmin", policy =>
        policy.RequireRole(RoleType.Paciente.ToString(), RoleType.Administrador.ToString()));

    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();

    Console.WriteLine("Politicas de autorizacion configuradas");
});

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

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

Console.WriteLine("Versionamiento de API configurado");

builder.Services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();
builder.Services.AddScoped<IDapperContext, DapperContext>();

var databaseProvider = builder.Configuration["DatabaseProvider"] ?? "SqlServer";
Console.WriteLine($"Proveedor de base de datos: {databaseProvider}");

if (databaseProvider == "SqlServer")
{
    var connectionString = builder.Configuration.GetConnectionString("ConnectionSqlServer");

    if (string.IsNullOrEmpty(connectionString))
    {
        Console.WriteLine("Advertencia: ConnectionStrings:ConnectionSqlServer no configurada");
        Console.WriteLine("Para desarrollo, use: dotnet user-secrets set \"ConnectionStrings:ConnectionSqlServer\" \"Server=localhost;Database=CitasMedicas;Trusted_Connection=True;TrustServerCertificate=True;\"");
        Console.WriteLine("Para produccion, configure la variable de entorno: ConnectionStrings__ConnectionSqlServer");

        if (builder.Environment.IsDevelopment())
        {
            connectionString = "Server=localhost;Database=CitasMedicas;Trusted_Connection=True;TrustServerCertificate=True;";
            Console.WriteLine($"   Usando cadena de conexion de ejemplo: {connectionString.Substring(0, 30)}...");
        }
    }

    builder.Services.AddDbContext<CitasMedicasContext>(options =>
        options.UseSqlServer(connectionString));

    Console.WriteLine("SQL Server configurado");
}
else
{
    throw new InvalidOperationException($"Proveedor de base de datos no soportado: {databaseProvider}. Solo se soporta 'SqlServer'");
}

builder.Services.AddAutoMapper(typeof(MappingProfile));
Console.WriteLine("AutoMapper configurado");

builder.Services.Configure<PasswordOptions>(
    builder.Configuration.GetSection("PasswordOptions"));

builder.Services.AddSingleton<IPasswordService, PasswordService>();

// Validaciones FluentValidation
builder.Services.AddScoped<IValidator<MedicoDto>, MedicoDtoValidator>();
builder.Services.AddScoped<IValidator<PacienteDto>, PacienteDtoValidator>();
builder.Services.AddScoped<IValidator<DisponibilidadDto>, DisponibilidadDtoValidator>();
builder.Services.AddScoped<IValidator<ReservaCitaDto>, ReservaCitaDtoValidator>();
builder.Services.AddScoped<IValidator<SecurityDto>, SecurityDtoValidator>();
builder.Services.AddScoped<IValidator<UserLogin>, UserLoginValidator>();
Console.WriteLine("Validadores FluentValidation registrados");

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IMedicoService, MedicoService>();
builder.Services.AddScoped<IPacienteService, PacienteService>();
builder.Services.AddScoped<IDisponibilidadService, DisponibilidadService>();
builder.Services.AddScoped<ICitaService, CitaService>();
builder.Services.AddScoped<ISecurityService, SecurityService>();
Console.WriteLine("Servicios registrados");

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

Console.WriteLine("Controladores configurados");

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "Citas Medicas API - v1",
        Version = "v1",
        Description = "API para gestion de citas medicas - Version 1.0",
        Contact = new()
        {
            Name = "Equipo de Desarrollo UCB",
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
            Name = "Equipo de Desarrollo UCB",
            Email = "natalia.arrazola@ucb.edu.bo"
        }
    });

    options.EnableAnnotations();

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
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

    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Introduce el token JWT en el formato: Bearer {token}"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
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

    Console.WriteLine("Swagger configurado");
});

var app = builder.Build();

Console.WriteLine("\nAplicacion construida exitosamente");
Console.WriteLine("========================================");

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Citas Medicas API v1");
    options.SwaggerEndpoint("/swagger/v2/swagger.json", "Citas Medicas API v2");
    options.RoutePrefix = string.Empty; 
    options.DisplayRequestDuration();
    options.EnableFilter();
    options.EnableDeepLinking();
    options.EnableTryItOutByDefault();
});

Console.WriteLine("Swagger UI disponible en la raiz (/)");

if (app.Environment.IsDevelopment())
{
    Console.WriteLine("Modo desarrollo activado");
    app.UseDeveloperExceptionPage();
}
else
{
    Console.WriteLine("Modo produccion activado");
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Map("/error", (HttpContext context) =>
{
    var exception = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()?.Error;
    return Results.Problem(
        title: "Error inesperado",
        detail: exception?.Message,
        statusCode: StatusCodes.Status500InternalServerError
    );
});

Console.WriteLine("\nMiddleware configurado");
Console.WriteLine("================================");

app.MapGet("/", () => Results.Redirect("/swagger")).AllowAnonymous();
app.MapGet("/health", () => Results.Json(new
{
    status = "healthy",
    timestamp = DateTime.UtcNow,
    environment = app.Environment.EnvironmentName,
    version = "1.0"
})).AllowAnonymous();

Console.WriteLine("\nEndpoints disponibles:");
Console.WriteLine("  GET  /                -> Redirige a Swagger UI");
Console.WriteLine("  GET  /health          -> Health Check");
Console.WriteLine("  POST /api/Token       -> Autenticacion JWT");
Console.WriteLine("  GET  /api/Token/TestConexion -> Prueba de conexion");
Console.WriteLine("  GET  /api/Token/Config -> Configuracion del sistema");
Console.WriteLine("\nEndpoints protegidos:");
Console.WriteLine("  /api/v1/Medico/*      -> Requiere autenticacion");
Console.WriteLine("  /api/v1/Paciente/*    -> Requiere autenticacion");
Console.WriteLine("  /api/v1/Cita/*        -> Requiere autenticacion");
Console.WriteLine("  /api/v1/Disponibilidad/* -> Requiere autenticacion");

app.Run();