using Cloud.HabboHotel.Rooms;
using Cloud.Communication.Packets.Outgoing.Navigator;
using Cloud.HabboHotel.Navigator;

namespace Cloud.Communication.Packets.Incoming.Navigator
{
    class CreateFlatEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null)
                return;
            
            if (Session.GetHabbo().UsersRooms.Count >= 500)
            {
                Session.SendMessage(new CanCreateRoomComposer(true, 500));
                return;
            }

            string word;
            string Name = Packet.PopString();
            Name = CloudServer.GetGame().GetChatManager().GetFilter().IsUnnaceptableWord(Name, out word) ? "Spam" : Name;
            string Description = Packet.PopString();
            Description = CloudServer.GetGame().GetChatManager().GetFilter().IsUnnaceptableWord(Description, out word) ? "Spam" : Description;
            string ModelName = Packet.PopString();

            int Category = Packet.PopInt();
            int MaxVisitors = Packet.PopInt();//10 = min, 25 = max.
            int TradeSettings = Packet.PopInt();//2 = All can trade, 1 = owner only, 0 = no trading.

            if (Name.Length < 3)
                return;

            if (Name.Length > 25)
                return;

            RoomModel RoomModel = null;
            if (!CloudServer.GetGame().GetRoomManager().TryGetModel(ModelName, out RoomModel))
                return;

            SearchResultList SearchResultList = null;
            if (!CloudServer.GetGame().GetNavigator().TryGetSearchResultList(Category, out SearchResultList))
                Category = 36;
            
            if (SearchResultList.CategoryType != NavigatorCategoryType.CATEGORY || SearchResultList.RequiredRank > Session.GetHabbo().Rank)
                Category = 36;

            if (MaxVisitors < 10 || MaxVisitors > 25)
                MaxVisitors = 10;

            if (TradeSettings < 0 || TradeSettings > 2)
                TradeSettings = 0;

            RoomData NewRoom = CloudServer.GetGame().GetRoomManager().CreateRoom(Session, Name, Description, ModelName, Category, MaxVisitors, TradeSettings);
            if (NewRoom != null)
            {
                Session.SendMessage(new FlatCreatedComposer(NewRoom.Id, Name));
            }
        }
    }
}
