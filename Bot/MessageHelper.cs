using System;
using System.Collections.Generic;
using System.Text;
using jabber.protocol.client;
using jabber.connection;

namespace Temnenkov.SJB.Bot
{
    internal static class MessageHelper
    {
        internal static bool IsDelayed(Message msg)
        {
            return msg.X != null;
        }

        internal static bool IsEmpty(Message msg)
        {
            return string.IsNullOrEmpty(msg.Body);
        }

        private static bool IsCommang(Message msg, string cmd)
        {
            return !IsDelayed(msg) && !IsEmpty(msg) &&
                msg.Body.Equals(cmd, StringComparison.InvariantCultureIgnoreCase);
        }

        internal static bool IsPingCommand(Message msg)
        {
            return IsCommang(msg, "ping");
        }

        internal static bool IsLogCommand(Message msg)
        {
            return IsCommang(msg, "log");
        }

        internal static bool IsFromRoomMessage(Message msg, Room room)
        {
            return msg.From.Bare != null &&
                   room != null &&
                   room.JID != null &&
                   room.JID.Bare != null &&
                   msg.From.Bare == room.JID.Bare;        
        }

    }
}
