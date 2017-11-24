using System.Collections.Generic;
using Cloud.HabboHotel.Users;
using Cloud.HabboHotel.Groups;
using Cloud.Communication.Packets.Outgoing.Users;
using Cloud.Database.Interfaces;


namespace Cloud.Communication.Packets.Incoming.Users
{
    class OpenPlayerProfileEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            int userID = Packet.PopInt();

            Habbo targetData = CloudServer.GetHabboById(userID);
            if (targetData == null)
            {
                Session.SendNotification("Ocorreu um erro ao encontrar o perfil do usuário.");
                return;
            }

            List<Group> groups = CloudServer.GetGame().GetGroupManager().GetGroupsForUser(targetData.Id);

            int friendCount = 0;
            using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT COUNT(0) FROM `messenger_friendships` WHERE (`user_one_id` = @userid OR `user_two_id` = @userid)");
                dbClient.AddParameter("userid", userID);
                friendCount = dbClient.getInteger();
            }

            Session.SendMessage(new ProfileInformationComposer(targetData, Session, groups, friendCount));
        }
    }
}
