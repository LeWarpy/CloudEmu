﻿namespace Cloud.Communication.Packets.Outgoing.Inventory.Trading
{
	class TradingErrorComposer : ServerPacket
    {
        public TradingErrorComposer(int Error, string Username)
            : base(ServerPacketHeader.TradingErrorMessageComposer)
        {
			WriteInteger(Error);
			WriteString(Username);
        }
    }
}
