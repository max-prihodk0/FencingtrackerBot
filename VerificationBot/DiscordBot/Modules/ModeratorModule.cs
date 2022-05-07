
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Timers;
using System;
using System.Threading.Tasks;
using FencingtrackerBot.References;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;

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

            await User.KickAsync(Reason);
        }

        [Command("bans")]
        [Summary("Bans the specified user from the server.")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        public async Task BanAsync(SocketGuildUser User, int Days, [Remainder]string Reason)
        {
            await ReplyAsync(embed: Utilities.MakeSuccessEmbed($"Successfuly banned {User.Mention} from the server."));

            await User.BanAsync(Days, Reason);
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
                Timer.Stop();
            };

            EmbedBuilder Builder = new EmbedBuilder();

            Builder.AddField("Muted", $"You have been muted in fencingtracker.com for ``{Reason}``. \nYou will be unmuted on **{Current.AddMilliseconds(Duration.TotalMilliseconds).ToString()}**.\nIf you this this is a mistake please contact a moderator.")
                       .WithColor(Color.LightGrey)
                       .WithCurrentTimestamp()
                       .WithFooter("fencingtracker.com");

            await User.SendMessageAsync(embed: Builder.Build());

            await ReplyAsync(embed: Utilities.MakeSuccessEmbed($"Successfuly muted {User.Mention} for {Duration.Days} {(Duration.Days != 1 ? "days" : "day")} {Duration.Hours} {(Duration.Hours != 1 ? "hours" : "hour")} {Duration.Minutes} {(Duration.Minutes != 1 ? "minutes" : "minute")}."));    
        }

        [Command("purge")]
        [Summary("Deletes an amount of messages from the specified channel.")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task PurgeChannelAsync(ISocketMessageChannel Channel, int Amount)
        {
            if (Amount <= 0)
            {
                await ReplyAsync(embed: Utilities.MakeErrorEmbed("The amount of messages to remove must be positive."));
                return;
            }

            var Messages = await Channel.GetMessagesAsync(Amount).FlattenAsync();
            var FilteredMessages = Messages.Where(x => (DateTimeOffset.UtcNow - x.Timestamp).TotalDays <= 14);

            int Count = FilteredMessages.Count();

            if (Count == 0)
            {
                await ReplyAsync(embed: Utilities.MakeErrorEmbed("There are no messages to delete."));
            }
            else
            {
                await (Channel as ITextChannel).DeleteMessagesAsync(FilteredMessages);
                await ReplyAsync(embed: Utilities.MakeSuccessEmbed($"Successfuly removed {Count} {(Count > 1 ? "messages" : "message")} from the <#{Channel.Id}> channel."));
            }
        }

        [Command("purge")]
        [Summary("Deletes all messages 14 days or younger from the specified channel.")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task PurgeAllChannelsAsync(ISocketMessageChannel Channel)
            => await PurgeChannelAsync(Channel, 9999);

        private readonly string[] numbers = new string[10]
        {
            "1️⃣",
            "2️⃣",
            "3️⃣",
            "4️⃣",
            "5️⃣",
            "6️⃣",
            "7️⃣",
            "8️⃣",
            "9️⃣",
            "🔟"
        };

        [Command("poll")]
        [Summary("Creates a poll with the specified question with a maximum of 10 options. The bot will post and notify everyone in the announcements channel.")]
        [RequireContext(ContextType.Guild)]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task CreatePollAsync([Remainder]string Message)
        {
            IList<string> options = new List<string>();

            // Parse input
            foreach (Match match in Regex.Matches(Message, "\"([^\"]*)\""))
                options.Add(match.ToString().Trim('"'));

            string name = options.First();
            options.RemoveAt(0);


            if (options.Count > 10)
            {
                await ReplyAsync(embed: Utilities.MakeErrorEmbed("You can only have a maximum of 10 options"));
                return;
            }

            string Description = name + "\n--------------------------------------------------------------------------\n\n**Options:**\n";

            for (int i = 0; i < options.Count; i++)
            {
                Description += $"{options[i]}\t{numbers[i]}  ";
            }

            var pollMessage = await (Context.Guild.GetChannel(ulong.Parse(Configuration["discord:channels:announcements"])) as ISocketMessageChannel).SendMessageAsync(embed: Utilities.MakeInfoImbed("Poll :ballot_box:", Description));

            for (int i = 0; i < options.Count; i++)
                await pollMessage.AddReactionAsync(new Emoji(numbers[i]));
        
        }
    }
}
