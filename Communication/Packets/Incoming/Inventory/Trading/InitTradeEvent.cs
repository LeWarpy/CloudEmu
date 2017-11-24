using Cloud.HabboHotel.Rooms;
using Cloud.Communication.Packets.Outgoing.Inventory.Trading;

using Cloud.Database.Interfaces;


namespace Cloud.Communication.Packets.Incoming.Inventory.Trading
{
    class InitTradeEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null || !Session.GetHabbo().InRoom)
                return;

            Room Room;

            if (!CloudServer.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room))
                return;

            if (!Room.CanTradeInRoom)
                return;

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
                return;

            if (Session.GetHabbo().TradingLockExpiry > 0)
            {
                if (Session.GetHabbo().TradingLockExpiry > CloudServer.GetUnixTimestamp())
                {
                    Session.SendNotification("Atualmente sua conta está bloqueada para trocas!");
                    return;
                }
                else
                {
                    Session.GetHabbo().TradingLockExpiry = 0;
                    Session.SendNotification("Suas trocas estão ativas novamente, não cometa o erro novamente!");

                    using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.RunQuery("UPDATE `user_info` SET `trading_locked` = '0' WHERE `id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
                    }
                }
            }

            RoomUser TargetUser = Room.GetRoomUserManager().GetRoomUserByVirtualId(Packet.PopInt());

            if (TargetUser == null || TargetUser.GetClient() == null || TargetUser.GetClient().GetHabbo() == null)
                return;

            if (TargetUser.IsTrading)
            {
                Session.SendMessage(new TradingErrorComposer(8, TargetUser.GetUsername()));
                return;
            }

            if (!Session.GetHabbo().GetPermissions().HasRight("room_trade_override"))
            {
                if (Room.TradeSettings == 1 && Room.OwnerId != Session.GetHabbo().Id)//Owner only.
                {
                    Session.SendMessage(new TradingErrorComposer(6, TargetUser.GetUsername()));
                    return;
                }
                else if (Room.TradeSettings == 0 && Room.OwnerId != Session.GetHabbo().Id)//Trading is disabled.
                {
                    Session.SendMessage(new TradingErrorComposer(6, TargetUser.GetUsername()));
                    return;
                }
            }

            if (TargetUser.GetClient().GetHabbo().TradingLockExpiry > 0)
            {
                Session.SendNotification("Ops, aparentemente, este usuário está com a troca desativadas!");
                return;
            }

            Room.TryStartTrade(User, TargetUser);
        }
    }
}