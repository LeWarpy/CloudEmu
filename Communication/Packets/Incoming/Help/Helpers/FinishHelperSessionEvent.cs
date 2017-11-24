using Cloud.Communication.Packets.Outgoing.Rooms.Notifications;
using Cloud.HabboHotel.GameClients;
using Cloud.HabboHotel.Helpers;

namespace Cloud.Communication.Packets.Incoming.Help.Helpers
{
    class FinishHelperSessionEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            var Voted = Packet.PopBoolean();
            var Element = HelperToolsManager.GetElement(Session);
            if (Element is HelperCase)
            {
                if (Voted)
                    Element.OtherElement.Session.SendMessage(RoomNotificationComposer.SendBubble("embaixador", "" + Session.GetHabbo().Username + ", Obrigado por participar no programa Alfa, você satisfatoriamente abordou a questão do usuário.", "catalog/open/habbiween"));
                else
                    Element.OtherElement.Session.SendMessage(RoomNotificationComposer.SendBubble("embaixador", "" + Session.GetHabbo().Username + ", Obrigado por participar no programa Alfa, você satisfatoriamente abordou a questão do usuário.", "catalog/open/habbiween"));
            }

            Element.Close();
        }
    }
}
