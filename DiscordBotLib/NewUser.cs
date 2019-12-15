using Discord.WebSocket;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HassBotData;

namespace DiscordBotLib
{
    public class NewUser
    {
        private static readonly string FUNFACT_URL =
            "http://api.icndb.com/jokes/random?firstName={0}&lastName=&limitTo=[nerdy]";

        private static readonly string POST_DATA =
            @"{""object"":{""name"":""Name""}}";

        public static async Task NewUserJoined(SocketGuildUser user)
        {
            string message = GetWelcomeMessage(user);

            // Send a Direct Message to the new user with instructions
            var dmChannel = await user.GetOrCreateDMChannelAsync();
            await dmChannel.SendMessageAsync(message);
        }

        private static string GetWelcomeMessage(SocketGuildUser user)
        {
            StringBuilder sb = new StringBuilder(512);

            sb.Append($"Hello, {user.Mention}! Welcome to Home Assistant Discord Channel.\n\n");
            string welcomeData = WelcomeMessage.Instance.Message;
            string[] lines = welcomeData.Split('\n');
            foreach ( string line in lines )
            {
                if (line.StartsWith("//"))
                    continue;
                sb.Append(line);
                sb.Append("\n");
            }
            sb.Append(string.Format("Once again, Welcome to the {0} Channel!\n\n", user.Guild.Name));

            return sb.ToString();
        }

        private static string GetRandomFunFact(string userHandle)
        {
            string url = string.Format(FUNFACT_URL, userHandle);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = POST_DATA.Length;
            StreamWriter requestWriter = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.ASCII);
            requestWriter.Write(POST_DATA);
            requestWriter.Close();

            try
            {
                WebResponse webResponse = request.GetResponse();
                Stream webStream = webResponse.GetResponseStream();
                StreamReader responseReader = new StreamReader(webStream);
                string response = responseReader.ReadToEnd();
                responseReader.Close();
                dynamic stuff = JObject.Parse(response);
                return stuff.value.joke;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
