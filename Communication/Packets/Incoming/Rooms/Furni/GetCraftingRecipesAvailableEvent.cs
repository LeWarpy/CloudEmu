using System.Linq;
using System.Collections.Generic;
using Cloud.HabboHotel.Items;
using Cloud.Communication.Packets.Outgoing.Rooms.Furni;
using Cloud.HabboHotel.Items.Crafting;

namespace Cloud.Communication.Packets.Incoming.Rooms.Furni
{
    class GetCraftingRecipesAvailableEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int craftingTable = Packet.PopInt();
            List<Item> items = new List<Item>();

            var count = Packet.PopInt();
            for (var i = 1; i <= count; i++)
            {
                var id = Packet.PopInt();

                var item = Session.GetHabbo().GetInventoryComponent().GetItem(id);
                if (item == null || items.Contains(item))
                    return;

                items.Add(item);
            }

            CraftingRecipe craftingRecipe = null;
            foreach (var recipe in CloudServer.GetGame().GetCraftingManager().CraftingRecipes)
            {
                bool found = false;

                foreach (var item in recipe.Value.ItemsNeeded)
                {
                    if (item.Value != items.Count(item2 => item2.GetBaseItem().ItemName == item.Key))
                    {
                        found = false;
                        break;
                    }

                    found = true;
                }

                if (found == false)
                    continue;

                craftingRecipe = recipe.Value;
                break;
            }

            if (craftingRecipe == null)
            {
                Session.SendMessage(new CraftingFoundComposer(count, false));
                return;
            }


            Session.SendMessage(new CraftingFoundComposer(count, true));

        }

    }
}