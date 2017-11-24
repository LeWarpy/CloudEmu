using System.Linq;
using Cloud.HabboHotel.Rooms;
using Cloud.HabboHotel.Cache.Type;

namespace Cloud.Communication.Packets.Outgoing.Rooms.Settings
{
	class GetRoomBannedUsersComposer : ServerPacket
    {
        public GetRoomBannedUsersComposer(Room Instance)
            : base(ServerPacketHeader.GetRoomBannedUsersMessageComposer)
        {
			WriteInteger(Instance.Id);

			WriteInteger(Instance.GetBans().BannedUsers().Count);//Count
            foreach (int Id in Instance.GetBans().BannedUsers().ToList())
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
