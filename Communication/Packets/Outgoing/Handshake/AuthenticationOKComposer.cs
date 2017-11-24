﻿namespace Cloud.Communication.Packets.Outgoing.Handshake
{
    public class AuthenticationOKComposer : ServerPacket
    {
        public AuthenticationOKComposer()
            : base(ServerPacketHeader.AuthenticationOKMessageComposer)
        {
        }
    }
}