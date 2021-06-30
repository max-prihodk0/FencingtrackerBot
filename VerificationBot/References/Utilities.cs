using Discord;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

        public static Embed MakeWarningEmbed(string Message)
        {
            EmbedBuilder Builder = new EmbedBuilder();

            Builder.AddField("Warning", Message + "\n ")
                        .WithColor(Color.LightOrange)
                        .WithCurrentTimestamp()
                        .WithFooter("fencingtracker.com");

            return Builder.Build();
        }

        public static string RemoveDuplicates(string Input)
        {
            return new string(new HashSet<char>(Input).ToArray());
        }

        public static bool FilterMessage(string Message) 
        {
            string[] Phrases = File.ReadAllText("./badwords.txt").Split(',');

            foreach (string Phrase in Phrases)
            {
                string New = Phrase.Trim();
                string[] WordsInMessage = Message.ToLower().Split(' ');

                if (!New.Contains(" "))
                {
                    for (int i = 0; i < WordsInMessage.Length; i++)
                    {
                        if (WordsInMessage[i].ToLower() == New || RemoveDuplicates(WordsInMessage[i].ToLower()) == New)
                            return true;
                    }
                }
                else
                {
                    string[] WordsInPhrase = New.Split(' ');
                    
                    for (int i = 0; i < WordsInMessage.Length; i++)
                    {
                        if (WordsInMessage[i] == WordsInPhrase[0])
                        {
                            if (Message.ToLower().Substring(i, Message.Length) == New
                                || RemoveDuplicates(Message.ToLower()).Substring(i, Message.Length) == New)
                                return true;
                        }

                    }
                }
            }

            return false;
        }
    }
}
