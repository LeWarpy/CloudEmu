using Cloud.Communication.Packets.Outgoing.Rooms.Furni;
using Cloud.HabboHotel.Items.Crafting;

namespace Cloud.Communication.Packets.Incoming.Rooms.Furni
{
	class SetCraftingRecipeEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            var result = Packet.PopString();

            CraftingRecipe recipe = null;
            foreach (CraftingRecipe Receta in CloudServer.GetGame().GetCraftingManager().CraftingRecipes.Values)
            {
                if (Receta.Result.Contains(result))
                {
                    recipe = Receta;
                    break;
                }
            }

            var Final = CloudServer.GetGame().GetCraftingManager().GetRecipe(recipe.Id);
            if (Final == null) return;
            Session.SendMessage(new CraftingRecipeComposer(Final));
        }

    }
}