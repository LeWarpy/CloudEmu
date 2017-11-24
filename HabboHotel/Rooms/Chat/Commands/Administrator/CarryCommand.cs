using System;

namespace Cloud.HabboHotel.Rooms.Chat.Commands.Administrator
{
    class CarryCommand : IChatCommand
    {
        public string PermissionRequired => "command_carry";
        public string Parameters => "[ITEMID]";
        public string Description => "Ele permite que você carregue um item na mão.";

        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)
        {

            int ItemId = 0;
            if (!int.TryParse(Convert.ToString(Params[1]), out ItemId))
            {
                Session.SendWhisper("Por favor, introduza um número válido.");
                return;
            }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
                return;

            User.CarryItem(ItemId);
        }
    }
}
