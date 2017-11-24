using Cloud.HabboHotel.Rooms;
using Cloud.Communication.Packets.Outgoing.Rooms.Settings;

namespace Cloud.Communication.Packets.Incoming.Rooms.Settings
{
    class GetRoomSettingsEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            Room Room = CloudServer.GetGame().GetRoomManager().LoadRoom(Packet.PopInt());
            if (Room == null )
                return;

            if (!Room.CheckRights(Session, true))
                return;

            Session.SendMessage(new RoomSettingsDataComposer(Room));
        }
    }
}
