using Cloud.HabboHotel.Rooms;
using Cloud.Communication.Packets.Outgoing.Rooms.AI.Pets;
using Cloud.Database.Interfaces;


namespace Cloud.Communication.Packets.Incoming.Rooms.AI.Pets.Horse
{
	class ModifyWhoCanRideHorseEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().InRoom)
                return;

            Room Room = null;
            if (!CloudServer.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room))
                return;

            int PetId = Packet.PopInt();
           
            RoomUser Pet = null;
            if (!Room.GetRoomUserManager().TryGetPet(PetId, out Pet))
                return;

            if (Pet.PetData.AnyoneCanRide == 1)
                Pet.PetData.AnyoneCanRide = 0;
            else
                Pet.PetData.AnyoneCanRide = 1;


            using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.runFastQuery("UPDATE `bots_petdata` SET `anyone_ride` = '" + Pet.PetData.AnyoneCanRide + "' WHERE `id` = '" + PetId + "' LIMIT 1");
            }

            Room.SendMessage(new PetInformationComposer(Pet.PetData));
        }
    }
}
