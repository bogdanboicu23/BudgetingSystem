using ReportingService.API.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddApiServices();
builder.Services.AddApplicationServices();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();