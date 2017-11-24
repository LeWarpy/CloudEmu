﻿using System.Collections.Generic;
using Cloud.Utilities;
using Cloud.HabboHotel.Rooms;
using Cloud.HabboHotel.Users;

namespace Cloud.Communication.Packets.Outgoing.Moderation
{
    class ModeratorUserRoomVisitsComposer : ServerPacket
    {
        public ModeratorUserRoomVisitsComposer(Habbo Data, Dictionary<double, RoomData> Visits)
            : base(ServerPacketHeader.ModeratorUserRoomVisitsMessageComposer)
        {
			WriteInteger(Data.Id);
			WriteString(Data.Username);
			WriteInteger(Visits.Count);

            foreach (KeyValuePair<double, RoomData> Visit in Visits)
            {
				WriteInteger(Visit.Value.Id);
				WriteString(Visit.Value.Name);
				WriteInteger(UnixTimestamp.FromUnixTimestamp(Visit.Key).Hour);
				WriteInteger(UnixTimestamp.FromUnixTimestamp(Visit.Key).Minute);
            }
        }
    }
}
