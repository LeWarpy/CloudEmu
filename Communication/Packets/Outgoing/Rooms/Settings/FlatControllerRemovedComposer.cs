
using Cloud.HabboHotel.Rooms;

namespace Cloud.Communication.Packets.Outgoing.Rooms.Settings
{
	class FlatControllerRemovedComposer : ServerPacket
    {
        public FlatControllerRemovedComposer(Room Instance, int UserId)
            : base(ServerPacketHeader.FlatControllerRemovedMessageComposer)
        {
			WriteInteger(Instance.Id);
			WriteInteger(UserId);
        }
    }
}
