///////////////////////////////////////////////////////////////////////////////
//  AUTHOR          : Suresh Kalavala
//  DATE            : 02/02/2018
//  FILE            : HassBot.cs
//  DESCRIPTION     : A class that implements ~lmgtfy command
///////////////////////////////////////////////////////////////////////////////
using Discord.Commands;
using System.Text;
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

            // mention users if any
            string mentionedUsers = base.MentionedUsers();
            if (string.Empty != mentionedUsers) {
                foreach (string user in mentionedUsers.Split(' '))
                    if (string.Empty != user) {
                        string userHandle = user.Replace("!", string.Empty);
                        cmd = cmd.Replace(userHandle.Trim(), string.Empty);
                    }
            }
            StringBuilder message = new StringBuilder();

            // Make sure the generated URL is correct.
            string encoded = HttpUtility.UrlEncode(cmd.Trim());

            // Create the message
            message.Append($"Here {mentionedUsers}, try this:");
            message.Append("\n"); // New line
            message.Append($"<http://lmgtfy.com/?q={encoded}>");


            await base.CreateEmbed(Constants.EMOJI_POINT_UP, // Emoji to title
                                   Constants.LET_ME_GOOGLE, // Title
                                   message.ToString()); // Content of the message
        }
    }
}