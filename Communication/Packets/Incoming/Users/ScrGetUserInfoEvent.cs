using Cloud.HabboHotel.GameClients;
using Cloud.Communication.Packets.Outgoing.Users;
using Cloud.Communication.Packets.Outgoing.Handshake;

namespace Cloud.Communication.Packets.Incoming.Users
{
    class ScrGetUserInfoEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Session.SendMessage(new ScrSendUserInfoComposer(Session.GetHabbo()));
            Session.SendMessage(new UserRightsComposer(Session.GetHabbo()));
        }
    }
}
