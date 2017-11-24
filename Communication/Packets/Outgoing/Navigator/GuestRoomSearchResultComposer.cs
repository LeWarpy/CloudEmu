using System.Collections.Generic;
using Cloud.HabboHotel.Rooms;

namespace Cloud.Communication.Packets.Outgoing.Navigator
{
	class GuestRoomSearchResultComposer : ServerPacket
    {
       public GuestRoomSearchResultComposer(int Mode, string UserQuery, ICollection<RoomData> Rooms)
           : base(ServerPacketHeader.GuestRoomSearchResultMessageComposer)
       {
			WriteInteger(Mode);
			WriteString(UserQuery);

			WriteInteger(Rooms.Count);
           foreach (RoomData data in Rooms)
           {
               RoomAppender.WriteRoom(this, data, data.Promotion);
           }

			WriteBoolean(false);
       }
    }
}
