using System.Collections.Generic;

namespace Cloud.Communication.Packets.Outgoing.Rooms.Notifications
{
	internal class RoomNotificationComposer : ServerPacket
    {
        public RoomNotificationComposer(string Type, string Key, string Value) : base(ServerPacketHeader.RoomNotificationMessageComposer)
        {
			WriteString(Type);
			WriteInteger(1);
			WriteString(Key);
			WriteString(Value);
        }

        public RoomNotificationComposer(string Type) : base(ServerPacketHeader.RoomNotificationMessageComposer)
        {
			WriteString(Type);
			WriteInteger(0);
        }

        public RoomNotificationComposer(string Title, string Message, string Image, string HotelName = "", string HotelURL = "", bool isBubble = false) : base(ServerPacketHeader.RoomNotificationMessageComposer)
        {
			WriteString(Image);
			WriteInteger(5);
			WriteString("title");
			WriteString(Title);
			WriteString("message");
			WriteString(Message);
			WriteString("linkUrl");
			WriteString(HotelURL);
			WriteString("linkTitle");
			WriteString(HotelName);
			WriteString("display");
			WriteString(isBubble ? "BUBBLE" : "POP_UP");
        }

        public RoomNotificationComposer(string Type, Dictionary<string, string> Keys) : base(ServerPacketHeader.RoomNotificationMessageComposer)
        {
			WriteString(Type);
			WriteInteger(Keys.Count);
            foreach (KeyValuePair<string, string> current in Keys)
            {
				WriteString(current.Key);
				WriteString(current.Value);
            }
        }

        public static ServerPacket SendBubble(string image, string message, string linkUrl = "")
        {
            var bubbleNotification = new ServerPacket(ServerPacketHeader.RoomNotificationMessageComposer);
            bubbleNotification.WriteString(image);
            bubbleNotification.WriteInteger(string.IsNullOrEmpty(linkUrl) ? 2 : 3);
            bubbleNotification.WriteString("display");
            bubbleNotification.WriteString("BUBBLE");
            bubbleNotification.WriteString("message");
            bubbleNotification.WriteString(message);
            if (string.IsNullOrEmpty(linkUrl)) return bubbleNotification;
            bubbleNotification.WriteString("linkUrl");
            bubbleNotification.WriteString(linkUrl);
            return bubbleNotification;
        }

        public static ServerPacket SendCustom(string Message)
        {
            var cuz = new ServerPacket(ServerPacketHeader.RoomNotificationMessageComposer);

            cuz.WriteInteger(1);
            cuz.WriteString(Message);

            return cuz;
        }

        public RoomNotificationComposer(string Text, string Image) : base(ServerPacketHeader.RoomNotificationMessageComposer)
        {
			WriteString(Image);
			WriteInteger(2);
			WriteString("message");
			WriteString(Text);
			WriteString("display");
			WriteString("BUBBLE");
        }

        public RoomNotificationComposer(string image, int messageType, string message, string link)
            : base(ServerPacketHeader.RoomNotificationMessageComposer)
        {
			WriteString(image);
			WriteInteger(messageType);
			WriteString("display");
			WriteString("BUBBLE");
			WriteString("message");
			WriteString(message);
			WriteString("linkUrl");
			WriteString(link);

        }
    }
}
