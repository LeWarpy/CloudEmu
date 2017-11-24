using System;



using Cloud.HabboHotel.GameClients;
using Cloud.Communication.Packets.Outgoing.Rooms.Chat;

namespace Cloud.HabboHotel.Rooms.Chat.Commands.User.Fun
{
	class SexCommand : IChatCommand
	{
		public string PermissionRequired
		{
			get { return "command_sexo"; }
		}
		public string Parameters
		{
			get { return "[nick]"; }
		}
		public string Description
		{
			get { return "Faça sexo com alguém."; }
		}
		public void Execute(GameClient Session, Room Room, string[] Params)
		{
			if (Params.Length == 1)
			{
				Session.SendWhisper("Digite o nick de quem você deseja sarrar.");
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
				Session.SendWhisper("Você não pode fazer sexo com você mesmo!");
				return;
			}
			RoomUser ThisUser = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
			if (ThisUser == null)
				return;

			if (!(Math.Abs(TargetUser.X - ThisUser.X) >= 2) || (Math.Abs(TargetUser.Y - ThisUser.Y) >= 2))
			{
				Room.SendMessage(new ChatComposer(ThisUser.VirtualId, "*Virando* " + Params[1] + " Pra começar a fazer um sexo gostoso", 0, ThisUser.LastBubble));
				Room.SendMessage(new ChatComposer(TargetUser.VirtualId, "Gostei da ideia* Vem me foder gostoso, filho da puta! " + Session.GetHabbo().Username + "*", 0, ThisUser.LastBubble));
				Room.SendMessage(new ChatComposer(ThisUser.VirtualId, "*Agarra, beija, chupa, coloca devagarzinho " + Params[1] + " deliciosamente até eu goza*", 0, ThisUser.LastBubble));
				Room.SendMessage(new ChatComposer(TargetUser.VirtualId, "Ai, ai, ai, to quase lá, não para vai " + Session.GetHabbo().Username + " *Vai com força, vaai sem dó*", 0, ThisUser.LastBubble));
				Room.SendMessage(new ChatComposer(TargetUser.VirtualId, "Isso, mais forte, mais forte, AAAAAAAH " + Session.GetHabbo().Username + "*Gozei gostoso*", 0, ThisUser.LastBubble));
				System.Threading.Thread.Sleep(1000);
				Room.SendMessage(new ChatComposer(TargetUser.VirtualId, "*Cai no chão de tanto tesão* vamos novamente?", 0, ThisUser.LastBubble));
				TargetUser.Statusses.Add("lay", "0.1");
				TargetUser.isLying = true;
				TargetUser.UpdateNeeded = true;
			}
			else
			{
                TimeSpan span2 = DateTime.Now - CloudServer.lastEvent;
                Session.SendWhisper("Espera " + (1 - span2.Minutes) + " para fazer mais sexo.", 0);
                return;
			}
		}
	}
}