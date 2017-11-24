namespace Cloud.Communication.Packets.Outgoing.Moderation
{
	class ModeratorSupportTicketResponseComposer : ServerPacket
    {
        public ModeratorSupportTicketResponseComposer(int Result)
            : base(ServerPacketHeader.ModeratorSupportTicketResponseMessageComposer)
        {
			WriteInteger(Result);
			WriteString("");
        }
    }
}