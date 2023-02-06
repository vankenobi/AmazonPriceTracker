using AmazonPriceTrackerAPI.Persistence;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;
using Serilog.Formatting.Display;
using Serilog.Formatting.Compact;
using Serilog.Core;

var loggerConfig = new LoggerConfiguration()
    .WriteTo.File("all-daily-.logs", rollingInterval: RollingInterval.Day,outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {RequestId} {Message:lj}{NewLine}{Exception}")
    .MinimumLevel.Debug()
    .WriteTo.Seq("http://localhost:5341/")
    .WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {RequestId} {Message:lj}{NewLine}{Exception}")
    .Enrich.FromLogContext();

try
{
    Log.Information("Application starting up.");
}
catch (Exception ex)
{
    Log.Fatal(ex, "The application failed to start correctly.");
}
finally 
{
    Log.CloseAndFlush();
}

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddPersistenceServices();
builder.Services.AddSingleton<Serilog.ILogger>(loggerConfig.CreateLogger());

builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
    policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()
));

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
}

if (app.Environment.IsProduction()) 
{
    
}

if (app.Environment.IsStaging()) 
{

}



app.UseCors();
app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();

app.Run();
