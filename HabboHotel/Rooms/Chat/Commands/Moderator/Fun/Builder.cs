using Cloud.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Cloud.HabboHotel.Rooms.Chat.Commands.Moderator.Fun
{
    class Builder : IChatCommand
    {
        public string PermissionRequired => "command_builder";
        public string Parameters => "";
        public string Description => "Teletransporte permite que o espaço para construir mais facilmente.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (!Room.CheckRights(Session, true))
            return;

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
                return;
            
            User.TeleportEnabled = !User.TeleportEnabled;
            Room.GetGameMap().GenerateMaps();

            Session.SendMessage(RoomNotificationComposer.SendBubble("builders_club_room_locked_small", "Acaba de ativar o modo construtor.", ""));
        }
    }
}
