using Cloud.Communication.Packets.Outgoing.Rooms.Notifications;
using Cloud.HabboHotel.Rooms.Chat.Styles;
using Cloud.Database.Interfaces;

namespace Cloud.HabboHotel.Rooms.Chat.Commands.Administrator
{
    class BubbleCommand : IChatCommand
    {
        public string PermissionRequired => "command_bubble";
        public string Parameters => "[BUBBLEID]";
        public string Description => "Use uma fala como para conversar.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
                return;

            if (Params.Length == 1)
            {
                Session.SendWhisper("Oh, esqueceu-se de introduzir um ID!");
                return;
            }

            int Bubble = 0;
            if (!int.TryParse(Params[1].ToString(), out Bubble))
            {
                Session.SendWhisper("Por favor ultilize um número valido.");
                return;
            }

            if ((Bubble == 33) && !Session.GetHabbo().GetPermissions().HasRight("mod_tool"))
            {
                Session.LogsNotif("Desculpe, apenas os membros da equipe podem usar essas falas", "command_notification");
                return;
            }

            ChatStyle Style = null;
            if (!CloudServer.GetGame().GetChatManager().GetChatStyles().TryGetStyle(Bubble, out Style) || (Style.RequiredRight.Length > 0 && !Session.GetHabbo().GetPermissions().HasRight(Style.RequiredRight)))
            {
                Session.SendWhisper("Bem, você não pode usar esta fala por causa do seu cargo, sorry!");
                return;
            }

            User.LastBubble = Bubble;
            Session.GetHabbo().CustomBubbleId = Bubble;
            Session.SendWhisper("bolha estabelecida: " + Bubble);
            using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.runFastQuery("UPDATE `users` SET `bubble_id` = '" + Bubble + "' WHERE `id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
            }
        }
    }
}