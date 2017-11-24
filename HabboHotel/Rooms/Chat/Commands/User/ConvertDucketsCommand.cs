using System;
using System.Data;
using Cloud.HabboHotel.Items;
using Cloud.Communication.Packets.Outgoing.Inventory.Purse;
using Cloud.Database.Interfaces;

namespace Cloud.HabboHotel.Rooms.Chat.Commands.User
{
    class ConvertDucketsCommand : IChatCommand
    {
        public string PermissionRequired => "command_convert_credits";
        public string Parameters => "";
        public string Description => "Converter suas moedas por duckets.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            int TotalDuckets = 0;

            try
            {
                DataTable Table = null;
                using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("SELECT `id` FROM `items` WHERE `user_id` = '" + Session.GetHabbo().Id + "' AND (`room_id`=  '0' OR `room_id` = '')");
                    Table = dbClient.getTable();
                }

                if (Table == null)
                {
                    Session.SendWhisper("¡Você não tem nenhuma moeda em seu inventário!");
                    return;
                }

                foreach (DataRow Row in Table.Rows)
                {
                    Item Item = Session.GetHabbo().GetInventoryComponent().GetItem(Convert.ToInt32(Row[0]));
                    if (Item == null)
                        continue;

                    if (!Item.GetBaseItem().ItemName.StartsWith("DU_") && !Item.GetBaseItem().ItemName.StartsWith("DUC_"))
                        continue;

                    if (Item.RoomId > 0)
                        continue;

                    string[] Split = Item.GetBaseItem().ItemName.Split('_');
                    int Value = int.Parse(Split[1]);

                    using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.runFastQuery("DELETE FROM `items` WHERE `id` = '" + Item.Id + "' LIMIT 1");
                    }

                    Session.GetHabbo().GetInventoryComponent().RemoveItem(Item.Id);

                    TotalDuckets += Value;

                    if (Value > 0)
                    {
                        Session.GetHabbo().Duckets += Value;
                        Session.SendMessage(new ActivityPointsComposer(Session.GetHabbo().Duckets, Session.GetHabbo().Diamonds, Session.GetHabbo().GOTWPoints));
                    }
                }

                if (TotalDuckets > 0)
                    Session.SendWhisper("¡Você resgatou corretamente " + TotalDuckets + " duckets do seu inventario!");
                else
                    Session.SendWhisper("¡Ocorreu algum Erro!");
            }
            catch
            {
                Session.SendNotification("¡Sinto muito, Ocorreu algum erro!");
            }
        }
    }
}
