using Cloud.Communication.Packets.Outgoing.Rooms.Engine;
using Cloud.Database.Interfaces;

namespace Cloud.HabboHotel.Rooms.Chat.Commands.User.Fun
{
    class FacelessCommand :IChatCommand
    {
        public string PermissionRequired => "command_faceless";
        public string Parameters => "";
        public string Description => "Te permite tirar o rosto!";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
    
            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null || User.GetClient() == null)
                return;

            string[] headParts;
            string[] figureParts = Session.GetHabbo().Look.Split('.');
            foreach (string Part in figureParts)
            {
                if (Part.StartsWith("hd"))
                {
                    headParts = Part.Split('-');
                    if (!headParts[1].Equals("99999"))
                        headParts[1] = "99999";
                    else
                        return;

                    Session.GetHabbo().Look = Session.GetHabbo().Look.Replace(Part, "hd-" + headParts[1] + "-" + headParts[2]);
                    break;
                }
            }
            Session.GetHabbo().Look = CloudServer.GetGame().GetFigureManager().ProcessFigure(Session.GetHabbo().Look, Session.GetHabbo().Gender, Session.GetHabbo().GetClothing().GetClothingParts, true);
            using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `users` SET `look` = @Look WHERE `id` = '" + Session.GetHabbo().Id + "' LIMIT 1");
                dbClient.AddParameter("look", Session.GetHabbo().Look);
                dbClient.RunQuery();
            }

            Session.SendMessage(new UserChangeComposer(User, true));
            Session.GetHabbo().CurrentRoom.SendMessage(new UserChangeComposer(User, false));
            return;
        }
    }
}
