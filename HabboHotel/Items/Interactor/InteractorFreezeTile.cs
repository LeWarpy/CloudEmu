using System;

using Cloud.HabboHotel.Items;
using Cloud.HabboHotel.Rooms;
using Cloud.HabboHotel.Rooms.Games;
using Cloud.HabboHotel.GameClients;
using Cloud.HabboHotel.Rooms.Games.Teams;

namespace Cloud.HabboHotel.Items.Interactor
{
    class InteractorFreezeTile : IFurniInteractor
    {
        public void OnPlace(GameClient Session, Item Item)
        {
        }

        public void OnRemove(GameClient Session, Item Item)
        {
        }

        public void OnTrigger(GameClient Session, Item Item, int Request, bool HasRights)
        {
            if (Session == null || !Session.GetHabbo().InRoom || Item == null || Item.InteractingUser > 0)
                return;

            RoomUser User = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
                return;

            if (User.Team != TEAM.NONE)
            {
                User.FreezeInteracting = true;
                Item.InteractingUser = Session.GetHabbo().Id;

                if (Item.Data.InteractionType == InteractionType.FREEZE_TILE_BLOCK)
                {
                    if (Gamemap.TileDistance(User.X, User.Y, Item.GetX, Item.GetY) < 2)
                        Item.GetRoom().GetFreeze().onFreezeTiles(Item, Item.freezePowerUp);
                }
            }
        }

        public void OnWiredTrigger(Item Item)
        {

        }
    }
}