namespace Cloud.Communication.Packets.Outgoing.Inventory.AvatarEffects
{
	class AvatarEffectAddedComposer : ServerPacket
    {
        public AvatarEffectAddedComposer(int SpriteId, int Duration)
            : base(ServerPacketHeader.AvatarEffectAddedMessageComposer)
        {
			WriteInteger(SpriteId);
			WriteInteger(1);//Types
			WriteInteger(Duration);
			WriteBoolean(false);//Permanent
        }
    }
}
