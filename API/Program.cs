using Microsoft.OpenApi.Models;
using Configurator.API.Endpoints;
using Configurator.API.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Configurator API", Version = "v1" });
});

// Temporary in-memory repository to bootstrap the first endpoint
builder.Services.AddSingleton<IApplicationRepository, ApplicationRepository>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();


// Map endpoints
app.MapApplicationEndpoints();

app.Run();

public partial class Program { }

