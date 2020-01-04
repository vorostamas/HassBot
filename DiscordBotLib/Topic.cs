///////////////////////////////////////////////////////////////////////////////
//  AUTHOR          : Joakim Sørensen
//  DATE            : 04/01/2020
//  FILE            : TopicModule.cs
//  DESCRIPTION     : A class that implements ~topic command
///////////////////////////////////////////////////////////////////////////////
using System.Threading.Tasks;
using Discord.Commands;
using System;
using System.Diagnostics;
using System.Linq;
using Discord;
using System.Collections.Generic;

namespace DiscordBotLib
{
    public class TopicModule : BaseModule
    {
        private Process _process;
        private static readonly log4net.ILog logger =
       log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public TopicModule()
        {
            _process = Process.GetCurrentProcess();
        }

        [Command("topic")]
        public async Task TopicCommand()
        {
            var channel = Context.Client.GetChannel(Context.Channel.Id) as Discord.WebSocket.SocketTextChannel;
            string topic = channel.Topic;


            // Create the message
            if (string.IsNullOrEmpty(topic))
            {
                topic = "No topic is set for this channel.";
                await Helper.CreateEmbed(Context, title: "The topic of this channel is", content: topic);
            }

            // Send response
            await Helper.CreateEmbed(Context, title: "The topic of this channel is", content: topic);

        }

    }
}