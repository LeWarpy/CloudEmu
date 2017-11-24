using Cloud.Communication.Packets.Outgoing.Rooms.Session;
using Cloud.HabboHotel.Rooms;

namespace Cloud.Communication.Packets.Incoming.Navigator
{
    class FindRandomFriendingRoomEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            Room Instance = CloudServer.GetGame().GetRoomManager().TryGetRandomLoadedRoom();
            if (Instance != null)
                Session.SendMessage(new RoomForwardComposer(Instance.Id));
        }
    }
}
