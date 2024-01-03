using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using MinimalApiPeliculas.Context;
using MinimalApiPeliculas.Endpoints;
using MinimalApiPeliculas.Entidades;
using MinimalApiPeliculas.Migrations;
using MinimalApiPeliculas.Repositorios;

var builder = WebApplication.CreateBuilder(args);
var ambiente = builder.Configuration.GetValue<string>("Ambiente");
var origenesPermitidos = builder.Configuration.GetValue<string>("OrigenesPermitidos")!;

#region Servicios
builder.Services.AddDbContext<ApplicationDBContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(origenesPermitidos).AllowAnyHeader().AllowAnyMethod();
    });

    options.AddPolicy("libre", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

builder.Services.AddOutputCache();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IRepositorioGeneros, RepositorioGeneros>();
builder.Services.AddAutoMapper(typeof(Program));
#endregion

var app = builder.Build();

#region Middleware
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();
app.UseOutputCache();

app.MapGet("/", [EnableCors(policyName: "libre")] () => ambiente);

//Agrupacion de Endpoints
app.MapGroup("/generos").MapGeneros();
#endregion

app.Run();