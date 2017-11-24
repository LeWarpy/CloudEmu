using System.Collections.Generic;
using Cloud.HabboHotel.Achievements;
using Cloud.Communication.Packets.Outgoing.Talents;

namespace Cloud.Communication.Packets.Incoming.Talents
{
    class GetTalentTrackEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            string Type = Packet.PopString();

            List<Talent> talents = CloudServer.GetGame().GetTalentManager().GetTalents(Type, -1);

            if (talents == null)
                return;

            Session.SendMessage(new TalentTrackComposer(Session, Type, talents));
        }
    }
}
