using Discord;
using Discord.Commands;
using Discord.WebSocket;
using FencingtrackerBot.References;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.IO;
using Newtonsoft.Json;

namespace FencingtrackerBot.DiscordBot.Modules
{
    [Name("Manager")]
    [RequireContext(ContextType.Guild)]
    public class ManagerModule : ModuleBase<SocketCommandContext>
    {
        private readonly IConfigurationRoot Configuration;
        private readonly DiscordSocketClient SocketClient;
        private dynamic JsonObject;

        public ManagerModule(IConfigurationRoot Configuration, DiscordSocketClient SocketClient)
        {
            this.Configuration = Configuration;
            this.SocketClient = SocketClient;

            JsonObject = JsonConvert.DeserializeObject(File.ReadAllText("./config.json"));
        }

        [Command("enable verify")]
        [Description("Enables the captcha verification system for the server.")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task EnableVerification()
        {
            if (Configuration["discord:settings:verification"] == "true")
            {
                await ReplyAsync(embed: Utilities.MakeErrorEmbed("Oops! It looks like the verification system is already enabled!"));
                return;
            }

            JsonObject["discord"]["settings"]["verification"] = "true";
            File.WriteAllText("./config.json", JsonConvert.SerializeObject(JsonObject, Formatting.Indented));
        }

        ulong MessageId = 0;

        [Command("disable verify")]
        [Description("Disables the captcha verification system for the server **(not recommended)**.")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task DisableVerification()
        {
            if (Configuration["discord:settings:verification"] == "false")
            {
                await ReplyAsync(embed: Utilities.MakeErrorEmbed("Oops! It looks like the verification system is already disabled!"));
                return;
            }

            var Message = await Context.Channel.SendMessageAsync(embed: Utilities.MakeInfoImbed("Conformation", "Are you sure you want to disable this feature? I do **not** recommend you disable this feature, as this will make your server vulnerable to malicious bot attacks."));

            Emoji Check = new Emoji("✅");
            Emoji Cross = new Emoji("❌");
            await Message.AddReactionsAsync(new IEmote[] { Check, Cross });
            MessageId = Message.Id;

            SocketClient.ReactionAdded += OnReactionAddedAsync;
        }

        /*[Command("enable filter")]
        [Description("Enables the chat filter across all channels in the current guild.")]
        public async Task EnableFilterAsync()
        {

        }*/

        private async Task OnReactionAddedAsync(Cacheable<IUserMessage, ulong> Message, ISocketMessageChannel Channel, SocketReaction Reaction)
        {
            if (MessageId == Message.Id)
            {
                if (Reaction.Emote.Name != "✅")
                    return;

                await ReplyAsync(embed: Utilities.MakeSuccessEmbed("Successfully disabled the verification system. If you would like to re-enable the system, you can use the **enable verify** command."));

                JsonObject["discord"]["settings"]["verification"] = "false";
                File.WriteAllText("./config.json", JsonConvert.SerializeObject(JsonObject, Formatting.Indented));
            }
        }
    }
}
