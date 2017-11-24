namespace Cloud.Communication.Packets.Outgoing.LandingView
{
    class CampaignCalendarGiftComposer : ServerPacket
    {
        public CampaignCalendarGiftComposer(string iconName = "throne")
            : base(ServerPacketHeader.CampaignCalendarGiftMessageComposer)
        {
			WriteBoolean(true); // never bothered to check
			WriteString("xmas14_starfish"); //productName
			WriteString(""); //customImage
			WriteString(iconName); //iconName
        }
    }
}