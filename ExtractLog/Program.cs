using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Temnenkov.SJB.Common;

namespace Temnenkov.SJB.ExtractLog
{
    class Program
    {
        static void Main(string[] args)
        {
            int shift;
            if (args.Length < 1 || !Int32.TryParse(args[0], out shift))
                shift = -1;

            string dirName;

            if (args.Length < 2 || !Directory.Exists(args[1]))
                dirName = Path.GetDirectoryName(Utils.GetExecutablePath());
            else
                dirName = args[1];

            var selectDate = DateTime.Now.AddDays(shift);
            var firstDate = selectDate.Date;
            var secondDate = selectDate.Date.AddDays(1).AddMilliseconds(-1);

            using (var db = new Database.Database(dirName, "Log"))
            {
                var resFile = Path.Combine(Path.GetDirectoryName(Utils.GetExecutablePath()), "exp.log");
                using (var sw = new System.IO.StreamWriter(resFile, false, Encoding.GetEncoding(1251)))
                {
                    using (var reader = db.ExecuteReader("SELECT [Date], [From], [Message], [IsDelay] from [Log] WHERE [Date] BETWEEN ? AND ? AND [Jid] = 'fido828@conference.jabber.ru' ORDER BY [Id]",
                        firstDate, secondDate))
                    {
                        while (reader.Read())
                        {
                            sw.WriteLine(string.Format("[{0}]{3} {1} {2}",
                                reader.GetDateTime(0).ToShortTimeString(),
                                InAp(reader.GetString(1)),
                                reader.GetString(2),
                                reader.GetBoolean(3) ? "*" : string.Empty));
                        }
                    }
                }
            }
        }

        private static string InAp(string arg)
        {
            return string.IsNullOrEmpty(arg) ? string.Empty : string.Format("'{0}'", arg);
        }
    }
}
