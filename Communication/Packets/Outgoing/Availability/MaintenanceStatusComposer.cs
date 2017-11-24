namespace Cloud.Communication.Packets.Outgoing.Availability
{
    class MaintenanceStatusComposer : ServerPacket
    {
        public MaintenanceStatusComposer(int Minutes, int Duration)
            : base(ServerPacketHeader.MaintenanceStatusMessageComposer)
        {
			WriteBoolean(false);
			WriteInteger(Minutes);//Time till shutdown.
			WriteInteger(Duration);//Duration
        }
    }
}