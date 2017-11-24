
using Cloud.HabboHotel.Users.Effects;

namespace Cloud.Communication.Packets.Outgoing.Inventory.AvatarEffects
{
	class AvatarEffectActivatedComposer : ServerPacket
    {
        public AvatarEffectActivatedComposer(AvatarEffect Effect)
            : base(ServerPacketHeader.AvatarEffectActivatedMessageComposer)
        {
			WriteInteger(Effect.SpriteId);
			WriteInteger((int)Effect.Duration);
			WriteBoolean(false);//Permanent
        }
    }
}