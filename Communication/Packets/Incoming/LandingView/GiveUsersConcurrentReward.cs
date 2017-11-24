using Cloud.Communication.Packets.Outgoing.Inventory.Purse;
using Cloud.Communication.Packets.Outgoing.Rooms.Notifications;


namespace Cloud.Communication.Packets.Incoming.LandingView
{
    class GiveConcurrentUsersReward : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session.GetHabbo().GetStats().PurchaseUsersConcurrent)
            {
                Session.SendMessage(new RoomAlertComposer("Você recebeu este prêmio."));
            }

            string badge = CloudServer.GetGame().GetSettingsManager().TryGetValue("usersconcurrent_badge");
            int pixeles = int.Parse(CloudServer.GetGame().GetSettingsManager().TryGetValue("usersconcurrent_pixeles"));

            Session.GetHabbo().GOTWPoints = Session.GetHabbo().GOTWPoints + pixeles;
            Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().GOTWPoints, pixeles, 103));
            Session.GetHabbo().GetBadgeComponent().GiveBadge(badge, true, Session);
            Session.SendMessage(new RoomAlertComposer("Você recebeu um emblema e " + pixeles + " pixels."));
            Session.GetHabbo().GetStats().PurchaseUsersConcurrent = true;
        }
    }
}
