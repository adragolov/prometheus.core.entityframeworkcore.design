using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.CommandLine;
using System;
using System.IO;

namespace Prometheus.Core.EntityFrameworkCore.Design
{
    /// <summary>
    ///     A base factory for creating derived Microsoft.EntityFrameworkCore.DbContext instances.
    ///     Implement the abtract methods to enable design-time services for context types that
    ///     do not have a public default constructor. 
    ///     
    ///     At design-time, derived Microsoft.EntityFrameworkCore.DbContext
    ///     instances can be created in order to enable specific design-time experiences
    ///     such as Migrations. Design-time services will automatically discover implementations
    ///     of this interface that are in the startup assembly or the same assembly as the
    ///     derived context.
    /// </summary>
    /// <typeparam name="TContext">The target DbContext type.</typeparam>
    public abstract class DbContextFactoryBase<TContext> : IDesignTimeDbContextFactory<TContext>
        where TContext : DbContext
    {
        /// <summary>
        ///     Gets the design time factory settings.
        /// </summary>
        public virtual DbContextFactorySettings Settings { get; protected set; }
            = new DbContextFactorySettings();

        /// <summary>
        ///      Creates a new instance of a derived context.
        /// </summary>
        /// <param name="args">Contextual arguments, usually passed by the CLI environment.</param>
        public abstract TContext CreateDbContext(string[] args);

        /// <summary>
        ///     Builds the host configuration root.
        /// </summary>
        /// <param name="commandLineArgs">Command line interface arguments.</param>
        protected virtual IConfigurationRoot BuildHostConfiguration(string[] commandLineArgs)
        {
            if (!Settings.LoggingDisabled)
            {
                Console.WriteLine(@" ---> Preparing service host configuration...");

                if (commandLineArgs != null && commandLineArgs.Length > 0)
                {
                    Console.WriteLine(@" ---> Command line arguments:");
                    foreach (var arg in commandLineArgs)
                    {
                        Console.WriteLine($"   -> {arg}");
                    }
                    Console.WriteLine();
                }
            }

            var appSettingsRoot = $"{Directory.GetCurrentDirectory()}/";

            if (!Settings.LoggingDisabled)
            {
                Console.WriteLine($" ---> Root path is {appSettingsRoot}");
            }

            var configurationBuilder = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddCommandLine(commandLineArgs)
                .SetBasePath(appSettingsRoot)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            string environmentFromCommandLine = null;

            if (commandLineArgs != null && commandLineArgs.Length > 0)
            {
                var commandLineProvider = new CommandLineConfigurationProvider(commandLineArgs);

                commandLineProvider.Load();
                
                if (commandLineProvider.TryGet("environment", out string env))
                {
                    environmentFromCommandLine = env;
                }
            }

            var environmentName = environmentFromCommandLine ??
                (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production");

            if (!Settings.LoggingDisabled)
            {
                Console.WriteLine($" ---> Environment is {environmentName}");
            }

            configurationBuilder = configurationBuilder.AddJsonFile($"appsettings.{environmentName}.json", optional: true);

            return configurationBuilder.Build();
        }
    }
}