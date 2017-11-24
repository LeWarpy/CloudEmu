using Cloud.Communication.Packets.Outgoing.Rooms.Chat;
using Cloud.HabboHotel.Items.Wired;

namespace Cloud.HabboHotel.Rooms.Chat.Commands.Moderator.Fun
{
    class MakeSayCommand : IChatCommand
    {
        public string PermissionRequired => "command_makesay";
        public string Parameters => "[USUARIO] [MENSAJE]";
        public string Description => "Que otro usuario diga algo.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            
            RoomUser ThisUser = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (ThisUser == null)
                return;

            if (Params.Length == 1)
                Session.SendWhisper("Debe introducir un nombre de usuario y mensaje que desee para obligarlos a decir.");
            else
            {
                string Message = CommandManager.MergeParams(Params, 2);
                RoomUser TargetUser = Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Params[1]);
                if (TargetUser != null)
                {
                    if (TargetUser.GetClient() != null && TargetUser.GetClient().GetHabbo() != null)
                        if (!TargetUser.GetClient().GetHabbo().GetPermissions().HasRight("mod_make_say_any"))
                        {
                            Room.SendMessage(new ChatComposer(TargetUser.VirtualId, Message, 0, TargetUser.LastBubble));
                            Room.GetWired().TriggerEvent(WiredBoxType.TriggerUserSaysCommand, Session.GetHabbo(), Message);
                            Room.GetWired().TriggerEvent(WiredBoxType.TriggerUserSays, Session.GetHabbo(), Message);
                        }
                        else
                        {
                            Session.SendWhisper("No se puede utilizar makesay de este usuario.");
                        }
                }
                else
                    Session.SendWhisper("Este usuario no se ha encontrado en la habitación");
            }
        }
    }
}
