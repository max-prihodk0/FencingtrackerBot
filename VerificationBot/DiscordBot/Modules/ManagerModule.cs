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
using System.Security.Cryptography;

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

        [Command("system verify")]
        [Summary("Enables or disables the captcha verification system for the server.")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task StartAsync()
        {
            if (Configuration["discord:settings:verification"] == "true")
            {
                await DisableVerification();
                return;
            }

            JsonObject["discord"]["settings"]["verification"] = "true";
            File.WriteAllText("./config.json", JsonConvert.SerializeObject(JsonObject, Formatting.Indented));

            await ReplyAsync(embed: Utilities.MakeSuccessEmbed("Successfully enabled the verification system. If you would like to disable it, you can use the **disable verify** command."));
        }

        ulong MessageId = 0;
        public async Task DisableVerification()
        {
            var Message = await Context.Channel.SendMessageAsync(embed: Utilities.MakeInfoImbed("Conformation", "Are you sure you want to disable this feature? I do **not** recommend you disable this feature, as this will make your server vulnerable to malicious bot attacks."));

            Emoji Check = new Emoji("✅");
            Emoji Cross = new Emoji("❌");
            await Message.AddReactionsAsync(new IEmote[] { Check, Cross });
            MessageId = Message.Id;

            SocketClient.ReactionAdded += OnReactionAddedAsync;
        }

        [Command("system filter")]
        [Summary("Enables or disables the chat filter across all channels in the current guild.")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task StartAsync2()
        {
            if (Configuration["discord:settings:chat-filter"] == "true")
            {
                await DisableFilterAsync();
                return;
            }

            JsonObject["discord"]["settings"]["chat-filter"] = "true";
            File.WriteAllText("./config.json", JsonConvert.SerializeObject(JsonObject, Formatting.Indented));

            await ReplyAsync(embed: Utilities.MakeSuccessEmbed("Successfully enabled the chat filter. If you would like to disable it, you can use the **disable filter** command."));
        }

        ulong MessageId2 = 0;
        public async Task DisableFilterAsync()
        {
            var Message = await Context.Channel.SendMessageAsync(embed: Utilities.MakeInfoImbed("Conformation", "Are you sure you want to disable this feature?"));

            Emoji Check = new Emoji("✅");
            Emoji Cross = new Emoji("❌");
            await Message.AddReactionsAsync(new IEmote[] { Check, Cross });
            MessageId2 = Message.Id;

            SocketClient.ReactionAdded += OnReactionAddedAsync;
        }

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
            else if (MessageId2 == Message.Id)
            {
                if (Reaction.Emote.Name != "✅")
                    return;

                await ReplyAsync(embed: Utilities.MakeSuccessEmbed("Successfully disabled the chat filter. If you would like to re-enable the system, you can use the **enable filter** command."));

                JsonObject["discord"]["settings"]["chat-filter"] = "false";
                File.WriteAllText("./config.json", JsonConvert.SerializeObject(JsonObject, Formatting.Indented));
            }
        }
    }
}
