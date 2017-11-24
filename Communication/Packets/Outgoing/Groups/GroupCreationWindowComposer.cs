using System;
using System.Collections.Generic;

using Cloud.HabboHotel.Rooms;

namespace Cloud.Communication.Packets.Outgoing.Groups
{
	class GroupCreationWindowComposer : ServerPacket
    {
        public GroupCreationWindowComposer(ICollection<RoomData> Rooms)
            : base(ServerPacketHeader.GroupCreationWindowMessageComposer)
        {
			WriteInteger(Convert.ToInt32(CloudServer.GetGame().GetSettingsManager().TryGetValue("catalog.group.purchase.cost")));//Price

			WriteInteger(Rooms.Count);//Room count that the user has.
            foreach (RoomData Room in Rooms)
            {
				WriteInteger(Room.Id);//Room Id
				WriteString(Room.Name);//Room Name
				WriteBoolean(false);//What?
            }

			WriteInteger(5);
			WriteInteger(5);
			WriteInteger(11);
			WriteInteger(4);

			WriteInteger(6);
			WriteInteger(11);
			WriteInteger(4);

			WriteInteger(0);
			WriteInteger(0);
			WriteInteger(0);

			WriteInteger(0);
			WriteInteger(0);
			WriteInteger(0);

			WriteInteger(0);
			WriteInteger(0);
			WriteInteger(0);
        }
    }
}
