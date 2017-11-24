#region

using Cloud.HabboHotel.GameClients;
using Cloud.HabboHotel.Rooms;
using Cloud.HabboHotel.Items.Wired;

#endregion

namespace Cloud.HabboHotel.Items.Interactor
{
    internal class InteractorGate : IFurniInteractor
    {
        public void OnUserItem(RoomUser User, Item Item)
        {
        }

        public void OnPlace(GameClient session, Item item)
        {
        }

        public void OnRemove(GameClient session, Item item)
        {
        }

        public void OnTrigger(GameClient session, Item item, int request, bool hasRights)
        {
            if (!hasRights)
                return;
            if (item == null || item.GetBaseItem() == null || item.GetBaseItem().InteractionType != InteractionType.GATE)
                return;

            var modes = item.GetBaseItem().Modes - 1;
            if (modes <= 0)
                item.UpdateState(false, true);

            if (item.GetRoom() == null || item.GetRoom().GetGameMap() == null || item.GetRoom().GetGameMap().SquareHasUsers(item.GetX, item.GetY))
                return;

			int.TryParse(item.ExtraData, out int currentMode);
			int newMode;
            if (currentMode <= 0)
                newMode = 1;
            else if (currentMode >= modes)
                newMode = 0;
            else
                newMode = currentMode + 1;

            if (newMode == 0 && !item.GetRoom().GetGameMap().ItemCanBePlacedHere(item.GetX, item.GetY))
                return;

            item.ExtraData = newMode.ToString();
            item.UpdateState();
            item.GetRoom().GetGameMap().UpdateMapForItem(item);
            item.GetRoom().GetWired().TriggerEvent(WiredBoxType.TriggerStateChanges, session.GetHabbo(), item);
        }

        public void OnUserWalk(GameClient session, Item item, RoomUser user)
        {
        }

        public void OnWiredTrigger(Item item)
        {
            {
                var num = item.GetBaseItem().Modes - 1;
                if (num <= 0)
                {
                    item.UpdateState(false, true);
                }
                if (item.GetRoom() == null || item.GetRoom().GetGameMap() == null || item.GetRoom().GetGameMap().SquareHasUsers(item.GetX, item.GetY))
                    return;
				int.TryParse(item.ExtraData, out int num2);
				int num3;
                if (num2 <= 0)
                {
                    num3 = 1;
                }
                else
                {
                    if (num2 >= num)
                    {
                        num3 = 0;
                    }
                    else
                    {
                        num3 = num2 + 1;
                    }
                }
                if (num3 == 0 && !item.GetRoom().GetGameMap().ItemCanBePlacedHere(item.GetX, item.GetY))
                {
                    return;
                }
                item.ExtraData = num3.ToString();
                item.UpdateState();
                item.GetRoom().GetGameMap().UpdateMapForItem(item);
            }
        }
    }
}