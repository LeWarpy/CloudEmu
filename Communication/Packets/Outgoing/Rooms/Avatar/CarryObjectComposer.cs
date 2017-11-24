﻿namespace Cloud.Communication.Packets.Outgoing.Rooms.Avatar
{
	class CarryObjectComposer : ServerPacket
    {
        public CarryObjectComposer(int virtualID, int itemID)
            : base(ServerPacketHeader.CarryObjectMessageComposer)
        {
			WriteInteger(virtualID);
			WriteInteger(itemID);
        }
    }
}
