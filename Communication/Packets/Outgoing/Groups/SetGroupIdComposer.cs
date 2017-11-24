namespace Cloud.Communication.Packets.Outgoing.Groups
{
	class SetGroupIdComposer : ServerPacket
    {
        public SetGroupIdComposer(int Id)
            : base(ServerPacketHeader.SetGroupIdMessageComposer)
        {
			WriteInteger(Id);
        }
    }
}
