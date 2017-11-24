using Cloud.Communication.Packets.Outgoing.Groups;
using Cloud.HabboHotel.GameClients;
using Cloud.HabboHotel.Groups.Forums;
using System;
using System.Collections.Generic;
using System.Data;

namespace Cloud.Communication.Packets.Incoming.Groups
{
    class GetForumsListDataEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            var int1 = Packet.PopInt(); // View Order ID
            var int2 = Packet.PopInt(); // Forum List Index
            int int3 = Packet.PopInt(); //Forum List Length

            /*
             * My groups = 2
             * Most Active = 0
             * Most views = 1
             */

            var forums = new List<GroupForum>();
            DataTable table;

            switch (int1)
            {
                case 2:
                    var Forums = CloudServer.GetGame().GetGroupForumManager().GetForumsByUserId(Session.GetHabbo().Id);

                    if (Forums.Count - 1 >= int2)
                    {
                        Forums = Forums.GetRange(int2, Math.Min(int3, Forums.Count));
                    }
                    Session.SendMessage(new ForumsListDataComposer(Forums, Session, int1, int2, int3));
                    return;

                case 0:


                    using (var adap = CloudServer.GetDatabaseManager().GetQueryReactor())
                    {
                        adap.SetQuery("SELECT g.id FROM groups as g INNER JOIN group_forums_thread_posts as posts, group_forums_threads as threads WHERE posts.thread_id = threads.id AND @now - posts.`timestamp`<= @sdays AND threads.forum_id = g.id GROUP BY g.id ORDER BY posts.`timestamp` DESC LIMIT @index, @limit");
                        adap.AddParameter("limit", int3);
                        adap.AddParameter("index", int2);
                        adap.AddParameter("now", (int)CloudServer.GetUnixTimestamp());
                        adap.AddParameter("sdays", (60 * 60 * 24 * 7));
                        table = adap.getTable();
                    }



                    foreach (DataRow Row in table.Rows)
                    {
                        GroupForum forum;
                        if (CloudServer.GetGame().GetGroupForumManager().TryGetForum(Convert.ToInt32(Row["id"]), out forum))
                            forums.Add(forum);
                    }
                    break;

                case 1:
                    using (var adap = CloudServer.GetDatabaseManager().GetQueryReactor())
                    {
                        adap.SetQuery("SELECT g.id FROM groups as g INNER JOIN group_forums_thread_views as v, group_forums_threads as threads WHERE v.thread_id = threads.id AND threads.forum_id = g.id AND  @now - v.`timestamp` <= @sdays GROUP BY g.id ORDER BY v.`timestamp` DESC LIMIT @index, @limit");
                        adap.AddParameter("limit", int3);
                        adap.AddParameter("index", int2);
                        adap.AddParameter("now", (int)CloudServer.GetUnixTimestamp());
                        adap.AddParameter("sdays", (60 * 60 * 24 * 7));

                        table = adap.getTable();
                    }



                    foreach (DataRow Row in table.Rows)
                    {
                        GroupForum forum;
                        if (CloudServer.GetGame().GetGroupForumManager().TryGetForum(Convert.ToInt32(Row["id"]), out forum))
                            forums.Add(forum);
                    }
                    break;
            }

            Session.SendMessage(new ForumsListDataComposer(forums, Session, int1, int2, int3));

        }
    }
}
