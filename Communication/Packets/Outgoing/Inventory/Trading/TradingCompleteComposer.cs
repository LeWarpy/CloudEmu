﻿namespace Cloud.Communication.Packets.Outgoing.Inventory.Trading
{
	class TradingCompleteComposer : ServerPacket
    {
        public TradingCompleteComposer()
            : base(ServerPacketHeader.TradingCompleteMessageComposer)
        {
        }
    }
}
