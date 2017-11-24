﻿using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Cloud.HabboHotel.Rooms;
using Cloud.HabboHotel.Users;
using Cloud.HabboHotel.Groups;
using System.Collections.ObjectModel;
using Cloud.HabboHotel.Rooms.AI;

namespace Cloud.Communication.Packets.Outgoing.Rooms.Engine
{
    class UsersComposer : ServerPacket
    {
        public UsersComposer(ICollection<RoomUser> Users)
            : base(ServerPacketHeader.UsersMessageComposer)
        {
            base.WriteInteger(Users.Count);
            foreach (RoomUser User in Users.ToList())
            {
                WriteUser(User);
            }
        }

        public UsersComposer(RoomUser User)
            : base(ServerPacketHeader.UsersMessageComposer)
        {
            base.WriteInteger(1);//1 avatar
            WriteUser(User);
        }

        private void WriteUser(RoomUser User)
        {
            if (!User.IsPet && !User.IsBot)
            {
                Habbo Habbo = User.GetClient().GetHabbo();

                Group Group = null;
                if (Habbo != null)
                {
                    if (Habbo.GetStats() != null)
                    {
                        if (Habbo.GetStats().FavouriteGroupId > 0)
                        {
                            if (!CloudServer.GetGame().GetGroupManager().TryGetGroup(Habbo.GetStats().FavouriteGroupId, out Group))
                                Group = null;
                        }
                    }
                }

                if (Habbo.PetId == 0)
                {
                    base.WriteInteger(Habbo.Id);
                    base.WriteString(Habbo.Username);
                    base.WriteString(Habbo.Motto);
                    base.WriteString(Habbo.Look);
                    base.WriteInteger(User.VirtualId);
                    base.WriteInteger(User.X);
                    base.WriteInteger(User.Y);
                    base.WriteDouble(User.Z);

                    base.WriteInteger(0);//2 for user, 4 for bot.
                    base.WriteInteger(1);//1 for user, 2 for pet, 3 for bot.
                    base.WriteString(Habbo.Gender.ToLower());

                    if (Group != null)
                    {
                        base.WriteInteger(Group.Id);
                        base.WriteInteger(0);
                        base.WriteString(Group.Name);
                    }
                    else
                    {
                        base.WriteInteger(0);
                        base.WriteInteger(0);
                        base.WriteString("");
                    }

                    base.WriteString("");//Whats this?
                    base.WriteInteger(Habbo.GetStats().AchievementPoints);//Achievement score
                    base.WriteBoolean(false);//Builders club?
                }
                else if (Habbo.PetId > 0 && Habbo.PetId != 100)
                {
                    base.WriteInteger(Habbo.Id);
                    base.WriteString(Habbo.Username);
                    base.WriteString(Habbo.Motto);
                    base.WriteString(PetFigureForType(Habbo.PetId));

                    base.WriteInteger(User.VirtualId);
                    base.WriteInteger(User.X);
                    base.WriteInteger(User.Y);
                    base.WriteDouble(User.Z);
                    base.WriteInteger(0);
                    base.WriteInteger(2);//Pet.

                    base.WriteInteger(Habbo.PetId);//pet type.
                    base.WriteInteger(Habbo.Id);//UserId of the owner.
                    base.WriteString(Habbo.Username);//Username of the owner.
                    base.WriteInteger(1);
                    base.WriteBoolean(false);//Has saddle.
                    base.WriteBoolean(false);//Is someone riding this horse?
                    base.WriteInteger(0);
                    base.WriteInteger(0);
                    base.WriteString("");
                }
                else if (Habbo.PetId > 0 && Habbo.PetId == 100)
                {
                    base.WriteInteger(Habbo.Id);
                    base.WriteString(Habbo.Username);
                    base.WriteString(Habbo.Motto);
                    base.WriteString(Habbo.Look.ToLower());
                    base.WriteInteger(User.VirtualId);
                    base.WriteInteger(User.X);
                    base.WriteInteger(User.Y);
                    base.WriteDouble(User.Z);
                    base.WriteInteger(0);
                    base.WriteInteger(4);

                    base.WriteString(Habbo.Gender.ToLower()); // ?
                    base.WriteInteger(Habbo.Id); //Owner Id
                    base.WriteString(Habbo.Username); // Owner name
                    base.WriteInteger(0);//Action Count
                }
            }
            else if (User.IsPet)
            {
                base.WriteInteger(User.BotAI.BaseId);
                base.WriteString(User.BotData.Name);
                base.WriteString(User.BotData.Motto);

                //base.WriteString("26 30 ffffff 5 3 302 4 2 201 11 1 102 12 0 -1 28 4 401 24");
                base.WriteString(User.BotData.Look.ToLower() + ((User.PetData.Saddle > 0) ? " 3 2 " + User.PetData.PetHair + " " + User.PetData.HairDye + " 3 " + User.PetData.PetHair + " " + User.PetData.HairDye + " 4 " + User.PetData.Saddle + " 0" : " 2 2 " + User.PetData.PetHair + " " + User.PetData.HairDye + " 3 " + User.PetData.PetHair + " " + User.PetData.HairDye + ""));

                base.WriteInteger(User.VirtualId);
                base.WriteInteger(User.X);
                base.WriteInteger(User.Y);
                base.WriteDouble(User.Z);
                base.WriteInteger(0);
                base.WriteInteger((User.BotData.AiType == BotAIType.PET) ? 2 : 4);
                base.WriteInteger(User.PetData.Type);
                base.WriteInteger(User.PetData.OwnerId); // userid
                base.WriteString(User.PetData.OwnerName); // username
                base.WriteInteger(1);
                base.WriteBoolean(User.PetData.Saddle > 0);
                base.WriteBoolean(User.RidingHorse);
                base.WriteInteger(0);
                base.WriteInteger(0);
                base.WriteString("");
            }
            else if (User.IsBot)
            {
                base.WriteInteger(User.BotAI.BaseId);
                base.WriteString(User.BotData.Name);
                base.WriteString(User.BotData.Motto);
                base.WriteString(User.BotData.Look.ToLower());
                base.WriteInteger(User.VirtualId);
                base.WriteInteger(User.X);
                base.WriteInteger(User.Y);
                base.WriteDouble(User.Z);
                base.WriteInteger(0);
                base.WriteInteger((User.BotData.AiType == BotAIType.PET) ? 2 : 4);

                base.WriteString(User.BotData.Gender.ToLower()); // ?
                base.WriteInteger(User.BotData.ownerID); //Owner Id
                base.WriteString(CloudServer.GetUsernameById(User.BotData.ownerID)); // Owner name
                base.WriteInteger(5);//Action Count
                base.WriteShort(1);//Copy looks
                base.WriteShort(2);//Setup speech
                base.WriteShort(3);//Relax
                base.WriteShort(4);//Dance
                base.WriteShort(5);//Change name
            }
        }

        public string PetFigureForType(int Type)
        {
            Random _random = new Random();

            switch (Type)
            {
                #region Dog Figures
                default:
                case 60:
                    {
                        int RandomNumber = _random.Next(1, 4);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "0 0 f08b90 2 2 -1 1 3 -1 1";
                            case 2:
                                return "0 15 ffffff 2 2 -1 0 3 -1 0";
                            case 3:
                                return "0 20 d98961 2 2 -1 0 3 -1 0";
                            case 4:
                                return "0 21 da9dbd 2 2 -1 0 3 -1 0";
                        }
                    }
                #endregion

                #region Cat Figures.
                case 1:
                    {
                        int RandomNumber = _random.Next(1, 5);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "1 18 d5b35f 2 2 -1 0 3 -1 0";
                            case 2:
                                return "1 0 ff7b3a 2 2 -1 0 3 -1 0";
                            case 3:
                                return "1 18 d98961 2 2 -1 0 3 -1 0";
                            case 4:
                                return "1 0 ff7b3a 2 2 -1 0 3 -1 1";
                            case 5:
                                return "1 24 d5b35f 2 2 -1 0 3 -1 0";
                        }
                    }
                #endregion

                #region Terrier Figures
                case 2:
                    {
                        int RandomNumber = _random.Next(1, 6);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "3 3 eeeeee 2 2 -1 0 3 -1 0";
                            case 2:
                                return "3 0 ffffff 2 2 -1 0 3 -1 0";
                            case 3:
                                return "3 5 eeeeee 2 2 -1 0 3 -1 0";
                            case 4:
                                return "3 6 eeeeee 2 2 -1 0 3 -1 0";
                            case 5:
                                return "3 4 dddddd 2 2 -1 0 3 -1 0";
                            case 6:
                                return "3 5 dddddd 2 2 -1 0 3 -1 0";
                        }
                    }
                #endregion

                #region Croco Figures
                case 3:
                    {
                        int RandomNumber = _random.Next(1, 5);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "2 10 84ce84 2 2 -1 0 3 -1 0";
                            case 2:
                                return "2 8 838851 2 2 0 0 3 -1 0";
                            case 3:
                                return "2 11 b99105 2 2 -1 0 3 -1 0";
                            case 4:
                                return "2 3 e8ce25 2 2 -1 0 3 -1 0";
                            case 5:
                                return "2 2 fcfad3 2 2 -1 0 3 -1 0";
                        }
                    }
                #endregion

                #region Bear Figures
                case 4:
                    {
                        int RandomNumber = _random.Next(1, 4);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "4 2 e4feff 2 2 -1 0 3 -1 0";
                            case 2:
                                return "4 3 e4feff 2 2 -1 0 3 -1 0";
                            case 3:
                                return "4 1 eaeddf 2 2 -1 0 3 -1 0";
                            case 4:
                                return "4 0 ffffff 2 2 -1 0 3 -1 0";
                        }
                    }
                #endregion

                #region Pig Figures
                case 5:
                    {
                        int RandomNumber = _random.Next(1, 7);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "5 2 ffffff 2 2 -1 0 3 -1 0";
                            case 2:
                                return "5 0 ffffff 2 2 -1 0 3 -1 0";
                            case 3:
                                return "5 3 ffffff 2 2 -1 0 3 -1 0";
                            case 4:
                                return "5 5 ffffff 2 2 -1 0 3 -1 0";
                            case 5:
                                return "5 7 ffffff 2 2 -1 0 3 -1 0";
                            case 6:
                                return "5 1 ffffff 2 2 -1 0 3 -1 0";
                            case 7:
                                return "5 8 ffffff 2 2 -1 0 3 -1 0";
                        }
                    }
                #endregion

                #region Lion Figures
                case 6:
                    {
                        int RandomNumber = _random.Next(1, 11);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "6 0 ffffff 2 2 -1 0 3 -1 0";
                            case 2:
                                return "6 1 ffffff 2 2 -1 0 3 -1 0";
                            case 3:
                                return "6 2 ffffff 2 2 -1 0 3 -1 0";
                            case 4:
                                return "6 3 ffffff 2 2 -1 0 3 -1 0";
                            case 5:
                                return "6 4 ffffff 2 2 -1 0 3 -1 0";
                            case 6:
                                return "6 0 ffd8c9 2 2 -1 0 3 -1 0";
                            case 7:
                                return "6 5 ffffff 2 2 -1 0 3 -1 0";
                            case 8:
                                return "6 11 ffffff 2 2 -1 0 3 -1 0";
                            case 9:
                                return "6 2 ffe49d 2 2 -1 0 3 -1 0";
                            case 10:
                                return "6 11 ff9ae 2 2 -1 0 3 -1 0";
                            case 11:
                                return "6 2 ff9ae 2 2 -1 0 3 -1 0";
                        }
                    }
                #endregion

                #region Rhino Figures
                case 7:
                    {
                        int RandomNumber = _random.Next(1, 7);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "7 5 aeaeae 2 2 -1 0 3 -1 0";
                            case 2:
                                return "7 7 ffc99a 2 2 -1 0 3 -1 0";
                            case 3:
                                return "7 5 cccccc 2 2 -1 0 3 -1 0";
                            case 4:
                                return "7 5 9adcff 2 2 -1 0 3 -1 0";
                            case 5:
                                return "7 5 ff7d6a 2 2 -1 0 3 -1 0";
                            case 6:
                                return "7 6 cccccc 2 2 -1 0 3 -1 0";
                            case 7:
                                return "7 0 cccccc 2 2 -1 0 3 -1 0";
                        }
                    }
                #endregion

                #region Spider Figures
                case 8:
                    {
                        int RandomNumber = _random.Next(1, 13);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "8 0 ffffff 2 2 -1 0 3 -1 0";
                            case 2:
                                return "8 1 ffffff 2 2 -1 0 3 -1 0";
                            case 3:
                                return "8 2 ffffff 2 2 -1 0 3 -1 0";
                            case 4:
                                return "8 3 ffffff 2 2 -1 0 3 -1 0";
                            case 5:
                                return "8 4 ffffff 2 2 -1 0 3 -1 0";
                            case 6:
                                return "8 14 ffffff 2 2 -1 0 3 -1 0";
                            case 7:
                                return "8 11 ffffff 2 2 -1 0 3 -1 0";
                            case 8:
                                return "8 8 ffffff 2 2 -1 0 3 -1 0";
                            case 9:
                                return "8 6 ffffff 2 2 -1 0 3 -1 0";
                            case 10:
                                return "8 5 ffffff 2 2 -1 0 3 -1 0";
                            case 11:
                                return "8 9 ffffff 2 2 -1 0 3 -1 0";
                            case 12:
                                return "8 10 ffffff 2 2 -1 0 3 -1 0";
                            case 13:
                                return "8 7 ffffff 2 2 -1 0 3 -1 0";
                        }
                    }
                #endregion

                #region Turtle Figures
                case 9:
                    {
                        int RandomNumber = _random.Next(1, 9);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "9 0 ffffff 2 2 -1 0 3 -1 0";
                            case 2:
                                return "9 1 ffffff 2 2 -1 0 3 -1 0";
                            case 3:
                                return "9 2 ffffff 2 2 -1 0 3 -1 0";
                            case 4:
                                return "9 3 ffffff 2 2 -1 0 3 -1 0";
                            case 5:
                                return "9 4 ffffff 2 2 -1 0 3 -1 0";
                            case 6:
                                return "9 5 ffffff 2 2 -1 0 3 -1 0";
                            case 7:
                                return "9 6 ffffff 2 2 -1 0 3 -1 0";
                            case 8:
                                return "9 7 ffffff 2 2 -1 0 3 -1 0";
                            case 9:
                                return "9 8 ffffff 2 2 -1 0 3 -1 0";
                        }
                    }
                #endregion

                #region Chick Figures
                case 10:
                    {
                        int RandomNumber = _random.Next(1, 1);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "10 0 ffffff 2 2 -1 0 3 -1 0";
                        }
                    }
                #endregion

                #region Frog Figures
                case 11:
                    {
                        int RandomNumber = _random.Next(1, 13);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "11 1 ffffff 2 2 -1 0 3 -1 0";
                            case 2:
                                return "11 2 ffffff 2 2 -1 0 3 -1 0";
                            case 3:
                                return "11 3 ffffff 2 2 -1 0 3 -1 0";
                            case 4:
                                return "11 4 ffffff 2 2 -1 0 3 -1 0";
                            case 5:
                                return "11 5 ffffff 2 2 -1 0 3 -1 0";
                            case 6:
                                return "11 9 ffffff 2 2 -1 0 3 -1 0";
                            case 7:
                                return "11 10 ffffff 2 2 -1 0 3 -1 0";
                            case 8:
                                return "11 6 ffffff 2 2 -1 0 3 -1 0";
                            case 9:
                                return "11 12 ffffff 2 2 -1 0 3 -1 0";
                            case 10:
                                return "11 11 ffffff 2 2 -1 0 3 -1 0";
                            case 11:
                                return "11 15 ffffff 2 2 -1 0 3 -1 0";
                            case 12:
                                return "11 13 ffffff 2 2 -1 0 3 -1 0";
                            case 13:
                                return "11 18 ffffff 2 2 -1 0 3 -1 0";
                        }
                    }
                #endregion

                #region Dragon Figures
                case 12:
                    {
                        int RandomNumber = _random.Next(1, 6);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "12 0 ffffff 2 2 -1 0 3 -1 0";
                            case 2:
                                return "12 1 ffffff 2 2 -1 0 3 -1 0";
                            case 3:
                                return "12 2 ffffff 2 2 -1 0 3 -1 0";
                            case 4:
                                return "12 3 ffffff 2 2 -1 0 3 -1 0";
                            case 5:
                                return "12 4 ffffff 2 2 -1 0 3 -1 0";
                            case 6:
                                return "12 5 ffffff 2 2 -1 0 3 -1 0";
                        }
                    }
                #endregion

                #region Monkey Figures
                case 14:
                    {
                        int RandomNumber = _random.Next(1, 14);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "14 0 ffffff 2 2 -1 0 3 -1 0";
                            case 2:
                                return "14 1 ffffff 2 2 -1 0 3 -1 0";
                            case 3:
                                return "14 2 ffffff 2 2 -1 0 3 -1 0";
                            case 4:
                                return "14 3 ffffff 2 2 -1 0 3 -1 0";
                            case 5:
                                return "14 6 ffffff 2 2 -1 0 3 -1 0";
                            case 6:
                                return "14 4 ffffff 2 2 -1 0 3 -1 0";
                            case 7:
                                return "14 5 ffffff 2 2 -1 0 3 -1 0";
                            case 8:
                                return "14 7 ffffff 2 2 -1 0 3 -1 0";
                            case 9:
                                return "14 8 ffffff 2 2 -1 0 3 -1 0";
                            case 10:
                                return "14 9 ffffff 2 2 -1 0 3 -1 0";
                            case 11:
                                return "14 10 ffffff 2 2 -1 0 3 -1 0";
                            case 12:
                                return "14 11 ffffff 2 2 -1 0 3 -1 0";
                            case 13:
                                return "14 12 ffffff 2 2 -1 0 3 -1 0";
                            case 14:
                                return "14 13 ffffff 2 2 -1 0 3 -1 0";
                        }
                    }
                #endregion

                #region Horse Figures
                case 15:
                    {
                        int RandomNumber = _random.Next(1, 20);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "15 2 ffffff 2 2 -1 0 3 -1 0";
                            case 2:
                                return "15 3 ffffff 2 2 -1 0 3 -1 0";
                            case 3:
                                return "15 4 ffffff 2 2 -1 0 3 -1 0";
                            case 4:
                                return "15 5 ffffff 2 2 -1 0 3 -1 0";
                            case 5:
                                return "15 6 ffffff 2 2 -1 0 3 -1 0";
                            case 6:
                                return "15 7 ffffff 2 2 -1 0 3 -1 0";
                            case 7:
                                return "15 8 ffffff 2 2 -1 0 3 -1 0";
                            case 8:
                                return "15 9 ffffff 2 2 -1 0 3 -1 0";
                            case 9:
                                return "15 10 ffffff 2 2 -1 0 3 -1 0";
                            case 10:
                                return "15 11 ffffff 2 2 -1 0 3 -1 0";
                            case 11:
                                return "15 12 ffffff 2 2 -1 0 3 -1 0";
                            case 12:
                                return "15 13 ffffff 2 2 -1 0 3 -1 0";
                            case 13:
                                return "15 14 ffffff 2 2 -1 0 3 -1 0";
                            case 14:
                                return "15 15 ffffff 2 2 -1 0 3 -1 0";
                            case 15:
                                return "15 16 ffffff 2 2 -1 0 3 -1 0";
                            case 16:
                                return "15 17 ffffff 2 2 -1 0 3 -1 0";
                            case 17:
                                return "15 78 ffffff 2 2 -1 0 3 -1 0";
                            case 18:
                                return "15 77 ffffff 2 2 -1 0 3 -1 0";
                            case 19:
                                return "15 79 ffffff 2 2 -1 0 3 -1 0";
                            case 20:
                                return "15 80 ffffff 2 2 -1 0 3 -1 0";
                        }
                    }
                #endregion

                #region Bunny Figures
                case 17:
                    {
                        int RandomNumber = _random.Next(1, 8);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "17 1 ffffff";
                            case 2:
                                return "17 2 ffffff";
                            case 3:
                                return "17 3 ffffff";
                            case 4:
                                return "17 4 ffffff";
                            case 5:
                                return "17 5 ffffff";
                            case 6:
                                return "18 0 ffffff";
                            case 7:
                                return "19 0 ffffff";
                            case 8:
                                return "20 0 ffffff";
                        }
                    }
                #endregion

                #region Pigeon Figures (White & Black)
                case 21:
                    {
                        int RandomNumber = _random.Next(1, 3);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "21 0 ffffff";
                            case 2:
                                return "22 0 ffffff";
                        }
                    }
                #endregion

                #region Demon Monkey Figures
                case 23:
                    {
                        int RandomNumber = _random.Next(1, 3);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "23 0 ffffff";
                            case 2:
                                return "23 1 ffffff";
                            case 3:
                                return "23 3 ffffff";
                        }
                    }
                #endregion

                #region Baby Bear Figures
                case 24:
                    {
                        int RandomNumber = _random.Next(1, 3);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "24 0 ffffff";
                            case 2:
                                return "24 1 ffffff";
                        }
                    }
                #endregion

                #region Baby Terrier Figures
                case 25:
                    {
                        int RandomNumber = _random.Next(1, 3);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "25 0 ffffff";
                            case 2:
                                return "25 1 ffffff";
                        }
                    }
                #endregion

                #region Gnome Figures
                case 26:
                    {
                        int RandomNumber = _random.Next(1, 4);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "26 1 ffffff 5 0 -1 0 4 402 5 3 301 4 1 101 2 2 201 3";
                            case 2:
                                return "26 1 ffffff 5 0 -1 0 1 102 13 3 301 4 4 401 5 2 201 3";
                            case 3:
                                return "26 6 ffffff 5 1 102 8 2 201 16 4 401 9 3 303 4 0 -1 6";
                            case 4:
                                return "26 30 ffffff 5 0 -1 0 3 303 4 4 401 5 1 101 2 2 201 3";
                        }
                    }
                #endregion

                #region Kitten Figures
                case 28:
                    {
                        int RandomNumber = _random.Next(1, 3);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "28 0 ffffff";
                            case 2:
                                return "28 1 ffffff";
                        }
                    }
                #endregion


                #region Puppy Figures
                case 29:
                    {
                        int RandomNumber = _random.Next(1, 11);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "29 0 ffffff";
                            case 2:
                                return "29 1 ffffff";
                            case 3:
                                return "29 2 ffffff";
                            case 4:
                                return "29 3 ffffff";
                            case 5:
                                return "29 4 ffffff";
                            case 6:
                                return "29 5 ffffff";
                            case 7:
                                return "29 6 ffffff";
                            case 8:
                                return "29 7 ffffff";
                            case 9:
                                return "29 8 ffffff";
                            case 10:
                                return "29 9 ffffff";
                        }
                    }
                #endregion

                #region Piglet Figures
                case 30:
                    {
                        int RandomNumber = _random.Next(1, 3);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "30 0 ffffff";
                            case 2:
                                return "30 1 ffffff";
                        }
                    }
                #endregion


                #region Haloompa Figures
                case 31:
                    {
                        int RandomNumber = _random.Next(1, 3);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "31 0 ffffff";
                            case 2:
                                return "31 1 ffffff";
                        }
                    }
                #endregion


                #region Rock/Stone Figures 
                case 32:
                    {
                        int RandomNumber = _random.Next(1, 3);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "32 0 ffffff";
                            case 2:
                                return "32 1 ffffff";
                        }
                    }
                #endregion

                #region Pterosaur Figures 
                case 33:
                    {
                        int RandomNumber = _random.Next(1, 3);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "33 0 ffffff";
                            case 2:
                                return "33 1 ffffff";
                        }
                    }
                #endregion

                #region Velociraptor Figures 
                case 34:
                    {
                        int RandomNumber = _random.Next(1, 3);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "34 0 ffffff";
                            case 2:
                                return "34 1 ffffff";
                        }
                    }
                #endregion
                #region Cow Figures
                case 35:
                    {
                        int RandomNumber = _random.Next(1, 3);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "35 0 ffffff";
                            case 2:
                                return "35 1 ffffff";
                        }
                    }
                #endregion
                #region Penguin Figures
                case 36:
                    {
                        int RandomNumber = _random.Next(1, 3);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "36 0 ffffff";
                            case 2:
                                return "36 1 ffffff";
                        }
                    }
                #endregion
                #region Elephant Figures
                case 37:
                    {
                        int RandomNumber = _random.Next(1, 3);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "37 0 ffffff";
                            case 2:
                                return "37 1 ffffff";
                        }
                    }
                #endregion
                #region Handsome Baby Figures
                case 38:
                    {
                        int RandomNumber = _random.Next(1, 3);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "38 0 ffffff";
                            case 2:
                                return "38 1 ffffff";
                        }
                    }
                #endregion
                #region Ugly Baby Figures
                case 39:
                    {
                        int RandomNumber = _random.Next(1, 3);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "39 0 ffffff";
                            case 2:
                                return "39 1 ffffff";
                        }
                    }
                #endregion
                #region Mario Figures
                case 40:
                    {
                        int RandomNumber = _random.Next(1, 3);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "40 0 ffffff";
                            case 2:
                                return "40 1 ffffff";
                        }
                    }
                #endregion
                #region Pikachu Figures
                case 41:
                    {
                        int RandomNumber = _random.Next(1, 3);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "41 0 ffffff";
                            case 2:
                                return "41 1 ffffff";
                        }
                    }
                #endregion
                #region Wolf Figures
                case 42:
                    {
                        int RandomNumber = _random.Next(1, 3);
                        switch (RandomNumber)
                        {
                            default:
                            case 1:
                                return "42 0 ffffff";
                            case 2:
                                return "42 1 ffffff";
                        }
                    }
                    #endregion

            }
        }
    }
}