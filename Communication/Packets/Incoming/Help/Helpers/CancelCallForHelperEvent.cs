using Cloud.Communication.Packets.Outgoing.Help.Helpers;
using Cloud.HabboHotel.GameClients;
using Cloud.HabboHotel.Helpers;

namespace Cloud.Communication.Packets.Incoming.Help.Helpers
{
    class CancelCallForHelperEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            var call = HelperToolsManager.GetCall(Session);
            HelperToolsManager.RemoveCall(call);
            Session.SendMessage(new CloseHelperSessionComposer());
            if (call.Helper != null)
            {
                call.Helper.CancelCall();
            }

            Session.SendMessage(new CloseHelperSessionComposer());
        }
    }
}
