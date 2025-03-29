using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Data.Services;
using MyOrderService = OrderService.Data.Services.OrderService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService((sp) => sp.GetRequiredService<PriceConsumer>());
builder.Services.AddSingleton<PriceConsumer>();

builder.Services.AddDbContext<OrderDBContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

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
