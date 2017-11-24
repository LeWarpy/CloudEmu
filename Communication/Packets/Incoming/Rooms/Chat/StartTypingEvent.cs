
using Cloud.HabboHotel.Rooms;
using Cloud.HabboHotel.GameClients;
using Cloud.Communication.Packets.Outgoing.Rooms.Chat;

namespace Cloud.Communication.Packets.Incoming.Rooms.Chat
{
    public class StartTypingEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().InRoom)
                return;

            Room Room = Session.GetHabbo().CurrentRoom;
            if (Room == null)
                return;

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Username);
            if (User == null)
                return;

            Session.GetHabbo().CurrentRoom.SendMessage(new UserTypingComposer(User.VirtualId, true));
        }
    }
}