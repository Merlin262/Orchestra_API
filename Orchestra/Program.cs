using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Orchestra.Data.Context;
using Orchestra.Models.Orchestra.Models;
using Orchestra.Repoitories;
using Orchestra.Repoitories.Interfaces;
using Orchestra.Services;
using Orchestra.Serviecs;
using Orchestra.Serviecs.Intefaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost3000",
        policy => policy
            .WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

// Configura��es de servi�os
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

// Adicione esta linha para registrar o reposit�rio
builder.Services.AddScoped<IBpmnProcessRepository, BpmnProcessRepository>();
builder.Services.AddScoped<IBpmnProcessInstanceRepository, BpmnProcessInstanceRepository>();
builder.Services.AddScoped<IProcessStepRepository, ProcessStepRepository>();
builder.Services.AddScoped<ITasksRepository, TasksRepository>();
builder.Services.AddScoped<IBpmnProcessBaselineRepository, BpmnProcessBaselineRepository>();
builder.Services.AddScoped<IGenericRepository<BpmnProcessInstance>, GenericRepository<BpmnProcessInstance>>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IBpmnProcessInstanceService, BpmnProcessInstanceService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IBpmnBaselineService, BpmnBaselineService>();

// Build do app (deve vir depois da configura��o de servi�os)
var app = builder.Build();

app.UseCors("AllowLocalhost3000");

// Pipeline de requisi��es
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
