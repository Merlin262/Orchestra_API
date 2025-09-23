using HotChocolate.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Orchestra.Data.Context;
using Orchestra.Domain.Repositories;
using Orchestra.Domain.Services;
using Orchestra.GraphQLConfig;
using Orchestra.Hubs;
using Orchestra.Infrastructure.Repositories;
using Orchestra.Models.Orchestra.Models;
using Orchestra.Repoitories;
using Orchestra.Repoitories.Interfaces;
using Orchestra.Services;
using Orchestra.Serviecs;
using Orchestra.Serviecs.Intefaces;
using System.Text;
using System.Reflection;
using Orchestra.Services.Serviecs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy
            .WithOrigins(
                "http://localhost:3000",
                "https://lively-pond-0eeb6930f.2.azurestaticapps.net"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});

builder.AddServiceDefaults();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();

// JWT Config
builder.Services.AddScoped<JwtService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!);
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

//GraphQL
builder.Services
       .AddGraphQLServer()
       .AddQueryType<BpmnProcessInstanceQuery>()
       .AddProjections()
       .AddFiltering()
       .AddSorting();

// SignalR
builder.Services.AddSignalR();

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
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    cfg.RegisterServicesFromAssembly(Assembly.Load("Orchestra.Application"));
});

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
builder.Services.AddScoped<IBaselineHistoryRepository, BaselineHistoryRepository>();
builder.Services.AddScoped<IBaselineFileService, BaselineFileService>();
builder.Services.AddScoped<IBaselineFileRepository, BaselineFileRepository>();

// Build do app (deve vir depois da configura��o de servi�os)
var app = builder.Build();

app.MapHub<TasksHub>("/hubs/taskshub");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

app.UseCors("AllowFrontend");

// Pipeline de requisi��es
app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment() || app.Environment.IsStaging() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseAuthorization();
app.MapControllers();
app.MapGraphQL();

app.Run();
