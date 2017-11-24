﻿using System;
using System.Linq;
using Cloud.HabboHotel.Users;
using Cloud.Communication.Packets.Outgoing.Users;

namespace Cloud.Communication.Packets.Incoming.Users
{
    class GetRelationshipsEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            Habbo Habbo = CloudServer.GetHabboById(Packet.PopInt());
            if (Habbo == null)
                return;

            var rand = new Random();
            Habbo.Relationships = Habbo.Relationships.OrderBy(x => rand.Next()).ToDictionary(item => item.Key, item => item.Value);

            int Loves = Habbo.Relationships.Count(x => x.Value.Type == 1);
            int Likes = Habbo.Relationships.Count(x => x.Value.Type == 2);
            int Hates = Habbo.Relationships.Count(x => x.Value.Type == 3);

            Session.SendMessage(new GetRelationshipsComposer(Habbo, Loves, Likes, Hates));
        }
    }
}
