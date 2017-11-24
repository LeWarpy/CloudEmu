using System;

namespace Cloud.HabboHotel.Items.Interactor
{
	internal class InteractorTrax : IFurniInteractor
	{
        public Item Item;
        public InteractorTrax(Item item)
        {
            Item = item;
        }

        public void OnPlace(GameClients.GameClient Session, Item Item)
        {
        }
        public void OnRemove(GameClients.GameClient Session, Item Item)
        {
        }

        public void OnWiredTrigger()
        {
        }

        public void OnTrigger(GameClients.GameClient Session, Item item, int Request, bool HasRights)
        {
        }

        public void OnWiredTrigger(Item item)
        {
        }
    }
}
