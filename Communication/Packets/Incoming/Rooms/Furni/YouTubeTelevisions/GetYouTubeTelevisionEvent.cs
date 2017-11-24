using System;
using System.Linq;
using System.Collections.Generic;
using Cloud.HabboHotel.Items.Televisions;
using Cloud.Communication.Packets.Outgoing.Rooms.Furni.YouTubeTelevisions;

namespace Cloud.Communication.Packets.Incoming.Rooms.Furni.YouTubeTelevisions
{
	class GetYouTubeTelevisionEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().InRoom)
                return;

            int ItemId = Packet.PopInt();
            ICollection<TelevisionItem> Videos = CloudServer.GetGame().GetTelevisionManager().TelevisionList;
            if (Videos.Count == 0)
            {
                Session.SendNotification("Ah, parece que o gerente do hotel não adicionou nenhum vídeo para você ver! :(");
                return;
            }

            Dictionary<int, TelevisionItem> dict = CloudServer.GetGame().GetTelevisionManager()._televisions;
            foreach (TelevisionItem value in RandomValues(dict).Take(1))
            {
                Session.SendMessage(new GetYouTubeVideoComposer(ItemId, value.YouTubeId));
            }

            Session.SendMessage(new GetYouTubePlaylistComposer(ItemId, Videos));
        }

        public IEnumerable<TValue> RandomValues<TKey, TValue>(IDictionary<TKey, TValue> dict)
        {
            Random rand = new Random();
            List<TValue> values = Enumerable.ToList(dict.Values);
            int size = dict.Count;
            while (true)
            {
                yield return values[rand.Next(size)];
            }
        }
    }
}