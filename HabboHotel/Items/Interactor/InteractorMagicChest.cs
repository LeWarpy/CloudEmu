using Cloud.Communication.Packets.Outgoing.Rooms.Engine;
using Cloud.Communication.Packets.Outgoing.Rooms.Notifications;
using Cloud.HabboHotel.Rooms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloud.HabboHotel.Items.Interactor
{
    class InteractorMagicChest : IFurniInteractor
    {
        public void OnPlace(GameClients.GameClient Session, Item Item)
        {
        }

        public void OnRemove(GameClients.GameClient Session, Item Item)
        {
        }

        public void OnTrigger(GameClients.GameClient Session, Item Item, int Request, bool HasRights)
        {
            if (Session == null || Session.GetHabbo() == null || Item == null)
                return;

            Room Room = Session.GetHabbo().CurrentRoom;
            if (Room == null)
                return;

            RoomUser Actor = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (Actor == null)
                return;

            var tick = int.Parse(Item.ExtraData);

            if (tick < 1)
            {
                    if (Gamemap.TileDistance(Actor.X, Actor.Y, Item.GetX, Item.GetY) < 2)
                    {
                        tick++;
                        Item.ExtraData = tick.ToString();
                        Item.UpdateState(true, true);
                        int X = Item.GetX, Y = Item.GetY, Rot = Item.Rotation;
                        Double Z = Item.GetZ;
                        if (tick == 1)
                        {
                            CloudServer.GetGame().GetPinataManager().ReceiveCrackableReward(Actor, Room, Item);
                            CloudServer.GetGame().GetAchievementManager().ProgressAchievement(Actor.GetClient(), "ACH_PinataWhacker", 1);
                            CloudServer.GetGame().GetAchievementManager().ProgressAchievement(Actor.GetClient(), "ACH_PinataBreaker", 1);
                        }
                }
            }
        }

        public void OnWiredTrigger(Item Item)
        {

        }
    }
}
