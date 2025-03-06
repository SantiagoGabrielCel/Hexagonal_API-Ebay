using Microsoft.EntityFrameworkCore;
using Api_EbayStocks.Application.Services;
using Api_EbayStocks.Core.Interface;
using Api_EbayStocks.data.Infraestructura;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddScoped<IEbayStockRepositorio, EbayStockRepositorio>();
builder.Services.AddScoped<EbayStockServicios>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();