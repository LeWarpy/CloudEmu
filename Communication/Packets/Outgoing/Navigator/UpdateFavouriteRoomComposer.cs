namespace Cloud.Communication.Packets.Outgoing.Navigator
{
    public class UpdateFavouriteRoomComposer : ServerPacket
    {
        public UpdateFavouriteRoomComposer(int RoomId, bool Added)
            : base(ServerPacketHeader.UpdateFavouriteRoomMessageComposer)
        {
			WriteInteger(RoomId);
			WriteBoolean(Added);
        }
    }
}