///////////////////////////////////////////////////////////////////////////////
//  AUTHOR          : Suresh Kalavala
//  DATE            : 02/02/2018
//  FILE            : WelcomeModule.cs
//  DESCRIPTION     : A class tha implements ~welcome command
///////////////////////////////////////////////////////////////////////////////
using Discord;
using Discord.Commands;
using HassBotData;
using HassBotUtils;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotLib
{
    public class WelcomeModule : BaseModule {

        [Command("welcome")]
        public async Task WelcomeAsync() {
            await WelcomeCommand();
        }

        [Command("welcome")]
        public async Task WelcomeAsync([Remainder]string cmd) {
            await WelcomeCommand();
        }

        [Command("welcome_message")]
        public async Task WelcomeMessageAsync()
        {
            await GetWelcomeMessage();
        }

        private async Task GetWelcomeMessage()
        {
            StringBuilder sb = new StringBuilder(512);

            sb.Append($"Hello! Welcome to Home Assistant Discord Channel.\n\n");
            string welcomeData = WelcomeMessage.Instance.Message;
            string[] lines = welcomeData.Split('\n');
            foreach (string line in lines)
            {
                if (line.StartsWith("//"))
                    continue;
                sb.Append(line);
                sb.Append("\n");
            }
            sb.Append(string.Format("Once again, Welcome to the Home Assistant Channel!\n\n"));

            // DM welcome message
            var dmChannel = await Context.User.GetOrCreateDMChannelAsync();
            await dmChannel.SendMessageAsync(sb.ToString());
        }

        private async Task WelcomeCommand() {

            string serverName = AppSettingsUtil.AppSettingsString(
                "discordServerName", true, string.Empty);
            string welcomerulesChannel = AppSettingsUtil.AppSettingsString(
                "welcomerulesChannel", false, string.Empty);

            string emoji = Constants.EMOJI_NAMASTE;
            string title = string.Format(Constants.WELCOME_MESSAGE, serverName);

            // mentioned users
            string mentionedUsers = base.MentionedUsers();

            string content = string.Format(Constants.WELCOME_RULES_MESSAGE,
                mentionedUsers, welcomerulesChannel);

            content += Constants.CODE_SHARING_MESSAGE;

            // Send response
            await Helper.CreateEmbed(
                Context, emoji, title, content);
        }
    }
}