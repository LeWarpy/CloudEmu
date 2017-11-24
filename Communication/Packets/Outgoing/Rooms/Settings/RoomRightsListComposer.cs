using System.Linq;

using Cloud.HabboHotel.Rooms;
using Cloud.HabboHotel.Cache.Type;

namespace Cloud.Communication.Packets.Outgoing.Rooms.Settings
{
	class RoomRightsListComposer : ServerPacket
    {
        public RoomRightsListComposer(Room Instance)
            : base(ServerPacketHeader.RoomRightsListMessageComposer)
        {
			WriteInteger(Instance.Id);

			WriteInteger(Instance.UsersWithRights.Count);
            foreach (int Id in Instance.UsersWithRights.ToList())
            {
                UserCache Data = CloudServer.GetGame().GetCacheManager().GenerateUser(Id);
                if (Data == null)
                {
					WriteInteger(0);
					WriteString("Unknown Error");
                }
                else
                {
					WriteInteger(Data.Id);
					WriteString(Data.Username);
                }
            }
        }
    }
}
