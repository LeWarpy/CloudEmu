using Cloud.HabboHotel.Moderation;
using Cloud.Communication.Packets.Outgoing.Moderation;

namespace Cloud.Communication.Packets.Incoming.Moderation
{
    class GetModeratorTicketChatlogsEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null || !Session.GetHabbo().GetPermissions().HasRight("mod_tickets"))
                return;

            int ticketId = Packet.PopInt();

            ModerationTicket ticket = null;
            if (!CloudServer.GetGame().GetModerationManager().TryGetTicket(ticketId, out ticket) || ticket.Room == null)
                return;

            if (ticket.Room == null)
                return;

            Session.SendMessage(new ModeratorTicketChatlogComposer(ticket, ticket.Room, ticket.Timestamp));
        }
    }
}
