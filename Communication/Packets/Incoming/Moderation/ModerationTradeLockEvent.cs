using Cloud.Database.Interfaces;
using Cloud.HabboHotel.Users;


namespace Cloud.Communication.Packets.Incoming.Moderation
{
    class ModerationTradeLockEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null || !Session.GetHabbo().GetPermissions().HasRight("mod_trade_lock"))
                return;

            int UserId = Packet.PopInt();
            string Message = Packet.PopString();
            double Days = (Packet.PopInt() / 1440);
            string Unknown1 = Packet.PopString();
            string Unknown2 = Packet.PopString();
            

            double Length = (CloudServer.GetUnixTimestamp() + (Days * 86400));

            Habbo Habbo = CloudServer.GetHabboById(UserId);
            if (Habbo == null)
            {
                Session.SendWhisper("Ocorreu um erro em encontrar esse usuário no banco de dados.");
                return;
            }

            if (Habbo.GetPermissions().HasRight("mod_trade_lock") && !Session.GetHabbo().GetPermissions().HasRight("mod_trade_lock_any"))
            {
                Session.SendWhisper("Ops, você não pode bloquear outro usuário avaliado em 5 ou superior.");
                return;
            }

            if (Days < 1)
                Days = 1;

            if (Days > 365)
                Days = 365;

            using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.runFastQuery("UPDATE `user_info` SET `trading_locked` = '" + Length + "', `trading_locks_count` = `trading_locks_count` + '1' WHERE `user_id` = '" + Habbo.Id + "' LIMIT 1");
            }

            if (Habbo.GetClient() != null)
            {
                Habbo.TradingLockExpiry = Length;
                Habbo.GetClient().SendNotification("Suas trocas foram proibidas! " + Days + " día(s)!\r\rRazão:\r\r" + Message);
            }
        }
    }
}
