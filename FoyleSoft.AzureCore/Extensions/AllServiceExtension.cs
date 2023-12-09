using Azure.Storage.Blobs;
using FoyleSoft.AzureCore.Implementations;
using FoyleSoft.AzureCore.Interfaces;
using FoyleSoft.AzureCore.Interfaces.Repositories.Roles;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
namespace FoyleSoft.AzureCore.Extensions
{
    public static class AllServiceExtension
    {
        private static Assembly _dataDll;

        public static IFunctionsHostBuilder ApplyAllServices(this IFunctionsHostBuilder builder,
            //List<Claim> claims,
            //List<string> roleNames,
            List<Assembly> dataDlls,
            List<Assembly> serviceDlls,
            Type customSesionRepository,
            Type sessionService,
            Assembly apiDll, string configurationKey, string clientConfigurationKey)
        {

            var dummy = new Dummy();
            var _configuration = builder.Services.BuildServiceProvider().GetService<IConfiguration>();

            //services.AddTransient<IRoleService, RoleService>();
            builder.Services.AddSingleton<IAzureConfigurationService, AzureConfigurationService>(f => new AzureConfigurationService(_configuration, configurationKey, clientConfigurationKey));
            builder.Services.AddScoped<IAzureADJwtBearerValidation, AzureADJwtBearerValidation>();
            builder.Services.AddSingleton<ICacheService, CacheService>();
            builder.Services.AddScoped<IRoleService, RoleService>();
            builder.Services.AddScoped<IGraphApiService, GraphApiService>();
            builder.Services.AddHttpContextAccessor();

            var azureConfigurationService = builder.Services.BuildServiceProvider().GetService<IAzureConfigurationService>();
            string apiName = apiDll.GetName().Name ?? "NONAME";
            ApplyLogConfiguration(azureConfigurationService, apiName);
            ApplyCacheConfiguration(builder, azureConfigurationService, apiName);
            builder.Services.AddLogging(configure =>
            {
                configure.AddSerilog(Log.Logger);
                //configure.AddConsole();
            });


            ApplyDataConfiguration(dummy, builder, dataDlls, azureConfigurationService);

            dataDlls.ForEach(dataDll =>
            {
                dummy.RunMongoRepository(builder.Services, azureConfigurationService, dataDll, typeof(IMongoCollectionRepositoryAsync<>));
                dummy.RunRepository(builder.Services, azureConfigurationService, dataDll, typeof(IBaseRepositoryAsync<>), customSesionRepository);
            });
            serviceDlls.ForEach(serviceDll =>
            {
                dummy.RunRepository(builder.Services, azureConfigurationService, serviceDll, typeof(IBaseService), customSesionRepository);
            });

            dummy.RunRepositorySessionService(builder.Services, sessionService);


            var azureCacheService = builder.Services.BuildServiceProvider().GetService<ICacheService>();

            builder.Services.AddTransient<IBlobStorageService, BlobStorageService>
                (f => new BlobStorageService(azureCacheService, azureConfigurationService,
                    new BlobServiceClient(azureConfigurationService.AzureConfig.StorageAccountConnection)));
            return builder;

        }

        private static void ApplyDataConfiguration(Dummy dummy, IFunctionsHostBuilder builder, List<Assembly> dataDlls, IAzureConfigurationService azureConfigurationService)
        {
            dataDlls.ForEach(dataDll =>
            {
                dummy.ImplementMySqlDbContext(builder.Services, dataDll, typeof(FoyleSoftMySqlContext), azureConfigurationService.AzureConfig.ConnectionStringMySQL);
                dummy.ImplementMySqlDbContext(builder.Services, dataDll, typeof(FoyleSoftMySqlLogContext), azureConfigurationService.AzureConfig.ConnectionStringMySQL);
            });
        }


        private static void ApplyCacheConfiguration(IFunctionsHostBuilder builder, IAzureConfigurationService azureConfigurationService, string instanceName)
        {
            if (azureConfigurationService is not null)
            {
                builder.Services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = azureConfigurationService.AzureConfig.AzureRedisConnection;
                    options.InstanceName = instanceName;
                });
            }
            else
            {
                builder.Services.AddDistributedMemoryCache();
            }

        }
        private static void ApplyLogConfiguration(IAzureConfigurationService azureConfigurationService, string logFileName)
        {
            var telemetryClient = new TelemetryConfiguration { InstrumentationKey = azureConfigurationService.AzureConfig.InstrumentationKey };
            var logLevel = LogEventLevel.Information;
            string consoleTemplate = "{Timestamp:HH:mm:ss} {Level:u3} {userName} {SourceContext}.{Method} {Message:lj}{NewLine}{Exception}";
            string fileTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3} {userName} {SourceContext}.{Method} {Message:lj}{NewLine}{Exception}";

            string rootDirectory = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "..\\..\\..\\..\\"));
            string logFilePath = Path.Combine(rootDirectory, $"Logs/{logFileName}.log");
            string errorLogFilePath = Path.Combine(rootDirectory, $"Logs/errors/{logFileName}.log");

            LoggerConfiguration logConfig = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console(theme: SystemConsoleTheme.Literate)
                .WriteTo.ApplicationInsights(telemetryClient, TelemetryConverter.Traces)
                .WriteTo.Console(restrictedToMinimumLevel: logLevel, outputTemplate: consoleTemplate)
                .WriteTo.Console(theme: SystemConsoleTheme.Literate)
                .WriteTo.File(logFilePath, outputTemplate: fileTemplate, restrictedToMinimumLevel: logLevel, rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true, fileSizeLimitBytes: 5 * 1024 * 1024)
                .WriteTo.File(errorLogFilePath, outputTemplate: fileTemplate, restrictedToMinimumLevel: LogEventLevel.Error, rollingInterval: RollingInterval.Month, rollOnFileSizeLimit: true, fileSizeLimitBytes: 5 * 1024 * 1024);

            switch (logLevel)
            {
                case LogEventLevel.Debug:
                    logConfig.MinimumLevel.Debug();
                    break;
                case LogEventLevel.Verbose:
                    logConfig.MinimumLevel.Verbose();
                    break;
                case LogEventLevel.Information:
                    logConfig.MinimumLevel.Information();
                    break;
                case LogEventLevel.Warning:
                    logConfig.MinimumLevel.Warning();
                    break;
                case LogEventLevel.Error:
                    logConfig.MinimumLevel.Error();
                    break;
            }
            Log.Logger = logConfig.CreateLogger();
        }
        public static DbContextOptionsBuilder AssignAssembly([NotNullAttribute] this DbContextOptionsBuilder optionsBuilder, [NotNullAttribute] Assembly dataDll)
        {
            _dataDll = dataDll;
            return optionsBuilder;
        }
        internal static Assembly GetAssembly([NotNullAttribute] this DbContextOptionsBuilder optionsBuilder)
        {
            return _dataDll;
        }

    }
    public class Dummy
    {
        public void ImplementMsSqlDbContext(IServiceCollection services,
            IConfiguration configuration,
            Assembly dataDll, Type contextType, string connectionStringName = null)
        {
            var userContexts = dataDll.GetTypes().Where(f => f.IsClass && contextType.IsAssignableFrom(f)).ToList();
            userContexts.ForEach(userContext =>
            {
                MethodInfo? method = typeof(Dummy).GetMethod(nameof(Dummy.AddMsSqlDbContextPool),
                          BindingFlags.Public | BindingFlags.Instance);
                if (method != null)
                {
                    method = method.MakeGenericMethod(userContext);
                    method.Invoke(this, new object[] { services, configuration, dataDll, connectionStringName });
                }
            });
        }
        public void AddMsSqlDbContextPool<T>(IServiceCollection services,
            IConfiguration configuration,
            Assembly dataDll, string connectionStringName = null) where T : DbContext
        {
            string conStr = connectionStringName != null ? connectionStringName : configuration.GetConnectionString(typeof(T).Name);
            services.AddDbContextPool<T>(options => options.UseSqlServer(conStr,
                                                                            sqlServerOptionsAction: sqlOptions =>
                                                                            {
                                                                                sqlOptions.EnableRetryOnFailure(
                                                                                maxRetryCount: 10,
                                                                                maxRetryDelay: TimeSpan.FromSeconds(10),
                                                                                errorNumbersToAdd: null);
                                                                            }));
        }
        public void ImplementMySqlDbContext(IServiceCollection services,
           Assembly dataDll, Type contextType, string connectiontring)
        {
            var userContexts = dataDll.GetTypes().Where(f => f.IsClass && contextType.IsAssignableFrom(f)).ToList();
            userContexts.ForEach(userContext =>
            {
                MethodInfo? method = typeof(Dummy).GetMethod(nameof(Dummy.AddMySqlDbContextPool),
                          BindingFlags.Public | BindingFlags.Instance);
                if (method != null)
                {
                    method = method.MakeGenericMethod(userContext);
                    method.Invoke(this, new object[] { services, dataDll, connectiontring });
                }
            });
        }
        public void AddMySqlDbContextPool<T>(IServiceCollection services,
            Assembly dataDll, string connectionStringName) where T : DbContext
        {
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 31));
            services.AddDbContextPool<T>(options => options
                .AssignAssembly(dataDll)
              .UseMySql(
                  connectionStringName,
                  serverVersion,
                  sqlOptions =>
                  {
                      sqlOptions.EnableRetryOnFailure(
                      maxRetryCount: 10,
                      maxRetryDelay: TimeSpan.FromSeconds(30),
                      errorNumbersToAdd: null);
                  }));

        }
        public void RunMongoRepository(IServiceCollection services,
            IAzureConfigurationService configuration,
            Assembly dataDll,
            Type interfaceType)
        {
            var classes = dataDll.GetTypes().Where(f => f.IsClass && f.GetInterface(interfaceType.Name) != null
            && f.Name != "MongoCollectionRepositoryAsync`1").ToList();
            var exceptionClasses = dataDll.GetTypes().Where(f => f.IsClass && f.GetInterface("IExceptionService") != null && f.Name != "MongoCollectionRepositoryAsync`1").Select(f => f.Name).ToList();
            classes.Where(c => !exceptionClasses.Contains(c.Name)).ToList().ForEach(f =>
            {
                MethodInfo? method = typeof(Dummy).GetMethod(nameof(Dummy.AddScoped),
                    BindingFlags.Public | BindingFlags.Instance);
                if (method != null)
                {
                    method = method.MakeGenericMethod(f.GetInterfaces().First(p => p.Name == "I" + f.Name), f);
                    method.Invoke(this, new object[] { services });
                }
            });
        }
        public void RunRepository(IServiceCollection services,
            IAzureConfigurationService configuration,
            Assembly dataDll,
            Type interfaceType, Type customSesionRepository)
        {
            var classes = dataDll.GetTypes().Where(f => f.IsClass && f.GetInterface(interfaceType.Name) != null
            && f.Name != "BaseRepositoryAsync`1").ToList();
            var exceptionClasses = dataDll.GetTypes().Where(f => f.IsClass && (f.GetInterface("IExceptionService") != null || f.Name == customSesionRepository.Name) && f.Name != "BaseRepositoryAsync`1").Select(f => f.Name).ToList();
            classes.Where(c => !exceptionClasses.Contains(c.Name)).ToList().ForEach(f =>
            {
                MethodInfo? method = typeof(Dummy).GetMethod(nameof(Dummy.AddScoped),
                    BindingFlags.Public | BindingFlags.Instance);
                if (method != null)
                {
                    if (f.Name.IndexOf("BaseRepositoryAsync") >= 0)
                    {
                    }
                    method = method.MakeGenericMethod(f.GetInterfaces().First(p => p.Name == "I" + f.Name), f);
                    method.Invoke(this, new object[] { services });
                }
            });
        }
        public void RunRepositorySessionService(IServiceCollection services,
            Type sessionService)
        {


            MethodInfo? methodSessionService = typeof(Dummy).GetMethod(nameof(Dummy.AddScoped),
                BindingFlags.Public | BindingFlags.Instance);
            if (methodSessionService != null)
            {
                var sessionServiceInterface = sessionService.GetInterface("I" + sessionService.Name);
                if (sessionServiceInterface != null)
                {
                    methodSessionService = methodSessionService.MakeGenericMethod(sessionServiceInterface, sessionService);
                    methodSessionService.Invoke(this, new object[] { services });
                }
            }
            MethodInfo? methodSessionUser = typeof(Dummy).GetMethod(nameof(Dummy.AddScoped),
                BindingFlags.Public | BindingFlags.Instance);
            if (methodSessionUser != null)
            {
                methodSessionUser = methodSessionUser.MakeGenericMethod(typeof(ISessionUser), sessionService);
                methodSessionUser.Invoke(this, new object[] { services });
            }

        }
        public void AddTransient<I, C>(IServiceCollection services) where I : class where C : class, I
        {
            services.AddTransient<I, C>();
        }
        public void AddScoped<I, C>(IServiceCollection services) where I : class where C : class, I
        {
            services.AddScoped<I, C>();
        }
    }
}
