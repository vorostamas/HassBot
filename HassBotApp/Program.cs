using DiscordBotLib;

namespace HassBotApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // initialize the log4net.
            log4net.Config.XmlConfigurator.Configure();

            // start the bot
            new DiscordBot().StartBotAsync().GetAwaiter().GetResult();
        }
    }
}