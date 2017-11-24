namespace Cloud.Communication.Packets.Outgoing.Catalog
{
    public class RecyclerStateComposer : ServerPacket
    {
        public RecyclerStateComposer(int ItemId = 0)
            : base(ServerPacketHeader.RecyclerStateComposer)
        {
			WriteInteger(1);
			WriteInteger(ItemId);
        }
    }
}