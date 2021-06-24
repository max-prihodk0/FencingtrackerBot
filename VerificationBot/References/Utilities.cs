using Discord;
using System;
using System.Collections.Generic;
using System.Text;

namespace FencingtrackerBot.References
{
    public class Utilities
    {
        public static Embed MakeErrorEmbed(string Message)
        {
            EmbedBuilder Builder = new EmbedBuilder();

            Builder.AddField("Error", Message + "\n ")
                        .WithColor(Color.Red)
                        .WithCurrentTimestamp()
                        .WithFooter("fencingtracker.com");

            return Builder.Build();
        }

        public static Embed MakeSuccessEmbed(string Message)
        {
            EmbedBuilder Builder = new EmbedBuilder();

            Builder.AddField("Success", Message + "\n ")
                        .WithColor(Color.Green)
                        .WithCurrentTimestamp()
                        .WithFooter("fencingtracker.com");

            return Builder.Build();
        }
    }
}
