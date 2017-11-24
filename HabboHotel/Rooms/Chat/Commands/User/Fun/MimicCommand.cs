using Cloud.HabboHotel.GameClients;
using Cloud.Communication.Packets.Outgoing.Rooms.Engine;
using Cloud.Database.Interfaces;
using System.Data;

namespace Cloud.HabboHotel.Rooms.Chat.Commands.User.Fun
{
    class MimicCommand : IChatCommand
    {
        public string PermissionRequired => "command_mimic";
        public string Parameters => "[USUARIO]"; 
        public string Description => "Copiar visual!";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            
            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor, coloque o nome do usuário que deseja copiar!");
                return;
            }

            GameClient TargetClient = CloudServer.GetGame().GetClientManager().GetClientByUsername(Params[1]);

            DataRow Table;
            using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `users` WHERE username = '" + Params[1] + "' LIMIT 1");
                Table = dbClient.getRow();
            }

            Session.GetHabbo().Gender = Table["gender"].ToString();
            Session.GetHabbo().Look = Table["look"].ToString();

            using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `users` SET `gender` = @gender, `look` = @look WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("gender", Session.GetHabbo().Gender);
                dbClient.AddParameter("look", Session.GetHabbo().Look);
                dbClient.AddParameter("id", Session.GetHabbo().Id);
                dbClient.RunQuery();
            }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User != null)
            {
                Session.SendMessage(new UserChangeComposer(User, true));
                Room.SendMessage(new UserChangeComposer(User, false));
                Session.SendMessage(new AvatarAspectUpdateMessageComposer(Session.GetHabbo().Look, Session.GetHabbo().Gender)); //esto
            }
        }
    }
}