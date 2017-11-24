using System;


using Cloud.HabboHotel.GameClients;
using Cloud.Communication.Packets.Outgoing.Rooms.Chat;

namespace Cloud.HabboHotel.Rooms.Chat.Commands.User
{
    class UziCPCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get { return "command_kiss"; }
        }

        public string Parameters
        {
            get { return "%username%"; }
        }

        public string Description
        {
            get { return "Dispara com a uzi!"; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Escreva um nome de usuário.");
                return;
            }

            GameClient TargetClient = CloudServer.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetClient == null)
            {
                Session.SendWhisper("*ERRO* Nome do usuário não foi encontrado.");
                return;
            }

            RoomUser TargetUser = Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Id);
            RoomUser SessionUser = Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (TargetUser == null)
            {
                Session.SendWhisper("*ERRO* Nome de utilizador não foi encontrado.");
                return;
            }
            if (Session.GetHabbo().Username == TargetClient.GetHabbo().Username)
            {
                Session.SendWhisper("Você não pode, desculpe :(");
                return;
            }
            if (!((Math.Abs(TargetUser.X - SessionUser.X) >= 2) || (Math.Abs(TargetUser.Y - SessionUser.Y) >= 2)))
            {
                RoomUser ThisUser = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
                Room.SendMessage(new ChatComposer(ThisUser.VirtualId, "*Peguei a Uzi e começar a atirar " + Params[1] + " *", 0, 0));
                Room.SendMessage(new ChatComposer(TargetUser.VirtualId, "*Ahiii, você me mato :( *", 0, 0));
                Session.GetHabbo().Effects().ApplyEffect(580);
                TargetClient.GetHabbo().Effects().ApplyEffect(93);
                TargetUser.Statusses.Add("lay", "1.0 null");
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