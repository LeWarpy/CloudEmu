using System;
using Cloud.Core;
using Cloud.HabboHotel.GameClients;
using Cloud.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Cloud.HabboHotel.Rooms.Chat.Commands.User
{
    class InfoCommand : IChatCommand
    {
        public string PermissionRequired => "command_pickall";
        public string Parameters => "";
        public string Description => "Mostra Informaçoes do servidor.";

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            TimeSpan Uptime = DateTime.Now - CloudServer.ServerStarted;
            int OnlineUsers = CloudServer.GetGame().GetClientManager().Count;
            int RoomCount = CloudServer.GetGame().GetRoomManager().Count;

            Session.SendMessage(new RoomNotificationComposer("Dual Server" + CloudServer.PrettyBuild + " " + CloudServer.VersionCloud + ":",
                 "<font color=\"#8904B1\"><b> Dual Server</b></font>\n" +
                 "<font size=\"11\" color=\"#1C1C1C\"> Dual Server " + CloudServer.HotelName + " </font>" +
                 "<font size=\"11\" color=\"#1C1C1C\">Baseado no Quasar server  tem um ótimo desempenho e funções Básicas para Habbo!!</font>\n\n" +
                 "<font color=\"#3f51b5\" size=\"13\"><b>Estatísticas:</b></font>\n" +
                 "<font size=\"11\" color=\"#1C1C1C\">  <b> · Usuarios: </b> " + OnlineUsers + "</font>\n" +
                 "<font size=\"11\" color=\"#1C1C1C\">  <b> · Salas: </b> " + RoomCount + "</font>\n" +
                 "<font size=\"11\" color=\"#1C1C1C\">  <b> · Update: </b> " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt") + "</font>\n" +
                 "<font size=\"11\" color=\"#1C1C1C\">  <b> · Tempo: </b> " + Uptime.Days + " día(s), " + Uptime.Hours + " horas & " + Uptime.Minutes + " minutos.</font>\n" +
				 "<font size=\"11\" color=\"#1C1C1C\"> <b>  · Recorde: </b>  " + Game.SessionUserRecord + "</font>\n\n" +
				 "<font color=\"#3f51b5\" size=\"13\"><b>Developers:</b></font>\n" +
				 "<font color=\"#1C1C1C\" size=\"11\"> · Xjoao (Fix+Coder).</font>\n" +
				 "<font color=\"#1C1C1C\" size=\"11\"> · Paulo (Bibliotecas+Otimização+Tradução).</font>\n\n" +
                 "<font color=\"#8904B1\">Licenca:  <b>" + CloudServer.Licenseto + "</b></font>\n\n" +
				 "<font color=\"#303f9f\" size=\"13\"><b>Últimas Características:</b></font>\n" +
                 "<font color=\"#1C1C1C\" size=\"9\"> · 95% DO LAG RETIRADO.</font>\n" +
                 "<font color=\"#1C1C1C\" size=\"9\"> · FIX WIRED GAME END + TIMER.</font>\n" +
                 "<font color=\"#1C1C1C\" size=\"9\"> · COMANDO ROOM RETIRADO + FIX DE ULTRAPASSAR + NOVOS COMANDOS.</font>\n" +
                 "<font color=\"#1C1C1C\" size=\"9\"> · RELEASE 2017 > 2018 COMING SOON.</font>\n", NotificationSettings.NOTIFICATION_ABOUT_IMG, ""));
        }
    }
}
