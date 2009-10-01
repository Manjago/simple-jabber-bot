using System;
using System.Collections.Generic;
using System.Text;
using Temnenkov.SJB.Common;

namespace Temnenkov.SJB.ConfLogPlugin.Business
{
    internal class Protocol 
    {
        private List<PersistentLine> Lines {get; set;}

        private Protocol()
        {
            Lines = new List<PersistentLine>();
        }

        internal static Protocol Load(PersistentLineDataLayer dal, string jid,
            DateTime perBeg, DateTime perEnd)
        {
            var result = new Protocol();
            dal.Load(result.Lines, jid, perBeg, perEnd);
            return result;
        }

        internal string Export(bool withDate)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < Lines.Count - 1; ++i)
                sb.AppendLine(Lines[i].DisplayString(withDate));
            return sb.ToString();
        }

    }
}
