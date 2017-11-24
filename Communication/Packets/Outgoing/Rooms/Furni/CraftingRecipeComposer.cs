using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Cloud.HabboHotel.Items.Crafting;

namespace Cloud.Communication.Packets.Outgoing.Rooms.Furni
{
    class CraftingRecipeComposer : ServerPacket
    {
        public CraftingRecipeComposer(CraftingRecipe recipe) : base(ServerPacketHeader.CraftingRecipeMessageComposer)
        {
            base.WriteInteger(recipe.ItemsNeeded.Count);
            foreach (var item in recipe.ItemsNeeded)
            {
                base.WriteInteger(item.Value);
                base.WriteString(item.Key);
            }
        }
    }
}