using System.Linq;
using System.Collections.Generic;
using Cloud.HabboHotel.Rooms;
using Cloud.HabboHotel.GameClients;
using Cloud.Communication.Packets.Outgoing.Catalog;

namespace Cloud.Communication.Packets.Incoming.Catalog
{
	class GetPromotableRoomsEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            List<RoomData> Rooms = Session.GetHabbo().UsersRooms;
            Rooms = Rooms.Where(x => (x.Promotion == null || x.Promotion.TimestampExpires < CloudServer.GetUnixTimestamp())).ToList();
            Session.SendMessage(new PromotableRoomsComposer(Rooms));
        }
    }
}
