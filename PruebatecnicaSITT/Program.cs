using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PruebatecnicaSITT.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configuraci�n de servicios en el contenedor

// Conexi�n a la base de datos usando Entity Framework y SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configuraci�n de autenticaci�n JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])) // Aseg�rate de que la clave tiene al menos 32 caracteres
        };
    });

// Agregar servicios de controladores
builder.Services.AddControllers();

// Agregar pol�tica de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularClient", policy =>
    {
        policy.WithOrigins("http://localhost:4200") // Permitir solicitudes desde Angular
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Agregar Swagger para la documentaci�n de API (opcional, pero recomendado)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Construir la aplicaci�n
var app = builder.Build();

// Configuraci�n del middleware para la aplicaci�n
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Aplicar la pol�tica de CORS
app.UseCors("AllowAngularClient");

// Middleware para la autenticaci�n
app.UseAuthentication();

// Middleware para la autorizaci�n
app.UseAuthorization();

app.MapControllers();

app.Run();
