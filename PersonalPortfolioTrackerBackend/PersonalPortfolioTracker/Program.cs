using Microsoft.EntityFrameworkCore;
using PersonalPortfolioTracker.Data;
using PersonalPortfolioTracker.Repositories.Interfaces;
using PersonalPortfolioTracker.Repositories.Implementations;
using PersonalPortfolioTracker.Integrations.Interfaces;
using PersonalPortfolioTracker.Integrations.Implementations;
using PersonalPortfolioTracker.Services.Interfaces;
using PersonalPortfolioTracker.Services.Implementations;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(5000);
});

builder.Services.AddCors(options =>
{
    string? origin = Environment.GetEnvironmentVariable("PERSONAL_PORTFOLIO_TRACKER_FRONTEND");
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.SetIsOriginAllowed(string.IsNullOrWhiteSpace(origin) ? _ => false : o => o == origin)
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

builder.Services.AddControllers();

builder.Services.AddHttpClient();

builder.Services.AddMemoryCache();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    string? connectionString = Environment.GetEnvironmentVariable("PERSONAL_PORTFOLIO_TRACKER_DB_CONNECTION_STRING");
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});


builder.Services.AddScoped<IStockRepository, StockRepository>();
builder.Services.AddScoped<IPositionRepository, PositionRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<IDividendRepository, DividendRepository>();
builder.Services.AddScoped<ICapitalChangeRepository, CapitalChangeRepository>();

builder.Services.AddScoped<IStockInfoHandler, StockInfoHandler>();

builder.Services.AddScoped<IService, Service>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

app.UseCors("AllowFrontend");

app.UseAuthorization();

app.MapControllers();

app.Run();