using Cloud.HabboHotel.Navigator;
using Cloud.HabboHotel.GameClients;
using Cloud.Database.Interfaces;
using Cloud.HabboHotel.Rooms;
using Cloud.Communication.Packets.Outgoing.Navigator;
using Cloud.Communication.Packets.Outgoing.Rooms.Settings;

namespace Cloud.Communication.Packets.Incoming.Navigator
{
    class ToggleStaffPickEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (!session.GetHabbo().GetPermissions().HasRight("room.staff_picks.management"))
                return;

            Room room = null;
            if (!CloudServer.GetGame().GetRoomManager().TryGetRoom(packet.PopInt(), out room))
                return;

            StaffPick staffPick = null;
            if (!CloudServer.GetGame().GetNavigator().TryGetStaffPickedRoom(room.Id, out staffPick))
            {
                if (CloudServer.GetGame().GetNavigator().TryAddStaffPickedRoom(room.Id))
                {
                    using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.SetQuery("INSERT INTO `navigator_staff_picks` (`room_id`,`image`) VALUES (@roomId, null)");
                        dbClient.AddParameter("roomId", room.Id);
                        dbClient.RunQuery();
                    }
                    CloudServer.GetGame().GetAchievementManager().ProgressAchievement(session, "ACH_Spr", 1, false);
                }
            }
            else
            {
                if (CloudServer.GetGame().GetNavigator().TryRemoveStaffPickedRoom(room.Id))
                {
                    using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.SetQuery("DELETE FROM `navigator_staff_picks` WHERE `room_id` = @roomId LIMIT 1");
                        dbClient.AddParameter("roomId", room.Id);
                        dbClient.RunQuery();
                    }
                }
            }

            room.SendMessage(new RoomSettingsSavedComposer(room.RoomId));
            room.SendMessage(new RoomInfoUpdatedComposer(room.RoomId));
        }
    }
}