using BudgetingSystem.Shared.Domain.Interfaces;
using BudgetingSystem.Shared.Infrastructure.Repositories;
using BudgetingSystem.Users.Application.Services;
using BudgetingSystem.Users.Domain.Entities;
using BudgetingSystem.Budgets.Application.Services;
using BudgetingSystem.Budgets.Domain.Entities;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register repositories
builder.Services.AddSingleton<IRepository<User>, InMemoryRepository<User>>();
builder.Services.AddSingleton<IRepository<Budget>, InMemoryRepository<Budget>>();

// Register services
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<BudgetService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();