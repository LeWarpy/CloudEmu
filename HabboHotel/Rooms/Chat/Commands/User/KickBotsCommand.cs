using System;
using System.Linq;
using Cloud.HabboHotel.Users.Inventory.Bots;
using Cloud.Communication.Packets.Outgoing.Inventory.Bots;
using Cloud.Database.Interfaces;

namespace Cloud.HabboHotel.Rooms.Chat.Commands.User
{
    class KickBotsCommand : IChatCommand
    {
        public string PermissionRequired => "command_kickbots";
        public string Parameters => "";
        public string Description => "Kikar todos os bots da sala";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {


            if (!Room.CheckRights(Session, true))
            {
                Session.SendWhisper("Bem, somente o proprietário da sala pode executar este comando!");
                return;
            }

            foreach (RoomUser User in Room.GetRoomUserManager().GetUserList().ToList())
            {
                if (User == null || User.IsPet || !User.IsBot)
                    continue;

                RoomUser BotUser = null;
                if (!Room.GetRoomUserManager().TryGetBot(User.BotData.Id, out BotUser))
                    return;

                using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("UPDATE `bots` SET `room_id` = '0' WHERE `id` = @id LIMIT 1");
                    dbClient.AddParameter("id", User.BotData.Id);
                    dbClient.RunQuery();
                }

                Session.GetHabbo().GetInventoryComponent().TryAddBot(new Bot(Convert.ToInt32(BotUser.BotData.Id), Convert.ToInt32(BotUser.BotData.ownerID), BotUser.BotData.Name, BotUser.BotData.Motto, BotUser.BotData.Look, BotUser.BotData.Gender));
                Session.SendMessage(new BotInventoryComposer(Session.GetHabbo().GetInventoryComponent().GetBots()));
                Room.GetRoomUserManager().RemoveBot(BotUser.VirtualId, false);
            }

            Session.SendWhisper("Sucesso, eliminar todos os Bots");
        }
    }
}
