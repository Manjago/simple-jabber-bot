using System;
using Temnenkov.SJB.Common;

namespace Temnenkov.SJB.Bot
{
    internal class Program
    {
        internal static void Main(string[] args)
        {
            Logger.Log(LogType.Info, "Starting in console mode");

            var bot = new Bot();
            if (bot.Connect())
            {
                Logger.Log(LogType.Info, "Successfully connected");

                if (bot.JoinRoom(Settings.RoomJid))
                {
                    Console.WriteLine("Type \"quit\" to end.");
                    while (!Console.ReadLine().Equals("quit", StringComparison.InvariantCultureIgnoreCase))
                        Console.WriteLine("Type \"quit\" to end.");
                }
                else
                    Logger.Log(LogType.Warn, "Fail join room");
            }
            else
                Logger.Log(LogType.Warn, "Fail connect");

            Logger.Log(LogType.Info, "Quitting from console mode");
            bot.Disconnect();
        }
    }
}
