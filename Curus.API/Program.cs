using Curus.API;
using Curus.API.Extensions;
using Curus.Repository;
using Hangfire;
using Serilog;
using System.Text.Json.Serialization;
using Curus.Repository.Data;

var builder = WebApplication.CreateBuilder(args);

// Cấu hình Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
    });
builder.Services.AddWebAPIService(builder.Configuration);

// Add Sentry
builder.WebHost.UseSentry(o =>
{
    o.Dsn = "https://c89ec4d88ac03a3bd9b2ff1d4eb67978@o4507695181135872.ingest.de.sentry.io/4507695192211536";
    // When configuring for the first time, to see what the SDK is doing:
    o.Debug = true;
    // Set TracesSampleRate to 1.0 to capture 100%
    // of transactions for performance monitoring.
    // We recommend adjusting this value in production
    o.TracesSampleRate = 1.0;
});

var app = builder.Build();

app.UseHangfireServer();
app.UseHangfireDashboard();
app.UseStaticFiles();

app.UseHttpsRedirection();



app.UseDeveloperExceptionPage();
app.UseSwagger();
app.UseSwaggerUI(c =>
{

    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Cursus Web API v1");
    c.InjectJavascript("/custom-swagger.js");


});
app.ApplyMigrations(app.Logger);
app.UseExceptionHandler("/Error");


app.MapControllers();

// Apply DbInitializer
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<CursusDbContext>();
    await DbInitializer.Initialize(context);
}
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.Run();