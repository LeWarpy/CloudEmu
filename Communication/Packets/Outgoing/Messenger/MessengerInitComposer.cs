using System;

namespace Cloud.Communication.Packets.Outgoing.Messenger
{
	class MessengerInitComposer : ServerPacket
    {
        public MessengerInitComposer(HabboHotel.GameClients.GameClient Session)
            : base(ServerPacketHeader.MessengerInitMessageComposer)
        {
			WriteInteger(Convert.ToInt32(CloudServer.GetGame().GetSettingsManager().TryGetValue("messenger.buddy_limit")));//Friends max.
			WriteInteger(300);
			WriteInteger(800);
			WriteInteger(1); // category count
			WriteInteger(1);
			WriteString("Grupos");
        }
    }
}
