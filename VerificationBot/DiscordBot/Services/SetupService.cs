using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FencingtrackerBot.DiscordBot.Services
{
    public class SetupService
    {
        private readonly IConfigurationRoot Configuration;
        private readonly DiscordSocketClient SocketClient;
        private readonly IServiceProvider Provider;
        private readonly CommandService Commands;

        public SetupService(IServiceProvider Provider, CommandService Commands, IConfigurationRoot Configuration, DiscordSocketClient SocketClient)
        {
            this.Configuration = Configuration;
            this.SocketClient = SocketClient;
            this.Provider = Provider;
            this.Commands = Commands;
        }

        public async Task StartAsync()
        {
            // Retrieve the bot's discord token from the configuration file
            string DiscordToken = Configuration["discord:token"];

            if (string.IsNullOrWhiteSpace(DiscordToken))
                throw new Exception("The token field in \"config.json\" is empty.");

            await SocketClient.LoginAsync(TokenType.Bot, DiscordToken);

            // Call the ReadyAsync method that will setup our bot's appearence
            SocketClient.Ready += ReadyAsync;

            SocketClient.UserJoined += UserJoinedAsync;
            SocketClient.UserLeft += UserLeftAsync;

            // Start the socket client
            await SocketClient.StartAsync();

            await Commands.AddModulesAsync(Assembly.GetEntryAssembly(), Provider);
        }

        private async Task UserLeftAsync(SocketGuildUser User)
        {
            IMessageChannel Channel = SocketClient.GetChannel(ulong.Parse(Configuration["discord:channels:welcome"])) as IMessageChannel;
            await Channel.SendMessageAsync(text: $"Sadly, {User.Mention} has left the server. 😥");
        }

        private async Task UserJoinedAsync(SocketGuildUser User)
        { 
            IMessageChannel Channel = SocketClient.GetChannel(ulong.Parse(Configuration["discord:channels:welcome"])) as IMessageChannel;
            await Channel.SendMessageAsync(text: $"Welcome {User.Mention} to the server! 🎉");
        }

        private async Task ReadyAsync()
        {
            // All code that will be applied to the bot on startup
            await SocketClient.SetGameAsync("fencingtracker.com");
        }
    }
}
