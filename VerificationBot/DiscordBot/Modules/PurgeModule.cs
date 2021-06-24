using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FencingtrackerBot.References;

namespace FencingtrackerBot.DiscordBot.Modules
{
    [Name("Purge")]
    [RequireContext(ContextType.Guild)]
    public class PurgeModule : ModuleBase<SocketCommandContext>
    {
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

            IEnumerable<IMessage> Messages = await Channel.GetMessagesAsync(Amount).FlattenAsync();
            IEnumerable<IMessage> FilteredMessages = Messages.Where(x => (DateTimeOffset.UtcNow - x.Timestamp).TotalDays <= 14);

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
    }
}
