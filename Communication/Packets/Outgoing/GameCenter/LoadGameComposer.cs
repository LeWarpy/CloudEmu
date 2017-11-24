using Cloud.HabboHotel.GameClients;
using Cloud.HabboHotel.Games;

namespace Cloud.Communication.Packets.Outgoing.GameCenter
{
    class LoadGameComposer : ServerPacket
    {
        public LoadGameComposer(GameClient Session, GameData GameData, string SSOTicket)
            : base(ServerPacketHeader.LoadGameMessageComposer)
        {
			WriteInteger(GameData.GameId);
			WriteString(Session.GetHabbo().Id.ToString());
			WriteString(GameData.ResourcePath + GameData.GameSWF);
			WriteString("best");
			WriteString("showAll");
			WriteInteger(60);//FPS?
			WriteInteger(10);
			WriteInteger(8);
			WriteInteger(6);//Asset count
			WriteString("assetUrl");
			WriteString(GameData.ResourcePath + GameData.GameAssets);
			WriteString("habboHost");
			WriteString("http://fuseus-private-httpd-fe-1");
			WriteString("accessToken");
			WriteString(SSOTicket);
			WriteString("gameServerHost");
			WriteString((GameData.GameServerHost == "clientip") ? Session.GetConnection().getIp() : GameData.GameServerHost);
			WriteString("gameServerPort");
			WriteString(GameData.GameServerPort);
			WriteString("socketPolicyPort");
			WriteString(GameData.GameServerHost);
        }
    }
}
