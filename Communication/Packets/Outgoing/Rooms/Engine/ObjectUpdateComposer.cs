using System;

using Cloud.Utilities;
using Cloud.HabboHotel.Items;


namespace Cloud.Communication.Packets.Outgoing.Rooms.Engine
{
	class ObjectUpdateComposer : ServerPacket
    {
        public ObjectUpdateComposer(Item Item, int UserId)
            : base(ServerPacketHeader.ObjectUpdateMessageComposer)
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
                if (Item.ExtraData.Length <= 0)
                {
					WriteString("6");
					WriteInteger(0);
                }
                else
                {
					WriteString((int.Parse(Item.ExtraData) == 1) ? "8" : "6");
					WriteInteger(int.Parse(Item.ExtraData));
                }
				WriteInteger(1);
            }
            else if (Item.Data.InteractionType == InteractionType.PINATATRIGGERED)
            {
				WriteInteger(0);
				WriteInteger(7);  // miran2 grafic xq no c acuerda xdddddd kva men xDDDDDDDD esk me mandaron un guasap menju eeeer xqude popddddduddddddddddddddddxdd
				WriteString((Item.ExtraData.Length <= 0) ? "0" : "2");
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
			WriteInteger(UserId);
        }
    }
        class UpdateFootBallComposer : ServerPacket
        {

            public UpdateFootBallComposer(Item Item, int newX, int newY)
                : base(ServerPacketHeader.ObjectUpdateMessageComposer)
            {
			WriteInteger(Item.Id);
			WriteInteger(Item.GetBaseItem().SpriteId);
			WriteInteger(newX);
			WriteInteger(newY);
			WriteInteger(4); // rot;
			WriteString((String.Format("{0:0.00}", TextHandling.GetString(Item.GetZ))));
			WriteString((String.Format("{0:0.00}", TextHandling.GetString(Item.GetZ))));
			WriteInteger(0);
			WriteInteger(0);
			WriteString(Item.ExtraData);
			WriteInteger(-1);
			WriteInteger(0);
			WriteInteger(Item.UserID);
            }
        }
    }