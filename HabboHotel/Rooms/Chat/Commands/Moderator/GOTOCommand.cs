using Cloud.HabboHotel.GameClients;

namespace Cloud.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class GOTOCommand : IChatCommand
    {
        public string PermissionRequired => "command_goto";
        public string Parameters => "[IDSALA]";
        public string Description => "Ir a una sala.";

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            
            if (Params.Length == 1)
            {
                Session.SendWhisper("Debe especificar un identificador de habitación!");
                return;
            }

            int roomId = 0;
            if (!int.TryParse(Params[1], out roomId))
            {
                Session.SendWhisper("You must enter a valid room ID");
            }
            else
            {
                Room room = null;
                if (!CloudServer.GetGame().GetRoomManager().TryGetRoom(roomId, out room))
                {
                    Session.SendWhisper("This room does not exist!");
                    return;
                }

                Session.GetHabbo().PrepareRoom(room.Id, "");
            }
        }
    }
}