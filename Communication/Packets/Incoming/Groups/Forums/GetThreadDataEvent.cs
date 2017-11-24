using Cloud.Communication.Packets.Outgoing.Groups;
using Cloud.HabboHotel.GameClients;

namespace Cloud.Communication.Packets.Incoming.Groups
{
    class GetThreadDataEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            var ForumId = Packet.PopInt(); //Maybe Forum ID
            var ThreadId = Packet.PopInt(); //Maybe Thread ID
            var StartIndex = Packet.PopInt(); //Start index
            var length = Packet.PopInt(); //List Length

            var Forum = CloudServer.GetGame().GetGroupForumManager().GetForum(ForumId);

            if (Forum == null)
            {
                Session.SendNotification("Forum não encontrato!");
                return;
            }

            var Thread = Forum.GetThread(ThreadId);
            if (Thread == null)
            {
                Session.SendNotification("Tópico deste Fórum não foi encontrado!");
                return;
            }


            Session.SendMessage(new ThreadDataComposer(Thread, StartIndex, length));

        }
    }
}
