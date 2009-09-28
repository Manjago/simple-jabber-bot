using System;
using Temnenkov.SJB.Common;

namespace Temnenkov.SJB.ConfLogPlugin.Business
{
    internal enum LineTypeEnum
    {
        Normal = 'N',
        Delay = 'D',
        TopicChange = 'T',
        DeleayTopicChange = 'S',
        SomebodyJoinLeave = 'J'
	}

    internal abstract class PersistentLine : PersistentObject
    {
        protected LineTypeEnum LineType { get; private set; }
        protected DateTime Date { get; private set; }
        protected abstract string Hash {get;}
        protected abstract string InternalDisplayString();

        private PersistentLine()
        {
        }

        protected PersistentLine(LineTypeEnum lineType, DateTime date)
            : this()
        {
            LineType = lineType;
            Date = date;
        }

        public static void Check(IDatabase db)
        {
            if (!db.TableExists("Log"))
                db.ExecuteCommand(Sql.Create);
        }

        protected static string InAp(string arg)
        {
            return string.IsNullOrEmpty(arg) ? string.Empty : string.Format("'{0}'", arg);
        }

        public string DisplayString(bool withDate)
        {
            return string.Format("[{0}]{1}", withDate ?
                string.Format("{0} {1}", Date.ToShortDateString(),
                Date.ToShortTimeString()) :
                Date.ToShortDateString(),
                InternalDisplayString());
        }
    }

    internal class ProtocolLine : PersistentLine
    {
        private string Jid { get; set; }
        private string From { get; set; }
        private string Message { get; set; }
        protected override string Hash 
        {
            get
            {
                return Utils.GetMd5Hash(string.Format("{0}{1}{2}",
                    Jid, From, Message));
            }
        }
        private bool IsValid
        {
            get
            {
                return !string.IsNullOrEmpty(Message) && !string.IsNullOrEmpty(From);
            }
        }

        internal ProtocolLine(string jid, string from, string message, DateTime date)
            : this(jid, from, message, date, LineTypeEnum.Normal)
        {
        }

        protected ProtocolLine(string jid, string from, string message, DateTime date, LineTypeEnum lineType)
            : base(lineType, date)
        {
            Jid = jid;
            From = from;
            Message = message;
        }

        public override void Save(IDatabase db)
        {
            if (db != null && IsValid)
                db.ExecuteCommand("INSERT INTO [Log] ([Jid], [From], [Message], [Date], [Hash], [Type]) VALUES (?, ?, ?, ?, ?, ?);",
                Jid, From, Message, Date, Hash, (char)LineType);
        }

        protected override string InternalDisplayString()
        {
            return string.Format("{0}{1} {2}",
                            LineType == LineTypeEnum.Delay ? "* " : string.Empty,
                            InAp(From),
                            Message
                            );
        }

    }

    internal class ProtocolDelayLine : ProtocolLine
    {
        internal ProtocolDelayLine(string jid, string from, string message, DateTime date)
            : base(jid, from, message, date, LineTypeEnum.Delay) { }

    }

    internal class ChangeSubjectLine : PersistentLine
    {
        private string Jid { get; set; }
        private string Who { get; set; }
        private string Subject { get; set; }
        protected override string Hash
        {
            get
            {
                return Utils.GetMd5Hash(string.Format("{0}{1}{2}",
                    Jid, Who, Subject));
            }
        }

        internal ChangeSubjectLine(string jid, string who, string subject, DateTime date)
            : this(jid, who, subject, date, LineTypeEnum.TopicChange)
        {
        }

        protected ChangeSubjectLine(string jid, string who, string subject, DateTime date, LineTypeEnum lineType)
            : base(lineType, date)
        {
            Jid = jid;
            Who = who;
            Subject = subject;
        }

        public override void Save(IDatabase db)
        {
            if (db != null)
                db.ExecuteCommand("INSERT INTO [Log] ([Jid], [From], [Message], [Date], [Hash], [Type]) VALUES (?, ?, ?, ?, ?, ?);",
                Jid, Who, Subject, Date, Hash, (char)LineType);
        }

    }

    internal class ChangeSubjectDelayLine : ChangeSubjectLine
    {
        internal ChangeSubjectDelayLine(string jid, string who, string subject, DateTime date)
            : base(jid, who, subject, date, LineTypeEnum.DeleayTopicChange) { }

    }

    internal class LeaveJoinLine : PersistentLine
    {
        private string Jid { get; set; }
        private string Who { get; set; }
        private bool IsJoin { get; set; }
        private string IsJoinAsStr 
        {
            get
            {
                return IsJoin ? "T" : "F";
            }
        } 
        protected override string Hash
        {
            get
            {
                return Utils.GetMd5Hash(string.Format("{0}{1}{2}",
                    Jid, Who, IsJoinAsStr));
            }
        }

        internal LeaveJoinLine(string jid, string who, bool isJoin, DateTime date)
            : base(LineTypeEnum.SomebodyJoinLeave, date)
        {
            Jid = jid;
            Who = who;
            IsJoin = isJoin;
        }

        public override void Save(IDatabase db)
        {
            if (db != null)
                db.ExecuteCommand("INSERT INTO [Log] ([Jid], [From], [Message], [Date], [Hash], [Type]) VALUES (?, ?, ?, ?, ?, ?);",
                Jid, Who, IsJoinAsStr, Date, Hash, (char)LineType);
        }
    }

}
