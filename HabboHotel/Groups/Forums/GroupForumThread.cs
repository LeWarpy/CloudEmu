using Cloud.Communication.Packets.Outgoing;
using Cloud.HabboHotel.GameClients;
using Cloud.HabboHotel.Users;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Cloud.HabboHotel.Groups.Forums
{
    public class GroupForumThread
    {
        public int Id;
        public int UserId;
        public string Caption;
        public int Timestamp;

        //Stats
        public bool Pinned;
        public bool Locked;
        public int DeletedLevel;
        public int DeleterUserId;
        public int DeletedTimestamp;

        public GroupForum ParentForum;
        public List<GroupForumThreadPost> Posts;

        public List<GameClient> UsersOnThread;
        public List<GroupForumThreadPostView> Views;


        public GroupForumThread(GroupForum parent, int id, int userid, int timestamp, string caption, bool pinned, bool locked, int deletedlevel, int deleterid)
        {
            CloudServer.GetGame().GetClientManager().OnClientDisconnect += GroupForumThread_OnClientDisconnect;
            Views = new List<GroupForumThreadPostView>(); ;
            UsersOnThread = new List<GameClient>();
            ParentForum = parent;

            Id = id;
            UserId = userid;
            Timestamp = timestamp;
            Caption = caption;
            Posts = new List<GroupForumThreadPost>();

            Pinned = pinned;
            Locked = locked;
            DeletedLevel = deletedlevel;
            DeleterUserId = deleterid;
            DeletedTimestamp = (int)CloudServer.GetUnixTimestamp();

            DataTable table;
            using (var adap = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                adap.SetQuery("SELECT * FROM group_forums_thread_posts WHERE thread_id = @id");
                adap.AddParameter("id", this.Id);
                table = adap.getTable();
            }

            foreach (DataRow Row in table.Rows)
            {
                Posts.Add(new GroupForumThreadPost(this, Convert.ToInt32(Row["id"]), Convert.ToInt32(Row["user_id"]), Convert.ToInt32(Row["timestamp"]), Row["message"].ToString(), Convert.ToInt32(Row["deleted_level"]), Convert.ToInt32(Row["deleter_user_id"])));
            }


            //DataTable table;
            using (var Adap = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                Adap.SetQuery("SELECT * FROM group_forums_thread_views WHERE thread_id = @id");
                Adap.AddParameter("id", Id);
                table = Adap.getTable();
            }


            foreach (DataRow Row in table.Rows)
            {
                Views.Add(new GroupForumThreadPostView(Convert.ToInt32(Row["id"]), Convert.ToInt32(Row["user_id"]), Convert.ToInt32(Row["count"])));
            }

        }

        private void GroupForumThread_OnClientDisconnect(GameClient client)
        {
            if (UsersOnThread.Contains(client))
                UsersOnThread.Remove(client);
        }


        public void AddView(int userid, int count = -1)
        {
            GroupForumThreadPostView v;
            if ((v = GetView(userid)) != null)
            {
                v.Count = count >= 0 ? count : Posts.Count;
                using (var adap = CloudServer.GetDatabaseManager().GetQueryReactor())
                {
                    adap.SetQuery("UPDATE group_forums_thread_views SET count = @c WHERE thread_id = @p AND user_id = @u");
                    adap.AddParameter("c", v.Count);
                    adap.AddParameter("p", this.Id);
                    adap.AddParameter("u", userid);
                    adap.RunQuery();
                }
            }
            else
            {
                v = new GroupForumThreadPostView(0, userid, Posts.Count);
                using (var adap = CloudServer.GetDatabaseManager().GetQueryReactor())
                {
                    adap.SetQuery("INSERT INTO group_forums_thread_views (thread_id, user_id, count) VALUES (@t, @u, @c)");
                    adap.AddParameter("t", this.Id);
                    adap.AddParameter("u", userid);
                    adap.AddParameter("c", v.Count);
                    v.Id = (int)adap.InsertQuery();
                    Views.Add(v);
                }
            }
        }

        public GroupForumThreadPostView GetView(int userid)
        {
            return Views.FirstOrDefault(c => c.UserId == userid);
        }

        public int GetUnreadMessages(int userid)
        {
            GroupForumThreadPostView v;
            return (v = GetView(userid)) != null ? Posts.Count - v.Count : Posts.Count;
        }

        public List<GroupForumThreadPost> GetUserPosts(int userid)
        {
            return Posts.Where(c => c.UserId == userid).ToList();
        }

        public Habbo GetAuthor()
        {
            return CloudServer.GetHabboById(UserId);
        }

        public Habbo GetDeleter()
        {
            return CloudServer.GetHabboById(DeleterUserId);
        }

        public GroupForumThreadPost CreatePost(int userid, string message)
        {
            var now = (int)CloudServer.GetUnixTimestamp();
            var Post = new GroupForumThreadPost(this, 0, userid, now, message, 0, 0);

            using (var adap = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                adap.SetQuery("INSERT INTO group_forums_thread_posts (thread_id, user_id, message, timestamp) VALUES (@a, @b, @c, @d)");
                adap.AddParameter("a", this.Id);
                adap.AddParameter("b", userid);
                adap.AddParameter("c", message);
                adap.AddParameter("d", now);
                Post.Id = (int)adap.InsertQuery();
            }

            Posts.Add(Post);
            return Post;

        }

        public void AddClientToThread(GameClient Session)
        {
            UsersOnThread.Add(Session);
        }

        public void RemoveClientFromThread(GameClient Session)
        {
            if (UsersOnThread.Contains(Session))
                UsersOnThread.Add(Session);
        }

        public GroupForumThreadPost GetLastMessage()
        {
            return Posts.LastOrDefault();
        }

        public void SerializeData(GameClient Session, ServerPacket Packet)
        {
            var lastpost = GetLastMessage();
            var isn = lastpost == null;
            Packet.WriteInteger(Id); //Thread ID
            Packet.WriteInteger(GetAuthor().Id);
            Packet.WriteString(GetAuthor().Username); //Thread Author
            Packet.WriteString(Caption); //Thread Title
            Packet.WriteBoolean(Pinned); //Pinned
            Packet.WriteBoolean(Locked); //Locked
            Packet.WriteInteger((int)(CloudServer.GetUnixTimestamp() - Timestamp)); //Created Secs Ago
            Packet.WriteInteger(Posts.Count); //Message count
            Packet.WriteInteger(GetUnreadMessages(Session.GetHabbo().Id)); //Unread message count
            Packet.WriteInteger(0); // Message List Lentgh

            Packet.WriteInteger(!isn ? lastpost.GetAuthor().Id : 0);// Las user to message id
            Packet.WriteString(!isn ? lastpost.GetAuthor().Username : ""); //Last user to message name
            Packet.WriteInteger(!isn ? (int)(CloudServer.GetUnixTimestamp() - lastpost.Timestamp) : 0); //Last message timestamp

            Packet.WriteByte(DeletedLevel * 10); //thread Deleted Level

            var deleter = GetDeleter();
            if (deleter != null)
            {
                Packet.WriteInteger(deleter.Id);// deleter user id
                Packet.WriteString(deleter.Username); //deleter user name
                Packet.WriteInteger((int)(CloudServer.GetUnixTimestamp() - DeletedTimestamp));//deleted secs ago
            }
            else
            {
                Packet.WriteInteger(1);// deleter user id
                Packet.WriteString("unknow"); //deleter user name
                Packet.WriteInteger(0);//deleted secs ago
            }
        }

        public GroupForumThreadPost GetPost(int postId)
        {
            return Posts.FirstOrDefault(c => c.Id == postId);
        }


        public void Save()
        {
            using (var adap = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                adap.SetQuery("UPDATE group_forums_threads SET pinned = @pinned, locked = @locked, deleted_level = @dl, deleter_user_id = @duid WHERE id = @id");
                adap.AddParameter("pinned", Pinned ? 1 : 0);
                adap.AddParameter("locked", Locked ? 1 : 0);
                adap.AddParameter("dl", DeletedLevel);
                adap.AddParameter("duid", DeleterUserId);
                adap.AddParameter("id", Id);
                adap.RunQuery();
            }
        }
    }
}

