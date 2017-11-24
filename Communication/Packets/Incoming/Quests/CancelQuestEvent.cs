namespace Cloud.Communication.Packets.Incoming.Quests
{
	class CancelQuestEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            CloudServer.GetGame().GetQuestManager().CancelQuest(Session, Packet);
        }
    }
}
