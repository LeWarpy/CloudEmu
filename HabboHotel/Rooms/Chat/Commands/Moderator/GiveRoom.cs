using Cloud.Communication.Packets.Outgoing.Inventory.Purse;
using Cloud.HabboHotel.GameClients;

namespace Cloud.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class GiveRoom : IChatCommand
    {
        public string PermissionRequired => "command_give_room";
        public string Parameters => "[CANTIDAD]";
        public string Description => "Dar créditos a todos.";

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor, introduzca el nombre del identificador que le gustaría dar a la habitación.");
                return;
            }
			if (int.TryParse(Params[1], out int Amount))

				foreach (RoomUser RoomUser in Room.GetRoomUserManager().GetRoomUsers())
				{
					if (RoomUser == null || RoomUser.GetClient() == null || Session.GetHabbo().Id == RoomUser.UserId)
						continue;
					RoomUser.GetClient().GetHabbo().Credits += Amount;
					RoomUser.GetClient().SendMessage(new CreditBalanceComposer(RoomUser.GetClient().GetHabbo().Credits));
				}
		}
}
}
  