﻿using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Cloud.Communication.Packets.Outgoing.Rooms.Furni
{
    class GnomeBoxComposer : ServerPacket
    {
        public GnomeBoxComposer(int ItemId)
            : base(ServerPacketHeader.GnomeBoxMessageComposer)
        {
            base.WriteInteger(ItemId);
        }
    }
}
