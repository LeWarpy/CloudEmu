using System;
using Cloud.Database.Interfaces;
using Cloud.HabboHotel.Groups;
using Cloud.HabboHotel.Rooms;
using Cloud.Communication.Packets.Outgoing.Messenger;

namespace Cloud.Communication.Packets.Incoming.Groups
{
    class DeleteGroupEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            Group Group = null;
            if (!CloudServer.GetGame().GetGroupManager().TryGetGroup(Packet.PopInt(), out Group))
            {
                Session.SendNotification("Ops! Não conseguimos encontrar o grupo!");
                return;
            }

            if (Group.CreatorId != Session.GetHabbo().Id && !Session.GetHabbo().GetPermissions().HasRight("group_delete_override"))//Maybe a FUSE check for staff override?
            {
                Session.SendNotification("Ops! só o dono pode excluir!");
                return;
            }

            if (Group.MemberCount >= Convert.ToInt32(CloudServer.GetGame().GetSettingsManager().TryGetValue("group.delete.member.limit")) && !Session.GetHabbo().GetPermissions().HasRight("group_delete_limit_override"))
            {
                Session.SendNotification("Ops seu grupo já superrous a quantidade maxima  de mebros! (" + Convert.ToInt32(CloudServer.GetGame().GetSettingsManager().TryGetValue("group.delete.member.limit")) + ") que un grupo puede exceder antes de ser elegible para su eliminación. Solicitar asistencia de un miembro del staff.");
                return;
            }

            Room Room = CloudServer.GetGame().GetRoomManager().LoadRoom(Group.RoomId);

            if (Room != null)
            {
                Room.Group = null;
                Room.RoomData.Group = null;//I'm not sure if this is needed or not, becauseof inheritance, but oh well.
            }

            //Remove it from the cache.
            CloudServer.GetGame().GetGroupManager().DeleteGroup(Group.Id);

            //Now the :S stuff.
            using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.runFastQuery("DELETE FROM `groups` WHERE `id` = '" + Group.Id + "'");
                dbClient.runFastQuery("DELETE FROM `group_memberships` WHERE `group_id` = '" + Group.Id + "'");
                dbClient.runFastQuery("DELETE FROM `group_requests` WHERE `group_id` = '" + Group.Id + "'");
                dbClient.runFastQuery("UPDATE `rooms` SET `group_id` = '0' WHERE `group_id` = '" + Group.Id + "' LIMIT 1");
                dbClient.runFastQuery("UPDATE `user_stats` SET `groupid` = '0' WHERE `groupid` = '" + Group.Id + "' LIMIT 1");
                dbClient.runFastQuery("DELETE FROM `items_groups` WHERE `group_id` = '" + Group.Id + "'");
            }

            bool forumEnabled = Group.ForumEnabled;
            if (forumEnabled)
            {
                CloudServer.GetGame().GetGroupForumManager().RemoveGroup(Group);
                return;
            }

            //Unload it last.
            CloudServer.GetGame().GetRoomManager().UnloadRoom(Room.Id);

            var Client = CloudServer.GetGame().GetClientManager().GetClientByUserID(Session.GetHabbo().Id);
            if (Client != null)
            {
                Client.SendMessage(new FriendListUpdateComposer(Group, -1));
                Client.SendMessage(new FriendListUpdateComposer(-Group.Id));
            }

            //Say hey!
            Session.SendNotification("Eliminou corretamente o grupo.");
        }
    }
}
