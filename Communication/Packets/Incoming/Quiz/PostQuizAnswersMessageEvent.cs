using Cloud.HabboHotel.GameClients;
using Cloud.Communication.Packets.Outgoing.Quiz;

namespace Cloud.Communication.Packets.Incoming.Quiz
{
    class PostQuizAnswersMessageEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Session.SendMessage(new PostQuizAnswersMessageComposer(Session));
        }
    }
}
