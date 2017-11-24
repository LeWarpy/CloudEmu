using Cloud.HabboHotel.GameClients;

namespace Cloud.Communication.Packets.Incoming.Help.Helpers
{
    class HelperSessioChatTypingEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            var element = HabboHotel.Helpers.HelperToolsManager.GetElement(Session);
            if (element != null && element.OtherElement != null)
                element.OtherElement.Session.SendMessage(new Outgoing.Help.Helpers.HelperSessionChatIsTypingComposer(Packet.PopBoolean()));
        }
    }
}
