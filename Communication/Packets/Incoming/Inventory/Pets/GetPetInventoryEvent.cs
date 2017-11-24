using System.Collections.Generic;

using Cloud.HabboHotel.Rooms.AI;
using Cloud.Communication.Packets.Outgoing.Inventory.Pets;

namespace Cloud.Communication.Packets.Incoming.Inventory.Pets
{
    class GetPetInventoryEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session.GetHabbo().GetInventoryComponent() == null)
                return;

            ICollection<Pet> Pets = Session.GetHabbo().GetInventoryComponent().GetPets();
            Session.SendMessage(new PetInventoryComposer(Pets));
        }
    }
}