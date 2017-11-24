using System;
using System.Drawing;
using Cloud.HabboHotel.GameClients;
using Cloud.HabboHotel.Rooms.Pathfinding;
using Cloud.HabboHotel.Rooms;
using Cloud.Communication.Packets.Incoming;

using Cloud.Communication.Packets.Outgoing.Rooms.Engine;

namespace Cloud.HabboHotel.Items.Interactor
{
    public class InteractorPuzzleBox : IFurniInteractor
    {
        public void OnPlace(GameClient Session, Item Item)
        {
        }

        public void OnRemove(GameClient Session, Item Item)
        {
        }
        public void OnTrigger(GameClient Session, Item Item, int Request, bool HasRights)
        {
            if (Session == null)
                return;
            RoomUser User = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
            {
                return;
            }

            if (!((Math.Abs((User.X - Item.GetX)) >= 2) || (Math.Abs((User.Y - Item.GetY)) >= 2)))
            {
                User.SetRot(Rotation.Calculate(User.X, User.Y, Item.GetX, Item.GetY), false);
                if (User.RotBody%2 != 0)
                {
                    User.MoveTo(Item.GetX + 1, Item.GetY);
                    return;
                }
                Room Room = Item.GetRoom();
                var NewPoint = new Point(0, 0);
                if (User.RotBody == 4)
                {
                    NewPoint = new Point(Item.GetX, Item.GetY + 1);
                }

                if (User.RotBody == 0)
                {
                    NewPoint = new Point(Item.GetX, Item.GetY - 1);
                }

                if (User.RotBody == 6)
                {
                    NewPoint = new Point(Item.GetX - 1, Item.GetY);
                }

                if (User.RotBody == 2)
                {
                    NewPoint = new Point(Item.GetX + 1, Item.GetY);
                }

                if (Room.GetGameMap().ValidTile(NewPoint.X, NewPoint.Y) &&
                    Room.GetGameMap().ItemCanBePlacedHere(NewPoint.X, NewPoint.Y) &&
                    Room.GetGameMap().CanRollItemHere(NewPoint.X, NewPoint.Y))
                {
                    Double NewZ = Item.GetRoom().GetGameMap().SqAbsoluteHeight(NewPoint.X, NewPoint.Y);
                    Room.SendMessage(new SlideObjectBundleComposer(Item.GetX, Item.GetY, Item.GetZ, NewPoint.X, NewPoint.Y, NewZ, 0, 0, Item.Id));
                    Item.GetRoom()  .GetRoomItemHandler() .SetFloorItem(User.GetClient(), Item, NewPoint.X, NewPoint.Y, Item.Rotation, false, false, false);
                }
            }
            else
            {
                User.MoveTo(Item.GetX + 1, Item.GetY);
            }
        }

        public void OnWiredTrigger(Item Item)
        {
        }
    }
}