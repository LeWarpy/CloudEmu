using System;

using Cloud.HabboHotel.GameClients;
using Cloud.Communication.Packets.Outgoing.Help;

namespace Cloud.Communication.Packets.Incoming.Help
{
    class SubmitBullyReportEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            //0 = sent, 1 = blocked, 2 = no chat, 3 = already reported.
            if (Session == null)
                return;

            int UserId = Packet.PopInt();
            if (UserId == Session.GetHabbo().Id)//Hax
                return;

            if (Session.GetHabbo().AdvertisingReportedBlocked)
            {
                Session.SendMessage(new SubmitBullyReportComposer(1));//This user is blocked from reporting.
                return;
            }

            GameClient Client = CloudServer.GetGame().GetClientManager().GetClientByUserID(Convert.ToInt32(UserId));
            if (Client == null)
            {
                Session.SendMessage(new SubmitBullyReportComposer(0));//Just say it's sent, the user isn't found.
                return;
            }

            if (Session.GetHabbo().LastAdvertiseReport > CloudServer.GetUnixTimestamp())
            {
                Session.SendNotification("Os relatórios só podem ser enviados acada 5 minutos!");
                return;
            }

            if (Client.GetHabbo().GetPermissions().HasRight("mod_tool"))//Reporting staff, nope!
            {
                Session.SendNotification("Desculpe, você não pode informar os membros da equipe através desta ferramenta.");
                return;
            }

            //This user hasn't even said a word, nope!
            if (!Client.GetHabbo().HasSpoken)
            {
                Session.SendMessage(new SubmitBullyReportComposer(2));
                return;
            }

            //Already reported, nope.
            if (Client.GetHabbo().AdvertisingReported && Session.GetHabbo().Rank < 2)
            {
                Session.SendMessage(new SubmitBullyReportComposer(3));
                return;
            }

            if (Session.GetHabbo().Rank <= 1)
                Session.GetHabbo().LastAdvertiseReport = CloudServer.GetUnixTimestamp() + 300;
            else
                Session.GetHabbo().LastAdvertiseReport = CloudServer.GetUnixTimestamp();

            Client.GetHabbo().AdvertisingReported = true;
            Session.SendMessage(new SubmitBullyReportComposer(0));
            //CloudServer.GetGame().GetClientManager().ModAlert("New advertising report! " + Client.GetHabbo().Username + " has been reported for advertising by " + Session.GetHabbo().Username +".");
            CloudServer.GetGame().GetClientManager().DoAdvertisingReport(Session, Client);     
            return;
        }
    }
}