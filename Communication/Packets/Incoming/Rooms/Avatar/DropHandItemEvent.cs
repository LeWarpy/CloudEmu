using Cloud.HabboHotel.Rooms;


namespace Cloud.Communication.Packets.Incoming.Rooms.Avatar
{
    class DropHandItemEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().InRoom)
                return;

            Room Room;

            if (!CloudServer.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room))
                return;

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
                return;

            if (User.CarryItemID > 0 && User.CarryTimer > 0)
                User.CarryItem(0);
        }
    }
}
