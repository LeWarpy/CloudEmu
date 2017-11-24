

namespace Cloud.HabboHotel.Rooms.Chat.Commands.User.Fun
{
	class ColourCommand : IChatCommand
	{

		public string PermissionRequired
		{
			get { return "command_dnd"; }
		}
		public string Parameters
		{
			get { return ""; }
		}
		public string Description
		{
			get { return "off/red/green/blue/purple//BETA"; }
		}
		public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
		{
			if (Params.Length == 1)
			{
				Session.SendWhisper("Você deve selecionar a cor que você quiser.");
				return;
			}
			string chatColour = Params[1];
			string Colour = chatColour.ToLower();
			switch (chatColour)
			{
				case "none":
				case "black":
				case "off":
					Session.GetHabbo().chatColour = "";
					Session.SendWhisper("Colorir o chat foi desativado.");
					break;
				case "blue":
				case "red":
				case "cyan":
				case "purple":
				case "green":
					Session.GetHabbo().chatColour = chatColour;
					Session.SendWhisper("@" + Colour + "@Eles estabeleceram sua cor: " + Colour + "");
					break;
				default:
					Session.SendWhisper("A cor: " + Colour + " nao existe.");
					break;
			}
			return;
		}
	}
}