using System.Collections.Generic;

using Cloud.HabboHotel.Users.Effects;

namespace Cloud.Communication.Packets.Outgoing.Inventory.AvatarEffects
{
	class AvatarEffectsComposer : ServerPacket
    {
        public AvatarEffectsComposer(ICollection<AvatarEffect> Effects)
            : base(ServerPacketHeader.AvatarEffectsMessageComposer)
        {
			WriteInteger(Effects.Count);

            foreach (AvatarEffect Effect in Effects)
            {
				WriteInteger(Effect.SpriteId);//Effect Id
				WriteInteger(0);//Type, 0 = Hand, 1 = Full
				WriteInteger((int)Effect.Duration);
				WriteInteger(Effect.Activated ? Effect.Quantity - 1 : Effect.Quantity);
				WriteInteger(Effect.Activated ? (int)Effect.TimeLeft : -1);
				WriteBoolean(false);//Permanent
            }
        }
    }
}
