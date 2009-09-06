using System;
using System.Collections.Generic;
using System.Text;

namespace Temnenkov.SimpleJabberBot
{
    internal class MessageLogger
    {
        private Database db;

        internal void Init()
        {
            if (db != null) return;

            db = new Database("Log");

            try
            {
                db.ExecuteReader(Sql.check);
            }
            catch
            {
               Logger.Log(LogType.Info, "Create database");
                db.ExecuteCommand(Sql.create);
                db.ExecuteCommand(Sql.pragma);
            }
        }

        internal void LogMessage(string jid, string from, string message)
        {
            Logger.Log(LogType.Debug, string.Format("Log message {0} {1} {2}", jid, from, message));
            db.ExecuteCommand(Sql.insert, jid, from, message, DateTime.Now);
        }
    }
}
