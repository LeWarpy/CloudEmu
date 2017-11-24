using System;
using System.Collections.Generic;
using Cloud.Database.Interfaces;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloud.HabboHotel.Groups.Forums
{
    public class GroupForumManager
    {
        List<GroupForum> Forums;

        public GroupForumManager()
        {
            Forums = new List<GroupForum>();

        }

        public GroupForum GetForum(int GroupId)
        {
            GroupForum f = null;
            return TryGetForum(GroupId, out f) ? f : null;
        }

        public GroupForum CreateGroupForum(Group Gp)
        {
            GroupForum GF;
            if (TryGetForum(Gp.Id, out GF))
                return GF;

            using (var adap = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                adap.SetQuery("INSERT INTO group_forums_settings (group_id) VALUES (@gp)");
                adap.AddParameter("gp", Gp.Id);
                adap.RunQuery();

                adap.SetQuery("UPDATE groups SET forum_enabled = '1' WHERE id = @id");
                adap.AddParameter("id", Gp.Id);
                adap.RunQuery();
            }

            GF = new GroupForum(Gp);
            Forums.Add(GF);
            return GF;
        }

        public bool TryGetForum(int Id, out GroupForum Forum)
        {
            if ((Forum = Forums.FirstOrDefault(c => c.Id == Id)) != null)
                return true;

            Group Gp;
            if (!CloudServer.GetGame().GetGroupManager().TryGetGroup(Id, out Gp))
                return false;

            if (!Gp.ForumEnabled)
                return false;

            Forum = new GroupForum(Gp);
            Forums.Add(Forum);
            return true;
        }

        public void RemoveGroup(Group Group)
        {
            using (IQueryAdapter queryReactor = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                queryReactor.runFastQuery("DELETE FROM `group_forums_settings` WHERE `group_id` = '" + Group.Id + "'");
                queryReactor.runFastQuery("DELETE post FROM group_forums_thread_posts post INNER JOIN group_forums_threads threads ON threads.forum_id = '" + Group.Id + "' WHERE threads.id = post.thread_id");
                queryReactor.runFastQuery("DELETE v FROM group_forums_thread_views v INNER JOIN group_forums_threads threads ON threads.forum_id = '" + Group.Id + "' WHERE v.thread_id = threads.id");
                queryReactor.runFastQuery("DELETE t FROM group_forums_threads t WHERE t.forum_id = '" + Group.Id + "'");
            }
        }

        public int GetUnreadThreadForumsByUserId(int Id)
        {
            return (from c in this.GetForumsByUserId(Id)
                    where c.UnreadMessages(Id) > 0
                    select c).Count<GroupForum>();
        }

        public List<GroupForum> GetForumsByUserId(int Userid)
        {
            GroupForum F;
            return CloudServer.GetGame().GetGroupManager().GetGroupsForUser(Userid).Where(c => TryGetForum(c.Id, out F)).Select(c => GetForum(c.Id)).ToList();
        }
    }
}
