using Cloud.HabboHotel.Rooms;

namespace Cloud.Communication.Packets.Incoming.Navigator
{
    class GoToHotelViewEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null)
                return;


            if (Session.GetHabbo().InRoom)
            {
                Room OldRoom;

                if (!CloudServer.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out OldRoom))
                    return;

                if (OldRoom.GetRoomUserManager() != null)
                    OldRoom.GetRoomUserManager().RemoveUserFromRoom(Session, true, false);
            }

            Session.GetHabbo().IsTeleporting = false;
            Session.GetHabbo().TeleportingRoomID = 0;
            Session.GetHabbo().TeleporterId = 0;
            Session.GetHabbo().CurrentRoomId = 0;
        }
    }
}
