using System;
using jabber.client;
using System.Threading;
using jabber.protocol.client;
using jabber;
using jabber.connection.sasl;
using jabber.connection;
using Temnenkov.SJB.Common;
using Temnenkov.SJB.PingerPlugin;
using Temnenkov.SJB.ConfLogPlugin;

namespace Temnenkov.SJB.Bot
{
    internal class Bot
    {
        private readonly JabberClient _client;
        private readonly ConferenceManager _conferenceManager;
        private Room _room;
        private readonly RosterManager _rosterManager;
        private readonly Translator _translator;
        // temp
        private readonly Plugin _ping;
        private readonly ConfLogger _confLog;


        internal Bot()
        {
            _client = new JabberClient
                      	{
                      		AutoRoster = true,
                      		AutoLogin = false,
                      		Resource = String.Format("SimpleJabberBot {0}", Environment.Version)
                      	};

        	_client.OnInvalidCertificate += ClientOnInvalidCertificate;
            _client.OnLoginRequired += ClientOnLoginRequired;
            _client.OnRegisterInfo += ClientOnRegisterInfo;
            _client.OnRegistered += ClientOnRegistered;
            _client.OnError += ClientOnError;
            _client.OnMessage += ClientOnMessage;

            _conferenceManager = new ConferenceManager {Stream = _client};

        	_rosterManager = new RosterManager
        	                 	{
        	                 		Stream = _client,
        	                 		AutoAllow = AutoSubscriptionHanding.AllowAll,
        	                 		AutoSubscribe = true
        	                 	};


            _translator = new Translator(this);

            //temp
            _ping = new Ping(_translator)
            {
                OperatorJid = Settings.OperatorJid
            };
            _confLog = new ConfLogger(_translator);
            _ping.Init();
            _confLog.Init();
        }

    	public RosterManager RosterManager1
    	{
    		get { return _rosterManager; }
    	}

    	void ClientOnMessage(object sender, Message msg)
        {
            var timeStamp = DateTime.Now;
            switch (msg.Type)
            {
                //toDO refactoring
                case MessageType.groupchat:
#pragma warning disable 618,612
                    if (msg.X == null)
#pragma warning restore 618,612
                        _translator.OnRoomPublicMessage(
                            new RoomMessageEventArgs(
                        msg.From.Bare ?? string.Empty,
                        msg.From.Resource ?? string.Empty,
                        msg.Body ?? string.Empty,
                        timeStamp, Settings.NameInRoom));
                    else
                        _translator.OnRoomDelayPublicMessage (
                            new RoomDelayMessageEventArgs(
                        msg.From.Bare ?? string.Empty,
                        msg.From.Resource ?? string.Empty,
                        msg.Body ?? string.Empty,
                        timeStamp, Settings.NameInRoom, timeStamp /*временно*/));
                    Logger.Log(LogType.Info, String.Format("Groupchat message received from resource {0} in room {1}: {2}", msg.From.Resource, msg.From.Bare, msg.Body));


                    break;
                case MessageType.chat:
                    {
                        var isRoomMesage = MessageHelper.IsFromRoomMessage(msg, _room);

                        if (isRoomMesage)
                            // значит, в комнате
                            _translator.OnRoomPrivateMessage(
                                new RoomMessageEventArgs(
                            msg.From.Bare ?? string.Empty,
                            msg.From.Resource ?? string.Empty,
                            msg.Body ?? string.Empty,
                            timeStamp, Settings.NameInRoom));
                        else
                            _translator.OnNormalMessage(
                                new NormalMessageEventArgs(
                                    msg.From.Bare, msg.Body, timeStamp));

                        Logger.Log(LogType.Info, String.Format("room {3} chat message resource {0} bare {1} body {2}", msg.From.Resource, msg.From.Bare, msg.Body, isRoomMesage));

                    }

                    break;
                case MessageType.normal:
                    Logger.Log(LogType.Info, String.Format("normal message received from {0}@{1}: {2}", msg.From.User, msg.From.Server, msg.Body));
                    _translator.OnNormalMessage(
                        new NormalMessageEventArgs(
                            msg.From.Bare, msg.Body, timeStamp));
                    break;
                default:
                    Logger.Log(LogType.Info, String.Format("default message received from {0}@{1}: {2}", msg.From.User, msg.From.Server, msg.Body));
                    break;
            }
        }

    	internal void SendRoomPrivateMessage(string roomJid, string to, string message)
        {
            if (_room != null && _room.IsParticipating && _room.JID == roomJid)
                _room.PrivateMessage(to, message);
        }

        internal void SendMessage(string to, string message)
        {
            _client.Message(to, message);
        }

    	static void ClientOnError(object sender, Exception ex)
        {
            if (ex is AuthenticationFailedException)
            {
                Logger.Log(LogType.Fatal, "Authentication failed. Check your Jabber credentials. The user you want to register probably already exists on the server.");
                Environment.Exit(-1);
            }
            Logger.Log(LogType.Fatal, "The Jabber client threw an exception: ", ex);
            Environment.Exit(-1);
        }

        void ClientOnRegistered(object sender, IQ iq)
        {
            Logger.Log(LogType.Info, "Logging in");
            _client.Login();
        }

    	static bool ClientOnRegisterInfo(object sender, jabber.protocol.iq.Register register)
        {
            return true;
        }

        void ClientOnLoginRequired(object sender)
        {
            Logger.Log(LogType.Info, "Registering");
            _client.Register(new JID(Settings.JabberUser, Settings.JabberServer, null));
        }

    	static bool ClientOnInvalidCertificate(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
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
        	Logger.Log(LogType.Warn, "Fail auth");
        	return false;
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

            if (_room != null)
            {
                _room.OnParticipantJoin -= RoomOnParticipantJoin;
                _room.OnParticipantLeave -= RoomOnParticipantLeave;
                _room.OnPrivateMessage -= RoomOnPrivateMessage;
                _room.OnRoomMessage -= RoomOnRoomMessage;
                _room.OnAdminMessage -= RoomOnAdminMessage;
                _room.OnLeave -= RoomOnLeave;
                _room.OnJoin -= RoomOnJoin;
                _room.OnSubjectChange -= RoomOnSubjectChange;
            }


            _room = _conferenceManager.GetRoom(jid);
            _room.Join();

            var retryCount = 50;
            while (!_room.IsParticipating && retryCount-- > 0)
                Thread.Sleep(500);

            var result = _room.IsParticipating;

            _room.OnParticipantJoin += RoomOnParticipantJoin;
            _room.OnParticipantLeave += RoomOnParticipantLeave;
            _room.OnPrivateMessage += RoomOnPrivateMessage;
            _room.OnRoomMessage += RoomOnRoomMessage;
            _room.OnAdminMessage += RoomOnAdminMessage;
            _room.OnLeave += RoomOnLeave;
            _room.OnJoin += RoomOnJoin;
            _room.OnSubjectChange += RoomOnSubjectChange;

            if (result)
            {
                Logger.Log(LogType.Info, String.Format("Join in room {0} sucessfully", jid));
                _room.PublicMessage("Превед!");


            }
            else
                Logger.Log(LogType.Warn, String.Format("Join in room {0} fail", jid));

            return result;
        }

        void RoomOnSubjectChange(object sender, Message msg)
        {
            Logger.Log(LogType.Info, string.Format("{1} change subject {0}",
                string.IsNullOrEmpty(msg.Subject) ? string.Empty : msg.Subject, 
                msg.From.Resource));

            var room = sender as Room;
			if (room == null) return;

#pragma warning disable 618,612
            if (msg.X == null)
#pragma warning restore 618,612
                _translator.OnChangeSubject(new ChangeSubjectEventArgs(
                    room.JID.Bare, msg.From.Resource,
                    string.IsNullOrEmpty(msg.Subject) ? string.Empty : msg.Subject,
                    DateTime.Now));
            else
                _translator.OnChangeSubjectDelay(new ChangeSubjectDelayEventArgs(
                    room.JID.Bare, msg.From.Resource,
                    string.IsNullOrEmpty(msg.Subject) ? string.Empty : msg.Subject,
                    DateTime.Now, DateTime.Now));
                    
        }

    	private void RoomOnJoin(Room room)
        {
            Logger.Log(LogType.Info, string.Format("join room {0}",
                room.JID));
            _translator.OnRoomLeaveJoin(new RoomLeaveJoinEventArgs(
                room.JID.Bare, Settings.NameInRoom, true, DateTime.Now));
        }

        private void RoomOnLeave(Room room, Presence pres)
        {
            Logger.Log(LogType.Info, string.Format("leave room {0} with presence {1}",
                room.JID, pres));
            _translator.OnRoomLeaveJoin(new RoomLeaveJoinEventArgs(
                room.JID.Bare, Settings.NameInRoom, false, DateTime.Now));
            JoinRoom(string.Format("{0}/{1}", Settings.RoomJid, Settings.NameInRoom));
        }

    	static void RoomOnAdminMessage(object sender, Message msg)
        {
            Logger.Log(LogType.Info, string.Format("admin message {0}", msg.OuterXml));
        }

    	static void RoomOnRoomMessage(object sender, Message msg)
        {
            Logger.Log(LogType.Info, string.Format("room message {0}", msg.OuterXml));
        }

    	static void RoomOnPrivateMessage(object sender, Message msg)
        {
            Logger.Log(LogType.Info, string.Format("private message {0}", msg.OuterXml));
        }

    	private void RoomOnParticipantLeave(Room room, RoomParticipant participant)
        {
            Logger.Log(LogType.Info, string.Format("leave {0}", participant.Nick));
            _translator.OnRoomLeaveJoin(new RoomLeaveJoinEventArgs(
                room.JID.Bare, participant.Nick, false, DateTime.Now));
        }

    	private void RoomOnParticipantJoin(Room room, RoomParticipant participant)
        {
            Logger.Log(LogType.Info, string.Format("join {0}", participant.Nick));
            _translator.OnRoomLeaveJoin(new RoomLeaveJoinEventArgs(
                room.JID.Bare, participant.Nick, true, DateTime.Now));
        }

        internal void SendRoomPublicMessage(string jid, string message)
        {
            if (_room != null && _room.IsParticipating && _room.JID == jid)
                _room.PublicMessage(message);
        }

    }
}


// toDO dirty log clean log

// toDO convert base - 1.hash 2. structure
// todo base check const

//toDo arch rebuild 
//todo localization
//toDo prevent doubling of delayed messages
//toDo multiple channels
//toDo not use msg.X
//toDo improve external Application for unload Log
//toDO mprove logging
//toDo refactoring
//toDo release ver 1.0

