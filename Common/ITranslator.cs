using System;

namespace Temnenkov.SJB.Common
{
    public interface ITranslator
    {
        event RoomMessageHandler RoomPublicMessage;
        event RoomMessageHandler RoomPrivateMessage;
        event RoomDelayMessageHandler RoomDelayPublicMessage;
        event NormalMessageHandler NormalMessage;
        event ChangeSubjectHandler ChangeSubject;
        event ChangeSubjectDelayHandler ChangeSubjectDelay;
        event RoomLeaveJoinHandler RoomLeaveJoin;
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

    public class ChangeSubjectEventArgs : EventArgs
    {
        public string RoomJid { get; private set; }
        public string Who { get; private set; }
        public string Subject { get; private set; }
        public DateTime Date { get; private set; }
        protected ChangeSubjectEventArgs() { }
        public ChangeSubjectEventArgs(string roomJid,
            string who, string subject, DateTime date)
        {
            RoomJid = roomJid;
            Who = who;
            Subject = subject;
            Date = date;
        }
    }

    public delegate void ChangeSubjectHandler(Object sender, ChangeSubjectEventArgs e);

    public class ChangeSubjectDelayEventArgs : ChangeSubjectEventArgs
    {
        public DateTime ServerDate { get; private set; }
        private ChangeSubjectDelayEventArgs() { }
        public ChangeSubjectDelayEventArgs(string roomJid,
            string who, string subject, DateTime date, DateTime serverDate):
            base(roomJid, who, subject, date)
        {
            ServerDate = serverDate;
        }
    }

    public delegate void ChangeSubjectDelayHandler(Object sender, ChangeSubjectDelayEventArgs e);

    public class RoomLeaveJoinEventArgs : EventArgs
    {
        public string RoomJid { get; private set; }
        public string Who { get; private set; }
        public bool IsJoin { get; private set; }
        public DateTime Date { get; private set; }
        private RoomLeaveJoinEventArgs() { }
        public RoomLeaveJoinEventArgs(string roomJid,
            string who, bool isJoin, DateTime date)
        {
            RoomJid = roomJid;
            Who = who;
            IsJoin = isJoin;
            Date = date;
        }
    }

    public delegate void RoomLeaveJoinHandler(Object sender, RoomLeaveJoinEventArgs e);
}
