
using System.Collections.Generic;

using Cloud.HabboHotel.Games;
using Cloud.Communication.Packets.Outgoing.GameCenter;

namespace Cloud.Communication.Packets.Incoming.GameCenter
{
    class GetGameListingEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            ICollection<GameData> Games = CloudServer.GetGame().GetGameDataManager().GameData;

            Session.SendMessage(new GameListComposer(Games));
        }
    }
}
