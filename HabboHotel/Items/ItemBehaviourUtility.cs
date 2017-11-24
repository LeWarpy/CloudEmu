using System;
using System.Linq;
using System.Collections.Generic;
using Cloud.Core;
using Cloud.HabboHotel.Groups;
using Cloud.HabboHotel.Cache.Type;
using Cloud.HabboHotel.Items.Data.Toner;
using Cloud.Communication.Packets.Outgoing;

namespace Cloud.HabboHotel.Items
{
    static class ItemBehaviourUtility
    {
        public static void GenerateExtradata(Item Item, ServerPacket Message)
        {
            switch (Item.GetBaseItem().InteractionType)
            {
                default:
                    Message.WriteInteger(1);
                    Message.WriteInteger(0);
                    Message.WriteString(Item.GetBaseItem().InteractionType != InteractionType.FOOTBALL_GATE ? Item.ExtraData : string.Empty);
                    break;
                case InteractionType.wired_score_board:
                    string username;
                    string name = Item.GetBaseItem().ItemName;
                    string type = name.Split('*')[1];

                    if (type != null)
                    {
                        Dictionary<int, KeyValuePair<int, string>> ScoreBordata = new Dictionary<int, KeyValuePair<int, string>>();
                        Message.WriteInteger(0);
                        Message.WriteInteger(6);
                        Message.WriteString("1");
                        Message.WriteInteger(1);


                        if (Item.GetRoom() != null)
                        {
                            switch (type)
                            {
                                case "2":
                                    Message.WriteInteger(1);
                                    Message.WriteInteger(Item.GetRoom().WiredScoreBordDay.Count);
                                    ScoreBordata = Item.GetRoom().WiredScoreBordDay;
                                    break;

                                case "3":
                                    Message.WriteInteger(2);
                                    Message.WriteInteger(Item.GetRoom().WiredScoreBordWeek.Count);
                                    ScoreBordata = Item.GetRoom().WiredScoreBordWeek;
                                    break;


                                case "4":
                                    Message.WriteInteger(3);
                                    Message.WriteInteger(Item.GetRoom().WiredScoreBordMonth.Count);
                                    ScoreBordata = Item.GetRoom().WiredScoreBordMonth;
                                    break;

                                default:
                                    Message.WriteInteger(1);
                                    Message.WriteInteger(0);
                                    ScoreBordata = null;
                                    break;
                            }
                        }
                        else
                        {
                            Message.WriteInteger(1);
                            Message.WriteInteger(1);
                            Message.WriteInteger(0);
                            Message.WriteInteger(1);
                            Message.WriteString("Dit Scorebord werkt nog niet :(");
                        }

                        if (ScoreBordata.Count != 0)
                        {
                            foreach (KeyValuePair<int, string> value in (
                                from i in ScoreBordata
                                orderby i.Value.Key descending
                                select i).ToDictionary<KeyValuePair<int, KeyValuePair<int, string>>, int, KeyValuePair<int, string>>((KeyValuePair<int, KeyValuePair<int, string>> i) => i.Key, (KeyValuePair<int, KeyValuePair<int, string>> i) => i.Value).Values)
                            {
                                username = value.Value;
                                Message.WriteInteger(value.Key);
                                Message.WriteInteger(1);
                                Message.WriteString((string.IsNullOrEmpty(username) ? string.Empty : username));
                            }
                        }
                    }
                    break;              
                case InteractionType.MUSIC_DISC:
                        int issx = 0;
                        int.TryParse(Item.ExtraData, out issx);
                        Message.WriteInteger(issx);
                        Message.WriteInteger(0);
                        Message.WriteString(Item.ExtraData);
                        break;

                case InteractionType.GNOME_BOX:
                    Message.WriteInteger(0);
                    Message.WriteInteger(0);
                    Message.WriteString("");
                    break;

                case InteractionType.PET_BREEDING_BOX:
                case InteractionType.PURCHASABLE_CLOTHING:
                    Message.WriteInteger(0);
                    Message.WriteInteger(0);
                    Message.WriteString("0");
                    break;

                case InteractionType.STACKTOOL:
                    Message.WriteInteger(0);
                    Message.WriteInteger(0);
                    Message.WriteString("");
                    break;

                case InteractionType.WALLPAPER:
                    Message.WriteInteger(2);
                    Message.WriteInteger(0);
                    Message.WriteString(Item.ExtraData);

                    break;
                case InteractionType.FLOOR:
                    Message.WriteInteger(3);
                    Message.WriteInteger(0);
                    Message.WriteString(Item.ExtraData);
                    break;

                case InteractionType.LANDSCAPE:
                    Message.WriteInteger(4);
                    Message.WriteInteger(0);
                    Message.WriteString(Item.ExtraData);
                    break;

                case InteractionType.GUILD_ITEM:
                case InteractionType.GUILD_GATE:
                case InteractionType.GUILD_FORUM:
                    Group Group = null;
                    if (!CloudServer.GetGame().GetGroupManager().TryGetGroup(Item.GroupId, out Group))
                    {
                        Message.WriteInteger(1);
                        Message.WriteInteger(0);
                        Message.WriteString(Item.ExtraData);
                    }
                    else
                    {
                        Message.WriteInteger(0);
                        Message.WriteInteger(2);
                        Message.WriteInteger(5);
                        Message.WriteString(Item.ExtraData);
                        Message.WriteString(Group.Id.ToString());
                        Message.WriteString(Group.Badge);
                        Message.WriteString(CloudServer.GetGame().GetGroupManager().GetColourCode(Group.Colour1, true));
                        Message.WriteString(CloudServer.GetGame().GetGroupManager().GetColourCode(Group.Colour2, false));
                    }
                    break;

                case InteractionType.BACKGROUND:
                    Message.WriteInteger(0);
                    Message.WriteInteger(1);
                    if (!String.IsNullOrEmpty(Item.ExtraData))
                    {
                        Message.WriteInteger(Item.ExtraData.Split(Convert.ToChar(9)).Length / 2);

                        for (int i = 0; i <= Item.ExtraData.Split(Convert.ToChar(9)).Length - 1; i++)
                        {
                            Message.WriteString(Item.ExtraData.Split(Convert.ToChar(9))[i]);
                        }
                    }
                    else
                    {
                        Message.WriteInteger(0);
                    }
                    break;

                case InteractionType.GIFT:
                    {
                        string[] ExtraData = Item.ExtraData.Split(Convert.ToChar(5));
                        if (ExtraData.Length != 7)
                        {
                            Message.WriteInteger(0);
                            Message.WriteInteger(0);
                            Message.WriteString(Item.ExtraData);
                        }
                        else
                        {
                            int Style = int.Parse(ExtraData[6]) * 1000 + int.Parse(ExtraData[6]);

                            UserCache Purchaser = CloudServer.GetGame().GetCacheManager().GenerateUser(Convert.ToInt32(ExtraData[2]));
                            if (Purchaser == null)
                            {
                                Message.WriteInteger(0);
                                Message.WriteInteger(0);
                                Message.WriteString(Item.ExtraData);
                            }
                            else
                            {
                                Message.WriteInteger(Style);
                                Message.WriteInteger(1);
                                Message.WriteInteger(6);
                                Message.WriteString("EXTRA_PARAM");
                                Message.WriteString("");
                                Message.WriteString("MESSAGE");
                                Message.WriteString(ExtraData[1]);
                                Message.WriteString("PURCHASER_NAME");
                                Message.WriteString(Purchaser.Username);
                                Message.WriteString("PURCHASER_FIGURE");
                                Message.WriteString(Purchaser.Look);
                                Message.WriteString("PRODUCT_CODE");
                                Message.WriteString("A1 KUMIANKKA");
                                Message.WriteString("state");
                                Message.WriteString(Item.MagicRemove == true ? "1" : "0");
                            }
                        }
                    }
                    break;

                case InteractionType.MANNEQUIN:
                    Message.WriteInteger(0);
                    Message.WriteInteger(1);
                    Message.WriteInteger(3);
                    if (Item.ExtraData.Contains(Convert.ToChar(5).ToString()))
                    {
                        string[] Stuff = Item.ExtraData.Split(Convert.ToChar(5));
                        Message.WriteString("GENDER");
                        Message.WriteString(Stuff[0]);
                        Message.WriteString("FIGURE");
                        Message.WriteString(Stuff[1]);
                        Message.WriteString("OUTFIT_NAME");
                        Message.WriteString(Stuff[2]);
                    }
                    else
                    {
                        Message.WriteString("GENDER");
                        Message.WriteString("");
                        Message.WriteString("FIGURE");
                        Message.WriteString("");
                        Message.WriteString("OUTFIT_NAME");
                        Message.WriteString("");
                    }
                    break;

                case InteractionType.TONER:
                    if (Item.RoomId != 0)
                    {
                        if (Item.GetRoom().TonerData == null)
                            Item.GetRoom().TonerData = new TonerData(Item.Id);

                        Message.WriteInteger(0);
                        Message.WriteInteger(5);
                        Message.WriteInteger(4);
                        Message.WriteInteger(Item.GetRoom().TonerData.Enabled);
                        Message.WriteInteger(Item.GetRoom().TonerData.Hue);
                        Message.WriteInteger(Item.GetRoom().TonerData.Saturation);
                        Message.WriteInteger(Item.GetRoom().TonerData.Lightness);
                    }
                    else
                    {
                        Message.WriteInteger(0);
                        Message.WriteInteger(0);
                        Message.WriteString(string.Empty);
                    }
                    break;

                case InteractionType.BADGE_DISPLAY:
                    Message.WriteInteger(0);
                    Message.WriteInteger(2);
                    Message.WriteInteger(4);

                    string[] BadgeData = Item.ExtraData.Split(Convert.ToChar(9));
                    if (Item.ExtraData.Contains(Convert.ToChar(9).ToString()))
                    {
                        Message.WriteString("0");//No idea
                        Message.WriteString(BadgeData[0]);//Badge name
                        Message.WriteString(BadgeData[1]);//Owner
                        Message.WriteString(BadgeData[2]);//Date
                    }
                    else
                    {
                        Message.WriteString("0");//No idea
                        Message.WriteString("DEV");//Badge name
                        Message.WriteString("Sledmore");//Owner
                        Message.WriteString("13-13-1337");//Date
                    }
                    break;

                case InteractionType.TELEVISION:
                    Message.WriteInteger(0);
                    Message.WriteInteger(1);
                    Message.WriteInteger(1);
                    Message.WriteString("THUMBNAIL_URL");
                    Message.WriteString(ExtraSettings.YOUTUBE_THUMBNAIL_SUBURL + "=" + CloudServer.GetGame().GetTelevisionManager().TelevisionList.OrderBy(x => Guid.NewGuid()).FirstOrDefault().YouTubeId);
                    break;

                case InteractionType.TRAX:
                    break;
                case InteractionType.LOVELOCK:
                    if (Item.ExtraData.Contains(Convert.ToChar(5).ToString()))
                    {
                        var EData = Item.ExtraData.Split((char)5);
                        int I = 0;
                        Message.WriteInteger(0);
                        Message.WriteInteger(2);
                        Message.WriteInteger(EData.Length);
                        while (I < EData.Length)
                        {
                            Message.WriteString(EData[I]);
                            I++;
                        }
                    }
                    else
                    {
                        Message.WriteInteger(0);
                        Message.WriteInteger(0);
                        Message.WriteString("0");
                    }
                    break;

                case InteractionType.MONSTERPLANT_SEED:
                    Message.WriteInteger(0);
                    Message.WriteInteger(1);
                    Message.WriteInteger(1);

                    Message.WriteString("rarity");
                    Message.WriteString("1");//Leve should be dynamic.
                    break;
            }
        }

        public static void GenerateWallExtradata(Item Item, ServerPacket Message)
        {
            switch (Item.GetBaseItem().InteractionType)
            {
                default:
                    Message.WriteString(Item.ExtraData);
                    break;               

                case InteractionType.POSTIT:
                    Message.WriteString(Item.ExtraData.Split(' ')[0]);
                    break;
            }
        }
    }
}