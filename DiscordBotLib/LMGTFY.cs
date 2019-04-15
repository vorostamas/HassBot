///////////////////////////////////////////////////////////////////////////////
//  AUTHOR          : Suresh Kalavala
//  DATE            : 02/02/2018
//  FILE            : HassBot.cs
//  DESCRIPTION     : A class that implements ~lmgtfy command
///////////////////////////////////////////////////////////////////////////////
using Discord.Commands;
using System.Threading.Tasks;
using System.Web;

namespace DiscordBotLib
{

    public class LMGTFY : BaseModule {

        [Command("lmgtfy")]
        public async Task LetMeGoogleThatForYouAsync() {
            await base.DisplayUsage(Constants.USAGE_LMGTFY);
        }

        [Command("lmgtfy")]
        public async Task LetMeGoogleThatForYouAsync([Remainder]string cmd) {

            string emoji = Constants.EMOJI_POINT_UP;
            string title = Constants.LET_ME_GOOGLE;
            string body = null;

            // mention users if any
            string mentionedUsers = base.MentionedUsers();
            if (string.Empty != mentionedUsers) {
                foreach (string user in mentionedUsers.Split(' '))
                    if (string.Empty != user) {
                        string userHandle = user.Replace("!", string.Empty);
                        cmd = cmd.Replace(userHandle.Trim(), string.Empty);
                    }
            }
            
            // Make sure the generated URL is correct.
            string encoded = HttpUtility.UrlEncode(cmd.Trim());

            // Create the message
            body = $"Here {mentionedUsers}, try this:";
            body += "\n"; // New line
            body += $"<http://lmgtfy.com/?q={encoded}>";

            // Send response
            await Helper.CreateEmbed(Context, emoji, title, body, null, true);
        }
    }
}