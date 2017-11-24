using Cloud.Communication.Packets.Outgoing.Moderation;
using Cloud.HabboHotel.GameClients;
using Cloud.HabboHotel.Moderation;

namespace Cloud.Communication.Packets.Incoming.Moderation
{
    class CallForHelpPendingCallsDeletedEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session == null || session.GetHabbo() == null)
                return;
            
            if (CloudServer.GetGame().GetModerationManager().UserHasTickets(session.GetHabbo().Id))
            {
                ModerationTicket PendingTicket = CloudServer.GetGame().GetModerationManager().GetTicketBySenderId(session.GetHabbo().Id);
                if (PendingTicket != null)
                {
                    PendingTicket.Answered = true;
                    CloudServer.GetGame().GetClientManager().SendMessage(new ModeratorSupportTicketComposer(session.GetHabbo().Id, PendingTicket), "mod_tool");
                }
            }
        }
    }
}
