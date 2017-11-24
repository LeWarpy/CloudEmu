using System;

using Cloud.HabboHotel.Items;
using Cloud.HabboHotel.Rooms;

using Cloud.Utilities;

namespace Cloud.Communication.Packets.Outgoing.Rooms.Engine
{
	class ObjectAddComposer : ServerPacket
    {
        public ObjectAddComposer(Item Item, Room Room)
            : base(ServerPacketHeader.ObjectAddMessageComposer)
        {
			WriteInteger(Item.Id);
			WriteInteger(Item.GetBaseItem().SpriteId);
			WriteInteger(Item.GetX);
			WriteInteger(Item.GetY);
			WriteInteger(Item.Rotation);
			WriteString(String.Format("{0:0.00}", TextHandling.GetString(Item.GetZ)));
			WriteString(String.Empty);

            if (Item.LimitedNo > 0)
            {
				WriteInteger(1);
				WriteInteger(256);
				WriteString(Item.ExtraData);

				WriteInteger(Item.LimitedNo);
				WriteInteger(Item.LimitedTot);
            }
            else if (Item.Data.InteractionType == InteractionType.INFO_TERMINAL)
            {
				WriteInteger(0);
				WriteInteger(1);
				WriteInteger(1);
				WriteString("internalLink");
				WriteString(Item.ExtraData);
            }
            else if (Item.Data.InteractionType == InteractionType.FX_PROVIDER)
            {
				WriteInteger(0);
				WriteInteger(1);
				WriteInteger(1);
				WriteString("effectId");
				WriteString(Item.ExtraData);
            }

            else if (Item.Data.InteractionType == InteractionType.PINATA)
            {
				WriteInteger(0);
				WriteInteger(7);
				WriteString("6");
                if (Item.ExtraData.Length <= 0) WriteInteger(0);
                else WriteInteger(int.Parse(Item.ExtraData));
				WriteInteger(100);
            }
            else if (Item.Data.InteractionType == InteractionType.PINATATRIGGERED)
            {
				WriteInteger(0);
				WriteInteger(7);
				WriteString("0");
                if (Item.ExtraData.Length <= 0) WriteInteger(0);
                else WriteInteger(int.Parse(Item.ExtraData));
				WriteInteger(1);
            }
            else if (Item.Data.InteractionType == InteractionType.MAGICEGG)
            {
				WriteInteger(0);
				WriteInteger(7);
				WriteString(Item.ExtraData);
                if (Item.ExtraData.Length <= 0)
                {
					WriteInteger(0);
                }
                else
                {
					WriteInteger(int.Parse(Item.ExtraData));
                }
				WriteInteger(23);
            }
            else if (Item.Data.InteractionType == InteractionType.MAGICCHEST)
            {
				WriteInteger(0);
				WriteInteger(7);
				WriteString(Item.ExtraData);
                if (Item.ExtraData.Length <= 0)
                {
					WriteInteger(0);
                }
                else
                {
					WriteInteger(int.Parse(Item.ExtraData));
                }
				WriteInteger(1);
            }
            else
            {
                ItemBehaviourUtility.GenerateExtradata(Item, this);
            }

			WriteInteger(-1); // to-do: check
			WriteInteger((Item.GetBaseItem().Modes > 1) ? 1 : 0);
			WriteInteger(Item.UserID);
			WriteString(Item.Username);
        }
    }
}