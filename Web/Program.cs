using Serilog;
using Serilog.Events;
using Microsoft.EntityFrameworkCore;
using ApplicationCore.DataAccess;
using Microsoft.AspNetCore.Identity;
using ApplicationCore.Models;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting web application");
    var builder = WebApplication.CreateBuilder(args);
    var Configuration = builder.Configuration;
    builder.Host.UseSerilog((context, services, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext());


    // Add services to the container.
    string connectionString = Configuration.GetConnectionString("Default")!;
    builder.Services.AddDbContext<DefaultContext>(options =>
                    options.UseNpgsql(connectionString));

    #region AddIdentity
    builder.Services.AddIdentity<User, IdentityRole>(options =>
    {
        options.User.RequireUniqueEmail = true;
    }).AddEntityFrameworkStores<DefaultContext>()
    .AddDefaultTokenProviders();
    #endregion

    builder.Services.AddControllers();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();
    AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    
    app.UseSerilogRequestLogging(); 

    if (app.Environment.IsDevelopment())
    {        
        
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    else
    {
        app.UseHttpsRedirection();
    }
    

    app.UseAuthorization();

    app.MapControllers();
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.Information("finally");
    Log.CloseAndFlush();
}






