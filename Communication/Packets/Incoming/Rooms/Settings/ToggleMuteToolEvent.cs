using System.Linq;
using System.Collections.Generic;
using Cloud.HabboHotel.Rooms;
using Cloud.Communication.Packets.Outgoing.Rooms.Settings;

namespace Cloud.Communication.Packets.Incoming.Rooms.Settings
{
    class ToggleMuteToolEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().InRoom)
                return;

            Room Room = Session.GetHabbo().CurrentRoom;
            if (Room == null || !Room.CheckRights(Session, true))
                return;

            Room.RoomMuted = !Room.RoomMuted;

            List<RoomUser> roomUsers = Room.GetRoomUserManager().GetRoomUsers();
            foreach (RoomUser roomUser in roomUsers.ToList())
            {
                if (roomUser == null || roomUser.GetClient() == null)
                    continue;

                if (Room.RoomMuted)
                    roomUser.GetClient().SendWhisper("Este quarto foi mutado");
                else
                    roomUser.GetClient().SendWhisper("Este quarto foi desmutado");
            }

            Room.SendMessage(new RoomMuteSettingsComposer(Room.RoomMuted));
        }
    }
}
