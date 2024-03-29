﻿using System;
using System.Data;
using Cloud.HabboHotel.Items;
using Cloud.Communication.Packets.Outgoing.Marketplace;
using Cloud.Communication.Packets.Outgoing.Inventory.Furni;
using Cloud.Database.Interfaces;

namespace Cloud.Communication.Packets.Incoming.Marketplace
{
    class CancelOfferEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null)
                return;

            DataRow Row = null;
            int OfferId = Packet.PopInt();

            using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `furni_id`, `item_id`, `user_id`, `extra_data`, `offer_id`, `state`, `timestamp`, `limited_number`, `limited_stack` FROM `catalog_marketplace_offers` WHERE `offer_id` = @OfferId LIMIT 1");
                dbClient.AddParameter("OfferId", OfferId);
                Row = dbClient.getRow();
            }

            if (Row == null)
            {
                Session.SendMessage(new MarketplaceCancelOfferResultComposer(OfferId, false));
                return;
            }

            if (Convert.ToInt32(Row["user_id"]) != Session.GetHabbo().Id)
            {
                Session.SendMessage(new MarketplaceCancelOfferResultComposer(OfferId, false));
                return;
            }

            ItemData Item = null;
            if (!CloudServer.GetGame().GetItemManager().GetItem(Convert.ToInt32(Row["item_id"]), out Item))
            {
                Session.SendMessage(new MarketplaceCancelOfferResultComposer(OfferId, false));
                return;
            }

            //CloudServer.GetGame().GetCatalog().DeliverItems(Session, Item, 1, Convert.ToString(Row["extra_data"]), Convert.ToInt32(Row["limited_number"]), Convert.ToInt32(Row["limited_stack"]), Convert.ToInt32(Row["furni_id"]));

            Item GiveItem = ItemFactory.CreateSingleItem(Item, Session.GetHabbo(), Convert.ToString(Row["extra_data"]), Convert.ToString(Row["extra_data"]), Convert.ToInt32(Row["furni_id"]), Convert.ToInt32(Row["limited_number"]), Convert.ToInt32(Row["limited_stack"]));
            Session.SendMessage(new FurniListNotificationComposer(GiveItem.Id, 1));
            Session.SendMessage(new FurniListUpdateComposer());

            using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("DELETE FROM `catalog_marketplace_offers` WHERE `offer_id` = @OfferId AND `user_id` = @UserId LIMIT 1");
                dbClient.AddParameter("OfferId", OfferId);
                dbClient.AddParameter("UserId", Session.GetHabbo().Id);
                dbClient.RunQuery();
            }

            Session.GetHabbo().GetInventoryComponent().UpdateItems(true);
            Session.SendMessage(new MarketplaceCancelOfferResultComposer(OfferId, true));
        }
    }
}
