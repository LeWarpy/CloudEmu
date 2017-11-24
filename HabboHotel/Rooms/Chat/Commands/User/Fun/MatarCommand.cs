using System;
using Cloud.HabboHotel.GameClients;
using Cloud.Communication.Packets.Outgoing.Rooms.Chat;

namespace Cloud.HabboHotel.Rooms.Chat.Commands.User.Fun
{
	class MatarCommand : IChatCommand
	{
		public string PermissionRequired
		{
			get { return "command_sit"; }
		}
		public string Parameters
		{
			get { return "[nick]"; }
		}
		public string Description
		{
			get { return "Mate alguém."; }
		}
		public void Execute(GameClient Session, Room Room, string[] Params)
		{
			if (Params.Length == 1)
			{
				Session.SendWhisper("Digite o nick de quem você deseja matar.");
				return;
			}
			GameClient TargetClient = CloudServer.GetGame().GetClientManager().GetClientByUsername(Params[1]);
			if (TargetClient == null)
			{
				Session.SendWhisper("Essa pessoa não se encontra no quarto ou não está online.");
				return;
			}
			RoomUser TargetUser = Room.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Id);
			if (TargetUser == null)
			{
				Session.SendWhisper("Ocorreu um erro, esse usuário não foi encontrado.");
			}
			if (TargetClient.GetHabbo().Username == Session.GetHabbo().Username)
			{
				Session.SendWhisper("Tá louco querendo se matar? Seu Nutella!");
				return;
			}
			RoomUser ThisUser = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
			if (ThisUser == null)
				return;

			if (!(Math.Abs(TargetUser.X - ThisUser.X) >= 2) || (Math.Abs(TargetUser.Y - ThisUser.Y) >= 2))
			{
				Room.SendMessage(new ChatComposer(ThisUser.VirtualId, "*Pow Pow, Te matei " + Params[1] + ", se fode aí arrombado*", 0, ThisUser.LastBubble));
				System.Threading.Thread.Sleep(1000);
				Room.SendMessage(new ChatComposer(TargetUser.VirtualId, "*Eu não esperava isso de você* :(", 0, ThisUser.LastBubble));
				TargetUser.Statusses.Add("lay", "0.1");
				TargetUser.isLying = true;
				TargetUser.UpdateNeeded = true;
			}
			else
			{
				Session.SendWhisper("Chegue mais perto da pessoa ou aguarde mais tempo para fazer novamente.");
				return;
			}
		}
	}
}