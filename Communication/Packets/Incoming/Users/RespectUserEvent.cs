using Cloud.HabboHotel.Rooms;
using Cloud.HabboHotel.Quests;
using Cloud.Communication.Packets.Outgoing.Rooms.Avatar;
using Cloud.Communication.Packets.Outgoing.Users;

namespace Cloud.Communication.Packets.Incoming.Users
{
    class RespectUserEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null)
                return;

            if (!Session.GetHabbo().InRoom || Session.GetHabbo().GetStats().DailyRespectPoints <= 0)
                return;

            Room Room = null;
            if (!CloudServer.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room))
                return;

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Packet.PopInt());
            if (User == null || User.GetClient() == null || User.GetClient().GetHabbo().Id == Session.GetHabbo().Id || User.IsBot)
                return;

            RoomUser ThisUser = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (ThisUser == null)
                return;
           
            CloudServer.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.SOCIAL_RESPECT);
            CloudServer.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_RespectGiven", 1);
            CloudServer.GetGame().GetAchievementManager().ProgressAchievement(User.GetClient(), "ACH_RespectEarned", 1);

            Session.GetHabbo().GetStats().DailyRespectPoints -= 1;
            Session.GetHabbo().GetStats().RespectGiven += 1;
            User.GetClient().GetHabbo().GetStats().Respect += 1;

            Room.SendMessage(new RespectNotificationComposer(User.GetClient().GetHabbo().Id, User.GetClient().GetHabbo().GetStats().Respect));
            Room.SendMessage(new ActionComposer(ThisUser.VirtualId, 7));
        }
    }
}