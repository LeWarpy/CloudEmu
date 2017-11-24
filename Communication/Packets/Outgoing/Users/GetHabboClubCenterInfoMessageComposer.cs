using System;
using Cloud.HabboHotel.GameClients;

namespace Cloud.Communication.Packets.Outgoing.Users
{
    class GetHabboClubCenterInfoMessageComposer : ServerPacket
    {
		public GetHabboClubCenterInfoMessageComposer(GameClient Session)
			: base(ServerPacketHeader.HabboClubCenterInfoMessageComposer)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(Session.GetHabbo().GetClubManager().GetSubscription("habbo_vip").ActivateTime);
			WriteInteger(2005);//streakduration in days 
            if (Session.GetHabbo().GetClubManager().HasSubscription("habbo_vip"))
				WriteString(origin.ToString("dd/MM/yyyy hh:mm:ss tt"));//joindate 
            else
		    WriteString("Nao foi ativada ainda!");
			WriteInteger(0);
			WriteInteger(0);//this should be a double 
			WriteInteger(0);//unused 
			WriteInteger(0);//unused 
			WriteInteger(10);//spentcredits 
			WriteInteger(20);//streakbonus 
			WriteInteger(10);//spentcredits 
			WriteInteger(60);//next pay in minutes
        }
    }
}