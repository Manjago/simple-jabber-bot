﻿using System;
using System.Collections.Generic;
using System.Text;
using Temnenkov.SJB.Database;
using Temnenkov.SJB.Common;

namespace Temnenkov.SJB.ConfLog
{
    public class MessageLogger
    {
        private Database.Database _db;
        private ILogger _log;
        private string _dirName;

        public MessageLogger(ILogger log)
        {
            _log = log;
        }

        public MessageLogger(ILogger log, string dirName)
            : this(log)
        {
            _dirName = dirName;
        }

        private void Init()
        {
            if (_db != null) return;

            if (string.IsNullOrEmpty(_dirName))
                _db = new Database.Database("Log");
            else
                _db = new Database.Database(_dirName, "Log");

            try
            {
                _db.ExecuteReader(Sql.Check);
            }
            catch
            {
                _log.Log(LogType.Info, "Create database");
                _db.ExecuteCommand(Sql.Create);
                _db.ExecuteCommand(Sql.Pragma);
            }
        }

        public void LogMessage(string jid, string from, string message, bool delayed)
        {
            //toDO no empty sender or empty message

            Init();
            _log.Log(LogType.Debug, string.Format("Log message {0} {1} {2}, delayed:{3}", jid, from, message, delayed,
                string.IsNullOrEmpty(from) || string.IsNullOrEmpty(message) ? 'T' : 'N'));
            _db.ExecuteCommand(Sql.Insert, jid, from, message, DateTime.Now, Utils.GetMd5Hash(message), delayed);
        }

        public StringBuilder GetLog(string jid, DateTime firstDate, DateTime secondDate,
            bool withDate)
        {
            Init();
            var sb = new StringBuilder();
            var filter = new Filter();
            using (var reader = _db.ExecuteReader(Sql.Getlog, firstDate, secondDate, jid))
            {
                while (reader.Read())
                {
                    var line = new ProtocolLine(reader.GetDateTime(0),
                        reader.GetString(1), reader.GetString(2),
                        reader.GetBoolean(3), reader.GetString(4));

                    if (filter.Approve(line))
                        sb.AppendLine(line.ToLogString(withDate));

                    PrintDeffered(withDate, sb, filter);
                }
                filter.Stop();
                PrintDeffered(withDate, sb, filter);
            }
            return sb;
        }

        private static void PrintDeffered(bool withDate, StringBuilder sb, Filter filter)
        {
            var lines = filter.GetLines();
            if (lines != null)
            {
                var enProtocolLine = lines.GetEnumerator();
                while (enProtocolLine.MoveNext())
                    sb.AppendLine(enProtocolLine.Current.ToLogString(withDate));
            }
        }

        private static string InAp(string arg)
        {
            return string.IsNullOrEmpty(arg) ? string.Empty : string.Format("'{0}'", arg);
        }

    }
}
