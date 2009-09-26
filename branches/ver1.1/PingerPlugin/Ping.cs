using System;
using System.Collections.Generic;
using System.Text;
using Temnenkov.SJB.Common;

namespace Temnenkov.SJB.PingerPlugin
{
    public sealed class Ping : Plugin
    {
        public Ping(ITranslator translator)
            : base(translator)
        {
            Translator.RoomPublicMessage += Translator_RoomMessage;
            Translator.RoomPrivateMessage += Translator_RoomPrivateMessage;
            Translator.NormalMessage += Translator_NormalMessage;
        }

        void Translator_NormalMessage(object sender, NormalMessageEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Message) && e.Message == "help")
                Translator.SendNormalMessage(e.From, RoomHelpMessage(e.From));
        }

        private string RoomHelpMessage(string to)
        {
            return string.Format("Hi, {0}, use commands \"ping\", \"log\", \"help\".", to);
        }

        private string NormalHelpMessage(string to)
        {
            return string.Format("Hi, {0}, use commands \"ping\", \"help\".", to);
        }

        void Translator_RoomPrivateMessage(object sender, RoomMessageEventArgs e)
        {
            if (e.From != e.Me && !string.IsNullOrEmpty(e.Message) && e.Message == "help")
                Translator.SendRoomPrivateMessage(e.RoomJid, e.From, RoomHelpMessage(e.From));
        }

        void Translator_RoomMessage(object sender, RoomMessageEventArgs e)
        {
            if (e.From != e.Me && !string.IsNullOrEmpty(e.Message) && e.Message == "help")
                Translator.SendRoomPublicMessage(e.RoomJid, RoomHelpMessage(e.From));
        }

    }
}
