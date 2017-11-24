using Cloud.Communication.Interfaces;
using Cloud.Communication.Packets.Outgoing.Inventory.Purse;
using Cloud.Communication.Packets.Outgoing.Rooms.Chat;
using System;
using System.Threading;

namespace Cloud.HabboHotel.Rooms.Chat.Commands.User
{
	internal class WeedCommand : IChatCommand
	{
		public string PermissionRequired
		{
			get
			{
				return "command_sit";
			}
		}

		public string Parameters
		{
			get
			{
				return "%sim%";
			}
		}

		public string Description
		{
			get
			{
				return "Fumando um baseado, os custos de maconha (5c).";
			}
		}

		public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
		{
			if (Params.Length == 1)
			{
				Session.SendNotification("Você gostaria de comprar maconha?\n\n" +
				 "Para confirmar, insira \":fumar sim\". \n\n Depois de ter feito você não pode voltar!\n\n(Se você não quiser comprar maconha, ignore esta mensagem! ;) )\n\n");
				return;
			}
			RoomUser roomUserByHabbo = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
			if (roomUserByHabbo == null)
				return;
			if (Params.Length == 2 && Params[1].ToString() == "sim")
			{
				roomUserByHabbo.GetClient().SendWhisper("Ganhou Maconha!");
				Thread.Sleep(1000);
				Room.SendMessage((IServerPacket)new ChatComposer(roomUserByHabbo.VirtualId, "* Enrola baseado *", 0, 6), false);
				Thread.Sleep(2000);
				Room.SendMessage((IServerPacket)new ChatComposer(roomUserByHabbo.VirtualId, "*vou acender e começar a fumar*", 0, 6), false);
				Thread.Sleep(2000);
				roomUserByHabbo.ApplyEffect(53);
				Thread.Sleep(2000);
				switch (new Random().Next(1, 4))
				{
					case 1:
						Room.SendMessage((IServerPacket)new ChatComposer(roomUserByHabbo.VirtualId, "Hehehe Eu vejo muitas aves :D  ", 0, 6), false);
						break;
					case 2:
						roomUserByHabbo.ApplyEffect(70);
						Room.SendMessage((IServerPacket)new ChatComposer(roomUserByHabbo.VirtualId, "Eu me sinto um panda :D ", 0, 6), false);
						break;
					default:
						Room.SendMessage((IServerPacket)new ChatComposer(roomUserByHabbo.VirtualId, "Hehehe to muito chapado :D ", 0, 6), false);
						break;
				}
				Thread.Sleep(2000);
				Room.SendMessage((IServerPacket)new ChatComposer(roomUserByHabbo.VirtualId, "HAHAAHHAHAHAHAAHAHAHHAHAHAHA LOL", 0, 6), false);
				Thread.Sleep(2000);
				roomUserByHabbo.ApplyEffect(0);
				Thread.Sleep(2000);
				Room.SendMessage((IServerPacket)new ChatComposer(roomUserByHabbo.VirtualId, "*que Maconha boa que eu ganhei *", 0, 6), false);
				Thread.Sleep(2000);
			}

		}
	}
}