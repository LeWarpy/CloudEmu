using System;
using Cloud.HabboHotel.Rooms;
using Cloud.HabboHotel.Quests;

namespace Cloud.Communication.Packets.Incoming.Rooms.Action
{
    class GiveHandItemEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null || !Session.GetHabbo().InRoom)
                return;

            Room Room = null;
            if (!CloudServer.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room))
                return;            

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
                return;

            RoomUser TargetUser = Room.GetRoomUserManager().GetRoomUserByHabbo(Packet.PopInt());
            if (TargetUser == null)
                return;

            if (!((Math.Abs((User.X - TargetUser.X)) >= 3) || (Math.Abs((User.Y - TargetUser.Y)) >= 3)) || Session.GetHabbo().GetPermissions().HasRight("mod_tool"))
            {
                if (User.CarryItemID > 0 && User.CarryTimer > 0)
                {
                    if (User.CarryItemID == 8)
                        CloudServer.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.GIVE_COFFEE);
                    TargetUser.CarryItem(User.CarryItemID);
                    User.CarryItem(0);
                    TargetUser.DanceId = 0;
                }
            }
        }
    }
}
