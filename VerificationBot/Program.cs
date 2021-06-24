using System;
using System.Threading.Tasks;
using FencingtrackerBot.DiscordBot;

namespace FencingtrackerBot
{
    class Program
    {
        static async Task Main(string[] args)
        {
            BotSetup Bot = new BotSetup();

            await Bot.RunSetupAsync();
        }
    }
}
