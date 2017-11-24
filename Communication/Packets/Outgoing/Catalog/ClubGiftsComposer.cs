

namespace Cloud.Communication.Packets.Outgoing.Catalog
{
    class ClubGiftsComposer : ServerPacket
    {
        public ClubGiftsComposer() 
            : base(ServerPacketHeader.ClubGiftsMessageComposer)
        {
			WriteInteger(-1);//Days until next gift.
			WriteInteger(0);//Gifts available
			WriteInteger(12);//Count?
            {
				WriteInteger(12701);
				WriteString("hc16_1");
				WriteBoolean(false);
				WriteInteger(1);
				WriteInteger(0);
				WriteInteger(0);
				WriteBoolean(true);
				WriteInteger(1);//Count for some reason
                {
					WriteString("s");
					WriteInteger(8228);
					WriteString("");
					WriteInteger(1);
					WriteBoolean(false);
                }
              //  base.WriteInteger(0);
                //base.WriteBoolean(true);
            }

			WriteInteger(0);//Count
            {
				//int, bool, int, bool
				WriteInteger(3253248);//Maybe the item id?

				WriteBoolean(false);//Can we get?
				WriteInteger(256);//idk
				WriteBoolean(false);//idk
				WriteInteger(0);
				WriteBoolean(true);

            }
        }
    }
}
