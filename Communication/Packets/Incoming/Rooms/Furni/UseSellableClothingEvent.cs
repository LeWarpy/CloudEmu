
using Cloud.HabboHotel.Items;
using Cloud.HabboHotel.Rooms;
using Cloud.HabboHotel.Catalog.Clothing;
using Cloud.Communication.Packets.Outgoing.Rooms.Notifications;
using Cloud.Communication.Packets.Outgoing.Inventory.AvatarEffects;
using Cloud.Database.Interfaces;

namespace Cloud.Communication.Packets.Incoming.Rooms.Furni
{
	class UseSellableClothingEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null || !Session.GetHabbo().InRoom)
                return;

            Room Room = Session.GetHabbo().CurrentRoom;
            if (Room == null)
                return;

            int ItemId = Packet.PopInt();

            Item Item = Room.GetRoomItemHandler().GetItem(ItemId);
            if (Item == null)
                return;

            if (Item.Data == null)
                return;

            if (Item.UserID != Session.GetHabbo().Id)
                return;

            if (Item.Data.InteractionType != InteractionType.PURCHASABLE_CLOTHING)
            {
                Session.SendNotification("Ops, deu ruim ae em, chama um staff!");
                return;
            }

            if (Item.Data.BehaviourData == 0)
            {
                Session.SendNotification("Ops, este artigo não tem nenhuma configurações de roupa, por favor informe!");
                return;
            }

            ClothingItem Clothing = null;
            if (!CloudServer.GetGame().GetCatalog().GetClothingManager().TryGetClothing(Item.Data.ClothingId, out Clothing))
            {
                Session.SendNotification("Ops! essa parte da roupa não foi encontrada!");
                return;
            }

            //Quickly delete it from the database.
            using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("DELETE FROM `items` WHERE `id` = @ItemId LIMIT 1");
                dbClient.AddParameter("ItemId", Item.Id);
                dbClient.RunQuery();
            }

            //Remove the item.
            Room.GetRoomItemHandler().RemoveFurniture(Session, Item.Id);

            Session.GetHabbo().GetClothing().AddClothing(Clothing.ClothingName, Clothing.PartIds);
            Session.SendMessage(new FigureSetIdsComposer(Session.GetHabbo().GetClothing().GetClothingParts));
            Session.SendMessage(new RoomNotificationComposer("figureset.redeemed.success"));
            Session.SendWhisper("Por algum motivo você não pode ver as suas roupas novas, tente novamente!");
        }
    }
}
