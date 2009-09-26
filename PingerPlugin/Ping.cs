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
            Translator.RoomMessage += new RoomMessageHandler(Translator_RoomMessage);
        }

        void Translator_RoomMessage(object sender, RoomMessageEventArgs e)
        {
            if (e.From != e.Me && !string.IsNullOrEmpty(e.Message) && e.Message == "help")
                Translator.SendRoomPublicMessage(e.RoomJid, 
                    string.Format("Hi, {0}, use commands \"ping\", \"log\".", e.From));
        }

    }
}
