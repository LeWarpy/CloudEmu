using Cloud.HabboHotel.GameClients;
using Cloud.Communication.Packets.Outgoing.Help.Helpers;
using Cloud.HabboHotel.Helpers;
using Cloud.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Cloud.Communication.Packets.Incoming.Help.Helpers
{
    class HandleHelperToolEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session.GetHabbo().Rank > 2 || Session.GetHabbo()._guidelevel > 0)
            {

                var onDuty = Packet.PopBoolean();
                var isGuide = Packet.PopBoolean();
                var isHelper = Packet.PopBoolean();
                var isGuardian = Packet.PopBoolean();
                if (onDuty)
                    HelperToolsManager.AddHelper(Session, isHelper, isGuardian, isGuide);
                else
                    HelperToolsManager.RemoveHelper(Session);
                Session.SendMessage(new HandleHelperToolComposer(onDuty));
            }
            else
            {
                Session.SendMessage(new RoomNotificationComposer("Ops! carregamento ferramentas alfa:", "Por razões desconhecidas você teve um erro ao tentar iniciar as ferramentas de alfa, entre em contato com um administrador.", "", "Ok", "event:close"));
            }

        }
    }
}
