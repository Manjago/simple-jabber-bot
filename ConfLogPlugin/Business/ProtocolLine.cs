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
            : base(LineTypeEnum.Normal, date)
        {
            Jid = jid;
            From = from;
            Message = message;
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

}
