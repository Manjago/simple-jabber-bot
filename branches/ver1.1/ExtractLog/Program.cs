﻿using System;
using System.Text;
using System.IO;
using Temnenkov.SJB.Common;
using Temnenkov.SJB.LogBase.Business;

namespace Temnenkov.SJB.ExtractLog
{
    class Program
    {
        static void Main(string[] args)
        {
        	if (args.Length < 1)
                return;
        	var jid = args[0];

        	int shift;
            if (args.Length < 2 || !Int32.TryParse(args[1], out shift))
                shift = -1;

            string dirName;

            if (args.Length < 3 || !Directory.Exists(args[2]))
                dirName = Path.GetDirectoryName(Utils.GetExecutablePath());
            else
                dirName = args[2];

            if (shift > 0)
            {
                var resFile = Path.Combine(Path.GetDirectoryName(Utils.GetExecutablePath()), "copy.sqlite");
                if (File.Exists(resFile)) File.Delete(resFile);
                Protocol.Load(new PersistentLineDataLayer(),
                        jid, new DateTime(1990, 1, 1), new DateTime(2990, 1, 1)).Save("copy");

            }
            else
            {
                var selectDate = DateTime.Now.AddDays(shift);
                var firstDate = selectDate.Date;
                var secondDate = selectDate.Date.AddDays(1).AddMilliseconds(-1);

                var resFile = Path.Combine(Path.GetDirectoryName(Utils.GetExecutablePath()), "exp.log");
                using (var sw = new StreamWriter(resFile, false, Encoding.GetEncoding(866)))
                    sw.Write(Protocol.Load(new PersistentLineDataLayer(),
                        jid, firstDate, secondDate).Export(true));
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
