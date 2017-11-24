using Cloud.Utilities;
using Cloud.HabboHotel.Rooms;
using Cloud.HabboHotel.Moderation;

namespace Cloud.Communication.Packets.Outgoing.Moderation
{
    class ModeratorTicketChatlogComposer : ServerPacket
    {
        public ModeratorTicketChatlogComposer(ModerationTicket Ticket, RoomData RoomData, double Timestamp)
            : base(ServerPacketHeader.ModeratorTicketChatlogMessageComposer)
        {
			WriteInteger(Ticket.Id);
			WriteInteger(Ticket.Sender != null ? Ticket.Sender.Id : 0);
			WriteInteger(Ticket.Reported != null ? Ticket.Reported.Id : 0);
			WriteInteger(RoomData.Id);

			WriteByte(1);
			WriteShort(2);//Count
			WriteString("roomName");
			WriteByte(2);
			WriteString(RoomData.Name);
			WriteString("roomId");
			WriteByte(1);
			WriteInteger(RoomData.Id);

			WriteShort(Ticket.ReportedChats.Count);
            foreach (string Chat in Ticket.ReportedChats)
            {
				WriteString(UnixTimestamp.FromUnixTimestamp(Timestamp).ToShortTimeString());
				WriteInteger(Ticket.Id);
				WriteString(Ticket.Reported != null ? Ticket.Reported.Username : "Sem usuário");
				WriteString(Chat);
				WriteBoolean(false);
            }
        }
    }
}
