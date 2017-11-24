using Cloud.Utilities;
using Cloud.HabboHotel.Rooms;
using Cloud.Database.Interfaces;
using Cloud.HabboHotel.GameClients;
using Cloud.HabboHotel.Rooms.Polls;
using Cloud.Communication.Packets.Outgoing.Inventory.Purse;
using Cloud.Communication.Packets.Outgoing.Rooms.Polls.Questions;

namespace Cloud.Communication.Packets.Incoming.Rooms.Polls
{
    class PollAnswerEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (!session.GetHabbo().InRoom)
                return;

            Room room = session.GetHabbo().CurrentRoom;
            if (room == null)
                return;

            RoomUser user = room.GetRoomUserManager().GetRoomUserByHabbo(session.GetHabbo().Id);
            if (user == null)
                return;

            RoomPoll poll = null;
            if (!CloudServer.GetGame().GetPollManager().TryGetPoll(packet.PopInt(), out poll))
                return;

            RoomPollQuestion question = null;
            if (!poll.Questions.TryGetValue(packet.PopInt(), out question))
                return;

            string answer = "";
            for (int i = 0; i < packet.PopInt(); ++i)
            {
                answer = answer + ":" + packet.PopString();
            }

            answer = answer.Substring(1);


            using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("INSERT INTO `user_room_poll_results` (`user_id`,`poll_id`,`question_id`,`answer`,`timestamp`) VALUES (@uid,@pid,@qid,@answer,@timestamp);");
                dbClient.AddParameter("uid", session.GetHabbo().Id);
                dbClient.AddParameter("pid", poll.Id);
                dbClient.AddParameter("qid", question.Id);
                dbClient.AddParameter("answer", answer);
                dbClient.AddParameter("timestamp", UnixTimestamp.GetNow());
                dbClient.RunQuery();
            }

            if (question.SeriesOrder >= poll.LastQuestionId)
            {
                session.GetHabbo().GetPolls().TryAdd(poll.Id);

                if (!string.IsNullOrEmpty(poll.BadgeReward))
                {
                    if (!session.GetHabbo().GetBadgeComponent().HasBadge(poll.BadgeReward))
                        session.GetHabbo().GetBadgeComponent().GiveBadge(poll.BadgeReward, true, session);
                }

                if (poll.CreditReward > 0)
                {
                    session.GetHabbo().Credits += poll.CreditReward;
                    session.SendMessage(new CreditBalanceComposer(session.GetHabbo().Credits));
                }

                if (poll.PixelReward > 0)
                {
                    session.GetHabbo().Duckets += poll.PixelReward;
                    session.SendMessage(new HabboActivityPointNotificationComposer(session.GetHabbo().Duckets, poll.PixelReward));
                }
            }
        }
    }
}