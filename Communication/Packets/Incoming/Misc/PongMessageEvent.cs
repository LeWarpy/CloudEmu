using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Cloud.Communication.Packets.Outgoing.Misc;

namespace Cloud.Communication.Packets.Incoming.Misc
{
    class PongMessageEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            Session.TimePingedReceived = DateTime.Now;
        }
    }
}
