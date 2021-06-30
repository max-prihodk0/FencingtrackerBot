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
using System.Timers;
using Discord.Rest;
using FencingtrackerBot.References.SQL;

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

        private async Task DirectMessageHandlerAsync(SocketUserMessage UserMessage)
        {
            Member Member = SQL.GetMember(UserMessage.Author.Id);

            if (Member.Verification != null)
            {
                bool leave = false;
                EmbedBuilder Builder = new EmbedBuilder();

                if (Member.Verification.Captcha == UserMessage.Content)
                {
                    Builder.AddField("Thank you!", $"You have been given access to the fencingtracker.com discord. Enjoy your stay!")
                            .WithColor(Color.LightGrey)
                            .WithFooter("fencingtracker.com")
                            .WithCurrentTimestamp();

                    await UserMessage.Author.SendMessageAsync(embed: Builder.Build());

                    await SocketClient.GetGuild(ulong.Parse(Configuration["discord:server"])).GetUser(UserMessage.Author.Id).AddRoleAsync(ulong.Parse(Configuration["discord:roles:verified"]));

                    SQL.Context.Verification.Remove(Member.Verification);
                    Member.Verification = null;
                    SQL.Context.SaveChanges();

                    return;
                }
                else
                {
                    Member.Verification.Tries--;

                    if (Member.Verification.Tries > 0)
                    {
                        string AttemptOrAttempts = Member.Verification.Tries == 1 ? "attempt" : "attempts";
                        Builder.AddField("Invalid Response", $"Sorry, the code that you just typed does not match the given captcha.\nYou have **{Member.Verification.Tries}** {AttemptOrAttempts} remaining.")
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

                SQL.Context.Verification.Remove(Member.Verification);
                Member.Verification = null;
                SQL.Context.SaveChanges();

                await UserMessage.Author.SendMessageAsync(embed: Builder.Build());

                if (leave)
                    await SocketClient.GetGuild(ulong.Parse(Configuration["discord:server"])).GetUser(UserMessage.Author.Id).KickAsync("Failed captia completion.");
            }
        }

        private async Task FilterMessageAsync(SocketUserMessage UserMessage, string Word)
        {
            if (Utilities.FilterMessage(Word))
            {
                await UserMessage.DeleteAsync();
                RestUserMessage Msg = await UserMessage.Channel.SendMessageAsync(embed: Utilities.MakeWarningEmbed($"{UserMessage.Author.Mention} watch your language! Try not to use swear words when communicating in this server. I know it's hard but I beleive in you! :heart:"));

                Timer Timer = new Timer(5000);
                Timer.Enabled = true;
                Timer.Elapsed += async (sender, e) =>
                {
                    await Msg.DeleteAsync();
                    Timer.Stop();
                };
            }
        }

        private async Task HandleUserMessageAsync(SocketUserMessage UserMessage)
        {
            await FilterMessageAsync(UserMessage, UserMessage.Content);

            Member Member = SQL.GetMember(UserMessage.Author.Id);
            Member.MessagesSent++;
            SQL.Context.SaveChanges();
        }

        private async Task OnMessageReceivedAsync(SocketMessage Message)
        {
            SocketUserMessage UserMessage = Message as SocketUserMessage;

            // Ensure that the message is from an actual user and not the bot or other bots
            if (UserMessage == null || UserMessage.Author.Id == SocketClient.CurrentUser.Id)
                return;

            if (Message.Channel is IDMChannel)
            {
                await DirectMessageHandlerAsync(UserMessage);
                return;
            }            

            SocketCommandContext SocketContext = new SocketCommandContext(SocketClient, UserMessage);

            int Position = 0;
            if ((UserMessage.HasStringPrefix(Configuration["discord:prefix"], ref Position) || UserMessage.HasMentionPrefix(SocketClient.CurrentUser, ref Position))
                && (UserMessage.Channel.Id == ulong.Parse(Configuration["discord:channels:bot-commands"])))
            {
                IResult Result = await Commands.ExecuteAsync(SocketContext, Position, Provider);

                if (!Result.IsSuccess)
                {
                    await SocketContext.Channel.SendMessageAsync(embed: Utilities.MakeErrorEmbed($"The command \"**{UserMessage.Content.Substring(1)}**\" does not exist in this context."));
                }
            }
            else
                await HandleUserMessageAsync(UserMessage);
        }
    }
}
