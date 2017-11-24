using Cloud.Communication.Packets.Outgoing.Sound;
using Cloud.HabboHotel.GameClients;


namespace Cloud.Communication.Packets.Incoming.Sound
{
    class GetJukeboxPlayListEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session.GetHabbo().CurrentRoom != null)
                Session.SendMessage(new SetJukeboxPlayListComposer(Session.GetHabbo().CurrentRoom));
        }
    }
}
