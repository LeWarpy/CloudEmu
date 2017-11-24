namespace Cloud.Communication.Packets.Outgoing.Handshake
{
    public class SecretKeyComposer : ServerPacket
    {
        public SecretKeyComposer(string PublicKey)
            : base(ServerPacketHeader.SecretKeyMessageComposer)
        {
			WriteString(PublicKey);
        }
    }
}