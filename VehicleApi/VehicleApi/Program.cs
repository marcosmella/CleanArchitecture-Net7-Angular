using Microsoft.EntityFrameworkCore;
using Vehicle.Infrastructure.Database;
using VehicleApi.Middleware;
using VehicleApi.Utils;
using NLog.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Logging.AddConsole();
NLog.Logger logger = NLogBuilder.ConfigureNLog("nlog.config")
                     .GetCurrentClassLogger();

logger.Info("init main");

builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(LogLevel.Information);
builder.Host.UseNLog();

builder.Services.AddDbContext<VehicleContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("stringSQL"));
});

builder.Services.ConfigureIoC();
builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddCors(options =>
{
    options.AddPolicy("newPolicy", app =>
    {
        app.AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});


var app = builder.Build();

/*************************************************
 * Associate a Global Error handler middleware with all your unhandled exceptions
 *************************************************/
app.UseMiddleware<ErrorHandlerMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("newPolicy");
app.UseAuthorization();

app.MapControllers();

app.Run();
