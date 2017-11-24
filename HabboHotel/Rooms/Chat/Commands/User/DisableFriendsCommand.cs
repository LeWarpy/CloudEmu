using Cloud.Database.Interfaces;

namespace Cloud.HabboHotel.Rooms.Chat.Commands.User
{
    class DisableFriendsCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_disable_friends"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Desativar solicitaçoes de Amizade."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {

            Session.GetHabbo().AllowFriendRequests = !Session.GetHabbo().AllowFriendRequests;
            Session.SendWhisper("você é " + (Session.GetHabbo().AllowFriendRequests == true ? "agora" : "nao") + " capaz de ser amigo.");

            using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `users` SET `block_newfriends` = '1' WHERE `id` = '" + Session.GetHabbo().Id + "'");
                dbClient.RunQuery();
            }
        }
    }
}