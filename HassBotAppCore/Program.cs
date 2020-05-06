using DiscordBotLib;
using log4net;
using log4net.Config;
using System.IO;
using System.Reflection;
using System.Xml;

namespace HassBotApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // initialize the log4net.
            // log4net.Config.XmlConfigurator.Configure();

            //var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            //XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

            XmlDocument log4netConfig = new XmlDocument();
            log4netConfig.Load(File.OpenRead("log4net.config"));
            var repo = log4net.LogManager.CreateRepository(Assembly.GetEntryAssembly(),
                       typeof(log4net.Repository.Hierarchy.Hierarchy));
            log4net.Config.XmlConfigurator.Configure(repo, log4netConfig["log4net"]);

            // start the bot
            new DiscordBot().StartBotAsync().GetAwaiter().GetResult();
        }
    }
}