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

        internal void OnRoomPublicMessage(RoomMessageEventArgs e)
        {
            if (RoomPublicMessage != null)
                RoomPublicMessage(this, e);
        }

        internal void OnRoomPrivateMessage(RoomMessageEventArgs e)
        {
            if (RoomPrivateMessage != null)
                RoomPrivateMessage(this, e);
        }

        internal void OnNormalMessage(NormalMessageEventArgs e)
        {
            if (NormalMessage != null)
                NormalMessage(this, e);
        }

        #region ITranslator Members

        public event RoomMessageHandler RoomPublicMessage;
        public event RoomMessageHandler RoomPrivateMessage;
        public event NormalMessageHandler NormalMessage;

        public void SendRoomPublicMessage(string roomJid, string message)
        {
            _bot.SendRoomPublicMessage(roomJid, message);
        }

        public void SendRoomPrivateMessage(string roomJid, string to, string message)
        {
            _bot.SendRoomPrivateMessage(roomJid, to, message);
        }

        public void SendNormalMessage(string to, string message)
        {
            _bot.SendMessage(to, message);
        }
        #endregion
    }
}
