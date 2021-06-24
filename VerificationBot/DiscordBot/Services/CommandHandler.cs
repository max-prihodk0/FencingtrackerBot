using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using FencingtrackerBot.References;

namespace FencingtrackerBot.DiscordBot.Services
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient SocketClient;
        private readonly CommandService Commands;
        private readonly IConfigurationRoot Configuration;
        private readonly IServiceProvider Provider;

        public CommandHandler(DiscordSocketClient SocketClient, CommandService Commands, IConfigurationRoot Configuration, IServiceProvider Provider)
        {
            this.SocketClient = SocketClient;
            this.Commands = Commands;
            this.Configuration = Configuration;
            this.Provider = Provider;

            SocketClient.MessageReceived += OnMessageReceivedAsync;
        }

        private async Task OnMessageReceivedAsync(SocketMessage Message)
        {
            SocketUserMessage UserMessage = Message as SocketUserMessage;

            // Ensure that the message is from an actual user and not the bot or other bots
            if (UserMessage == null || UserMessage.Author.Id == SocketClient.CurrentUser.Id)
                return;
            
            if (UserMessage.Channel.Name.Contains(UserMessage.Author.Username))
            {
                if (DataStorage.ContainsEntry(UserMessage.Author.Id))
                {
                    bool leave = false;
                    EmbedBuilder Builder = new EmbedBuilder();

                    if (DataStorage.ManageEntry(UserMessage.Author.Id, UserMessage.Content))
                    {
                        Builder.AddField("Thank you!", $"You have been given access to the fencingtracker.com discord. Enjoy your stay!")
                                .WithColor(Color.LightGrey)
                                .WithFooter("fencingtracker.com")
                                .WithCurrentTimestamp();

                        await UserMessage.Author.SendMessageAsync(embed: Builder.Build());

                        await SocketClient.GetGuild(ulong.Parse(Configuration["discord:server"])).GetUser(UserMessage.Author.Id).AddRoleAsync(ulong.Parse(Configuration["discord:roles:verified"]));                                      
                        return;
                    }
                    else
                    {
                        if (DataStorage.ContainsEntry(UserMessage.Author.Id))
                        {
                            string AttemptOrAttempts = DataStorage.GetTries(UserMessage.Author.Id) == 1 ? "attempt" : "attempts"; 
                            Builder.AddField("Invalid Response", $"Sorry, the code that you just typed does not match the given captcha.\nYou have **{DataStorage.GetTries(UserMessage.Author.Id)}** {AttemptOrAttempts} remaining.")
                                .WithColor(Color.LightGrey)
                                .WithFooter("fencingtracker.com")
                                .WithCurrentTimestamp();
                        }
                        else
                        {
                            Builder.AddField("Invalid Response", $"Sorry, the code that you just typed does not match the given captcha.\nYou have **0** attempts remaining.\n\nPlease rejoin our server to obtain a new captcha.")
                                .WithColor(Color.LightGrey)
                                .WithFooter("fencingtracker.com")
                                .WithCurrentTimestamp();

                            leave = true;
                        }
                    }

                    await UserMessage.Author.SendMessageAsync(embed: Builder.Build());

                    if (leave)
                        await SocketClient.GetGuild(ulong.Parse(Configuration["discord:server"])).GetUser(UserMessage.Author.Id).KickAsync("Failed captia completion.");
                    return;
                }          
            }
            else
            {
                if (Utilities.CalculateSimilarity(UserMessage.Content, "what is this") == 0.9)
                {

                }
            }

            SocketCommandContext SocketContext = new SocketCommandContext(SocketClient, UserMessage);

            int Position = 0;
            if ((UserMessage.HasStringPrefix(Configuration["discord:prefix"], ref Position) || UserMessage.HasMentionPrefix(SocketClient.CurrentUser, ref Position)) 
                && (UserMessage.Channel.Id == ulong.Parse(Configuration["discord:channels:bot-commands"]) || UserMessage.Channel.Id == ulong.Parse(Configuration["discord:channels:verify"])))
            {
                IResult Result = await Commands.ExecuteAsync(SocketContext, Position, Provider);

                if (!Result.IsSuccess)
                {
                    await SocketContext.Channel.SendMessageAsync(embed: Utilities.MakeErrorEmbed($"The command \"**{UserMessage.Content.Substring(1)}**\" does not exist in this context."));
                }     
            }
        }
    }
}
