using System;

namespace Cloud.Communication.Packets.Outgoing.Rooms.Furni
{
	class UpdateMagicTileComposer : ServerPacket
    {
        public UpdateMagicTileComposer(int ItemId, int Decimal)
            : base(ServerPacketHeader.UpdateMagicTileMessageComposer)
        {
			WriteInteger(Convert.ToInt32(ItemId));
			WriteInteger(Decimal);
        }
    }
}
