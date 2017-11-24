

namespace Cloud.Communication.Packets.Outgoing.Notifications
{
    class SuperNotificationComposer : ServerPacket
    {
        public SuperNotificationComposer(string image, string title, string message, string linkTitle = "", string linkUrl = "") :
            base(ServerPacketHeader.SuperNotificationMessageComposer)
        {
			WriteString(image);
			WriteInteger((linkTitle != string.Empty && linkUrl != string.Empty) ? 4 : 2);
			WriteString("titulo");
			WriteString(title);
			WriteString("menssage");
			WriteString(message);
			WriteString("linkTitle");
			WriteString(linkTitle);
			WriteString("linkUrl");
			WriteString(linkUrl);
        }
    }
}