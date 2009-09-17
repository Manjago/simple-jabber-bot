using System;
using jabber.client;
using System.Threading;
using jabber.protocol.client;
using jabber;
using jabber.connection.sasl;
using jabber.connection;
using jabber.protocol.iq;
using System.Text;
using Temnenkov.SJB.Common;
using Temnenkov.SJB.ConfLog;

namespace Temnenkov.SJB.Bot
{
    internal class Bot
    {
        private JabberClient _client;
        private ConferenceManager _conferenceManager;
        private MessageLogger messageLogger;
        private Room _room;
        private RosterManager _rosterManager;

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

            _rosterManager = new RosterManager();
            _rosterManager.Stream = _client;
            _rosterManager.AutoAllow = AutoSubscriptionHanding.AllowAll;
            _rosterManager.AutoSubscribe = true;

            messageLogger = new MessageLogger(new LogWrapper());
        }

        void _client_OnMessage(object sender, Message msg)
        {
            switch (msg.Type)
            {
                //toDO refactoring
                case MessageType.groupchat:
                    Logger.Log(LogType.Info, String.Format("Groupchat message received from resource {0} in room {1}: {2}", msg.From.Resource, msg.From.Bare, msg.Body));
                    messageLogger.LogMessage(msg.From.Bare != null ? msg.From.Bare : string.Empty, msg.From.Resource != null ? msg.From.Resource : string.Empty, msg.Body != null ? msg.Body : string.Empty, msg.X != null);
                    if (msg.X == null && !string.IsNullOrEmpty(msg.Body) && msg.Body.Equals("ping", StringComparison.InvariantCultureIgnoreCase))
                    {
                        Logger.Log(LogType.Info, String.Format("Pinging back to {0}", msg.From.User));
                        SendPrivateMessage(msg.From.Resource, String.Format("Hey {0}, it's {1}.", msg.From.Resource, DateTime.Now));
                    }

                    if (msg.X == null && !string.IsNullOrEmpty(msg.Body) && msg.Body.Equals("log", StringComparison.InvariantCultureIgnoreCase))
                    {
                        Logger.Log(LogType.Info, String.Format("Send log to {0}", msg.From.User));
                        SendLog(msg.From.Bare, msg.From.Resource);                        
                    }

                    break;
                case MessageType.chat:
                    Logger.Log(LogType.Info, String.Format("chat message received from resource {0} in room {1}: {2}", msg.From.Resource, msg.From.Bare, msg.Body));
                    if (!string.IsNullOrEmpty(msg.Body) && msg.Body.Equals("ping", StringComparison.InvariantCultureIgnoreCase))
                    {
                        Logger.Log(LogType.Info, String.Format("Pinging back to {0}", msg.From.User));
                        if (msg.From.Bare != null && msg.From.Bare == _room.JID.Bare)
                            // значит, в комнате
                            SendPrivateMessage(msg.From.Resource, String.Format("Hey {0}, it's {1}.", msg.From.Resource, DateTime.Now));
                        else // не в комнате
                            SendMessage(msg.From.Bare, String.Format("Hey {0}, it's {1}.", msg.From.Bare, DateTime.Now));
                    }

                    if (!string.IsNullOrEmpty(msg.Body) && msg.Body.Equals("log", StringComparison.InvariantCultureIgnoreCase))
                    {
                        Logger.Log(LogType.Info, String.Format("Send log to {0}", msg.From.User));
                        if (msg.From.Bare != null && msg.From.Bare == _room.JID.Bare) // логи только через приватное сообщение из комнаты
                            SendLog(msg.From.Bare, msg.From.Resource);
                    }
                    break;
                default:
                    Logger.Log(LogType.Info, String.Format("Message received from {0}@{1}: {2}", msg.From.User, msg.From.Server, msg.Body));
                    break;
            }
        }

        private void SendPrivateMessage(string to, string message)
        {
            if (_room != null && _room.IsParticipating)
                _room.PrivateMessage(to, message);
        }

        private void SendMessage(string to, string message)
        {
            _client.Message(to, message);
        }

        private void SendLog(string jid, string to)
        {
            var selectDate = DateTime.Now.AddDays(-1);
            var firstDate = selectDate.Date;
            var secondDate = DateTime.Now.AddDays(1);

            var sb = new MessageLogger(new LogWrapper()).GetLog(jid, firstDate, secondDate, true);
            if (sb.Length != 0)
                SendPrivateMessage(to, sb.ToString());
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
            _room = _conferenceManager.GetRoom(jid);
            _room.Join();

            var retryCount = 50;
            while (!_room.IsParticipating && retryCount-- > 0)
                Thread.Sleep(500);

            var result = _room.IsParticipating;

            _room.OnParticipantJoin += new RoomParticipantEvent(_room_OnParticipantJoin);
            _room.OnParticipantLeave += new RoomParticipantEvent(_room_OnParticipantLeave);
            _room.OnPrivateMessage += new MessageHandler(_room_OnPrivateMessage);
            _room.OnRoomMessage += new MessageHandler(_room_OnRoomMessage);

            if (result)
                Logger.Log(LogType.Info, String.Format("Join in room {0} sucessfully", jid));
            else
                Logger.Log(LogType.Warn, String.Format("Join in room {0} fail", jid));

            return result;
        }

        void _room_OnRoomMessage(object sender, Message msg)
        {
            Logger.Log(LogType.Info, string.Format("room message {0}", msg));
        }

        void _room_OnPrivateMessage(object sender, Message msg)
        {
            Logger.Log(LogType.Info, string.Format("private message {0}", msg));
        }

        void _room_OnParticipantLeave(Room room, RoomParticipant participant)
        {
            Logger.Log(LogType.Info, string.Format("leave {0}", participant.Nick));
        }

        void _room_OnParticipantJoin(Room room, RoomParticipant participant)
        {
            Logger.Log(LogType.Info, string.Format("join {0}", participant.Nick));
        }

    }
}


//toDO refactor log & ping in private messages
//toDo join leave etc
//toDo prevent doubling of delayed messages
//toDo prevent kick
//toDo multiple channels
//toDo not use msg.X
//toDo improve external Application for unload Log
//toDo refactoring
//toDo release ver 1.0

