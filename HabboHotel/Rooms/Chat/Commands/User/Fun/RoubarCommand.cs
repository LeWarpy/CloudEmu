using System;

using Cloud.HabboHotel.GameClients;
using Cloud.Communication.Packets.Outgoing.Rooms.Chat;

namespace Cloud.HabboHotel.Rooms.Chat.Commands.User
{
    class RoubarCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_kiss"; }
        }
        public string Parameters
        {
            get { return "[nick]"; }
        }
        public string Description
        {
            get { return "Roube alguém."; }
        }
        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Digite o nick de quem você deseja Roubar.");
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
                Session.SendWhisper("Você não pode assaltar você mesmo!");
                return;
            }
            RoomUser ThisUser = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (ThisUser == null)
                return;

            if (!(Math.Abs(TargetUser.X - ThisUser.X) > 2) || (Math.Abs(TargetUser.Y - ThisUser.Y) > 2))
            {
                ThisUser.ApplyEffect(101);
                Room.SendMessage(new ChatComposer(ThisUser.VirtualId, "*Perdeu playboy passa a grana antes que te meto um tiro*", 0, ThisUser.LastBubble));
                System.Threading.Thread.Sleep(1000);
                Room.SendMessage(new ChatComposer(ThisUser.VirtualId, "*HEHE TCHAU SEU(A) BACON*", 0, ThisUser.LastBubble));
                System.Threading.Thread.Sleep(1000);
                Room.SendMessage(new ChatComposer(TargetUser.VirtualId, "*AAAAAAAH, SOCORROOO FUI ASSALTADO(A)!!!!!*", 0, TargetUser.LastBubble));
                TargetUser.ApplyEffect(502);
                TargetUser.isLying = true;
                TargetUser.UpdateNeeded = true;
            }
            else
            {
                TimeSpan span2 = DateTime.Now - CloudServer.lastEvent;
                Session.SendWhisper("Espera " + (1 - span2.Minutes) + " para Atirar denovo.", 0);
                return;
            }
        }
    }
}
