using System;
namespace Temnenkov.SJB.Common
{
    public interface ILogger
    {
        void Log(LogType type, string message, Exception ex);
        void Log(LogType type, string message);
    }

    public enum LogType
    {
        Debug,
        Warn,
        Error,
        Info,
        Fatal
    }
}