
using System.Collections.Generic;


using Cloud.HabboHotel.GameClients;
using Cloud.HabboHotel.Items.Wired;
using Cloud.Communication.Packets.Outgoing.Rooms.Furni.Wired;



namespace Cloud.HabboHotel.Items.Interactor
{
    public class InteractorWired : IFurniInteractor
    {
        public void OnPlace(GameClient Session, Item Item)
        {
        }

        public void OnRemove(GameClient Session, Item Item)
        {
            //Room Room = Item.GetRoom();
            //Room.GetWiredHandler().RemoveWired(Item);
        }
        public void OnTrigger(GameClient Session, Item Item, int Request, bool HasRights)
        {
            if (Session == null || Item == null)
                return;

            if (!HasRights)
                return;

			if (!Item.GetRoom().GetWired().TryGet(Item.Id, out IWiredItem Box))
				return;

			Item.ExtraData = "1";
            Item.UpdateState(false, true);
            Item.RequestUpdate(2, true);

            if (Item.GetBaseItem().WiredType == WiredBoxType.AddonRandomEffect)
                return;
            if (Item.GetRoom().GetWired().IsTrigger(Item))
            {
                List<int> BlockedItems = WiredBoxTypeUtility.ContainsBlockedEffect(Box, Item.GetRoom().GetWired().GetEffects(Box));
                Session.SendMessage(new WiredTriggerConfigComposer(Box, BlockedItems));
            }
            else if (Item.GetRoom().GetWired().IsEffect(Item))
            {
                List<int> BlockedItems = WiredBoxTypeUtility.ContainsBlockedTrigger(Box, Item.GetRoom().GetWired().GetTriggers(Box));
                Session.SendMessage(new WiredEffectConfigComposer(Box, BlockedItems));
            }
            else if (Item.GetRoom().GetWired().IsCondition(Item))
                Session.SendMessage(new WiredConditionConfigComposer(Box));
        }


        public void OnWiredTrigger(Item Item)
        {
        }
    }
}