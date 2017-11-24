using Cloud.HabboHotel.Rooms;
using System;

namespace Cloud.Communication.Packets.Incoming.Rooms.Avatar
{
    class ApplySignEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int SignId = Packet.PopInt();
            Room Room;

            if (!CloudServer.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room))
                return;

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
                return;


            User.UnIdle();

            User.SetStatus("sign", Convert.ToString(SignId));
            User.UpdateNeeded = true;
            User.SignTime = CloudServer.GetUnixTimestamp() + 5;
        }
    }
}