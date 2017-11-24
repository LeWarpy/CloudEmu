namespace Cloud.Communication.Packets.Incoming.Quests
{
	class GetCurrentQuestEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            CloudServer.GetGame().GetQuestManager().GetCurrentQuest(Session, Packet);
        }
    }
}
