///////////////////////////////////////////////////////////////////////////////
//  AUTHOR          : Suresh Kalavala
//  DATE            : 02/02/2018
//  FILE            : AboutModule.cs
//  DESCRIPTION     : A class that implements ~about command
///////////////////////////////////////////////////////////////////////////////
using System.Threading.Tasks;
using Discord.Commands;
using System;
using System.Diagnostics;
using System.Linq;
using Discord;
using System.Collections.Generic;

namespace DiscordBotLib
{
    public class AboutModule : BaseModule {
        private Process _process;

        public AboutModule() {
            _process = Process.GetCurrentProcess();
        }

        [Command("about")]
        public async Task About() {
            await AboutCommand();
        }

        [Command("about")]
        public async Task About([Remainder]string cmd) {
            await AboutCommand();
        }

        private async Task AboutCommand() {
            var inline = new List<Tuple<string, string>>();
            string emoji = ":wave:";
            string title = "Hello! This is @HassBot, written by @skalavala";
            string body = "You can find the source code here https://github.com/skalavala/HassBot";

            inline.Add(new Tuple<string, string>("Up Since", $"{ GetUptime() }"));
            inline.Add(new Tuple<string, string>("Total Users", $"{Context.Client.Guilds.Sum(g => g.Users.Count)}"));
            inline.Add(new Tuple<string, string>("Heap Size", $"{GetHeapSize()} MiB"));
            inline.Add(new Tuple<string, string>("Memory", $"{ GetMemoryUsage() }"));
            inline.Add(new Tuple<string, string>("Discord Lib Version", $"{ GetLibrary() }"));
            inline.Add(new Tuple<string, string>("Latency", $" { GetLatency() }"));
            inline.Add(new Tuple<string, string>("Maintainers", " @skalavala and @ludeeus"));

            // mention users if any
            string mentionedUsers = base.MentionedUsers();
            if (string.Empty != mentionedUsers)
                body = string.Format("FYI {0} \n", mentionedUsers) + body;

            // Send response
            await Helper.CreateEmbed(Context, emoji, title, body, inline, true);
        }

        public string GetUptime() {
            var uptime = (DateTime.Now - _process.StartTime);
            return $"{uptime.Days}d {uptime.Hours}h {uptime.Minutes}m {uptime.Seconds}s";
        }

        private static string GetHeapSize()
            => Math.Round(GC.GetTotalMemory(true) / (1024.0 * 1024.0), 2).ToString();

        public string GetLibrary()
            => $"Discord.Net ({DiscordConfig.Version})";

        public string GetMemoryUsage()
            => $"{Math.Round(GC.GetTotalMemory(true) / (1024.0 * 1024.0), 2)}mb";

        public string GetLatency()
            => $"{Context.Client.Latency}ms";
    }
}