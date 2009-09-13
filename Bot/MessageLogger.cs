﻿using System;
using System.Collections.Generic;
using System.Text;
using Temnenkov.SJB.Database;

namespace Temnenkov.SJB.Bot
{
    internal class MessageLogger
    {
        private Database.Database db;

        internal void Init()
        {
            if (db != null) return;

            db = new Database.Database("Log");

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

        internal void LogMessage(string jid, string from, string message, bool delayed)
        {
            Logger.Log(LogType.Debug, string.Format("Log message {0} {1} {2}, delayed:{3}", jid, from, message, delayed));
            db.ExecuteCommand(Sql.insert, jid, from, message, DateTime.Now);
        }
    }
}
