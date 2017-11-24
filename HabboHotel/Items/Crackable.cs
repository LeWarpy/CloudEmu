using Cloud.Communication.Packets.Outgoing.Inventory.Purse;
using Cloud.Database.Interfaces;
using Cloud.HabboHotel.Rooms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Cloud.Communication.Packets.Outgoing.Rooms.Notifications;
using System.Threading.Tasks;
using Cloud.Core;

namespace Cloud.HabboHotel.Items
{
    internal class CrackableItem
    {
        internal UInt32 ItemId;
        internal List<CrackableRewards> Rewards;

        internal CrackableItem(DataRow dRow)
        {
            ItemId = Convert.ToUInt32(dRow["item_baseid"]);
            var rewardsString = (string)dRow["rewards"];

            Rewards = new List<CrackableRewards>();
            foreach (var reward in rewardsString.Split(';'))
            {
                var rewardType = reward.Split(',')[0];
                var rewardItem = reward.Split(',')[1];
                var rewardLevel = uint.Parse(reward.Split(',')[2]);
                Rewards.Add(new CrackableRewards(ItemId, rewardType, rewardItem, rewardLevel));
            }
        }
    }

    internal class CrackableRewards
    {
        internal UInt32 CrackableId, CrackableLevel;
        internal String CrackableRewardType, CrackableReward;

        internal CrackableRewards(uint crackableId, string crackableRewardType, string crackableReward, uint crackableLevel)
        {
            CrackableId = crackableId;
            CrackableRewardType = crackableRewardType;
            CrackableReward = crackableReward;
            CrackableLevel = crackableLevel;
        }
    }

    internal class CrackableManager
    {
        internal Dictionary<Int32, CrackableItem> Crackable;

        internal void Initialize(IQueryAdapter dbClient)
        {
            Crackable = new Dictionary<Int32, CrackableItem>();
            dbClient.SetQuery("SELECT * FROM catalog_crackable_rewards");
            var table = dbClient.getTable();
            foreach (DataRow dRow in table.Rows)
            {
                if (Crackable.ContainsKey(Convert.ToInt32(dRow["item_baseid"]))) continue;
                Crackable.Add(Convert.ToInt32(dRow["item_baseid"]), new CrackableItem(dRow));
            }
        }


        private List<CrackableRewards> GetRewardsByLevel(int itemId, int level)
        {
            var rewards = new List<CrackableRewards>();
            foreach (var reward in Crackable[itemId].Rewards.Where(furni => furni.CrackableLevel == level)) rewards.Add(reward);
            return rewards;
        }

        internal void ReceiveCrackableReward(RoomUser user, Room room, Item item)
        {

            if (room == null || item == null) return;
            if (item.GetBaseItem().InteractionType != InteractionType.PINATA && item.GetBaseItem().InteractionType != InteractionType.PINATATRIGGERED && item.GetBaseItem().InteractionType != InteractionType.MAGICEGG && item.GetBaseItem().InteractionType != InteractionType.MAGICCHEST) return;
            if (!Crackable.ContainsKey(item.GetBaseItem().Id)) return;
            CrackableItem crackable;
            Crackable.TryGetValue(item.GetBaseItem().Id, out crackable);
            if (crackable == null) return;
            int x = item.GetX, y = item.GetY;
            room.GetRoomItemHandler().RemoveFurniture(user.GetClient(), item.Id);
            var level = 0;
            var rand = new Random().Next(0, 100);
            if (rand >= 95) level = 5;                   // 005% de probabilidad de que salga nivel 5
            else if (rand >= 85 && rand < 95) level = 4; // 010% de probabilidad de que salga nivel 4
            else if (rand >= 65 && rand < 85) level = 3; // 020% de probabilidad de que salga nivel 3
            else if (rand >= 35 && rand < 65) level = 2; // 030% de probabilidad de que salga nivel 2
            else level = 1;                              // 035% de probabilidad de que salga nivel 1
                                                         // 100%

            var possibleRewards = GetRewardsByLevel((int)crackable.ItemId, level);
            var reward = possibleRewards[new Random().Next(0, (possibleRewards.Count - 1))];

            Task.Run(() =>
            {
                using (var dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
                {

                    #region REWARD TYPES
                    switch (reward.CrackableRewardType)
                    {
                        #region NORMAL ITEMS REWARD
                        case "item":
                            goto ItemType;
                        #endregion

                        #region CREDITS REWARD
                        case "credits":
                        case "coins":
                        case "creditos":
                            {
                                user.GetClient().GetHabbo().Credits += int.Parse(reward.CrackableReward);
                                user.GetClient().SendMessage(new CreditBalanceComposer(user.GetClient().GetHabbo().Credits));
                                user.GetClient().SendMessage(RoomNotificationComposer.SendBubble("cred", "Acabas de ganar " + int.Parse(reward.CrackableReward) + " créditos en la piñata.", ""));
                                user.GetClient().SendMessage(RoomNotificationComposer.SendBubble("award", "Acabas de sacar un " + rand + " en los dados. ¡Enhorabuena!", ""));
                                room.GetRoomItemHandler().RemoveFurniture(user.GetClient(), item.Id);
                                dbClient.runFastQuery("DELETE FROM items WHERE id = " + item.Id);
                                return;
                            }
                        #endregion

                        #region DUCKETS REWARD
                        case "duckets":
                            {
                                user.GetClient().GetHabbo().Duckets += int.Parse(reward.CrackableReward);
                                user.GetClient().SendMessage(new HabboActivityPointNotificationComposer(user.GetClient().GetHabbo().Duckets, user.GetClient().GetHabbo().Duckets));
                                user.GetClient().SendMessage(RoomNotificationComposer.SendBubble("duckets", "Acabas de ganar " + int.Parse(reward.CrackableReward) + " duckets en la piñata.", ""));
                                user.GetClient().SendMessage(RoomNotificationComposer.SendBubble("award", "Acabas de sacar un " + rand + " en los dados. ¡Enhorabuena!", ""));
                                room.GetRoomItemHandler().RemoveFurniture(user.GetClient(), item.Id);
                                dbClient.runFastQuery("DELETE FROM items WHERE id = " + item.Id);
                                return;
                            }
                        #endregion

                        #region DIAMONDS REWARD
                        case "diamonds":
                        case "diamantes":
                            {
                                user.GetClient().GetHabbo().Diamonds += int.Parse(reward.CrackableReward);
                                user.GetClient().SendMessage(new HabboActivityPointNotificationComposer(user.GetClient().GetHabbo().Diamonds, 0, 5));
                                user.GetClient().SendMessage(RoomNotificationComposer.SendBubble("diamonds", "Acabas de ganar " + int.Parse(reward.CrackableReward) + " diamantes en la piñata.", ""));
                                user.GetClient().SendMessage(RoomNotificationComposer.SendBubble("award", "Acabas de sacar un " + rand + " en los dados. ¡Enhorabuena!", ""));
                                room.GetRoomItemHandler().RemoveFurniture(user.GetClient(), item.Id);
                                dbClient.runFastQuery("DELETE FROM items WHERE id = " + item.Id);
                                return;
                            }
                        #endregion

                        #region HONOR REWARD
                        case "honors":
                        case "fame":
                        case "famepoints":
                        case "estrellas":
                        case "ptosfame":
                            {
                                user.GetClient().GetHabbo().GOTWPoints += int.Parse(reward.CrackableReward);
                                user.GetClient().SendMessage(new HabboActivityPointNotificationComposer(user.GetClient().GetHabbo().GOTWPoints, 0, 103));
                                user.GetClient().SendMessage(RoomNotificationComposer.SendBubble("honor", "Acabas de ganar " + int.Parse(reward.CrackableReward) + " "+ExtraSettings.PTOS_COINS+" en la piñata.", ""));
                                user.GetClient().SendMessage(RoomNotificationComposer.SendBubble("award", "Acabas de sacar un " + rand + " en los dados. ¡Enhorabuena!", ""));
                                room.GetRoomItemHandler().RemoveFurniture(user.GetClient(), item.Id);
                                dbClient.runFastQuery("DELETE FROM items WHERE id = " + item.Id);
                                return;
                            }
                        #endregion

                        #region BADGE REWARD
                        case "badge":
                            {
                                if (user.GetClient().GetHabbo().GetBadgeComponent().HasBadge(reward.CrackableReward)) return;
                                user.GetClient().SendMessage(RoomNotificationComposer.SendBubble("award", "Acabas de ganar la placa: " + int.Parse(reward.CrackableReward) + ".", ""));
                                user.GetClient().SendMessage(RoomNotificationComposer.SendBubble("award", "Acabas de sacar un " + rand + " en los dados. ¡Enhorabuena!", ""));
                                user.GetClient().GetHabbo().GetBadgeComponent().GiveBadge(reward.CrackableReward, true, user.GetClient());
                                room.GetRoomItemHandler().RemoveFurniture(user.GetClient(), item.Id);
                                dbClient.runFastQuery("DELETE FROM items WHERE id = " + item.Id);
                                return;
                            }
                            #endregion
                    }
                    #endregion

                    ItemType:
                    /*user.GetClient().SendMessage(new OpenGiftComposer(item.Data, item.ExtraData, item, true)); // custom tocan2
                    item.MagicRemove = true; // custom tocan2
                    room.SendMessage(new ObjectUpdateComposer(item, Convert.ToInt32(user.GetClient().GetHabbo().Id))); //custom tocan2*/
                    room.GetRoomItemHandler().RemoveFurniture(user.GetClient(), item.Id);
                    dbClient.runFastQuery("UPDATE items SET base_item = " + int.Parse(reward.CrackableReward) + ", extra_data = '' WHERE id = " + item.Id);
                    item.BaseItem = int.Parse(reward.CrackableReward);
                    item.ResetBaseItem();
                    item.ExtraData = string.Empty;
                    if (!room.GetRoomItemHandler().SetFloorItem(user.GetClient(), item, item.GetX, item.GetY, item.Rotation, true, false, true))
                    {
                        dbClient.runFastQuery("UPDATE items SET room_id = 0 WHERE id = " + item.Id);
                        user.GetClient().GetHabbo().GetInventoryComponent().UpdateItems(true);
                    }
                }

                user.GetClient().SendMessage(RoomNotificationComposer.SendBubble("award", "Acabas de sacar un " + rand + " en los dados. ¡Enhorabuena!", ""));
            });
        }

    }
}