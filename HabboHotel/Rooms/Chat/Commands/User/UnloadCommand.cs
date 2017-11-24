using System.Linq;
using System.Collections.Generic;
using Cloud.Communication.Packets.Outgoing.Rooms.Session;

namespace Cloud.HabboHotel.Rooms.Chat.Commands.User
{
    class UnloadCommand : IChatCommand
    {
        private readonly bool _reEnter;

        public string PermissionRequired => "command_unload";
        public string Parameters => "[ID]";
        public string Description => "Dar unload na sala..";
		
		public UnloadCommand(bool reEnter = false)
        {
            _reEnter = reEnter;
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (_reEnter == true)
            {
                if (Room.CheckRights(Session, true))
                {
                    var roomId = Session.GetHabbo().CurrentRoomId;
                    List<RoomUser> UsersToReturn = new List<RoomUser>(Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUsers().ToList());

                    if (!_reEnter) return;

                    RoomData Data = CloudServer.GetGame().GetRoomManager().GenerateRoomData(roomId);
                    Session.GetHabbo().PrepareRoom(Session.GetHabbo().CurrentRoom.RoomId, "");
                    CloudServer.GetGame().GetRoomManager().LoadRoom(roomId);

                    foreach (RoomUser User in UsersToReturn)
                    {
                        if (User == null || User.GetClient() == null)
                            continue;

                        User.GetClient().SendMessage(new RoomForwardComposer(roomId));
                    }
                }
            }

            if (Session.GetHabbo().GetPermissions().HasRight("room_unload_any"))
            {
                Room R = null;

                if (!CloudServer.GetGame().GetRoomManager().TryGetRoom(Room.Id, out R))
                    return;

                CloudServer.GetGame().GetRoomManager().UnloadRoom(R);
            }
            else
            {
                if (Room.CheckRights(Session, true))
                {
                    CloudServer.GetGame().GetRoomManager().UnloadRoom(Room);
                }
            }

        }
    }
}
