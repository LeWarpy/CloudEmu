﻿using Cloud.Communication.Packets.Outgoing;
using Cloud.Communication.Packets.Outgoing.Rooms.Furni.LoveLocks;
using Cloud.HabboHotel.GameClients;
using Cloud.HabboHotel.Rooms;
using System;
using System.Drawing;

namespace Cloud.HabboHotel.Items.Interactor
{
    public class InteractorLoveLock : IFurniInteractor
    {
        public void OnPlace(GameClient Session, Item Item)
        {
        }

        public void OnRemove(GameClient Session, Item Item)
        {
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
                if (Item.ExtraData == null || Item.ExtraData.Length <= 1 || !Item.ExtraData.Contains(Convert.ToChar(5).ToString()))
                {
                    Point pointOne;
                    Point pointTwo;

                    switch (Item.Rotation)
                    {
                        case 2:
                            pointOne = new Point(Item.GetX, Item.GetY + 1);
                            pointTwo = new Point(Item.GetX, Item.GetY - 1);
                            break;

                        case 4:
                            pointOne = new Point(Item.GetX - 1, Item.GetY);
                            pointTwo = new Point(Item.GetX + 1, Item.GetY);
                            break;

                        default:
                            return;
                    }

                    RoomUser UserOne = Item.GetRoom().GetRoomUserManager().GetUserForSquare(pointOne.X, pointOne.Y);
                    RoomUser UserTwo = Item.GetRoom().GetRoomUserManager().GetUserForSquare(pointTwo.X, pointTwo.Y);

                    if(UserOne == null || UserTwo == null)
                        Session.SendNotification("No pudimos encontrar un usuario válido para bloquear este bloqueo de amor.");
                    else if(UserOne.GetClient() == null || UserTwo.GetClient() == null)
                        Session.SendNotification("No pudimos encontrar un usuario válido para bloquear este bloqueo de amor.");
                    else if(UserOne.HabboId != Item.UserID && UserTwo.HabboId != Item.UserID)
                        Session.SendNotification("Usted puede utilizar solamente este artículo con el dueño del artículor.");
                    else
                    {
                        UserOne.CanWalk = false;
                        UserTwo.CanWalk = false;

                        Item.InteractingUser = UserOne.GetClient().GetHabbo().Id;
                        Item.InteractingUser2 = UserTwo.GetClient().GetHabbo().Id;

                        UserOne.GetClient().SendMessage(new LoveLockDialogueMessageComposer(Item.Id));
                        UserTwo.GetClient().SendMessage(new LoveLockDialogueMessageComposer(Item.Id));
                    }


                }
                else
                    return;
            }
            else
            {
                User.MoveTo(Item.SquareInFront);
            }
        }

        public void OnWiredTrigger(Item Item)
        {
        }
    }
}