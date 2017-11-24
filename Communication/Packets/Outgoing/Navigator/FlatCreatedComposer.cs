namespace Cloud.Communication.Packets.Outgoing.Navigator
{
	class FlatCreatedComposer : ServerPacket
    {
        public FlatCreatedComposer(int roomID, string roomName)
            : base(ServerPacketHeader.FlatCreatedMessageComposer)
        {
			WriteInteger(roomID);
			WriteString(roomName);
        }
    }
}
