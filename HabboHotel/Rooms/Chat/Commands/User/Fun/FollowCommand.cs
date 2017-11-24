using Cloud.HabboHotel.GameClients;

namespace Cloud.HabboHotel.Rooms.Chat.Commands.User.Fun
{
    class FollowCommand : IChatCommand
    {
        public string PermissionRequired => "command_follow";
        public string Parameters => "[USUARIO]"; 
        public string Description => "Seguir um usuário especifico!";

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            

            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor, coloque o nick de quem deseja seguir.");
                return;
            }

            GameClient TargetClient = CloudServer.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetClient == null)
            {
                Session.SendWhisper("Ops! Ocorreu um erro, o usuário não esta online!");
                return;
            }

            if (TargetClient.GetHabbo().CurrentRoom == Session.GetHabbo().CurrentRoom)
            {
                Session.SendWhisper("Abra seus olhos " + TargetClient.GetHabbo().Username + " já está nessa sala!");
                return;
            }

            if (TargetClient.GetHabbo().Username == Session.GetHabbo().Username)
            {
                Session.SendWhisper("Sadooooooooo!");
                return;
            }

            if (!TargetClient.GetHabbo().InRoom)
            {
                Session.SendWhisper("Este usuário não está em nenhuma sala!");
                return;
            }

            if (TargetClient.GetHabbo().CurrentRoom.Access != RoomAccess.OPEN && !Session.GetHabbo().GetPermissions().HasRight("mod_tool"))
            {
                Session.SendWhisper("O quarto em que seu amigo se encontra pode estar trancado ou invisivel!");
                return;
            }

            Session.GetHabbo().PrepareRoom(TargetClient.GetHabbo().CurrentRoom.RoomId, "");
        }
    }
}
