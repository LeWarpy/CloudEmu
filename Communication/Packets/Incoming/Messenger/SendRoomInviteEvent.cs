
using System.Collections.Generic;
using Cloud.Core;
using Cloud.Utilities;
using Cloud.HabboHotel.GameClients;
using Cloud.Communication.Packets.Outgoing.Messenger;
using Cloud.Database.Interfaces;
using Cloud.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Cloud.Communication.Packets.Incoming.Messenger
{
    class SendRoomInviteEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session.GetHabbo().TimeMuted > 0)
            {
                Session.SendNotification("Opa, esta silenciado - não pode enviar convites para quartos");
                return;
            }

            int Amount = Packet.PopInt();
            if (Amount > 500)
                return; // don't send at all

            List<int> Targets = new List<int>();
            for (int i = 0; i < Amount; i++)
            {
                int uid = Packet.PopInt();
                if (i < 100) // limit to 100 people, keep looping until we fulfil the request though
                {
                    Targets.Add(uid);
                }
            }

            string Message = StringCharFilter.Escape(Packet.PopString());
            if (Message.Length > 121)
                Message = Message.Substring(0, 121);

            string word;
            if (!Session.GetHabbo().GetPermissions().HasRight("word_filter_override") &&
                CloudServer.GetGame().GetChatManager().GetFilter().IsUnnaceptableWord(Message, out word))
            {
                Session.GetHabbo().BannedPhraseCount++;
                if (Session.GetHabbo().BannedPhraseCount >= 1)
                {
                    Session.GetHabbo().TimeMuted = 25;
                    Session.SendNotification("Você foi silenciado por divulgar um Hotel! " + Session.GetHabbo().BannedPhraseCount + "/3");
                    CloudServer.GetGame().GetClientManager().StaffAlert(new RoomNotificationComposer("Alerta de divulgador:",
                        "Atenção, você mencionou a palavra <b>" + word.ToUpper() + "</b><br><br><b>Frase:</b><br><i>" + Message +
                        "</i>.<br><br><b>Tipo</b><br>Spam por divulgação no chat.\r\n" + "- Este usuario: <b>" +
                        Session.GetHabbo().Username + "</b>", NotificationSettings.NOTIFICATION_FILTER_IMG, "", ""));
                }
                if (Session.GetHabbo().BannedPhraseCount >= 3)
                {
                    CloudServer.GetGame().GetModerationManager().BanUser("System", HabboHotel.Moderation.ModerationBanType.USERNAME, Session.GetHabbo().Username, "Banido por fazer spam com a frase (" + Message + ")", (CloudServer.GetUnixTimestamp() + 78892200));
                    Session.Disconnect();
                    return;
                }
                return;
            }

            foreach (int UserId in Targets)
            {
                if (!Session.GetHabbo().GetMessenger().FriendshipExists(UserId))
                    continue;

                GameClient Client = CloudServer.GetGame().GetClientManager().GetClientByUserID(UserId);
                if (Client == null || Client.GetHabbo() == null || Client.GetHabbo().AllowMessengerInvites == true || Client.GetHabbo().AllowConsoleMessages == false)
                    continue;

                Client.SendMessage(new RoomInviteComposer(Session.GetHabbo().Id, Message));

            }

            using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("INSERT INTO `chatlogs_console_invitations` (`user_id`,`message`,`timestamp`) VALUES ('" + Session.GetHabbo().Id + "', @message, UNIX_TIMESTAMP())");
                dbClient.AddParameter("message", Message);
                dbClient.RunQuery();
            }
        }
    }
}