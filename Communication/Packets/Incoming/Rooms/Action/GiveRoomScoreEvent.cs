using Cloud.HabboHotel.Rooms;
using Cloud.Communication.Packets.Outgoing.Navigator;

using Cloud.Database.Interfaces;


namespace Cloud.Communication.Packets.Incoming.Rooms.Action
{
    class GiveRoomScoreEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().InRoom)
                return;

            Room Room;

            if (!CloudServer.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room))
                return;

            if (Session.GetHabbo().RatedRooms.Contains(Room.RoomId) || Room.CheckRights(Session, true))
                return;

            int Rating = Packet.PopInt();
            switch (Rating)
            {
                case -1:

                    Room.Score--;
                    Room.RoomData.Score--;
                    break;

                case 1:

                    Room.Score++;
                    Room.RoomData.Score++;
                    break;

                default:

                    return;
            }


            using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.runFastQuery("UPDATE rooms SET score = '" + Room.Score + "' WHERE id = '" + Room.RoomId + "' LIMIT 1");
            }

            Session.GetHabbo().RatedRooms.Add(Room.RoomId);
            Session.SendMessage(new RoomRatingComposer(Room.Score, !(Session.GetHabbo().RatedRooms.Contains(Room.RoomId) || Room.CheckRights(Session, true))));
        }
    }
}
