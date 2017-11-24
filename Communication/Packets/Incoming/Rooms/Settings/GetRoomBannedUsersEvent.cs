using Cloud.HabboHotel.Rooms;
using Cloud.Communication.Packets.Outgoing.Rooms.Settings;

namespace Cloud.Communication.Packets.Incoming.Rooms.Settings
{
	class GetRoomBannedUsersEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().InRoom)
                return;

            Room Instance = Session.GetHabbo().CurrentRoom;
            if (Instance == null || !Instance.CheckRights(Session, true))
                return;

            if (Instance.GetBans().BannedUsers().Count > 0)
                Session.SendMessage(new GetRoomBannedUsersComposer(Instance));
        }
    }
}
