using System;

namespace HassBotData
{
    public class WelcomeMessage
    {
        private static string _welcomeMessage = string.Empty;
        private static readonly Lazy<WelcomeMessage> lazy = new Lazy<WelcomeMessage>(() => new WelcomeMessage());

        public static WelcomeMessage Instance
        {
            get { return lazy.Value; }
        }

        static WelcomeMessage()
        {
            ReloadData();
        }

        public string Message
        {
            get
            {
                return _welcomeMessage;
            }
        }

        public static void ReloadData()
        {
            try
            {
                _welcomeMessage = Persistence.LoadWelcomeMessage();
            }
            catch (Exception e)
            {
                throw new Exception(Constants.ERR_WELCOME_MSG_FILE, e);
            }
        }
    }
}