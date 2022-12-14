using Application.Interfaces;
using Application.Mappings;
using Application.Services;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Web;
using WebAPI.Extensions;

namespace WebAPI;

public class Startup
{
    private readonly WebApplicationBuilder _builder;
    private readonly Logger _logger;

    public Startup(string[] args)
    {
        _builder = WebApplication.CreateBuilder(args);
        _logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

        _logger.Debug("File Storage Service was started");
    }

    public Startup SetBuilderOptions()
    {
        _builder.Services.AddControllers().AddNewtonsoftJson();
        _builder.Services.AddEndpointsApiExplorer();
        _builder.Services.AddSwaggerGen(x => {
            x.EnableAnnotations();
            x.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "File Storage API", Version = "0.1" });
        });

        return this;
    }

    public Startup SetOpenCors()
    {
        _builder.Services.AddCors(options =>
        {
            options.AddPolicy("Open", builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .WithExposedHeaders("Content-Disposition");
            });
        });

        _logger.Debug("The CORS open policy was successfully added");

        return this;
    }

    public Startup SetDbContext()
    {
        var fileConnString = _builder.Configuration.GetConnectionString("FileStorageConnection");
        
        _builder.Services.AddDbContext<FileDbContext>(options =>
        {
            options.UseSqlServer(fileConnString).EnableSensitiveDataLogging();
        });

        _logger.Debug("SQL connection was successfully added");

        return this;
    }

    public Startup SetMapperConfig()
    {
        var mapperConfig = AutoMapperConfig.Initialize();

        _builder.Services.AddSingleton(mapperConfig);
        _logger.Debug("The mapping config was successfully added");

        return this;
    }

    public Startup AddInternalServices()
    {
        _builder.Services.AddTransient<IFileTransferService, FileTransferService>();
        _builder.Services.AddTransient<IFileStorageService, FileStorageService>();
        _logger.Debug("Internal services were successfully added");

        return this;
    }

    public Startup AddInternalRepositories()
    {
        _builder.Services.AddTransient<IFileStorageRepository, FileStorageRepository>();
        _builder.Services.AddTransient<IFileTransferRepository, FileTransferRepository>();
        _logger.Debug("Internal repositories were successfully added");

        return this;
    }

    public Startup AddLoggerConfig()
    {
        _builder.Logging.ClearProviders();
        _builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
        _builder.Host.UseNLog();

        _logger.Debug("Logger options were successfully added");

        return this;
    }

    public void Run()
    {
        var app = _builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.UseCors("Open");
        app.MigrateDatabase();
        app.UseHttpsRedirection();
        app.MapControllers();
        app.Run();

        _logger.Debug("Application has been successfully ran");
    }
}