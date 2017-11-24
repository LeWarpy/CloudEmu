using Cloud.HabboHotel.GameClients;
using Cloud.HabboHotel.Helpers;

namespace Cloud.Communication.Packets.Incoming.Help.Helpers
{
    class VisitHelperUserSessionEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            var Element = HelperToolsManager.GetElement(Session);
            var other = Element.OtherElement;
            if (other == null)
                return;
            if (other.Session.GetHabbo().CurrentRoom == null)
                return;

            Element.Session.SendMessage(new Outgoing.Help.Helpers.HelperSessionVisiteRoomComposer(other.Session.GetHabbo().CurrentRoom.Id));

        }
    }
}
