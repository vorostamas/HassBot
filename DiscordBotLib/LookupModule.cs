///////////////////////////////////////////////////////////////////////////////
//  AUTHOR          : Suresh Kalavala
//  DATE            : 02/02/2018
//  FILE            : LookupModule.cs
//  DESCRIPTION     : A class that implements ~lookup command
//                    It uses sitemap data to lookup
///////////////////////////////////////////////////////////////////////////////
using Discord;
using Discord.Commands;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using HassBotData;
using System;

namespace DiscordBotLib
{
    public class LookupModule : BaseModule {

        [Command("lookup")]
        public async Task LookupAsync() {
            await base.DisplayUsage(Constants.USAGE_LOOKUP);
        }

        [Command("deepsearch")]
        public async Task DeepSearchAsync() {
            await base.DisplayUsage(Constants.USAGE_DEEPSEARCH);
        }

        [Command("lookup")]
        public async Task LookupAsync([Remainder]string input) {
            await LookupCommand(input);
        }

        private async Task LookupCommand(string input) {

            string emoji = String.Empty;
            string title = String.Empty;
            string body = String.Empty;

            string result = Helper.SitemapLookup(input);
            result = result.Trim();

            var embed = new EmbedBuilder();
            if (result == string.Empty) {
                emoji = ":frowning:";
                title = string.Format("Searched for '{0}': ", input);
                body = string.Format("Couldn't find it!\n\nYou may try `~deepsearch {0}`.", input);
            }
            else {
                emoji = ":smile:";
                title = "Here is what I found:";
                body = result;
            }

            // Send response
            await CreateEmbed(Context, emoji, title, body, null, true);
        }

        [Command("deepsearch")]
        public async Task DeepSearchAsync([Remainder]string input) {
            XmlDocument doc = Sitemap.SiteMapXmlDocument;

            StringBuilder sb = new StringBuilder();
            foreach (XmlNode item in doc.DocumentElement.ChildNodes) {
                if (item.InnerText.Contains(input.Split(' ')[0])) {
                    sb.Append("<" + item.FirstChild.InnerText + ">\n");
                }
            }

            string result = string.Format("Deepsearch result:\n{0}", sb.ToString());

            if (result.Length > 1900) {
                result = result.Substring(0, 1850);
                result += "...\n\nThe message is truncated because it is too long. You may want to change the search criteria.";
            }

            // Send a Direct Message to the User with search information
            var dmChannel = await Context.User.GetOrCreateDMChannelAsync();
            if (result.Length == 0)
            {
                result = $"Deepsearch result: No results found for '{input}'";
            }

            await dmChannel.SendMessageAsync(result);
        }
    }
}