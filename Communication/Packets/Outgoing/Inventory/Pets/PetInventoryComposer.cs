using System.Linq;
using System.Collections.Generic;

using Cloud.HabboHotel.Rooms.AI;

namespace Cloud.Communication.Packets.Outgoing.Inventory.Pets
{
	class PetInventoryComposer : ServerPacket
    {
        public PetInventoryComposer(ICollection<Pet> Pets)
            : base(ServerPacketHeader.PetInventoryMessageComposer)
        {
			WriteInteger(1);
			WriteInteger(1);
			WriteInteger(Pets.Count);
            foreach (Pet Pet in Pets.ToList())
            {
				WriteInteger(Pet.PetId);
				WriteString(Pet.Name);
				WriteInteger(Pet.Type);
				WriteInteger(int.Parse(Pet.Race));
				WriteString(Pet.Color);
				WriteInteger(0);
                if (Pet.Type == 15)
                {
					WriteInteger(4);
					WriteInteger(1);
					WriteInteger(-1);
					WriteInteger(int.Parse(Pet.Race));
					WriteInteger(2);
					WriteInteger(Pet.PetHair);
					WriteInteger(Pet.HairDye);
					WriteInteger(3);
					WriteInteger(Pet.PetHair);
					WriteInteger(Pet.HairDye);
					WriteInteger(4);
					WriteInteger(Pet.Saddle);
					WriteInteger(0);
                }
                else
					WriteInteger(0);
				WriteInteger(0);
            }
        }
    }
}