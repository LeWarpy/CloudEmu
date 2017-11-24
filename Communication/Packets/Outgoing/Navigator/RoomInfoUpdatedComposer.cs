namespace Cloud.Communication.Packets.Outgoing.Navigator
{
	class RoomInfoUpdatedComposer : ServerPacket
    {
        public RoomInfoUpdatedComposer(int roomID)
            : base(ServerPacketHeader.RoomInfoUpdatedMessageComposer)
        {
			WriteInteger(roomID);
        }
    }
}
