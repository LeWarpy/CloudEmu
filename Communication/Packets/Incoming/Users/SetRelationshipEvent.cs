using System;
using Cloud.HabboHotel.GameClients;
using Cloud.HabboHotel.Users.Relationships;
using Cloud.HabboHotel.Users;
using Cloud.Communication.Packets.Outgoing.Messenger;
using Cloud.HabboHotel.Users.Messenger;
using Cloud.Database.Interfaces;
using Cloud.Communication.Packets.Outgoing.Moderation;

namespace Cloud.Communication.Packets.Incoming.Users
{
    class SetRelationshipEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null || Session.GetHabbo().GetMessenger() == null)
                return;

            int User = Packet.PopInt();
            int Type = Packet.PopInt();

            if (!Session.GetHabbo().GetMessenger().FriendshipExists(User))
            {
                Session.SendMessage(new BroadcastMessageAlertComposer("Ops, você só pode estabelecer um relacionamento aonde existe uma amizade."));
                return;
            }

            if (Type < 0 || Type > 3)
            {
                Session.SendMessage(new BroadcastMessageAlertComposer("Ops, você escolheu um relacionamento invalido."));
                return;
            }

            if (Session.GetHabbo().Relationships.Count > 2000)
            {
                Session.SendMessage(new BroadcastMessageAlertComposer("Sentimos muito, esta limitado a 2000 relacionamenos."));
                return;
            }

            using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                if (Type == 0)
                {
                    dbClient.SetQuery("SELECT `id` FROM `user_relationships` WHERE `user_id` = '" + Session.GetHabbo().Id + "' AND `target` = @target LIMIT 1");
                    dbClient.AddParameter("target", User);
                    int Id = dbClient.getInteger();

                    dbClient.SetQuery("DELETE FROM `user_relationships` WHERE `user_id` = '" + Session.GetHabbo().Id + "' AND `target` = @target LIMIT 1");
                    dbClient.AddParameter("target", User);
                    dbClient.RunQuery();

                    if (Session.GetHabbo().Relationships.ContainsKey(User))
                        Session.GetHabbo().Relationships.Remove(User);
                }
                else
                {
                    dbClient.SetQuery("SELECT id FROM `user_relationships` WHERE `user_id` = '" + Session.GetHabbo().Id + "' AND `target` = @target LIMIT 1");
                    dbClient.AddParameter("target", User);
                    int Id = dbClient.getInteger();

                    if (Id > 0)
                    {
                        dbClient.SetQuery("DELETE FROM `user_relationships` WHERE `user_id` = '" + Session.GetHabbo().Id + "' AND `target` = @target LIMIT 1");
                        dbClient.AddParameter("target", User);
                        dbClient.RunQuery();

                        if (Session.GetHabbo().Relationships.ContainsKey(Id))
                            Session.GetHabbo().Relationships.Remove(Id);
                    }

                    dbClient.SetQuery("INSERT INTO `user_relationships` (`user_id`,`target`,`type`) VALUES ('" + Session.GetHabbo().Id + "', @target, @type)");
                    dbClient.AddParameter("target", User);
                    dbClient.AddParameter("type", Type);
                    int newId = Convert.ToInt32(dbClient.InsertQuery());

                    if (!Session.GetHabbo().Relationships.ContainsKey(User))
                        Session.GetHabbo().Relationships.Add(User, new Relationship(newId, User, Type));
                }

                GameClient Client = CloudServer.GetGame().GetClientManager().GetClientByUserID(User);
                if (Client != null)
                    Session.GetHabbo().GetMessenger().UpdateFriend(User, Client, true);
                else
                {
                    Habbo Habbo = CloudServer.GetHabboById(User);
                    if (Habbo != null)
                    {
                        MessengerBuddy Buddy = null;
                        if (Session.GetHabbo().GetMessenger().TryGetFriend(User, out Buddy))
                            Session.SendMessage(new FriendListUpdateComposer(Session, Buddy));
                    }
                }
            }
        }
    }
}