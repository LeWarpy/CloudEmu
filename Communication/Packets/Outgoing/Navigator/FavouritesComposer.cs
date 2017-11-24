using System.Collections;

namespace Cloud.Communication.Packets.Outgoing.Navigator
{
	class FavouritesComposer : ServerPacket
    {
        public FavouritesComposer(ArrayList favouriteIDs)
            : base(ServerPacketHeader.FavouritesMessageComposer)
        {
			WriteInteger(50);
			WriteInteger(favouriteIDs.Count);

            foreach (int Id in favouriteIDs.ToArray())
            {
				WriteInteger(Id);
            }
        }
    }
}
