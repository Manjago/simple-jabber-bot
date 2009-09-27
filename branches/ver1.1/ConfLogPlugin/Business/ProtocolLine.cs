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
        internal string Jid { get; private set; }
        internal string From { get; private set; }
        internal string Message { get; private set; }
        internal string Hash { get; private set; }

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
            if (db != null)
                db.ExecuteCommand("INSERT INTO [Log] ([Jid], [From], [Message], [Date], [Hash], [Type]) VALUES (?, ?, ?, ?, ?, ?);",
                Jid, From, Message, Date, Utils.GetMd5Hash(Message), (char)LineType);
        }

    }

    internal class ProtocolDelayLine : ProtocolLine
    {
        internal ProtocolDelayLine(string jid, string from, string message, DateTime date)
            : base(jid, from, message, date, LineTypeEnum.Delay) { }

    }

}
