using System;

using Cloud.HabboHotel.Items;
using Cloud.Communication.Packets.Outgoing.Inventory.Furni;

using Cloud.Communication.Packets.Outgoing.Rooms.Furni;
using Cloud.HabboHotel.Items.Crafting;

namespace Cloud.Communication.Packets.Incoming.Rooms.Furni
{
	class ExecuteCraftingRecipeEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int craftingTable = Packet.PopInt();
            string RecetaFinal = Packet.PopString();
            //Console.WriteLine("Mesa:" + craftingTable);
            //Console.WriteLine("Items:" + Test);

            //List<Item> items = new List<Item>();

            //var count = Packet.PopInt();
            //Console.WriteLine("Item:" + count);
            //for (var i = 1; i <= count; i++)
            //{
            //    var id = Packet.PopInt();
            //    Console.WriteLine("Item:" + id);

            //    var item = Session.GetHabbo().GetInventoryComponent().GetItem(id);
            //    if (item == null || items.Contains(item))
            //        return;

            //    items.Add(item);
            //}



            CraftingRecipe recipe = CloudServer.GetGame().GetCraftingManager().GetRecipeByPrize(RecetaFinal);

            if (recipe == null) return;
            Console.WriteLine("Olá");
            ItemData resultItem = CloudServer.GetGame().GetItemManager().GetItemByName(recipe.Result);
            if (resultItem == null) return;
            bool success = true;
            foreach (var need in recipe.ItemsNeeded)
            {
                for (var i = 1; i <= need.Value; i++)
                {
                    ItemData item = CloudServer.GetGame().GetItemManager().GetItemByName(need.Key);
                    if (item == null)
                    {
                        success = false;
                        continue;
                    }

                    var inv = Session.GetHabbo().GetInventoryComponent().GetFirstItemByBaseId(item.Id);
                    if (inv == null)
                    {
                        success = false;
                        continue;
                    }

                    using (var dbClient = CloudServer.GetDatabaseManager().GetQueryReactor()) dbClient.RunQuery("DELETE FROM `items` WHERE `id` = '" + inv.Id + "' AND `user_id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
                    Session.GetHabbo().GetInventoryComponent().RemoveItem(inv.Id);
                    Console.WriteLine(inv.Id);
                }
            }

            Session.GetHabbo().GetInventoryComponent().UpdateItems(true);

            if (success)
            {
                Session.GetHabbo().GetInventoryComponent().AddNewItem(0, resultItem.Id, "", 0, true, false, 0, 0);
                Session.SendMessage(new FurniListUpdateComposer());
                Session.GetHabbo().GetInventoryComponent().UpdateItems(true);
                Session.SendMessage(new CraftableProductsComposer());


                //int xd = recipe.type;
                //Console.WriteLine(recipe.type);
                switch (recipe.Type)
                {
                    case 1:
                        CloudServer.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CrystalCracker", 1);
                        break;

                    case 2:
                        CloudServer.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_PetLover", 1);
                        break;

                    case 3:
                        CloudServer.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_PetLover", 1);
                        break;
                }
            }

            Session.SendMessage(new CraftingResultComposer(recipe, success));
            return;
        }
    }
}