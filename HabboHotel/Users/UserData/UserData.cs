using System.Collections.Generic;
using Cloud.HabboHotel.Achievements;
using Cloud.HabboHotel.Rooms;
using Cloud.HabboHotel.Users.Badges;
using Cloud.HabboHotel.Users.Messenger;
using Cloud.HabboHotel.Users.Relationships;
using System.Collections.Concurrent;
using Cloud.HabboHotel.Subscriptions;

namespace Cloud.HabboHotel.Users.UserData
{
    public class UserData
    {
        public int userID;
        public Habbo user;

        public Dictionary<int, Relationship> Relations;
        public ConcurrentDictionary<string, UserAchievement> achievements;
        public List<Badge> badges;
        public List<int> favouritedRooms;
        public Dictionary<int, MessengerRequest> requests;
        public Dictionary<int, MessengerBuddy> friends;
        public Dictionary<int, int> quests;
        public Dictionary<int, UserTalent> Talents;
        public List<RoomData> rooms;
        public Dictionary<string, Subscription> subscriptions;
        public List<string> Tags;

        public UserData(int userID, ConcurrentDictionary<string, UserAchievement> achievements, List<int> favouritedRooms, List<Badge> badges, Dictionary<int, MessengerBuddy> friends, Dictionary<int, MessengerRequest> requests, List<RoomData> rooms, Dictionary<int, int> quests, Habbo user, 
            Dictionary<int, Relationship> Relations, Dictionary<int, UserTalent> talents, Dictionary<string, Subscription> subscriptions, List<string> tags)
        {
            this.userID = userID;
            this.achievements = achievements;
            this.favouritedRooms = favouritedRooms;
            this.badges = badges;
            this.friends = friends;
            this.requests = requests;
            this.rooms = rooms;
            this.quests = quests;
            this.user = user;
            Talents = talents;
            this.Relations = Relations;
            this.subscriptions = subscriptions;
            Tags = tags;
        }
    }
}