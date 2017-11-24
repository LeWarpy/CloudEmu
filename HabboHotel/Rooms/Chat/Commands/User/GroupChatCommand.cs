using Cloud.Communication.Packets.Outgoing.Messenger;
using Cloud.Database.Interfaces;
using Cloud.HabboHotel.GameClients;

namespace Cloud.HabboHotel.Rooms.Chat.Commands.User
{
    class GroupChatCommand : IChatCommand
    {
        public string PermissionRequired => "command_groupchat";
        public string Parameters => "";
        public string Description => "Ativa chat de grupos ON/OFF";

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length < 2)
            {
                Session.SendWhisper("Ocorreu um erro, especifica ON / OFF");
                return;
            }

            if (!Room.CheckRights(Session, true))
            {
                Session.SendWhisper("Voce nao tem permissao.");
                return;
            }

            if (Room.Group == null)
            {
                Session.SendWhisper("Este quarto não tem um grupo, se você acabou de criar digite :unload");
                return;
            }

            if (Room.Group.Id != Session.GetHabbo().GetStats().FavouriteGroupId)
            {
                Session.SendWhisper("Você só pode criar um grupo se você tem favoritos.");
                return;
            }

            var mode = Params[1].ToLower();
            var group = Room.Group;

            if (mode == "on")
            {
                if (group.HasChat)
                {
                    Session.SendWhisper("Este grupo já bate-papo.");
                    return;
                }

                group.HasChat = true;

                using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("UPDATE groups SET has_chat = '1' WHERE id = @id");
                    dbClient.AddParameter("id", group.Id);
                    dbClient.RunQuery();
                }

                var Client = CloudServer.GetGame().GetClientManager().GetClientByUserID(Session.GetHabbo().Id);
                if (Client != null)
                {
                    Client.SendMessage(new FriendListUpdateComposer(group, 1));
                }

            }
            else if (mode == "off")
            {
                if (!group.HasChat)
                {
                    Session.SendWhisper("Este grupo não tem bate-papo .");
                    return;
                }

                group.HasChat = false;

                using (var adap = CloudServer.GetDatabaseManager().GetQueryReactor())
                {
                    adap.SetQuery("UPDATE groups SET has_chat = '0' WHERE id = @id");
                    adap.AddParameter("id", group.Id);
                    adap.RunQuery();
                }
                var Client = CloudServer.GetGame().GetClientManager().GetClientByUserID(Session.GetHabbo().Id);
                if (Client != null)
                {
                    Client.SendMessage(new FriendListUpdateComposer(group, -1));
                }
            }
            else
            {
                Session.SendNotification("Ocorreu um erro!");
            }
            

        }
    }
}
