using System;
using System.Linq;
using Cloud.HabboHotel.GameClients;
using Cloud.Communication.Packets.Outgoing.Rooms.Notifications;
using Cloud.Communication.Packets.Outgoing.Inventory.Purse;

namespace Cloud.HabboHotel.Rooms.Chat.Commands.Moderator
{
	class PremiarCommand : IChatCommand
	{
		public string PermissionRequired
		{
			get { return "command_give"; }
		}

		public string Parameters
		{
			get { return "%username% %badge%"; }
		}

		public string Description
		{
			get { return "Faz todas as funçoes para finalizar um evento."; }
		}

		public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
		{
			if (Params.Length == 1)
			{
				Session.SendWhisper("Por favor, digite o usuário que deseja premiar!");
				return;
			}

			GameClient Target = CloudServer.GetGame().GetClientManager().GetClientByUsername(Params[1]);
			if (Target == null)
			{
				Session.SendWhisper("Opa, não foi possível encontrar esse usuário!");
				return;
			}

			RoomUser TargetUser = Room.GetRoomUserManager().GetRoomUserByHabbo(Target.GetHabbo().Id);
			if (TargetUser == null)
			{
				Session.SendWhisper("Usuário não encontrado! Talvez ele não esteja online ou nesta sala.");
				return;
			}

			if (Target.GetHabbo().Username == Session.GetHabbo().Username)
			{
				Session.SendWhisper("Você não pode se premiar!");
				return;
			}

			if (Params.Length != 3)
			{
				Session.SendWhisper("Por favor, indique o código do emblema que gostaria de dar!");
				return;
			}


			RoomUser ThisUser = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
			if (ThisUser == null)
			{
				return;
			}
			else
			{
				CloudServer.GetGame().GetClientManager().SendMessage(new RoomNotificationComposer("rank", "message", "O usuário " + TargetUser.GetUsername() + " ganhou o evento!"));
				Target.GetHabbo().Credits = Target.GetHabbo().Credits += 500;
				Target.SendMessage(new CreditBalanceComposer(Target.GetHabbo().Credits));
				if (Session.GetHabbo().Id != Session.GetHabbo().Id)
					Target.SendWhisper("Parabens você ganhou um evento! No servidores de SAO");
				Session.SendWhisper("Concedido com sucesso " + 500 + " Credito(s) ao " + Target.GetHabbo().Username + "!");
				Target.SendMessage(new RoomNotificationComposer("goldapple", "message", "Você ganhou " + 500 + " Credito(s) parabens " + Target.GetHabbo().Username + "!"));

				Target.GetHabbo().Duckets += 500;
				Target.SendMessage(new HabboActivityPointNotificationComposer(Target.GetHabbo().Duckets, 500));
				if (Target.GetHabbo().Id != Session.GetHabbo().Id)
					Session.SendWhisper("Concedido com sucesso " + 500 + " Ducket(s) ao " + Target.GetHabbo().Username + "!");
				Target.SendMessage(new RoomNotificationComposer("coracao2", "message", "Você ganhou " + 500 + " Ducket(s)! parabens " + Target.GetHabbo().Username + "!"));

				Target.GetHabbo().Diamonds += 5;
				Target.SendMessage(new HabboActivityPointNotificationComposer(Target.GetHabbo().Diamonds, 5, 5));
				if (Target.GetHabbo().Id != Session.GetHabbo().Id)
					Session.SendWhisper("Concedido com sucesso " + 5 + " Diamond(s) ao " + Target.GetHabbo().Username + "!");

				Target.GetHabbo().GOTWPoints = Target.GetHabbo().GOTWPoints + 5;
				Target.SendMessage(new HabboActivityPointNotificationComposer(Target.GetHabbo().GOTWPoints, 5, 103));
				if (Target.GetHabbo().Id != Session.GetHabbo().Id)
					Session.SendWhisper("Concedido com sucesso " + 5 + " GOTW point(s) ao " + Target.GetHabbo().Username + "!");
				Target.SendMessage(new RoomNotificationComposer("control", "message", "Você ganhou " + 500 + " GOTW point(s)! parabens " + Target.GetHabbo().Username + "!"));

				if (!Target.GetHabbo().GetBadgeComponent().HasBadge(Params[2]))
				{
					Target.GetHabbo().GetBadgeComponent().GiveBadge(Params[2], true, Target);
					if (Target.GetHabbo().Id != Session.GetHabbo().Id)
						Target.SendMessage(new RoomNotificationComposer("game", "message", "Você acaba de receber um emblema!"));
				}
				else
					Session.SendWhisper("Uau, esse usuário já possui este emblema (" + Params[2] + ") !");

				foreach (RoomUser RoomUser in Room.GetRoomUserManager().GetUserList().ToList())
				{
					if (RoomUser == null || RoomUser.IsBot || RoomUser.GetClient() == null || RoomUser.GetClient().GetHabbo() == null || RoomUser.GetClient().GetHabbo().GetPermissions().HasRight("mod_tool") || RoomUser.GetClient().GetHabbo().Id == Session.GetHabbo().Id)
						continue;

					RoomUser.GetClient().SendNotification("Acabou o evento! ");

					Room.GetRoomUserManager().RemoveUserFromRoom(RoomUser.GetClient(), true, false);
				}
				Session.SendWhisper("Você deu com sucesso emblema " + Params[2] + "!");
				Session.SendWhisper("Kikado com sucesso a todos os usuários da sala.");
				Session.SendWhisper("Você acabou de finalizar um evento.");
			}
		}

		private void SendMessage(RoomNotificationComposer roomNotificationComposer)
		{
			throw new NotImplementedException();
		}
	}
}