namespace Cloud.Communication.Packets.Outgoing.Catalog
{
	class ReloadRecyclerComposer : ServerPacket
    {
        public ReloadRecyclerComposer()
            : base(ServerPacketHeader.ReloadRecyclerComposer)
        {
			WriteInteger(1);
			WriteInteger(0);
        }
    }
}