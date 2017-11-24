using Cloud.HabboHotel.GameClients;
using Cloud.Database.Interfaces;
using Cloud.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Cloud.HabboHotel.Rooms.AI.Types
{
    class CasinoCounter : BotAI
    {
        private int VirtualId;

        public CasinoCounter(int VirtualId)
        {
            this.VirtualId = VirtualId;
        }

        public override void OnSelfEnterRoom()
        {
        }

        public override void OnSelfLeaveRoom(bool Kicked)
        {
        }

        public override void OnUserEnterRoom(RoomUser User)
        {         
        }


        public override void OnUserLeaveRoom(GameClient Client)
        {
        }

        public override void OnUserSay(RoomUser User, string Message)
        {
            if (User == null || User.GetClient() == null || User.GetClient().GetHabbo() == null)
                return;

            if (Gamemap.TileDistance(GetRoomUser().X, GetRoomUser().Y, User.X, User.Y) > 8)
                return;

            long nowTime = CloudServer.CurrentTimeMillis();
            long timeBetween = nowTime - User.GetClient().GetHabbo()._lastTimeUsedHelpCommand;
            if (timeBetween < 60000 && Message.Length == 5)
            {
                User.GetClient().SendMessage(RoomNotificationComposer.SendBubble("abuse", "Espera al menos 1 minuto para volver a usar el sistema de revisión de rares.", ""));
                return;
            }

            User.GetClient().GetHabbo()._lastTimeUsedHelpCommand = nowTime;

            string Rare = Message.Split(' ')[2];
            string Username = Message.Split(' ')[4];

            GameClient Target = CloudServer.GetGame().GetClientManager().GetClientByUsername(Message.Split(' ')[4]);
            if (Target == null)
            {
                GetRoomUser().Chat("Oops, no se ha encontrado a esta persona, asegúrate de escribir bien su nombre.", false, 34);
                return;
            }

            int itemstotal = 0;
            using (IQueryAdapter query = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                query.SetQuery("SELECT COUNT(*) FROM items i LEFT JOIN furniture f ON(i.base_item = f.id) WHERE f.public_name = @itemsito AND i.user_id = @id AND f.is_rare = '1'");
                query.AddParameter("id", Target.GetHabbo().Id);
                query.AddParameter("itemsito", Message.Split(' ')[2]);
                itemstotal = query.getInteger();
            }

            if (itemstotal == 0)
            {
                GetRoomUser().Chat("<font color=\"#DF3A01\"><b>" + Username + "</b> no tiene ningún " + Rare + ", por lo que no puede apostar ningun@.</font>", false, 33);
                return;
            }

            GetRoomUser().Chat("<font color=\"#DF3A01\"><b>" + Username + "</b> tiene un total de <b>" + itemstotal + "</b> " + Rare + "s.</font>", false, 33);        
        }

        public override void OnUserShout(RoomUser User, string Message)
        {
        }

        public override void OnTimerTick()
        {
        }
    }
}