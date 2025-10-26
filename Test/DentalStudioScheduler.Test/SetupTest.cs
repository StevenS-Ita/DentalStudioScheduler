using Mch.ContextDbBase;
using Mch.StdCore.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace Mch.Internal.UnifiedContextDb.Test
{
    internal sealed class SetupTest
    {
        public static IServiceProvider Setup()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddJsonFile($"appsettings.{Environment.UserName}.json", optional: true, reloadOnChange: false);

            IConfiguration configuration = builder.Build();
            IServiceCollection services = new ServiceCollection();
            services.Configure<ConnectionString>(configuration.GetSection("ConnectionString"));
            services.AddReadAndWriteTransientContext<InternalContext>(configuration.GetSection("ConnectionString").Get<ConnectionString>().ToString());

            // Add logging
            services.AddLogging(logging =>
            {
                logging.AddConfiguration(configuration.GetSection("Logging"));
                logging.ClearProviders();
                logging.AddConsole();
            });

            // Add allservices classes
            //services.AddTransient<Mch.Internal.Srv.Services.Configuration.ArticleService>();


            // Build service provaider
            var serviceProvider = services.BuildServiceProvider();


            //// Remove and create database
            //using var ctx = serviceProvider.GetRequiredService<InternalContext>();
            //ctx.Database.EnsureDeleted();
            //ctx.Database.EnsureCreated();

            return serviceProvider;
        }

        public static ILogger CreateLogger(IServiceProvider serviceProvider, Type type)
        {
            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            return loggerFactory.CreateLogger(type);
        }

    }
}
