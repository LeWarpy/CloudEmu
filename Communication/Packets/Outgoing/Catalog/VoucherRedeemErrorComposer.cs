﻿namespace Cloud.Communication.Packets.Outgoing.Catalog
{
    public class VoucherRedeemErrorComposer : ServerPacket
    {
        public VoucherRedeemErrorComposer(int Type)
            : base(ServerPacketHeader.VoucherRedeemErrorMessageComposer)
        {
			WriteString(Type.ToString());
        }
    }
}