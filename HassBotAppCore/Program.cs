using DiscordBotLib;
using System.IO;
using System.Reflection;
using System.Xml;

/*
 * The bot server is also configured to run the app on restart... to do this,
 * run gpedit.msc from cmd
 * under Computer Configuration -> Windows Settings -> Scripts
 * double click on Startup, click Add
 * enter C:\Program Files\dotnet\dotnet.exe in script name
 * enter full path of the HassBotAppCore.dll in script parameters
 * enter OK and CLose the dialog box, and the console
 * 
 * Restart the server, it should automatically run the HassBot at startup!
 */
namespace HassBotApp
{
    class Program
    {
        static void Main(string[] args)
        {
            while(true)
            {
                string assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                XmlDocument log4netConfig = new XmlDocument();
                string log4netCOnfigPath = Path.Combine(assemblyPath, "log4net.config");
                log4netConfig.Load(File.OpenRead(log4netCOnfigPath));
                var repo = log4net.LogManager.CreateRepository(Assembly.GetEntryAssembly(),
                           typeof(log4net.Repository.Hierarchy.Hierarchy));
                log4net.Config.XmlConfigurator.Configure(repo, log4netConfig["log4net"]);

                // start the bot, if it crashes for any reason, restart in loop.
                try
                {
                    new DiscordBot().StartBotAsync().GetAwaiter().GetResult();
                }
                catch
                {
                    continue;
                }
            }
        }
    }
}