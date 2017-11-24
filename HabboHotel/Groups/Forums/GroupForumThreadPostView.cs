using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cloud.HabboHotel.Groups.Forums
{
    public class GroupForumThreadPostView
    {
        public int Id;
        public int UserId;
        //public int Timestamp;
        public int Count;

        public GroupForumThreadPostView(int id, int userid, int count)
        {
            Id = id;
            UserId = userid;
            //Timestamp = timestamp;
            Count = count;
        }
    }
}