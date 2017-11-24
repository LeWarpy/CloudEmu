using Cloud.HabboHotel.Users;
using Cloud.Core;

namespace Cloud.Communication.Packets.Outgoing.Handshake
{
    public class UserRightsComposer : ServerPacket
    {
        public UserRightsComposer(Habbo habbo)
            : base(ServerPacketHeader.UserRightsMessageComposer)
        {
            if (habbo.GetClubManager().HasSubscription("habbo_vip"))
                base.WriteInteger(2);
            else if (habbo.GetClubManager().HasSubscription("habbo_club"))
                base.WriteInteger(1);
            else
                base.WriteInteger(0);

            base.WriteInteger(habbo.Rank);
            if (habbo.Rank >= ExtraSettings.AmbassadorMinRank)
                base.WriteBoolean(true);
            else
                base.WriteBoolean(false);
        }
    }
}