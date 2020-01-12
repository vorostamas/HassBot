using Discord.Commands;
using System;
using System.Collections.Generic;
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
            await Helper.CreateEmbed(
                Context,
                Constants.EMOJI_INFORMATION,
                Constants.USAGE_TITLE,
                usageString
                );
        }

        protected async Task CreateEmbed(SocketCommandContext context, 
                                         string emoji = null, string title = null, 
                                         string content = null, List<Tuple<string, string>> inline = null, 
                                         bool forceremoveoriginalmessage = false, bool hidefooter = false)
        {
            if (string.IsNullOrEmpty(content))
                return;

            // mention users if any
            string mentionedUsers = MentionedUsers();
            if (!string.IsNullOrEmpty(mentionedUsers))
                content = string.Format("{0} ", mentionedUsers) + content;

            await Helper.CreateEmbed(context, emoji, title, content, inline, forceremoveoriginalmessage, false);
        }
    }
}