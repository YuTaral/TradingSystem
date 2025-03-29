using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Data.Services;
using MyOrderService = OrderService.Data.Services.OrderService;

var builder = WebApplication.CreateBuilder(args);

// Add the background service
builder.Services.AddHostedService((sp) => sp.GetRequiredService<OrderServicePriceConsumer>());
builder.Services.AddSingleton<OrderServicePriceConsumer>();

builder.Services.AddDbContext<AppDBContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddScoped<IOrderService, MyOrderService>();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
