using Discord;
using Discord.Commands;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace DiscordBotLib
{
    public abstract class BaseModule : ModuleBase<SocketCommandContext>
    {

        protected string MentionedUsers()
        {
            string mentionedUsers = string.Empty;
            foreach (var user in Context.Message.MentionedUsers)
            {
                mentionedUsers += $"{user.Mention} ";
            }

            return mentionedUsers;
        }

        protected string MentionedChannels()
        {
            string mentionedChannels = string.Empty;
            foreach (var channel in Context.Message.MentionedChannels)
            {
                mentionedChannels += $"{channel.Id} ";
            }

            return mentionedChannels.TrimEnd();
        }

        protected async Task DisplayUsage(string usageString)
        {
            await CreateEmbed(
                Constants.EMOJI_INFORMATION,
                Constants.USAGE_TITLE,
                usageString
                );
        }

        private static readonly log4net.ILog logger =
             log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public async Task DeleteMessage()
        {
            logger.Debug("Deleting command message " +
                Context.Message + " from " + Context.User.Username + " in " + Context.Channel.Name);
            await Context.Message.DeleteAsync();
        }

        public async Task CreateEmbed(
            string emoji = null,
            string title = null,
            string content = null,
            bool removeoriginalmessage = true)
        {

            var embed = new EmbedBuilder();

            // Add a random color to the embedded post
            embed.WithColor(Helper.GetRandomColor());

            // Add Title
            // Add emoji if any
            if (emoji != null || emoji != String.Empty)
            {
                embed.WithTitle(emoji + " " + title);
            }
            else
            {
                embed.WithTitle(title);
            }

            // Add content
            embed.WithDescription(content);

            // Footer
            // Add invoker
            embed.WithFooter(footer => footer.Text = string.Format(
                Constants.INVOKED_BY, Context.User.Username));

            // Add timestamp
            embed.WithCurrentTimestamp();

            // Remove original if needed
            if (removeoriginalmessage)
            {
                await DeleteMessage();
            }

            // Send message
            await ReplyAsync(string.Empty, false, embed);
        }
    }
}
