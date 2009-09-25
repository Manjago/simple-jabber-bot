using System;
using System.Collections.Generic;
using System.Text;

namespace Temnenkov.SJB.ConfLog
{
    internal class ProtocolLine
    {
        internal DateTime Date {get; private set;}
        internal string Nick { get; private set; }
        internal string Message { get; private set; }
        internal bool IsDelay { get; private set; }
        internal string Hash { get; private set; }
        internal string Twin { get; private set; }
        internal string NextTwin { get; private set; }

        private ProtocolLine() { }

        internal ProtocolLine(DateTime date, string nick, string message, bool isDelay,
            string hash)
        {
            Date = date;
            Nick = nick;
            Message = message;
            IsDelay = isDelay;
            Hash = hash;
        }

        internal string ToLogString(bool withDate)
        {
            return string.Format("[{4}{0}]{3} {1} {2}",
                            Date.ToShortTimeString(),
                            InAp(Nick),
                            Message,
                            IsDelay ? "*" : string.Empty,
                            withDate ? String.Format("{0} ", Date.ToShortDateString())
                            : string.Empty
                            );        
        }

        private static string InAp(string arg)
        {
            return string.IsNullOrEmpty(arg) ? string.Empty : string.Format("'{0}'", arg);
        }

    }
}
