using System;
using Temnenkov.SJB.Common;
using Temnenkov.SJB.LogBase.Business;

namespace Temnenkov.SJB.PingerPlugin
{
    public sealed class Ping : Plugin
    {
        public string OperatorJid { get; set; }

        public Ping(ITranslator translator)
            : base(translator)
        {
            Translator.RoomPublicMessage += Translator_RoomMessage;
            Translator.RoomPrivateMessage += Translator_RoomPrivateMessage;
            Translator.NormalMessage += Translator_NormalMessage;
        }

        private static string RoomHelpMessage(string to)
        {
			return string.Format("Привет, {0}! Используй команды \"zog\",\"ping\", \"log\", \"help\", \"харакири\",\"find(пробел)(слово)\".", to);
        }

        private static string NormalHelpMessage(string to)
        {
            return string.Format("Привет, {0}! Используй команды \"zog\",\"ping\", \"help\".", to);
        }

        private static string PingMessage(string to)
        {
            return string.Format("Привет {0}, сейчас {1}.", to, DateTime.Now);
        }

        private static string ZogMessage(string to)
        {
            return string.Format("Помни, {0}, что ЗОГ не дремлет, ЗОГ не спит!", to);
        }

        private static string LogMessage(string jid)
        {
            return Protocol.Load(new PersistentLineDataLayer(), 
                jid,
                DateTime.Now.AddDays(-1),
                DateTime.Now.AddDays(1)).Export(true); 
        }

		private static string FindMessage(string jid, string what)
		{
			return Protocol.Find(new PersistentLineDataLayer(), jid, what).Export(true); 
		}

        private static bool IsCommand(string message, string cmd)
        {
            return !string.IsNullOrEmpty(message) && message.Equals(cmd, StringComparison.InvariantCultureIgnoreCase);
        }

		private static bool IsTwinCommand(string message, string cmd, out string operand)
		{
			operand = string.Empty;
			if (string.IsNullOrEmpty(message)) return false;
			var twins = message.Split(' ');
			if (twins.Length != 2) return false;
			if (!IsCommand(twins[0], cmd)) return false;
			operand = twins[1];
			return true;
		}

		void Translator_NormalMessage(object sender, NormalMessageEventArgs e)
        {
            if (IsCommand(e.Message, "help"))
                Translator.SendNormalMessage(e.From, NormalHelpMessage(e.From));
            if (IsCommand(e.Message, "ping"))
                Translator.SendNormalMessage(e.From, PingMessage(e.From));
            if (IsCommand(e.Message, "zog"))
                Translator.SendNormalMessage(e.From, ZogMessage(e.From));
            if (IsCommand(e.Message, "shutdown") && 
                !string.IsNullOrEmpty(OperatorJid) && 
                OperatorJid.Equals(e.From, StringComparison.InvariantCultureIgnoreCase))
                Environment.Exit(-2);
        }

        void Translator_RoomPrivateMessage(object sender, RoomMessageEventArgs e)
        {
            if (e.From.Equals(e.Me, StringComparison.InvariantCultureIgnoreCase)) return;

            if (IsCommand(e.Message, "help"))
                Translator.SendRoomPrivateMessage(e.RoomJid, e.From, RoomHelpMessage(e.From));

            if (IsCommand(e.Message, "ping"))
                Translator.SendRoomPrivateMessage(e.RoomJid, e.From, PingMessage(e.From));

            if (IsCommand(e.Message, "zog"))
                Translator.SendRoomPrivateMessage(e.RoomJid, e.From, ZogMessage(e.From));

            if (IsCommand(e.Message, "log"))
                Translator.SendRoomPrivateMessage(e.RoomJid, e.From, LogMessage(e.RoomJid));

            if (IsCommand(e.Message, "харакири"))
                Translator.Kick(e.RoomJid, e.From, "Не знаю даже, что сказать. Я не пишу стихов и не люблю их. Да и к чему слова, когда на небе звезды?");

			{
				string what;
				if (IsTwinCommand(e.Message, "find", out what))
					Translator.SendRoomPrivateMessage(e.RoomJid, e.From, FindMessage(e.RoomJid, what));
			}
		}

        void Translator_RoomMessage(object sender, RoomMessageEventArgs e)
        {
            if (e.From.Equals(e.Me, StringComparison.InvariantCultureIgnoreCase)) return;

            if (IsCommand(e.Message, "help"))
                Translator.SendRoomPublicMessage(e.RoomJid, RoomHelpMessage(e.From));
            if (IsCommand(e.Message, "ping"))
                Translator.SendRoomPrivateMessage(e.RoomJid, e.From, PingMessage(e.From));
            if (IsCommand(e.Message, "zog"))
                Translator.SendRoomPublicMessage(e.RoomJid, ZogMessage(e.From));
            if (IsCommand(e.Message, "log"))
                Translator.SendRoomPrivateMessage(e.RoomJid, e.From, LogMessage(e.RoomJid));
            if (IsCommand(e.Message, "харакири"))
                Translator.Kick(e.RoomJid, e.From, "Не знаю даже, что сказать. Я не пишу стихов и не люблю их. Да и к чему слова, когда на небе звезды?");
			{
				string what;
				if (IsTwinCommand(e.Message, "find", out what))
					Translator.SendRoomPrivateMessage(e.RoomJid, e.From, FindMessage(e.RoomJid, what));
			}
		}

    }
}
