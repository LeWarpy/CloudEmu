using Cloud.HabboHotel.GameClients;
using Cloud.HabboHotel.Rooms;

namespace Cloud.HabboHotel.Items.Interactor
{
    public interface IFurniInteractor
    {
        void OnPlace(GameClient Session, Item Item);
        void OnRemove(GameClient Session, Item Item);
        void OnTrigger(GameClient Session, Item Item, int Request, bool HasRights);
        void OnWiredTrigger(Item Item);
    }
}