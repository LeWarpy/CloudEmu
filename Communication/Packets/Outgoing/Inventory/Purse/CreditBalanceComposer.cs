﻿namespace Cloud.Communication.Packets.Outgoing.Inventory.Purse
{
	class CreditBalanceComposer : ServerPacket
    {
        public CreditBalanceComposer(int creditsBalance)
            : base(ServerPacketHeader.CreditBalanceMessageComposer)
        {
			WriteString(creditsBalance + ".0");
        }
    }
}
