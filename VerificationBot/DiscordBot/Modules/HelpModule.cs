using Discord;
using Discord.Commands;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace FencingtrackerBot.DiscordBot.Modules
{
    [Name("Help")]
    public class HelpModule : ModuleBase<SocketCommandContext>
    {
        private readonly CommandService Commands;
        private readonly IConfigurationRoot Configuration;

        public HelpModule(CommandService Commands, IConfigurationRoot Configuration)
        {
            this.Commands = Commands;
            this.Configuration = Configuration;
        }

        [Command("help")]
        public async Task HelpAsync()
        {
            EmbedBuilder Builder = new EmbedBuilder()
                .WithTitle("Commands")
                .WithDescription("Here are all the commands avalible in the server. To get specific details about a command type **!help** ``<command>``.\r")
                .WithColor(Color.LightGrey)
                .WithCurrentTimestamp()
                .WithFooter("fencingtracker.com");

            foreach(ModuleInfo Module in Commands.Modules)
            {
                if (Module.Name == "Help")
                    continue;

                string Description = "";
                foreach (CommandInfo Command in Module.Commands)
                {
                    PreconditionResult Result = await Command.CheckPreconditionsAsync(Context);
                    if (Result.IsSuccess)
                    {
                        if (Command.Name == "poll")
                            Description += "!poll \"<message>\" \"<choice 1>\" \"<choice 2>\"";
                        else
                        {
                            Description += Configuration["discord:prefix"] + Command.Name;

                            foreach (ParameterInfo Parameter in Command.Parameters)
                            {
                                Description += " <" + Parameter.Name.ToLower() + ">";
                            }
                        }

                        Description += '\n';
                    }    
                }
                if (!string.IsNullOrWhiteSpace(Description))
                    Builder.AddField(Module.Name, Description);
            }

            await ReplyAsync(embed: Builder.Build());
        }

        [Command("help")]
        public async Task HelpAsync(string Command)
        {
            SearchResult Result = Commands.Search(Context, Command);
            EmbedBuilder Builder = new EmbedBuilder();

            if (!Result.IsSuccess)
            {    
                Builder.AddField("Error", $"Sorry, I could not find the command {Command} or anything similar to it.\n ")
                    .WithColor(Color.Red)
                    .WithCurrentTimestamp()
                    .WithFooter(footer => footer.Text = "fencingtracker.com");

                await ReplyAsync(embed: Builder.Build());
                return;
            }

            Builder.WithTitle("Commands")
                .WithDescription($"Here are some commands similar to **{Command}**.\r")
                .WithColor(Color.LightGrey)
                .WithCurrentTimestamp()
                .WithFooter("fencingtracker.com");

            foreach (CommandMatch Match in Result.Commands)
            {
                CommandInfo Cmd = Match.Command;

                string Description = null;

                foreach (ParameterInfo Parameter in Cmd.Parameters)
                {
                    Description += " <" + Parameter.Name.ToLower() + ">";
                }

                Builder.AddField(Cmd.Name + Description, Cmd.Summary);
            }

            await ReplyAsync(embed: Builder.Build());
        }
    }
}
