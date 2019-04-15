///////////////////////////////////////////////////////////////////////////////
//  AUTHOR          : Suresh Kalavala
//  DATE            : 11/11/2018
//  FILE            : BreakingChanges.cs
//  DESCRIPTION     : A class that implements ~breaking_changes command
///////////////////////////////////////////////////////////////////////////////
using System.Threading.Tasks;
using Discord.Commands;

namespace DiscordBotLib
{
    public class BreakingChanges : BaseModule
    {
        [Command("breaking_changes")]
        public async Task BreakingChangesAsync()
        {
            await base.DisplayUsage(Constants.USAGE_BREAKINGCHANGES);
        }

        [Command("breaking_changes")]
        public async Task BreakingChangesAsync([Remainder]string version)
        {
            string emoji = null;
            string title = null;
            string body = null;

            // get the release notes of the specified version number
            string url = Helper.SitemapLookup("release-" + version);
            url = url.Replace("\n", string.Empty);

            if (url == string.Empty)
            {
                emoji = Constants.EMOJI_THUMBSDOWN;
                title = "Sorry!";
                body = $"No release changes found for {version}!";
            }
            else
            {
                if (url.EndsWith("/"))
                    url += "#breaking-changes";
                else
                    url += "/#breaking-changes";

                body = "<" + url + ">";
            }

            // Send response
            await Helper.CreateEmbed(Context, emoji, title, body, null, true);
        }
    }
}