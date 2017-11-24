using Cloud.Communication.Packets.Outgoing.Rooms.Chat;
using Cloud.HabboHotel.GameClients;
using System;

namespace Cloud.HabboHotel.Rooms.Chat.Commands.User.Fun
{

	internal class BaterCommand : IChatCommand
	{
		public void Execute(GameClient Session, Room Room, string[] Params)
		{
			if (Params.Length == 1)
			{
				Session.SendWhisper("Coloque o nome do Usuário!");
			}
			else
			{
				GameClient clientByUsername = CloudServer.GetGame().GetClientManager().GetClientByUsername(Params[1]);
				if (clientByUsername == null)
				{
					Session.SendWhisper("Desculpe, mas não encontramos o usuário!");
				}
				else
				{
					RoomUser roomUserByHabbo = Room.GetRoomUserManager().GetRoomUserByHabbo(clientByUsername.GetHabbo().Id);
					if (roomUserByHabbo == null)
					{
						Session.SendWhisper("Desculpe, mas não encontramos o usuário!");
						return;
					}
					else if (clientByUsername.GetHabbo().Username == Session.GetHabbo().Username)
					{
						Session.SendWhisper("Você está louco? Você não pode bater-se seu doente.");
						Room.SendMessage(new ChatComposer(roomUserByHabbo.VirtualId, "Alguém ajude esse usuário, é um masoquista!", 0, 34));
						return;
					}
					else if (roomUserByHabbo.TeleportEnabled)
					{
						Session.SendWhisper("Desculpe, o usuário ativou o teletransporte!");
						return;
					}
					else
					{
						RoomUser user2 = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
						RoomUser TargetID = Room.GetRoomUserManager().GetRoomUserByHabbo(clientByUsername.GetHabbo().Id);
						if (user2 != null)
						{
							if ((Math.Abs((int)(roomUserByHabbo.X - user2.X)) < 2) && (Math.Abs((int)(roomUserByHabbo.Y - user2.Y)) < 2))
							{
								Room.SendMessage(new ChatComposer(user2.VirtualId, "*Pa Pa* batendo* Toma " + Params[1] + " esse socão na cara arrombado", 0, 3));
								Room.SendMessage(new ChatComposer(TargetID.VirtualId, "*Ai, isso dói* Para por favor :(", 0, 3));
								if (roomUserByHabbo.RotBody == 4)
								{
									roomUserByHabbo.Z -= 0.35;
									roomUserByHabbo.isLying = true;
									roomUserByHabbo.UpdateNeeded = true;
									roomUserByHabbo.ApplyEffect(0x9d);
								}
								if (user2.RotBody == 0)
								{
									roomUserByHabbo.Z -= 0.35;
									roomUserByHabbo.isLying = true;
									roomUserByHabbo.UpdateNeeded = true;
									roomUserByHabbo.ApplyEffect(0x9d);
								}
								if (user2.RotBody == 6)
								{
									roomUserByHabbo.Z -= 0.35;
									roomUserByHabbo.isLying = true;
									roomUserByHabbo.UpdateNeeded = true;
									roomUserByHabbo.ApplyEffect(0x9d);
								}
								if (user2.RotBody == 2)
								{
									roomUserByHabbo.Z -= 0.35;
									roomUserByHabbo.isLying = true;
									roomUserByHabbo.UpdateNeeded = true;
									roomUserByHabbo.ApplyEffect(0x9d);
								}
								if (user2.RotBody == 3)
								{
									roomUserByHabbo.Z -= 0.35;
									roomUserByHabbo.isLying = true;
									roomUserByHabbo.UpdateNeeded = true;
									roomUserByHabbo.ApplyEffect(0x9d);
								}
								if (user2.RotBody == 1)
								{
									roomUserByHabbo.Z -= 0.35;
									roomUserByHabbo.isLying = true;
									roomUserByHabbo.UpdateNeeded = true;
									roomUserByHabbo.ApplyEffect(0x9d);
								}
								if (user2.RotBody == 7)
								{
									roomUserByHabbo.Z -= 0.35;
									roomUserByHabbo.isLying = true;
									roomUserByHabbo.UpdateNeeded = true;
									roomUserByHabbo.ApplyEffect(0x9d);
								}
								if (user2.RotBody == 5)
								{
									roomUserByHabbo.Z -= 0.35;
									roomUserByHabbo.isLying = true;
									roomUserByHabbo.UpdateNeeded = true;
									roomUserByHabbo.ApplyEffect(0x9d);
								}
								return;
							}
							else
							{
                                TimeSpan span2 = DateTime.Now - CloudServer.lastEvent;
                                Session.SendWhisper("Espera " + (1 - span2.Minutes) + " para Bater Denovo.", 0);
                                return;
							}
						}
					}
				}
			}
        }

		public string Description =>
			"Bater em algum usuario.";

		public string Parameters =>
			"[ Usuario ]";

		public string PermissionRequired =>
			"command_sit";
	}
}