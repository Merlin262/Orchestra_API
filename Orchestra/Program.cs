using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Orchestra.Data.Context;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost3000",
        policy => policy
            .WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

// Configurações de serviços
builder.AddServiceDefaults();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();

// Controllers
builder.Services.AddControllers();
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 104857600; // Set appropriate file size limit
});


// DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add MediatR services
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

// Adicione esta linha para registrar o repositório
builder.Services.AddScoped<Orchestra.Repoitories.IBpmnProcessRepository, Orchestra.Repoitories.BpmnProcessRepository>();


// Build do app (deve vir depois da configuração de serviços)
var app = builder.Build();

app.UseCors("AllowLocalhost3000");

// Pipeline de requisições
app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    // app.MapOpenApi(); // pode ativar se quiser servir o OpenAPI direto
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
