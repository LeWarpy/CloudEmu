using Cloud.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Cloud.HabboHotel.Rooms.Chat.Commands.User
{
    class EmptyItems : IChatCommand
    {
        public string PermissionRequired => "command_empty_items";
        public string Parameters => "";
        public string Description => "Apagar todos mobis do inventario";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            Session.GetHabbo().GetInventoryComponent().ClearItems();
            Session.SendMessage(new RoomNotificationComposer("frank_notification", "mensagem", "Seu inventario foi limpo!"));
            return;
        }
    }
}


