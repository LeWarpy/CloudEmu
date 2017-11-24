namespace Cloud.Communication.Packets.Outgoing.Inventory.Purse
{
	class HabboActivityPointNotificationComposer : ServerPacket
    {
        public HabboActivityPointNotificationComposer(int Balance, int Notif, int currencyType = 0)
            : base(ServerPacketHeader.HabboActivityPointNotificationMessageComposer)
        {
			WriteInteger(Balance);
			WriteInteger(Notif);
			WriteInteger(currencyType);//Type
        }
    }
}
