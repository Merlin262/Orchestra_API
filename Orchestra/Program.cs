using Microsoft.EntityFrameworkCore;
using Orchestra.Data.Context;

var builder = WebApplication.CreateBuilder(args);

// Configurações de serviços
builder.AddServiceDefaults();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();

// Controllers
builder.Services.AddControllers();

// DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Build do app (deve vir depois da configuração de serviços)
var app = builder.Build();

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
