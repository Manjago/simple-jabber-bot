using System;
using System.Collections.Generic;
using System.Text;
using Temnenkov.SJB.Database;
using Temnenkov.SJB.Common;

namespace Temnenkov.SJB.ConfLogPlugin.Business
{
    internal enum LineTypeEnum
    {
        Normal = 'N',
        Delay = 'D',
        TopicChange = 'T',
        DeleayTopicChange = 'S',
        SomebodyJoin = 'J',
        SomebodyLeave = 'L'
    }

    internal abstract class PersistentLine : PersistentObject
    {
        protected LineTypeEnum LineType { get; private set; }
        protected DateTime Date { get; private set; }

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

    }

    internal class ProtocolLine : PersistentLine
    {
        private string Jid { get; set; }
        private string From { get; set; }
        private string Message { get; set; }
        private string Hash 
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
        private string Hash
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


}
