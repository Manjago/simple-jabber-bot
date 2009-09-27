using System;
using System.Collections.Generic;
using System.Text;

namespace Temnenkov.SJB.Common
{
    public interface ITranslator
    {
        event RoomMessageHandler RoomPublicMessage;
        event RoomMessageHandler RoomPrivateMessage;
        event RoomDelayMessageHandler RoomDelayPublicMessage;
        event NormalMessageHandler NormalMessage;
        void SendRoomPublicMessage(string roomJid, string message);
        void SendRoomPrivateMessage(string roomJid, string to, string message);
        void SendNormalMessage(string to, string message);
    }

    public class RoomMessageEventArgs : EventArgs
    {
        public string RoomJid { get; private set; }
        public string From { get; private set; }
        public string Message { get; private set; }
        public DateTime Date { get; private set; }
        public string Me { get; private set; }
        protected RoomMessageEventArgs() { }
        public RoomMessageEventArgs(string roomJid,
            string from, string message, DateTime date, string me) 
        {
            RoomJid = roomJid;
            From = from;
            Message = message;
            Date = date;
            Me = me;
        }
    }

    public delegate void RoomMessageHandler(Object sender, RoomMessageEventArgs e);

    public class RoomDelayMessageEventArgs : RoomMessageEventArgs
    {
        public DateTime ServerDate { get; private set; }
        private RoomDelayMessageEventArgs() { }
        public RoomDelayMessageEventArgs(string roomJid,
            string from, string message, DateTime date, string me, DateTime serverDate) :
            base(roomJid, from, message, date, me)
        {
            ServerDate = serverDate;
        }
    }

    public delegate void RoomDelayMessageHandler(Object sender, RoomDelayMessageEventArgs e);

    public class NormalMessageEventArgs : EventArgs
    {
        public string From { get; private set; }
        public string Message { get; private set; }
        public DateTime Date { get; private set; }
        private NormalMessageEventArgs() { }
        public NormalMessageEventArgs(
            string from, string message, DateTime date)
        {
            From = from;
            Message = message;
            Date = date;
        }
    }

    public delegate void NormalMessageHandler(Object sender, NormalMessageEventArgs e);
}
