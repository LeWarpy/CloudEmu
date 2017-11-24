using Cloud.Communication.Encryption;
using Cloud.Communication.Encryption.Crypto.Prng;
using Cloud.Communication.Packets.Outgoing.Handshake;

namespace Cloud.Communication.Packets.Incoming.Handshake
{
    public class GenerateSecretKeyEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            string CipherPublickey = Packet.PopString();
           
            BigInteger SharedKey = HabboEncryptionV2.CalculateDiffieHellmanSharedKey(CipherPublickey);
            if (SharedKey != 0)
            {
                Session.RC4Client = new ARC4(SharedKey.getBytes());
                Session.SendMessage(new SecretKeyComposer(HabboEncryptionV2.GetRsaDiffieHellmanPublicKey()));
            }
            else 
            {
                Session.SendNotification("Houve um log de erro.Por favor tente novamente.!");
                return;
            }
        }
    }
}