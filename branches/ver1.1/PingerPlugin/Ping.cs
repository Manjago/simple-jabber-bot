using System;
using System.Collections.Generic;
using System.Text;
using Temnenkov.SJB.Common;

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
            return string.Format("Hi, {0}, use commands \"ping\", \"log\", \"help\".", to);
        }

        private static string NormalHelpMessage(string to)
        {
            return string.Format("Hi, {0}, use commands \"ping\", \"help\".", to);
        }

        private static string PingMessage(string to)
        {
            return string.Format("Hey {0}, it's {1}.", to, DateTime.Now);
        }

        private static bool IsCommand(string message, string cmd)
        {
            return !string.IsNullOrEmpty(message) && message.Equals(cmd, StringComparison.InvariantCultureIgnoreCase);
        }

        void Translator_NormalMessage(object sender, NormalMessageEventArgs e)
        {
            if (IsCommand(e.Message, "help"))
                Translator.SendNormalMessage(e.From, NormalHelpMessage(e.From));
            if (IsCommand(e.Message, "ping"))
                Translator.SendNormalMessage(e.From, PingMessage(e.From));
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
        }

        void Translator_RoomMessage(object sender, RoomMessageEventArgs e)
        {
            if (e.From.Equals(e.Me, StringComparison.InvariantCultureIgnoreCase)) return;

            if (IsCommand(e.Message, "help"))
                Translator.SendRoomPublicMessage(e.RoomJid, RoomHelpMessage(e.From));
            if (IsCommand(e.Message, "ping"))
                Translator.SendRoomPrivateMessage(e.RoomJid, e.From, PingMessage(e.From));
        }

    }
}
