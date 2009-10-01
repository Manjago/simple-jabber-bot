﻿using System;
using Temnenkov.SJB.Common;
using System.Data;

namespace Temnenkov.SJB.LogBase.Business
{
    public enum LineTypeEnum
    {
        Normal = 'N',
        Delay = 'D',
        TopicChange = 'T',
        DeleayTopicChange = 'S',
        SomebodyJoinLeave = 'J'
	}

    public class PersistentLineDataLayer
    {
        private const int DATE_DATE = 0;
        private const int STRING_FROM = 1;
        private const int STRING_MESSAGE = 2;
        private const int CHAR_LINETYPEENUM = 3;
        private const int STRING_JID = 4;

        internal IDatabase _db;

        public PersistentLineDataLayer()
        {
            _db = new Database.Database("ConfLog");
        }

        public void Check()
        {
            if (!_db.TableExists("Log"))
                _db.ExecuteCommand(Sql.Create);
        }


        private PersistentLine Create(IDataReader dr)
        {
            var lineTypeEnum = (LineTypeEnum)dr.GetChar(CHAR_LINETYPEENUM);
            switch (lineTypeEnum)
            {
                case LineTypeEnum.Normal:
                    return new ProtocolLine(
                        dr.GetString(STRING_JID),
                        dr.GetString(STRING_FROM),
                        dr.GetString(STRING_MESSAGE),
                        dr.GetDateTime(DATE_DATE));
                case LineTypeEnum.Delay:
                    return new ProtocolDelayLine(
                        dr.GetString(STRING_JID),
                        dr.GetString(STRING_FROM),
                        dr.GetString(STRING_MESSAGE),
                        dr.GetDateTime(DATE_DATE));
                case LineTypeEnum.TopicChange:
                    return new ChangeSubjectLine(
                        dr.GetString(STRING_JID),
                        dr.GetString(STRING_FROM),
                        dr.GetString(STRING_MESSAGE),
                        dr.GetDateTime(DATE_DATE));
                case LineTypeEnum.DeleayTopicChange:
                    return new ChangeSubjectDelayLine(
                        dr.GetString(STRING_JID),
                        dr.GetString(STRING_FROM),
                        dr.GetString(STRING_MESSAGE),
                        dr.GetDateTime(DATE_DATE));
                case LineTypeEnum.SomebodyJoinLeave:
                    return new LeaveJoinLine(
                        dr.GetString(STRING_JID),
                        dr.GetString(STRING_FROM),
                        dr.GetString(STRING_MESSAGE) != "F",
                        dr.GetDateTime(DATE_DATE));
                default:
                    throw new ArgumentOutOfRangeException(string.Format("bad arg {0}", lineTypeEnum));
            }
        }

        internal void Save(PersistentLine pLine)
        {
            if (_db == null || pLine == null) return;

            var lineTypeEnum = pLine.LineType;
            switch (lineTypeEnum)
            {
                case LineTypeEnum.Normal:
                    {
                        var line = pLine as ProtocolLine;
                        if (line.IsValid)
                            _db.ExecuteCommand(Sql.Insert,
                            line.Jid, line.From, line.Message, line.Date, line.Hash, (char)line.LineType);
                    }
                    break;
                case LineTypeEnum.Delay:
                    {
                        var line = pLine as ProtocolDelayLine;
                        if (line.IsValid)
                            _db.ExecuteCommand(Sql.Insert,
                            line.Jid, line.From, line.Message, line.Date, line.Hash, (char)line.LineType);
                    }
                    break;
                case LineTypeEnum.TopicChange:
                    {
                        var line = pLine as ChangeSubjectLine;
                        _db.ExecuteCommand(Sql.Insert,
                            line.Jid, line.Who, line.Subject, line.Date, line.Hash, (char)line.LineType);
                    }
                    break;
                case LineTypeEnum.DeleayTopicChange:
                    {
                        var line = pLine as ChangeSubjectDelayLine;
                        _db.ExecuteCommand(Sql.Insert,
                            line.Jid, line.Who, line.Subject, line.Date, line.Hash, (char)line.LineType);
                    }
                    break;
                case LineTypeEnum.SomebodyJoinLeave:
                    {
                        var line = pLine as LeaveJoinLine;
                            _db.ExecuteCommand(Sql.Insert,
                            line.Jid, line.Who, line.IsJoinAsStr, line.Date, line.Hash, (char)line.LineType);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(string.Format("bad arg {0}", lineTypeEnum));
            }

        }

        internal void Load(System.Collections.Generic.List<PersistentLine> list, string jid, DateTime perBeg, DateTime perEnd)
        {
            using (var reader = _db.ExecuteReader(Sql.Getlog, perBeg, perEnd, jid))
            {
                while (reader.Read())
                    list.Add(Create(reader));
            }
        }
    }

    public abstract class PersistentLine 
    {
        internal LineTypeEnum LineType { get; private set; }
        internal DateTime Date { get; private set; }
        internal abstract string Hash {get;}
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

        public void Save(PersistentLineDataLayer dal)
        {
            dal.Save(this);
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

    public class ProtocolLine : PersistentLine
    {
        internal string Jid { get; private set; }
        internal string From { get; private set; }
        internal string Message { get; private set; }
        internal override string Hash 
        {
            get
            {
                return Utils.GetMd5Hash(string.Format("{0}{1}{2}",
                    Jid, From, Message));
            }
        }

        internal bool IsValid
        {
            get
            {
                return !string.IsNullOrEmpty(Message) && !string.IsNullOrEmpty(From);
            }
        }

        public ProtocolLine(string jid, string from, string message, DateTime date)
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

        protected override string InternalDisplayString()
        {
            return string.Format("{0}{1} {2}",
                            LineType == LineTypeEnum.Delay ? "* " : string.Empty,
                            InAp(From),
                            Message
                            );
        }

    }

    public class ProtocolDelayLine : ProtocolLine
    {
        public ProtocolDelayLine(string jid, string from, string message, DateTime date)
            : base(jid, from, message, date, LineTypeEnum.Delay) { }

    }

    public class ChangeSubjectLine : PersistentLine
    {
        internal string Jid { get; private set; }
        internal string Who { get; private set; }
        internal string Subject { get; private set; }
        internal override string Hash
        {
            get
            {
                return Utils.GetMd5Hash(string.Format("{0}{1}{2}",
                    Jid, Who, Subject));
            }
        }

        public ChangeSubjectLine(string jid, string who, string subject, DateTime date)
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

		protected override string InternalDisplayString()
		{
			return string.Format("{0}{1} изменил топик на {2}",
							LineType == LineTypeEnum.Delay ? "* " : string.Empty,
							InAp(Who),
							InAp(Subject)
							);
		}
	}

    public class ChangeSubjectDelayLine : ChangeSubjectLine
    {
        public ChangeSubjectDelayLine(string jid, string who, string subject, DateTime date)
            : base(jid, who, subject, date, LineTypeEnum.DeleayTopicChange) { }

    }

    public class LeaveJoinLine : PersistentLine
    {
        internal string Jid { get; private set; }
        internal string Who { get; private set; }
        internal bool IsJoin { get; private set; }
        internal string IsJoinAsStr 
        {
            get
            {
                return IsJoin ? "T" : "F";
            }
        } 
        internal override string Hash
        {
            get
            {
                return Utils.GetMd5Hash(string.Format("{0}{1}{2}",
                    Jid, Who, IsJoinAsStr));
            }
        }

        public LeaveJoinLine(string jid, string who, bool isJoin, DateTime date)
            : base(LineTypeEnum.SomebodyJoinLeave, date)
        {
            Jid = jid;
            Who = who;
            IsJoin = isJoin;
        }

		protected override string InternalDisplayString()
		{
			if (IsJoin)
				return string.Format("*** к нам пришел {0}",
							InAp(Who));
			return string.Format("*** от нас ушел {0}",
			                     InAp(Who));
		}
    }

}