﻿
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Timers;
using System;
using System.Threading.Tasks;
using FencingtrackerBot.References;
using Microsoft.Extensions.Configuration;

namespace FencingtrackerBot.DiscordBot.Modules
{
    [Name("Moderator")]
    [RequireContext(ContextType.Guild)]
    public class ModeratorModule : ModuleBase<SocketCommandContext>
    {
        private IConfigurationRoot Configuration;

        public ModeratorModule(IConfigurationRoot Configuration)
        {
            this.Configuration = Configuration;
        }

        [Command("kick")]
        [Summary("Kick the specified user from the server.")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        public async Task Kick(SocketGuildUser User, [Remainder]string Reason)
        {
            await ReplyAsync(embed: Utilities.MakeSuccessEmbed($"Successfuly kicked {User.Mention} from the server."));

            await User.KickAsync(reason: Reason);
        }

        [Command("mute")]
        [Summary("Mute the specified user for an amount of time.")]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        public async Task Mute(SocketGuildUser User, string Time, [Remainder]string Reason)
        {
            int Number = 0;
            if (!int.TryParse(Time.Substring(0, Time.Length - 1), result: out Number))
            {
                await ReplyAsync(embed: Utilities.MakeSuccessEmbed($"Unable to convert input ``{Time}`` to integer. Please make sure that your input does not contain any extra characters or letters."));
                return;
            }

            if (Number < 0)
            {
                await ReplyAsync(embed: Utilities.MakeSuccessEmbed($"Time must be greater than zero, you wrote: **{Number}**"));
                return;
            }

            char Type = Time[Time.Length - 1];

            TimeSpan Duration;

            switch (Type)
            {
                case 'm':
                    Duration = new TimeSpan(0, Number, 0);
                    break;
                case 'h':
                    Duration = new TimeSpan(Number, 0, 0);
                    break;
                case 'd':
                    Duration = new TimeSpan(Number, 0, 0, 0);
                    break;
                case 'w':
                    Duration = new TimeSpan(Number * 7, 0, 0, 0);
                    break;
                default:
                {
                    await ReplyAsync(embed: Utilities.MakeSuccessEmbed($"Unidentified character ``{Type}`` used for time."));
                    return;
                }
            }

            await User.AddRoleAsync(ulong.Parse(Configuration["discord:roles:muted"]));

            DateTime Current = DateTime.Now;

            Timer Timer = new Timer(Duration.TotalMilliseconds);
            Timer.Enabled = true;
            Timer.Elapsed += async (sender, e) => 
            {
                await User.RemoveRoleAsync(ulong.Parse(Configuration["discord:roles:muted"]));
            };

            EmbedBuilder Builder = new EmbedBuilder();

            Builder.AddField("Muted", $"You have been muted in fencingtracker.com for ``{Reason}``. \nYou will be unmuted on **{Current.AddMilliseconds(Duration.TotalMilliseconds).ToString()}**.\nIf you this this is a mistake please contact a moderator.")
                       .WithColor(Color.LightGrey)
                       .WithCurrentTimestamp()
                       .WithFooter("fencingtracker.com");

            await User.SendMessageAsync(embed: Builder.Build());

            await ReplyAsync(embed: Utilities.MakeSuccessEmbed($"Successfuly muted {User.Mention} for {Duration.Days} {(Duration.Days != 1 ? "days" : "day")} {Duration.Hours} {(Duration.Hours != 1 ? "hours" : "hour")} {Duration.Minutes} {(Duration.Minutes != 1 ? "minutes" : "minute")}."));    
        }
    }
}
