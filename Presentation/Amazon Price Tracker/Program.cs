using AmazonPriceTrackerAPI.Persistence;
using Serilog;

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

//builder.Configuration.SetBasePath(Path.Combine(Directory.GetCurrentDirectory()));
//var connectionString = builder.Configuration.GetValue<string>("EmailSettings:Port");


// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddPersistenceServices();

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

app.UseCors();
app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();

app.Run();
