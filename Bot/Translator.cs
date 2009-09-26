using System;
using System.Collections.Generic;
using System.Text;
using Temnenkov.SJB.Common;

namespace Temnenkov.SJB.Bot
{
    public sealed class Translator : ITranslator
    {
        private Bot _bot;

        private Translator()
        {
        }

        internal Translator(Bot bot)
        {
            _bot = bot;
        }

        internal void OnRoomMessage(RoomMessageEventArgs e)
        {
            if (RoomMessage != null)
                RoomMessage(this, e);
        }

        #region ITranslator Members

        public event RoomMessageHandler RoomMessage;

        public void SendRoomPublicMessage(string roomJid, string message)
        {
            _bot.RoomPublicMessage(roomJid, message);
        }

        #endregion
    }
}
