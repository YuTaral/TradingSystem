using Microsoft.EntityFrameworkCore;
using PortfolioService;
using PortfolioService.Data;
using PortfolioService.Data.Services;
using MyPortfolioService = PortfolioService.Data.Services.PortfolioService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService((sp) => sp.GetRequiredService<PriceConsumer>());
builder.Services.AddSingleton<PriceConsumer>();

builder.Services.AddSingleton<RabbitMQServer>();

builder.Services.AddDbContext<PortfolioDBContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IPortfolioService, MyPortfolioService>();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Start the RPCServer
var rpcServer = app.Services.GetRequiredService<RabbitMQServer>();
await rpcServer.StartAsync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
