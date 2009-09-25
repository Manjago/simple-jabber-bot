using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Temnenkov.SJB.Common;
using Temnenkov.SJB.ConfLog;

namespace Temnenkov.SJB.ExtractLog
{
    class Program
    {
        static void Main(string[] args)
        {
            string jid;
            if (args.Length < 1)
                return;
            else
                jid = args[0];

            int shift;
            if (args.Length < 2 || !Int32.TryParse(args[1], out shift))
                shift = -1;

            string dirName;

            if (args.Length < 3 || !Directory.Exists(args[2]))
                dirName = Path.GetDirectoryName(Utils.GetExecutablePath());
            else
                dirName = args[2];

            var selectDate = DateTime.Now.AddDays(shift);
            var firstDate = selectDate.Date;
            var secondDate = selectDate.Date.AddDays(1).AddMilliseconds(-1);

            var resFile = Path.Combine(Path.GetDirectoryName(Utils.GetExecutablePath()), "exp.log");
            using (var sw = new System.IO.StreamWriter(resFile, false, Encoding.GetEncoding(1251)))
            {
                var sb = new MessageLogger(new DummyLogger(), dirName).GetLog(jid, firstDate, secondDate, false);
                sw.Write(sb.ToString());
            }
        }

        internal class DummyLogger : ILogger
        {
            #region ILogger Members

            public void Log(LogType type, string message, Exception ex)
            {
            }

            public void Log(LogType type, string message)
            {
            }

            #endregion
        }
    }
}
