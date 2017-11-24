using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloud.HabboHotel.Groups.Forums
{
    public class GroupForum
    {
        public int GroupId;
        public Group Group;
        public GroupForumSettings Settings;
        public List<GroupForumThread> Threads;

        public int Id
        {
            get
            {
                return GroupId;
            }
        }

        public string Name
        {
            get
            {
                return Group.Name;
            }
        }

        public string Description
        {
            get
            {
                return Group.Description;
            }
        }


        public GroupForum(Group group)
        {
            GroupId = group.Id;
            Group = group;
            Settings = new GroupForumSettings(this);
            Threads = new List<GroupForumThread>();

            LoadThreads();
        }

        private void LoadThreads()
        {
            DataTable table;
            using (var adap = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                adap.SetQuery("SELECT * FROM group_forums_threads WHERE forum_id = @id ORDER BY id DESC");
                adap.AddParameter("id", Id);
                table = adap.getTable();
            }

            foreach (DataRow Row in table.Rows)
            {
                Threads.Add(new GroupForumThread(this, Convert.ToInt32(Row["id"]), Convert.ToInt32(Row["user_id"]), Convert.ToInt32(Row["timestamp"]), Row["caption"].ToString(), Convert.ToInt32(Row["pinned"]) == 1, Convert.ToInt32(Row["locked"]) == 1, Convert.ToInt32(Row["deleted_level"]), Convert.ToInt32(Row["deleter_user_id"])));
            }
        }

        public int MessagesCount
        {
            get
            {
                return Threads.SelectMany(c => c.Posts).Count();
            }
        }

        public int UnreadMessages(int userid)
        {
            int i=0;
            Threads.ForEach(c => i += c.GetUnreadMessages(userid));
            return i;


        }

        public GroupForumThreadPost GetLastPost()
        {
            var Posts = Threads.SelectMany(c => c.Posts);
            return Posts.OrderByDescending(c => c.Timestamp).FirstOrDefault();
        }

        public GroupForumThread GetThread(int ThreadId)
        {
            return Threads.FirstOrDefault(c => c.Id == ThreadId);
        }

        public GroupForumThread CreateThread(int Creator, string Caption)
        {
            var timestamp = (int)CloudServer.GetUnixTimestamp();
            var Thread = new GroupForumThread(this, 0, Creator, (int)timestamp, Caption, false, false, 0, 0);

            using (var adap = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                adap.SetQuery("INSERT INTO group_forums_threads (forum_id, user_id, caption, timestamp) VALUES (@a, @b, @c, @d)");
                adap.AddParameter("a", this.Id);
                adap.AddParameter("b", Creator);
                adap.AddParameter("c", Caption);
                adap.AddParameter("d", timestamp);
                Thread.Id = (int)adap.InsertQuery();
            }

            this.Threads.Add(Thread);
            return Thread;
        }

        public GroupForumThreadPost GetPost(int postid)
        {
            return Threads.SelectMany(c => c.Posts).Where(c => c.Id == postid).FirstOrDefault();
        }
    }
}
