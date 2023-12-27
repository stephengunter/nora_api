using Serilog;
using Serilog.Events;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using ApplicationCore.DataAccess;
using Microsoft.AspNetCore.Identity;
using ApplicationCore.Models;
using ApplicationCore.Settings;
using ApplicationCore.Consts;
using ApplicationCore.Helpers;
using ApplicationCore.DI;
using Web.Filters;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;

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

	#region Autofac
	builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
	builder.Host.ConfigureContainer<ContainerBuilder>(builder =>
	{
		builder.RegisterModule<ApplicationCoreModule>();
	});
	#endregion

	#region Add Configurations
	builder.Services.Configure<AppSettings>(Configuration.GetSection(SettingsKeys.App));
	builder.Services.Configure<AdminSettings>(Configuration.GetSection(SettingsKeys.Admin));
	builder.Services.Configure<AuthSettings>(Configuration.GetSection(SettingsKeys.Auth));
	builder.Services.Configure<MailSettings>(Configuration.GetSection(SettingsKeys.Mail));
	#endregion
	
	// Add services to the container.
	string connectionString = Configuration.GetConnectionString("Default")!;
	builder.Services.AddDbContext<DefaultContext>(options =>
						//options.UseNpgsql(connectionString));
						options.UseSqlServer(connectionString));

   #region AddIdentity
   builder.Services.AddIdentity<User, IdentityRole>(options =>
	{
		options.User.RequireUniqueEmail = true;
	})
	.AddEntityFrameworkStores<DefaultContext>()
	.AddDefaultTokenProviders();
	#endregion

	#region AddFilters
	builder.Services.AddScoped<DevelopmentOnlyFilter>();
	#endregion
	
	builder.Services.AddJwtBearer(Configuration);
	builder.Services.AddAuthorizationPolicy();
	builder.Services.AddCorsPolicy(Configuration);
	builder.Services.AddDtoMapper();

	builder.Services.AddControllers()
	.AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        });
	builder.Services.AddSwagger(Configuration);

	var app = builder.Build();
	AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

	app.UseSerilogRequestLogging(); 

	if (app.Environment.IsDevelopment())
	{
		if(Configuration[$"{SettingsKeys.Developing}:SeedDatabase"].ToBoolean())
		{
			// Seed Database
			using (var scope = app.Services.CreateScope())
			{
				var services = scope.ServiceProvider;
				try
				{
					await SeedData.EnsureSeedData(services, Configuration);
				}
				catch (Exception ex)
				{
					Log.Fatal(ex, "SeedData Error");
				}
			}
		}
		
		app.UseSwagger();
		app.UseSwaggerUI();
	}
	else
	{
		app.UseHttpsRedirection();
	}
	
	app.UseCors();
	app.UseAuthentication();
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






