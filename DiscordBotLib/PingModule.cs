///////////////////////////////////////////////////////////////////////////////
//  AUTHOR          : Suresh Kalavala
//  DATE            : 02/02/2018
//  FILE            : Ping.cs
//  DESCRIPTION     : A class that implements ping/pong commands
///////////////////////////////////////////////////////////////////////////////
using Discord;
using Discord.Commands;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBotLib
{
    public class PingModule : BaseModule {

        [Command("ping"), Alias("pong")]
        public async Task PingAsync() {
            Helper LocalHelper = new Helper();
            string response = string.Empty;
            string request = Context.Message.Content.ToLower();
            request = request.Replace("~", string.Empty).Replace(".", string.Empty);

            if (request == "ping")
                response = "PONG!";
            else if (request == "pong")
                response = "PING!!!";
            if (string.Empty == response)
                return;

            await LocalHelper.CreateEmbed(
                Context,
                Constants.EMOJI_PING_PONG, // Emoji to title
                request + "?", // Title
                response); // Content of the message
        }
    }
}