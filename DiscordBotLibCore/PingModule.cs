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
            string emoji = Constants.EMOJI_PING_PONG;
            string title = "{0}?";
            string body = null;

            string request = Context.Message.Content.ToLower();
            request = request.Replace("~", string.Empty).Replace(".", string.Empty);

            if (request == "ping")
                body = "PONG!";
            else if (request == "pong")
                body = "PING!!!";
            if (body == null)
                return;

            // Set title
            title = string.Format(title, request);

            // Send response
            await Helper.CreateEmbed(Context, emoji, title, body, null, true);
        }
    }
}