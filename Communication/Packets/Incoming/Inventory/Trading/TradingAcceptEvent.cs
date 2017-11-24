using Cloud.HabboHotel.Rooms;
using Cloud.HabboHotel.Rooms.Trading;

namespace Cloud.Communication.Packets.Incoming.Inventory.Trading
{
    class TradingAcceptEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null || !Session.GetHabbo().InRoom)
                return;

            Room Room;

            if (!CloudServer.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room))
                return;

            if (!Room.CanTradeInRoom)
                return;

            Trade Trade = Room.GetUserTrade(Session.GetHabbo().Id);
            if (Trade == null)
                return;

            Trade.Accept(Session.GetHabbo().Id);
        }
    }
}