using Discord;
using Discord.Commands;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading.Tasks;
using FencingtrackerBot.References;

namespace FencingtrackerBot.DiscordBot.Modules
{
    [Name("Verification")]
    [RequireContext(ContextType.Guild)]
    public class VerificationModule : ModuleBase<SocketCommandContext>
    {
        private readonly IConfigurationRoot Configuration;

        public VerificationModule(IConfigurationRoot Configuration)
        {
            this.Configuration = Configuration;
        }

        [Command("verify")]
        [Summary("Verifies an unverified user to the server.")]
        public async Task VerifyAsync()
        {
            if (Context.Channel.Id == ulong.Parse(Configuration["discord:channels:verify"]))
            {
                Captcha Captcha = new Captcha();
                string FileName = Captcha.GenerateCaptcha();

                EmbedBuilder Builder = new EmbedBuilder();

                if (DataStorage.ContainsEntry(Context.Message.Author.Id))
                {
                    Builder.AddField("Verification System", $"{Context.Message.Author.Mention}, you already have a pending Captcha. Please check your DMs from this bot!")
                        .WithColor(Color.LightGrey)
                        .WithCurrentTimestamp()
                        .WithFooter("fencingtracker.com");

                    await Context.Message.Channel.SendMessageAsync(embed: Builder.Build());
                    return;
                }

                Builder.Title = "Welcome to fencingtracker.com!";

                Builder.AddField("Captcha", "Please complete the Captcha below to gain access to the server.")
                    .AddField("Why?", "We have set up this Captcha service to ensure that you are human and not a robot that could potentially harm our server.")
                    .WithColor(Color.LightGrey)
                    .WithFooter(footer => footer.Text = "fencingtracker.com")
                    .WithCurrentTimestamp()
                    .WithImageUrl($"attachment://{FileName}.jpg");

                await Context.Message.Author.SendFileAsync(FileName + ".jpg", embed: Builder.Build());

                File.Delete(FileName + ".jpg");

                DataStorage.AddEntry(Context.Message.Author.Id, Captcha.CaptchaName);
            }
        }
    }
}
