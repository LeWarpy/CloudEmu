using Cloud.Communication.Packets.Outgoing.Catalog;
using Cloud.HabboHotel.GameClients;
using Cloud.HabboHotel.Items;

namespace Cloud.Communication.Packets.Incoming.Catalog
{
    public class GetSellablePetBreedsEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            string Type = Packet.PopString();

            ItemData Item = CloudServer.GetGame().GetItemManager().GetItemByName(Type);
            if (Item == null)
                return;

            int PetId = Item.BehaviourData;

            Session.SendMessage(new SellablePetBreedsComposer(Type, PetId, CloudServer.GetGame().GetCatalog().GetPetRaceManager().GetRacesForRaceId(PetId)));
        }
    }
}