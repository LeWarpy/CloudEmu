﻿

namespace Cloud.Communication.Packets.Outgoing.Rooms.Notifications
{
    class RoomErrorNotifComposer : ServerPacket
    {
        public RoomErrorNotifComposer(int Error)
            : base(ServerPacketHeader.RoomErrorNotifMessageComposer)
        {
			WriteInteger(Error);
        }
    }
}
