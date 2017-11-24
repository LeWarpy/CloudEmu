using Cloud.HabboHotel.Rooms;
using Cloud.HabboHotel.Items;

namespace Cloud.Communication.Packets.Incoming.Rooms.Games
{
	class FootballGateComponent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null)
                return;

            int RoomId = Session.GetHabbo().CurrentRoomId;

            Room Room = Session.GetHabbo().CurrentRoom;
            if (Room == null || !Room.CheckRights(Session, true))
                return;

            int ItemId = Packet.PopInt();

            Item Item = Room.GetRoomItemHandler().GetItem(ItemId);
            if (Item == null || Item.GetBaseItem() == null || Item.GetBaseItem().InteractionType != InteractionType.FOOTBALL_GATE)
                return;

            string Gender = Packet.PopString();
            string Look = Packet.PopString();

            if (Gender.ToUpper() == "M")
            {
                Item.ExtraData = Look + "," + Item.ExtraData.Split(',')[1];
            }
            else if (Gender.ToUpper() == "F")
            {
                Item.ExtraData = Item.ExtraData.Split(',')[0] + "," + Look;
            }

            Item.UpdateState();
        }
    }
}
