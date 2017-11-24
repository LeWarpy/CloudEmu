using Cloud.Communication.Packets.Outgoing.Inventory.Purse;
using Cloud.HabboHotel.GameClients;
using Cloud.HabboHotel.Rooms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloud.HabboHotel.Items.Interactor
{
    class InteractorAlertFurniture : IFurniInteractor
    {

        public void OnPlace(GameClient Session, Item Item)
        {
            if (Item.ExtraData == "-1")
            {
                Item.ExtraData = "0";
                Item.UpdateNeeded = true;
            }
        }

        public void OnRemove(GameClient Session, Item Item)
        {
            if (Item.ExtraData == "-1")
            {
                Item.ExtraData = "0";
            }
        }

        public void OnTrigger(GameClient Session, Item Item, int Request, bool HasRights)
        {
            RoomUser User = null;
            if (Session != null)
                User = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
                return;

            if (Gamemap.TilesTouching(Item.GetX, Item.GetY, User.X, User.Y))
            {
                //if (Item.NextCommand != 0)
                //{
                //    if (Item.NextCommand > CloudServer.Now())
                //    {
                //        Session.SendWhisper("Advent cooldown", 3);
                //        return;
                //    }
                //}
                //Item.NextCommand = CloudServer.Now() + 3600000;

                int Amount = 10;
                Session.SendShout("Aha! Its the christmas duck! *Enjoy your 10 diamonds!");
                Session.GetHabbo().Diamonds += Amount;
                Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Diamonds, Amount, 5));
                Session.SendNotification("Thank you for participating Pure's 2016 Christmas hunt! You have find the rare christmas duck! As a reward, you have recieved 10 diamonds!");
                Item.ExtraData = "0";
                Item.UpdateState();


            }
            else
            {
                User.MoveTo(Item.SquareInFront);
            }
        }

        public void OnWiredTrigger(Item Item)
        {
            Item.ExtraData = "-1";
            Item.UpdateState(false, true);
            Item.RequestUpdate(4, true);
        }
    }
}
