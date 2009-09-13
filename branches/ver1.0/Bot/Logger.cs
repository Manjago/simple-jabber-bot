using System;
using log4net;
using log4net.Config;

[assembly: XmlConfigurator(Watch = true)]
namespace Temnenkov.SJB.Bot
{
    internal static class Logger
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Bot));

        internal static void Log(LogType type, string message)
        {
            Log(type, message, null);
        }

        internal static void Log(LogType type, string message, Exception ex)
        {
            switch (type)
            {
                case LogType.Debug:
                    log.Debug(message, ex);
                    break;
                case LogType.Error:
                    log.Error(message, ex);
                    break;
                case LogType.Fatal:
                    log.Fatal(message, ex);
                    break;
                case LogType.Info:
                    log.Info(message, ex);
                    break;
                case LogType.Warn:
                    log.Warn(message, ex);
                    break;
            }
        }

    }

    internal enum LogType
    {
        Debug,
        Warn,
        Error,
        Info,
        Fatal
    }
}
