﻿using Discord;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using HassBotUtils;
using HassBotDTOs;
using HassBotData;

using Discord.WebSocket;
using Discord.Commands;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace DiscordBotLib
{
    public class Helper
    {

        private static readonly string YAML_START = @"```yaml";
        private static readonly string YAML_END = @"```";
        private static readonly string JSON_START = @"```json";
        private static readonly string JSON_END = @"```";
        private static readonly string GOOD_EMOJI = "✅";
        private static readonly string BAD_EMOJI = "❌";

        private static readonly log4net.ILog logger =
                    log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static Color GetRandomColor()
        {
            Random rnd = new Random();
            Color[] colors = { Color.Blue, Color.DarkBlue, Color.DarkerGrey,
                               Color.DarkGreen, Color.DarkGrey, Color.DarkMagenta,
                               Color.DarkOrange, Color.DarkPurple, Color.DarkRed,
                               Color.DarkTeal, Color.Gold, Color.Green,
                               Color.LighterGrey, Color.LightGrey,
                               Color.LightOrange, Color.Magenta, Color.Orange,
                               Color.Purple, Color.Red, Color.Teal };

            int r = rnd.Next(colors.Count());
            return colors[r];
        }

        public static Task LogMessage(LogMessage message)
        {
            if (message.Exception is CommandException cmdEx)
            {
                logger.Error($"{cmdEx.GetBaseException().GetType()} was thrown while executing {cmdEx.Command.Aliases.First()} in {cmdEx.Context.Channel} by {cmdEx.Context.User}.");
            }

            switch (message.Severity)
            {
                case LogSeverity.Critical:
                    logger.Fatal(message.Message);
                    break;
                case LogSeverity.Error:
                    logger.Error(message.Message);
                    break;
                case LogSeverity.Warning:
                    logger.Warn(message.Message);
                    break;
                case LogSeverity.Info:
                    logger.Info(message.Message);
                    break;
                case LogSeverity.Verbose:
                case LogSeverity.Debug:
                    logger.Debug(message.Message);
                    break;
            }
            return Task.CompletedTask;
        }

        public static async Task FilterBotMessages(SocketUserMessage message,
                                                       SocketCommandContext context,
                                                       SocketGuildChannel channel)
        {
            // A list of channels to monitor
            List<string> channelfilter = new List<string>() {"github", "circleci", "netlify" };

            if (null == channel || !channelfilter.Contains(channel.Name) || message.Embeds.Count <= 0)
                return;

            // A list of bots to monitor
            List<string> botfilter = new List<string>() { "houndci-bot" };

            // check if the sender is a known bot before deleting.
            foreach (Embed e in message.Embeds)
            {
                EmbedAuthor author = (EmbedAuthor)e.Author;
                if (botfilter.Contains(author.ToString()) || author.ToString().EndsWith("[bot]"))
                {
                    await context.Message.DeleteAsync();
                }
            }
        }

        public static async Task ProcessSubscriptions(SocketUserMessage message,
                                               SocketCommandContext context,
                                               SocketGuildChannel channel)
        {

            // only interested in processing incoming messages to the #github channel
            if (null == channel || channel.Name != "github" || message.Embeds.Count <= 0)
                return;

            // #github channel contains messages from many different sources. 
            foreach (Embed e in message.Embeds)
            {
                List<string> uniqueTags = SubscriptionManager.TheSubscriptionManager.GetDistinctTags();
                List<SubscribeDTO> uniqueUsers = new List<SubscribeDTO>(8);
                List<string> matchedTags = new List<string>(32);
                List<string> matchedLocations = new List<string>(32);

                // first get all the unique tags across all users and check if any of the tags match
                // if there is a match, keep a list of those tags
                foreach (string tag in uniqueTags)
                {
                    if (e.Description.ToLower().Contains(tag))
                    {
                        matchedLocations.Add("Description");
                        matchedTags.Add(tag);
                    }
                    if (e.Url.ToLower().Contains(tag))
                    {
                        matchedLocations.Add("Url");
                        matchedTags.Add(tag);
                    }
                    if (e.Author.ToString().ToLower().Contains(tag))
                    {
                        matchedLocations.Add("Author");
                        matchedTags.Add(tag);
                    }
                    if (e.Title.ToLower().Contains(tag))
                    {
                        matchedLocations.Add("Title");
                        matchedTags.Add(tag);
                    }
                }

                // filter out any duplicates
                matchedTags = matchedTags.Distinct().ToList();
                matchedLocations = matchedLocations.Distinct().ToList();

                // if there are matched tags, get unique list of users that are interested in those tags
                if (matchedTags.Count > 0)
                {
                    foreach (string tag in matchedTags)
                    {
                        uniqueUsers.AddRange(SubscriptionManager.TheSubscriptionManager.GetSubscribersByTag(tag));
                    }

                    // now that we have list of users that are interested in the tag, and the tags match in description/author name/title
                    // send the url of the message to each of the subscriber using a DM
                    foreach (SubscribeDTO usr in uniqueUsers.Distinct().ToList())
                    {
                        string msg = string.Format("Subscription Alert: Found '{0}' in the '{1}' of the PR/Issue: {2}",
                                                    GetListAsCommaSeparated(matchedTags),
                                                    GetListAsCommaSeparated(matchedLocations),
                                                    e.Url);

                        var dmChannel = await HAChannels.GetUsersDMChannel(context, usr.Id);
                        await dmChannel.SendMessageAsync(msg);
                    }
                }
            }
        }

        private static string GetListAsCommaSeparated(List<string> items)
        {
            if (items == null || items.Count == 0)
                return "none!";

            StringBuilder buffer = new StringBuilder();

            for (int i = 0; i < items.Count; i++)
            {
                if (i == 0) buffer.Append("[ ");
                buffer.Append(items[i]);
                if (i + 1 == items.Count)
                    buffer.Append(" ]");
                else
                    buffer.Append(", ");
            }

            return buffer.ToString();
        }

        public static async Task ReactToYaml(string content, SocketCommandContext context)
        {
            if (!(content.Contains(YAML_START) || content.Contains(YAML_END)))
                return;

            int start = content.IndexOf(YAML_START);
            int end = content.IndexOf(YAML_END, start + 3);

            if (start == -1 || end == -1 || end == start)
                return;

            string errMsg = string.Empty;
            string substring = content.Substring(start, (end - start));
            bool yamlCheck = YamlValidator.ValidateYaml(substring, out errMsg);
            if (yamlCheck)
            {
                var okEmoji = new Emoji(GOOD_EMOJI);
                await context.Message.AddReactionAsync(okEmoji);
            }
            else
            {
                var errorEmoji = new Emoji(BAD_EMOJI);
                await context.Message.AddReactionAsync(errorEmoji);
            }
        }

        public static async Task ReactToJson(string content, SocketCommandContext context)
        {
            if (!(content.Contains(JSON_START) || content.Contains(JSON_END)))
                return;

            int start = content.IndexOf(JSON_START);
            int end = content.IndexOf(JSON_END, start + 3);

            if (start == -1 || end == -1 || end == start)
                return;

            string errMsg = string.Empty;
            string substring = content.Substring(start, (end - start));
            bool jsonCheck = JSONValidator.ValidateJson(substring, out errMsg);
            if (jsonCheck)
            {
                var okEmoji = new Emoji(GOOD_EMOJI);
                await context.Message.AddReactionAsync(okEmoji);
            }
            else
            {
                var errorEmoji = new Emoji(BAD_EMOJI);
                await context.Message.AddReactionAsync(errorEmoji);
            }
        }

        public static async Task CheckBlockedDomains(string content, SocketCommandContext context)
        {
            List<BlockedDomainDTO> blockedDomains = BlockedDomains.Instance.Domains();
            foreach (BlockedDomainDTO domain in blockedDomains)
            {
                if (content.Contains(domain.Url))
                {
                    if (domain.Ban == true)
                    {
                        // exclude Mods from the bans
                        if (IsMod(context.User))
                            return;

                        // Ban the user
                        string reason = string.Format(Constants.BAN_MESSAGE, context.User.Username, domain.Reason);
                        await context.Guild.AddBanAsync(context.User, 1, reason, null);

                        // post a message in the channel about the permanent ban
                        await context.Message.Channel.SendMessageAsync("BAM!!! " + reason);

                        // send a message to #botspam channel as well
                        string detailedMessage = "Woohoo! " + reason + " Posted message: " + content;
                        await HAChannels.ModLogChannel(context).SendMessageAsync(detailedMessage, false, null);
                    }
                    else
                    {
                        string maskedUrl = domain.Url.Replace(".", "_dot_");
                        // DM the message to the user, so that they can copy/paste without domain name/links
                        // save time, so that tey don't have to re-type the whole message :)
                        var dmChannel = await context.User.GetOrCreateDMChannelAsync();
                        await dmChannel.SendMessageAsync(string.Format(Constants.USER_MESSAGE_BLOCKED_URL, maskedUrl, domain.Reason, content));

                        // delete the message
                        await context.Message.DeleteAsync();

                        // show message
                        string msg = string.Format(Constants.ERROR_BLOCKED_URL, context.User.Mention, maskedUrl, domain.Reason);
                        await context.Message.Channel.SendMessageAsync(msg);
                    }
                }
            }
        }

        public static async Task ChangeNickName(DiscordSocketClient client,
                                                SocketCommandContext context)
        {
            // Change Nick Name 💎
            // Get the Home Assistant Server Guild
            ulong serverGuild = (ulong)AppSettingsUtil.AppSettingsLong("serverGuild", true, 330944238910963714);
            var guild = client.GetGuild(serverGuild);
            if (null == guild)
                return;

            var user = guild.GetUser(context.User.Id);
            if (user.Nickname.Contains("🔹"))
            {
                await user.ModifyAsync(
                    x => {
                        string newNick = user.Nickname.Replace("🔹", string.Empty);
                        x.Nickname = newNick;
                    }
                );
            }
        }

        public static string SitemapLookup(string searchString)
        {
            string[] searchWords = null;
            StringBuilder sb = new StringBuilder();
            XmlDocument doc = Sitemap.SiteMapXmlDocument;

            searchString = searchString.Replace('.', ' ').Replace('_', ' ').Replace('-', ' ').ToLower();
            if (searchString.Contains(" "))
                searchWords = searchString.Split(' ');
            else
                searchWords = new string[] { searchString };

            if (null == searchWords)
                return string.Empty;

            Array.Sort(searchWords);

            foreach (XmlNode item in doc.DocumentElement.ChildNodes)
            {
                string location = string.Empty;
                string[] sitemapWords = null;

                string loc = item.FirstChild.InnerText;
                if (loc.EndsWith("/"))
                {
                    int index = loc.LastIndexOf("/", (loc.Length - 2));
                    location = loc.Substring((index) + 1, ((loc.Length - index) - 2));
                }
                else
                {
                    int index = loc.LastIndexOf("/", loc.Length);
                    location = loc.Substring((index) + 1, ((loc.Length - index) - 1));
                }

                location = location.Replace('.', ' ').Replace('_', ' ').Replace('-', ' ').ToLower();
                if (location.Contains(" "))
                    sitemapWords = location.Split(' ');
                else
                    sitemapWords = new string[] { location };

                if (null == sitemapWords)
                    continue;

                Array.Sort(sitemapWords);
                if (string.Join("", searchWords) == string.Join("", sitemapWords))
                {
                    sb.Append(item.FirstChild.InnerText);
                    sb.Append("\n");
                }
            }

            return sb.ToString();
        }

        public static async Task RefreshData(SocketCommandContext context)
        {
            string emoji = Constants.EMOJI_THUMBSUP;
            string title = "Success";
            string body = Constants.COMMAND_REFRESH_SUCCESSFUL;

            try
            {
                Sitemap.ReloadData();
                WelcomeMessage.ReloadData();
                BlockedDomains.ReloadData();
                HassBotCommands.ReloadData();
            }
            catch
            {
                emoji = Constants.EMOJI_FAIL;
                title = "Failed";
                body = Constants.COMMAND_REFRESH_FAILED;
            }

            // Send response
            await Helper.CreateEmbed(context, emoji, title, body, forceremoveoriginalmessage:true);
        }

        public static bool IsMod(SocketUser user)
        {
            // get the list of mods from config file
            string mods = AppSettingsUtil.AppSettingsString("mods",
                                                             true,
                                                             string.Empty);
            string[] moderators = mods.Split(',');
            var results = Array.FindAll(moderators,
                                        s => s.Trim().Equals(user.Username,
                                        StringComparison.OrdinalIgnoreCase));
            if (results.Length == 1)
                return true;
            else
                return false;
        }

        public static async Task<bool> VerifyMod(SocketCommandContext ctx)
        {
            if (!Helper.IsMod(ctx.User))
            {
                var embed = new EmbedBuilder()
                {
                    Title = Constants.EMOJI_STOPSIGN,
                    Color = Color.DarkRed,
                };
                embed.AddField(Constants.ACCESS_DENIED,
                                     Constants.ACCESS_DENIED_MESSAGE);

                await ctx.Channel.SendMessageAsync(string.Empty, false, embed.Build());
                return false;
            }
            return true;
        }

        /// <summary>
        ///     Deletes the message that triggered the bot, if the message did not contain any extra words.
        /// </summary>
        /// <param name="context">The context of the message that triggered the bot to react.</param>
        /// <param name="forceremoveoriginalmessage">Flag to indicate that the command post allways should be deleted, if false the logic in this method aplies.</param>
        public static async Task DeleteMessage(SocketCommandContext context, bool forceremoveoriginalmessage)
        {
            if (forceremoveoriginalmessage)
            {
                logger.Debug("Deleting command message " + context.Message + " from " + context.User.Username + " in " + context.Channel.Name);
                await context.Message.DeleteAsync();
            }
            else
            {
                string leftovercontent = context.Message.Content.ToString();
                string invokedcommand = leftovercontent.Split(' ')[0];
                leftovercontent = leftovercontent.Replace(invokedcommand, "");
                leftovercontent = leftovercontent.Replace(" ", "");

                if (context.Message.MentionedUsers.ToString().Length != 0)
                {
                    foreach (var item in context.Message.MentionedUsers)
                    {
                        leftovercontent = leftovercontent.Replace($"<@{item.Id}>", "");
                    }
                }

                if (leftovercontent.Length != 0)
                {
                    // logger.Debug("Message had extra content, skipping delete.");
                }
                else
                {
                    logger.Debug(context.User.Username + " invoked command " + context.Message + " in " + context.Channel.Name);

                    await context.Message.DeleteAsync();
                }
            }
        }

        /// <summary>
        ///     Post a nice looking embeded post in as a response from the bot.
        /// </summary>
        /// <param name="context">The context of the message that triggered the bot to react.</param>
        /// <param name="emoji">Emoji for the embedded post, this is inserted before the title.</param>
        /// <param name="title">Title for the embedded post.</param>
        /// <param name="content">Content(body) for the embedded post.</param>
        /// <param name="inline">Special inline items of the embedded post.</param>
        /// <param name="forceremoveoriginalmessage">Flag to indicate that the command post allways should be deleted, if false the logic in the DeleteMessage method aplies.</param>
        /// <param name="hidefooter">Hide the footer in the embeded post.</param>
        public static async Task CreateEmbed(SocketCommandContext context, string emoji = null, string title = null, string content = null, List<Tuple<string, string>> inline = null, bool forceremoveoriginalmessage = false, bool hidefooter = false)
        {

            var embed = new EmbedBuilder();

            // Add a random color to the embedded post
            embed.WithColor(Helper.GetRandomColor());

            // Add emoji to title if any
            if (emoji != null || emoji != String.Empty)
            {
                title = string.Format("{0} {1}", emoji, title);
            }

            // Add Title
            embed.WithTitle(title);
            
            // Add content
            embed.WithDescription(content);
            if (inline != null)
            {
                foreach (Tuple<string, string> inlineitem in inline)
                {
                    embed.AddField(inlineitem.Item1, inlineitem.Item2);
                }
            }

            // Footer
            if (!hidefooter)
            {
                embed.WithFooter(footer => footer.Text = string.Format(
                    Constants.INVOKED_BY, context.Message.Content.Split(' ')[0], context.User.Username));
            }

            // Remove original if needed
            if (!context.Channel.Name.StartsWith("@"))
            {
                await DeleteMessage(context, forceremoveoriginalmessage);
            }
            
            // Send message
            await context.Channel.SendMessageAsync(string.Empty, false, embed.Build());
        }
    }
}
