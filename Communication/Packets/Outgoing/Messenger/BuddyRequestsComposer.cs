using System.Collections.Generic;
using Cloud.HabboHotel.Users.Messenger;
using Cloud.HabboHotel.Cache.Type;

namespace Cloud.Communication.Packets.Outgoing.Messenger
{
	class BuddyRequestsComposer : ServerPacket
    {
        public BuddyRequestsComposer(ICollection<MessengerRequest> Requests)
            : base(ServerPacketHeader.BuddyRequestsMessageComposer)
        {
			WriteInteger(Requests.Count);
			WriteInteger(Requests.Count);

            foreach (MessengerRequest Request in Requests)
            {
				WriteInteger(Request.From);
				WriteString(Request.Username);

                UserCache User = CloudServer.GetGame().GetCacheManager().GenerateUser(Request.From);
				WriteString(User != null ? User.Look : "");
            }
        }
    }
}
