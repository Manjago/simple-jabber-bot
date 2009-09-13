using System;
using jabber.client;
using System.Threading;
using jabber.protocol.client;
using jabber;
using jabber.connection.sasl;
using jabber.connection;

namespace Temnenkov.SimpleJabberBot
{
    internal class Bot
    {
        private JabberClient _client;
        private ConferenceManager _conferenceManager;
        private MessageLogger messageLogger;

        internal Bot()
        {
            _client = new JabberClient();
            _client.AutoRoster = true;
            _client.AutoLogin = false;
            _client.Resource = String.Format("SimpleJabberBot {0}", Environment.Version.ToString());

            _client.OnInvalidCertificate += new System.Net.Security.RemoteCertificateValidationCallback(_client_OnInvalidCertificate);
            _client.OnLoginRequired += new bedrock.ObjectHandler(_client_OnLoginRequired);
            _client.OnRegisterInfo += new RegisterInfoHandler(_client_OnRegisterInfo);
            _client.OnRegistered += new IQHandler(_client_OnRegistered);
            _client.OnError += new bedrock.ExceptionHandler(_client_OnError);
            _client.OnMessage += new MessageHandler(_client_OnMessage);

            _conferenceManager = new ConferenceManager();
            _conferenceManager.Stream = _client;

            messageLogger = new MessageLogger();
        }

        internal void Init()
        {
            messageLogger.Init();
        }

        void _client_OnMessage(object sender, Message msg)
        {
            switch (msg.Type)
            {
                case MessageType.groupchat:
                    Logger.Log(LogType.Info, String.Format("Groupchat message received from resource {0} in room {1}: {2}", msg.From.Resource, msg.From.Bare, msg.Body));
                    messageLogger.LogMessage(msg.From.Bare!= null ? msg.From.Bare : string.Empty, msg.From.Resource != null ? msg.From.Resource : string.Empty, msg.Body != null ? msg.Body : string.Empty);
                    break;
                default:
                    Logger.Log(LogType.Info, String.Format("Message received from {0}@{1}: {2}", msg.From.User, msg.From.Server, msg.Body));
                    break;
            }
        }

        void _client_OnError(object sender, Exception ex)
        {
            if (ex is AuthenticationFailedException)
            {
                Logger.Log(LogType.Fatal, "Authentication failed. Check your Jabber credentials. The user you want to register probably already exists on the server.");
                Environment.Exit(-1);
            }
            Logger.Log(LogType.Fatal, "The Jabber client threw an exception: ", ex);
            Environment.Exit(-1);
        }

        void _client_OnRegistered(object sender, IQ iq)
        {
            Logger.Log(LogType.Info, "Logging in");
            _client.Login();
        }

        bool _client_OnRegisterInfo(object sender, jabber.protocol.iq.Register register)
        {
            return true;
        }

        void _client_OnLoginRequired(object sender)
        {
            Logger.Log(LogType.Info, "Registering");
            _client.Register(new JID(Settings.JabberUser, Settings.JabberServer, null));
        }

        bool _client_OnInvalidCertificate(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            Logger.Log(LogType.Info, "Invalid certificate");
            return true;
        }

        internal bool Connect()
        {
            Logger.Log(LogType.Info, "Try connect");

            _client.User = Settings.JabberUser;
            _client.Server = Settings.JabberServer;
            _client.Password = Settings.JabberPassword;
            _client.Connect();

            var retryCount = 50;
            while (!_client.IsAuthenticated && retryCount-- > 0)
                Thread.Sleep(500);

            if (_client.IsAuthenticated)
            {
                Logger.Log(LogType.Info, "Authenticated");
                SetStatus("I'm online.");
                return true;
            }
            else
            {
                Logger.Log(LogType.Warn, "Fail auth");
                return false;
            }

        }

        internal void Disconnect()
        {
            Logger.Log(LogType.Info, "Disconnect");
            _client.Close();
        }

        private void SetStatus(string status)
        {
            Logger.Log(LogType.Info, String.Format("Setting status to {0}", status));
            _client.Presence(PresenceType.available, status, null, 0);
        }

        internal bool JoinRoom(string jid)
        {
            Logger.Log(LogType.Info, String.Format("Join in room {0}", jid));

            JID rJid = new JID(jid);
            var room = _conferenceManager.GetRoom(jid);
            room.Join();

            var retryCount = 50;
            while (!room.IsParticipating && retryCount-- > 0)
                Thread.Sleep(500);

            var result = room.IsParticipating;

            if (result)
                Logger.Log(LogType.Info, String.Format("Join in room {0} sucessfully", jid));
            else
                Logger.Log(LogType.Warn, String.Format("Join in room {0} fail", jid));

            return result;
        }

    }
}

//toDo developer account
//toDo prevent doubling of delayed messages
//toDo prevent kick
//toDo multiple channels
//toDo improve external Application for unload Log
//toDo refactoring
//toDo release