using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Temnenkov.SimpleJabberBot
{
    internal class Bot
    {
        internal void Connect()
        {
            Logger.Log(LogType.Info, "Connect");
        }

        internal void Disconnect()
        {
            Logger.Log(LogType.Info, "Disconnect");
        }
    }
}
