using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FencingtrackerBot.DiscordBot.Services
{
    public class LoggingService
    {
        private readonly DiscordSocketClient SocketClient;
        private readonly CommandService Commands;

        private string LogDirectory { get; }
        private string LogFile => LogDirectory + $"{DateTime.UtcNow.ToString("yyyy-MM-dd")}.txt";

        public LoggingService(DiscordSocketClient SocketClient, CommandService Commands)
        {
            LogDirectory = "./logs/";

            this.SocketClient = SocketClient;
            this.Commands = Commands;

            SocketClient.Log += OnLogAsync;
            Commands.Log += OnLogAsync;
        }

        private Task OnLogAsync(LogMessage Message)
        {
            if (!Directory.Exists(LogDirectory))
                Directory.CreateDirectory(LogDirectory);
            if (!File.Exists(LogFile))               
                File.Create(LogFile).Dispose();

            string Text = $"{DateTime.UtcNow.ToString("hh:mm:ss")} [{Message.Severity}] {Message.Source}: {Message.Exception?.ToString() ?? Message.Message}";
            File.AppendAllText(LogFile, Text + "\n");

            switch (Message.Severity)
            {
                case LogSeverity.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogSeverity.Critical:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogSeverity.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
            }

            Console.WriteLine(Text);
            Console.ForegroundColor = ConsoleColor.Gray;

            return Task.CompletedTask;
        }
    }
}
