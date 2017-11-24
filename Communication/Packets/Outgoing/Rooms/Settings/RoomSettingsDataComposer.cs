
using Cloud.HabboHotel.Rooms;

namespace Cloud.Communication.Packets.Outgoing.Rooms.Settings
{
	class RoomSettingsDataComposer : ServerPacket
    {
        public RoomSettingsDataComposer(Room Room)
            : base(ServerPacketHeader.RoomSettingsDataMessageComposer)
        {
			WriteInteger(Room.RoomId);
			WriteString(Room.Name);
			WriteString(Room.Description);
			WriteInteger(RoomAccessUtility.GetRoomAccessPacketNum(Room.Access));
			WriteInteger(Room.Category);
			WriteInteger(Room.UsersMax);
			WriteInteger(((Room.RoomData.Model.MapSizeX * Room.RoomData.Model.MapSizeY) > 100) ? 50 : 25);

			WriteInteger(Room.Tags.Count);
            foreach (string Tag in Room.Tags.ToArray())
            {
				WriteString(Tag);
            }

			WriteInteger(Room.TradeSettings); //Trade
			WriteInteger(Room.AllowPets); // allows pets in room - pet system lacking, so always off
			WriteInteger(Room.AllowPetsEating);// allows pets to eat your food - pet system lacking, so always off
			WriteInteger(Room.RoomBlockingEnabled);
			WriteInteger(Room.Hidewall);
			WriteInteger(Room.WallThickness);
			WriteInteger(Room.FloorThickness);

			WriteInteger(Room.chatMode);//Chat mode
			WriteInteger(Room.chatSize);//Chat size
			WriteInteger(Room.chatSpeed);//Chat speed
			WriteInteger(Room.chatDistance);//Hearing Distance
			WriteInteger(Room.extraFlood);//Additional Flood

			WriteBoolean(true);

			WriteInteger(Room.WhoCanMute); // who can mute
			WriteInteger(Room.WhoCanKick); // who can kick
			WriteInteger(Room.WhoCanBan); // who can ban

        }
    }
}
