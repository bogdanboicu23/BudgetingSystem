using Budgets.Application;
using Budgets.Application.Reports;
using Budgets.Domain.Strategies;
using Budgets.Domain.Facades;
using Budgets.Domain.Decorators;
using Budgets.Infrastructure;
using Budgets.Infrastructure.Data;
using MachineLearning.Infrastructure;
using Transactions.Application;
using Transactions.Application.Services;
using Transactions.Domain.Processing;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Register our design pattern modules
builder.Services.AddBudgetsApplication();
builder.Services.AddBudgetsInfrastructure(builder.Configuration);
builder.Services.AddTransactionsApplication();
builder.Services.AddMachineLearningInfrastructure();

// Register Strategy Pattern implementations
builder.Services.AddScoped<FixedBudgetStrategy>();
builder.Services.AddScoped<RollingBudgetStrategy>();
builder.Services.AddScoped<PercentageBasedBudgetStrategy>();

// Register Facade Pattern
builder.Services.AddScoped<BudgetAnalyticsFacade>();

// Register Observer Pattern (these are likely already registered in modules)
// builder.Services.AddScoped<IEmailNotificationObserver, EmailNotificationObserver>();
// builder.Services.AddScoped<IDatabaseLogObserver, DatabaseLogObserver>();

var app = builder.Build();

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<BudgetDbSeeder>();
    await seeder.SeedAsync();
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Serve static files from wwwroot
app.UseStaticFiles();
app.UseDefaultFiles();

app.UseHttpsRedirection();

// Map controllers
app.MapControllers();

// Redirect root to our demo page
app.MapGet("/", () => Results.Redirect("/design-patterns-demo.html"));

app.Run();