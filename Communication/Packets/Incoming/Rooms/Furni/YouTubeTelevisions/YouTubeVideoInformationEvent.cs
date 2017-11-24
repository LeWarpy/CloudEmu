using System.Linq;

using Cloud.HabboHotel.Items.Televisions;
using Cloud.Communication.Packets.Outgoing.Rooms.Furni.YouTubeTelevisions;

namespace Cloud.Communication.Packets.Incoming.Rooms.Furni.YouTubeTelevisions
{
	class YouTubeVideoInformationEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int ItemId = Packet.PopInt();
            string VideoId = Packet.PopString();

            foreach (TelevisionItem Tele in CloudServer.GetGame().GetTelevisionManager().TelevisionList.ToList())
            {
                if (Tele.YouTubeId != VideoId)
                    continue;

                Session.SendMessage(new GetYouTubeVideoComposer(ItemId, Tele.YouTubeId));
            }
        }
    }
}