﻿using Cloud.Database.Interfaces;
using Cloud.HabboHotel.Users;

namespace Cloud.Communication.Packets.Incoming.Moderation
{
    class ModerationMuteEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null || !Session.GetHabbo().GetPermissions().HasRight("mod_mute"))
                return;

            int UserId = Packet.PopInt();
            string Message = Packet.PopString();
            double Length = (Packet.PopInt() * 60);
            string Unknown1 = Packet.PopString();
            string Unknown2 = Packet.PopString();

            Habbo Habbo = CloudServer.GetHabboById(UserId);
            if (Habbo == null)
            {
                Session.SendWhisper("Ocorreu um erro em encontrar esse usuário no banco de dados.");
                return;
            }

            if (Habbo.GetPermissions().HasRight("mod_mute") && !Session.GetHabbo().GetPermissions().HasRight("mod_mute_any"))
            {
                Session.SendWhisper("Ops, você não pode silenciar esse usuário.");
                return;
            }

            using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.runFastQuery("UPDATE `users` SET `time_muted` = '" + Length + "' WHERE `id` = '" + Habbo.Id + "' LIMIT 1");
            }

            if (Habbo.GetClient() != null)
            {
                Habbo.TimeMuted = Length;
                Habbo.GetClient().SendNotification("Você foi silenciando por " + Length + " segundos!");
            }
        }
    }
}

