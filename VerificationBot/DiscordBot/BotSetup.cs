using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FencingtrackerBot.DiscordBot.Services;

namespace FencingtrackerBot.DiscordBot
{
    public class BotSetup
    {
        // Configuration instance which will contain the bot configuration settings
        public IConfigurationRoot Configuration { get; }

        public BotSetup()
        {
            // Build our configuration instance with the given json file
            IConfigurationBuilder Builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("config.json", optional: false, reloadOnChange: true);

            Configuration = Builder.Build();
        }

        public async Task RunSetupAsync()
        {
            ServiceCollection Services = ConfigureServices();

            // Setup all of our required services (LoggingService, CommandHandler)
            ServiceProvider Provider = Services.BuildServiceProvider();
            Provider.GetRequiredService<LoggingService>();
            Provider.GetRequiredService<CommandHandler>();

            // Start our SetupService, this includes logging our bot in and starting our discord socket client 
            await Provider.GetRequiredService<SetupService>().StartAsync();
            await Task.Delay(-1);
        }

        private ServiceCollection ConfigureServices()
        {
            ServiceCollection Services = new ServiceCollection();

            Services.AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose,
                MessageCacheSize = 1000
            }))
            .AddSingleton(new CommandService(new CommandServiceConfig
            {
                LogLevel = LogSeverity.Verbose,
                DefaultRunMode = RunMode.Async
            }))
            .AddSingleton<SetupService>()
            .AddSingleton<LoggingService>()
            .AddSingleton<CommandHandler>()
            .AddSingleton(Configuration);

            return Services;
        }
    }
}
