using Cloud.HabboHotel.GameClients;
using Cloud.HabboHotel.Rooms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloud.HabboHotel.Items.Interactor
{
    class InteractorMuteSignal : IFurniInteractor
    {
        private const int Modes = 1;

        public void OnPlace(GameClient Session, Item Item) { }
        public void OnRemove(GameClient Session, Item Item) { }
        public void OnWiredTrigger(Item Item) { }
        public void OnTrigger(GameClient Session, Item Item, int Request, bool UserHasRights)
        {
            if (!UserHasRights)
            {
                return;
            }
            var currentMode = 0;
            var newMode = 0;

            try
            {
                currentMode = int.Parse(Item.ExtraData);
            }
            catch
            {

            }

            if (currentMode <= 0)
            {
                newMode = 1;
            }
            else if (currentMode >= Modes)
            {
                newMode = 0;
            }
            else
            {
                newMode = currentMode + 1;
            }

            //1 = muted, 0 = no mute
            Room UserRoom = Item.GetRoom();
            switch (newMode)
            {
                case 0:
                    UserRoom.muteSignalEnabled = false;
                    break;

                case 1:
                    UserRoom.muteSignalEnabled = true;
                    break;
            }

            Item.ExtraData = newMode.ToString();
            Item.UpdateState();
        }
    }
}
