using System;

using Cloud.Core;
using Cloud.Utilities;
using Cloud.HabboHotel.Quests;
using Cloud.HabboHotel.Rooms;
using Cloud.HabboHotel.GameClients;
using Cloud.HabboHotel.Rooms.Chat.Logs;
using Cloud.Communication.Packets.Outgoing.Rooms.Chat;
using Cloud.Communication.Packets.Outgoing.Moderation;
using Cloud.HabboHotel.Rooms.Chat.Styles;
using Cloud.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Cloud.Communication.Packets.Incoming.Rooms.Chat
{
	public class ChatEvent : IPacketEvent
	{
		public void Parse(GameClient Session, ClientPacket Packet)
		{
			if (Session == null || Session.GetHabbo() == null || !Session.GetHabbo().InRoom)
				return;

			Room Room = Session.GetHabbo().CurrentRoom;
			if (Room == null)
				return;

			RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
			if (User == null)
				return;

			string Message = StringCharFilter.Escape(Packet.PopString());
			if (Message.Length > 100)
				Message = Message.Substring(0, 100);

			int Colour = Packet.PopInt();

			if (!CloudServer.GetGame().GetChatManager().GetChatStyles().TryGetStyle(Colour, out ChatStyle Style) || (Style.RequiredRight.Length > 0 && !Session.GetHabbo().GetPermissions().HasRight(Style.RequiredRight)))
				Colour = 0;

			User.UnIdle();

			if (CloudServer.GetUnixTimestamp() < Session.GetHabbo().FloodTime && Session.GetHabbo().FloodTime != 0)
				return;

			if (Session.GetHabbo().TimeMuted > 0)
			{
				Session.SendMessage(new MutedComposer(Session.GetHabbo().TimeMuted));
				return;
			}

			if (!Session.GetHabbo().GetPermissions().HasRight("room_ignore_mute") && Room.CheckMute(Session))
			{
				Session.SendWhisper("Ops, você está mutado!");
				return;
			}

			User.LastBubble = Session.GetHabbo().CustomBubbleId == 0 ? Colour : Session.GetHabbo().CustomBubbleId;

			if (!Session.GetHabbo().GetPermissions().HasRight("mod_tool"))
			{
				if (User.IncrementAndCheckFlood(out int MuteTime))
				{
					Session.SendMessage(new FloodControlComposer(MuteTime));
					return;
				}
			}

			if (Message.StartsWith(":", StringComparison.CurrentCulture) && CloudServer.GetGame().GetChatManager().GetCommands().Parse(Session, Message))
				return;

			CloudServer.GetGame().GetChatManager().GetLogs().StoreChatlog(new ChatlogEntry(Session.GetHabbo().Id, Room.Id, Message, UnixTimestamp.GetNow(), Session.GetHabbo(), Room));

			if (!Session.GetHabbo().GetPermissions().HasRight("word_filter_override") &&
	CloudServer.GetGame().GetChatManager().GetFilter().IsUnnaceptableWord(Message, out string word))
			{
				Session.GetHabbo().BannedPhraseCount++;
				if (Session.GetHabbo().BannedPhraseCount >= 1)
				{

					User.MoveTo(Room.GetGameMap().Model.DoorX, Room.GetGameMap().Model.DoorY);
					Session.GetHabbo().TimeMuted = 25;
					Session.SendNotification("Você está mutado, peça a um moderador para rever seu caso! <b>Aviso: " + Session.GetHabbo().BannedPhraseCount + "/5</b>");
					CloudServer.GetGame().GetClientManager().StaffAlert(new RoomNotificationComposer("Alerta de divulgação:",
						"Atenção você mencionou a palavra <b>" + word.ToUpper() + "</b> na frase <i>" + Message +
						"</i> dentro de um quarto\r\n" + "- Este usuario: <b>" +
						Session.GetHabbo().Username + "</b>", NotificationSettings.NOTIFICATION_FILTER_IMG, "Ir ao quarto", "event:navigator/goto/" +
						Session.GetHabbo().CurrentRoomId));
				}
				if (Session.GetHabbo().BannedPhraseCount >= 5)
				{
					CloudServer.GetGame().GetModerationManager().BanUser("System", HabboHotel.Moderation.ModerationBanType.USERNAME, Session.GetHabbo().Username, "banido por spam (" + Message + ")", (CloudServer.GetUnixTimestamp() + 78892200));
					Session.Disconnect();
					return;
				}

				Session.SendMessage(new ChatComposer(User.VirtualId, "Mensagem inapropiado", 0, Colour));
				return;
			}


			CloudServer.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.SOCIAL_CHAT);

			User.OnChat(User.LastBubble, Message, false);
		}
	}
}