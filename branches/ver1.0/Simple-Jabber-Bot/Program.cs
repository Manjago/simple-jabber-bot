using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Temnenkov.SimpleJabberBot
{
    internal class Program
    {
        internal static void Main(string[] args)
        {
            Logger.Log(LogType.Info, "Starting in console mode");

            var bot = new Bot();
            bot.Connect();

            Console.WriteLine("Type \"quit\" to end.");
            while (!Console.ReadLine().Equals("quit", StringComparison.InvariantCultureIgnoreCase))
                Console.WriteLine("Type \"quit\" to end.");

            Logger.Log(LogType.Info, "Quitting from console mode");
            bot.Disconnect();
        }
    }
}
