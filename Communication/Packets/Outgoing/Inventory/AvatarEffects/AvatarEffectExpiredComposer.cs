
using Cloud.HabboHotel.Users.Effects;

namespace Cloud.Communication.Packets.Outgoing.Inventory.AvatarEffects
{
	class AvatarEffectExpiredComposer : ServerPacket
    {
        public AvatarEffectExpiredComposer(AvatarEffect Effect)
            : base(ServerPacketHeader.AvatarEffectExpiredMessageComposer)
        {
			WriteInteger(Effect.SpriteId);
        }
    }
}
