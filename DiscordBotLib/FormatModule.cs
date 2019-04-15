///////////////////////////////////////////////////////////////////////////////
//  AUTHOR          : Suresh Kalavala
//  DATE            : 02/02/2018
//  FILE            : FormatModule.cs
//  DESCRIPTION     : A class that implements ~format command
///////////////////////////////////////////////////////////////////////////////
using Discord.Commands;
using System.Threading.Tasks;

namespace DiscordBotLib
{
    public class FormatModule : BaseModule {
        [Command("format")]
        public async Task FormatAsync() {
            await FormatCommand();
        }

        [Command("format")]
        public async Task FormatAsync([Remainder]string cmd) {
            await FormatCommand();
        }

        private async Task FormatCommand() {
            string emoji = null;
            string title = null;
            string body = null;

            // mention users if any
            string mentionedUsers = base.MentionedUsers();

            if (mentionedUsers.Trim() != string.Empty )
                body += mentionedUsers + " ";

            body += "To format your text as code, enter three backticks on the first line, press Enter for a new line, paste your code, press Enter again for another new line, and lastly three more backticks. Here's an example:\n\n";
            body += "\\`\\`\\`\n";
            body += "code here\n";
            body += "\\`\\`\\`\n";
            body += "Watch the animated gif here: <https://bit.ly/2GbfRJE>\n";
            body += "**DO NOT** repeat posts. Please edit previously posted message, here is how -> <https://bit.ly/2qOOf1G>";

            // Send response
            await Helper.CreateEmbed(Context, emoji, title, body, null, true);
        }
    }
}