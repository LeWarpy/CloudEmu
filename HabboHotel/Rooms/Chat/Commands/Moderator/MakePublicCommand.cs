using Cloud.HabboHotel.GameClients;
using System.Collections.Generic;
using System.Linq;
using Cloud.Communication.Packets.Outgoing.Rooms.Session;

namespace Cloud.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class MakePublicCommand : IChatCommand
    {
        public string PermissionRequired => "command_make_public";
        public string Parameters => "";
        public string Description => "Convertir está sala en publica.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            var room = Session.GetHabbo().CurrentRoom;
            using (var queryReactor = CloudServer.GetDatabaseManager().GetQueryReactor())
                queryReactor.runFastQuery(string.Format("UPDATE rooms SET roomtype = 'public' WHERE id = {0}",
                    room.RoomId));

            var roomId = Session.GetHabbo().CurrentRoom.RoomId;
            var users = new List<RoomUser>(Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUsers().ToList());

            CloudServer.GetGame().GetRoomManager().UnloadRoom(Session.GetHabbo().CurrentRoom);

            RoomData Data = CloudServer.GetGame().GetRoomManager().GenerateRoomData(roomId);
            Session.GetHabbo().PrepareRoom(Session.GetHabbo().CurrentRoom.RoomId, "");

            CloudServer.GetGame().GetRoomManager().LoadRoom(roomId);

            var data = new RoomForwardComposer(roomId);

            foreach (var user in users.Where(user => user != null && user.GetClient() != null))
                user.GetClient().SendMessage(data);
        }
    }
}
