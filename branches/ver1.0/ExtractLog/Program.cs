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
            var selectDate = DateTime.Now.AddDays(-1);
            var firstDate = selectDate.Date;
            var secondDate = selectDate.Date.AddDays(1).AddMilliseconds(-1);

            using (var db = new Database.Database("Log"))
            {
                var resFile = Path.Combine(Path.GetDirectoryName(Utils.GetExecutablePath()), "exp");
                using (var sw = new System.IO.StreamWriter(resFile, false, Encoding.GetEncoding(1251)))
                {
                    using (var reader = db.ExecuteReader("SELECT [Date], [From], [Message] from [Log] WHERE [Date] BETWEEN ? AND ? AND [Jid] = 'fido828@conference.jabber.ru' ORDER BY [Id]",
                        firstDate, secondDate))
                    {
                        while (reader.Read())
                        {
                            sw.WriteLine(string.Format("{0} {1} {2}",
                                reader.GetDateTime(0).ToShortTimeString(),
                                reader.GetString(1),
                                reader.GetString(2)));
                        }
                    }
                }
            }
        }
    }
}
