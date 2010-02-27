using System;
using Temnenkov.SJB.Common;
using System.ServiceProcess;

namespace Temnenkov.SJB.Bot
{
    internal class Program
    {
        internal static void Main(string[] args)
        {

            if (args.Length > 0 && !args[0].Equals("srv", StringComparison.CurrentCultureIgnoreCase))
            {
                Logger.Log(LogType.Info, "Starting in console mode");

                var bot = new Bot();
                if (bot.Connect())
                {
                    Logger.Log(LogType.Info, "Successfully connected");

                    if (bot.JoinRoom(string.Format("{0}/{1}", Settings.RoomJid, Settings.NameInRoom)))
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
            else
                ServiceBase.Run(new ServiceBase[] { new BotAsService() });

        }
    }
}
