using Cloud.Communication.Packets.Outgoing.Navigator;
using Cloud.HabboHotel.Rooms;

namespace Cloud.Communication.Packets.Incoming.Navigator
{
    class GetGuestRoomEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int roomID = Packet.PopInt();

            RoomData roomData = CloudServer.GetGame().GetRoomManager().GenerateRoomData(roomID);
            if (roomData == null)
                return;

            bool isLoading = Packet.PopInt() == 1;
            bool checkEntry = Packet.PopInt() == 1;

            Session.SendMessage(new GetGuestRoomResultComposer(Session, roomData, isLoading, checkEntry));
        }
    }
}
