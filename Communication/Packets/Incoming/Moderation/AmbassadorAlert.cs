using Cloud.Communication.Packets.Outgoing.Notifications;
using Cloud.HabboHotel.GameClients;
using Cloud.Core;

namespace Cloud.Communication.Packets.Incoming.Moderation
{
    class AmbassadorAlert : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session.GetHabbo().Rank < ExtraSettings.AmbassadorMinRank) return;
            int userId = Packet.PopInt();
            GameClient user = CloudServer.GetGame().GetClientManager().GetClientByUserID(userId);
            if (user == null) return;
            user.SendMessage(new SuperNotificationComposer("", "${notification.ambassador.alert.warning.title}", "${notification.ambassador.alert.warning.message}", "", ""));
        }
    }
}