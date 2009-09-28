using System;
using System.Text;
using Temnenkov.SJB.Common;

namespace Temnenkov.SJB.ConfLog
{
    public class MessageLogger
    {
        private Database.Database _db;
        private readonly ILogger _log;
        private readonly string _dirName;

        public MessageLogger(ILogger log)
        {
            _log = log;
        }

        public MessageLogger(ILogger log, string dirName): this(log)
        {
            _dirName = dirName;
        }

        private void Init()
        {
            if (_db != null) return;

            _db = string.IsNullOrEmpty(_dirName) ? new Database.Database("Log") : new Database.Database(_dirName, "Log");

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
            Init();
            _log.Log(LogType.Debug, string.Format("Log message {0} {1} {2}, delayed:{3}", jid, from, message, delayed));
            _db.ExecuteCommand(Sql.Insert, jid, from, message, DateTime.Now, Utils.GetMd5Hash(message), delayed);
        }

        public StringBuilder GetLog(string jid, DateTime firstDate, DateTime secondDate,
            bool withDate)
        {
            Init();
            var sb = new StringBuilder();
                using (var reader = _db.ExecuteReader(Sql.Getlog, firstDate, secondDate, jid))
                {
                    while (reader.Read())
                    {
                        sb.AppendLine(string.Format("[{4}{0}]{3} {1} {2}",
                            reader.GetDateTime(0).ToShortTimeString(),
                            InAp(reader.GetString(1)),
                            reader.GetString(2),
                            reader.GetBoolean(3) ? "*" : string.Empty,
                            withDate ? String.Format("{0} ", reader.GetDateTime(0).ToShortDateString())
                            : string.Empty
                            ));
                    }
                }
                return sb;
        }

        private static string InAp(string arg)
        {
            return string.IsNullOrEmpty(arg) ? string.Empty : string.Format("'{0}'", arg);
        }

    }
}
