using Cloud.Database.Interfaces;
using System.Linq;
using System.Collections.Generic;
using Cloud.Communication.Packets.Outgoing.Moderation;
using Cloud.Communication.Packets.Outgoing.Messenger;
using Cloud.Communication.Packets.Outgoing.Rooms.Session;

namespace Cloud.HabboHotel.Rooms.Chat.Commands.Administrator
{
    class DeleteGroupCommand : IChatCommand
    {
        public string PermissionRequired => "command_delete_group";
        public string Parameters => "";
        public string Description => "Apaga um grupo do banco de dados e do hotel.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {

            Room = Session.GetHabbo().CurrentRoom;
            if (Room == null)
                return;

            if (Room.Group == null)
            {
                Session.SendWhisper("Bem, não há nenhum grupo aqui?");
                return;
            }

            using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.runFastQuery("DELETE FROM `groups` WHERE `id` = '" + Room.Group.Id + "'");
                dbClient.runFastQuery("DELETE FROM `group_memberships` WHERE `group_id` = '" + Room.Group.Id + "'");
                dbClient.runFastQuery("DELETE FROM `group_requests` WHERE `group_id` = '" + Room.Group.Id + "'");
                dbClient.runFastQuery("UPDATE `rooms` SET `group_id` = '0' WHERE `group_id` = '" + Room.Group.Id + "' LIMIT 1");
                dbClient.runFastQuery("UPDATE `user_stats` SET `groupid` = '0' WHERE `groupid` = '" + Room.Group.Id + "' LIMIT 1");
                dbClient.runFastQuery("DELETE FROM `items_groups` WHERE `group_id` = '" + Room.Group.Id + "'");
            }

            CloudServer.GetGame().GetGroupManager().DeleteGroup(Room.RoomData.Group.Id);

            Room.Group = null;
            Room.RoomData.Group = null;

            CloudServer.GetGame().GetRoomManager().UnloadRoom(Room);
            if (Room.RoomData.Group.HasChat)
            {
                var Client = CloudServer.GetGame().GetClientManager().GetClientByUserID(Session.GetHabbo().Id);
                if (Client != null)
                {
                    Client.SendMessage(new FriendListUpdateComposer(Room.RoomData.Group, -1));
                    Client.SendMessage(new BroadcastMessageAlertComposer(CloudServer.GetGame().GetLanguageManager().TryGetValue("server.console.alert") + "\n\n Você deixou o grupo, por favor, se você ver o grupo de chat, no entanto, relogue no jogo."));
                }
            }

            var roomId = Session.GetHabbo().CurrentRoomId;
            List<RoomUser> UsersToReturn = new List<RoomUser>(Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUsers().ToList());

            RoomData Data = CloudServer.GetGame().GetRoomManager().GenerateRoomData(roomId);
            Session.GetHabbo().PrepareRoom(Session.GetHabbo().CurrentRoom.RoomId, "");
            CloudServer.GetGame().GetRoomManager().LoadRoom(roomId);

            foreach (RoomUser User in UsersToReturn)
            {
                if (User == null || User.GetClient() == null)
                    continue;

                User.GetClient().SendMessage(new RoomForwardComposer(roomId));
            }

            Session.SendNotification("Éxito, grupo eliminado.");
            return;
        }
    }
}
