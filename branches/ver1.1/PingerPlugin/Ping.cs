using System;
using System.Collections.Generic;
using System.Text;
using Temnenkov.SJB.Common;

namespace Temnenkov.SJB.PingerPlugin
{
    public sealed class Ping: Plugin
    {
        public Ping(ITranslator translator)
            : base(translator)
        {
            Translator.RoomPublicMessage += new RoomMessageHandler(Translator_RoomMessage);
            Translator.RoomPrivateMessage += new RoomMessageHandler(Translator_RoomPrivateMessage);
        }

        private string HelpMessage(string to)
        {
            return string.Format("Hi, {0}, use commands \"ping\", \"log\".", to);
        }

        void Translator_RoomPrivateMessage(object sender, RoomMessageEventArgs e)
        {
            if (e.From != e.Me && !string.IsNullOrEmpty(e.Message) && e.Message == "help")
                Translator.SendRoomPrivateMessage(e.RoomJid, e.From, HelpMessage(e.From));
        }

        void Translator_RoomMessage(object sender, RoomMessageEventArgs e)
        {
            if (e.From != e.Me && !string.IsNullOrEmpty(e.Message) && e.Message == "help")
                Translator.SendRoomPublicMessage(e.RoomJid, HelpMessage(e.From));
        }

    }
}
