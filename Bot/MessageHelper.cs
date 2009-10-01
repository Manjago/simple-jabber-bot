using System;
using jabber.protocol.client;
using jabber.connection;

namespace Temnenkov.SJB.Bot
{
    internal static class MessageHelper
    {
        internal static bool IsDelayed(Message msg)
        {
#pragma warning disable 618,612
            return msg.X != null;
#pragma warning restore 618,612
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
