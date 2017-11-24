﻿using System.Linq;
using System.Collections.Generic;

namespace Cloud.HabboHotel.Items.Wired
{
    static class WiredBoxTypeUtility
    {
        public static WiredBoxType FromWiredId(int Id)
        {
            switch (Id)
            {
                default:
                    return WiredBoxType.None;
                case 1:
                    return WiredBoxType.TriggerUserSays;
                case 2:
                    return WiredBoxType.TriggerStateChanges;
                case 3:
                    return WiredBoxType.TriggerRepeat;
                case 4:
                    return WiredBoxType.TriggerRoomEnter;
                case 8:
                    return WiredBoxType.TriggerWalkOnFurni;
                case 9:
                    return WiredBoxType.TriggerWalkOffFurni;
                case 5:
                    return WiredBoxType.EffectShowMessage;
                case 6:
                    return WiredBoxType.EffectTeleportToFurni;
                case 7:
                    return WiredBoxType.EffectToggleFurniState;
                case 10:
                    return WiredBoxType.EffectKickUser;
                case 11:
                    return WiredBoxType.ConditionFurniHasUsers;
                case 12:
                    return WiredBoxType.ConditionFurniHasFurni;
                case 13:
                    return WiredBoxType.ConditionTriggererOnFurni;
                case 14:
                    return WiredBoxType.EffectMatchPosition;
                case 21:
                    return WiredBoxType.ConditionIsGroupMember;
                case 22:
                    return WiredBoxType.ConditionIsNotGroupMember;
                case 23:
                    return WiredBoxType.ConditionTriggererNotOnFurni;
                case 24:
                    return WiredBoxType.ConditionFurniHasNoUsers;
                case 25:
                    return WiredBoxType.ConditionIsWearingBadge;
                case 26:
                    return WiredBoxType.ConditionIsWearingFX;
                case 27:
                    return WiredBoxType.ConditionIsNotWearingBadge;
                case 28:
                    return WiredBoxType.ConditionIsNotWearingFX;
                case 29:
                    return WiredBoxType.ConditionMatchStateAndPosition;
                case 30:
                    return WiredBoxType.ConditionUserCountInRoom;
                case 31:
                    return WiredBoxType.ConditionUserCountDoesntInRoom;
                case 32:
                    return WiredBoxType.EffectMoveAndRotate;
                case 33:
                    return WiredBoxType.ConditionDontMatchStateAndPosition;
                case 34:
                    return WiredBoxType.ConditionFurniTypeMatches;
                case 35:
                    return WiredBoxType.ConditionFurniTypeDoesntMatch;
                case 36:
                    return WiredBoxType.ConditionFurniHasNoFurni;
                case 37:
                    return WiredBoxType.EffectMoveFurniToNearestUser;
                case 38:
                    return WiredBoxType.EffectMoveFurniFromNearestUser;
                case 39:
                    return WiredBoxType.EffectMuteTriggerer;
                case 40:
                    return WiredBoxType.EffectGiveReward;
                case 41:
                    return WiredBoxType.AddonRandomEffect;
                case 42:
                    return WiredBoxType.TriggerGameStarts;
                case 43:
                    return WiredBoxType.TriggerGameEnds;
                case 44:
                    return WiredBoxType.TriggerUserFurniCollision;
                case 45:
                    return WiredBoxType.EffectMoveFurniToNearestUser;
                case 46:
                    return WiredBoxType.EffectExecuteWiredStacks;
                case 47:
                    return WiredBoxType.EffectTeleportBotToFurniBox;
                case 48:
                    return WiredBoxType.EffectBotChangesClothesBox;
                case 49:
                    return WiredBoxType.EffectBotMovesToFurniBox;
                case 50:
                    return WiredBoxType.EffectBotCommunicatesToAllBox;
                case 51:
                    return WiredBoxType.EffectBotCommunicatesToUserBox;
                case 52:
                    return WiredBoxType.EffectBotFollowsUserBox;
                case 53:
                    return WiredBoxType.EffectBotGivesHanditemBox;
                case 54:
                    return WiredBoxType.ConditionActorHasHandItemBox;
                case 55:
                    return WiredBoxType.ConditionActorIsInTeamBox;
                case 56:
                    return WiredBoxType.EffectAddActorToTeam;
                case 57:
                    return WiredBoxType.EffectRemoveActorFromTeam;
                case 58:
                    return WiredBoxType.TriggerUserSaysCommand;
                case 59:
                    return WiredBoxType.EffectSetRollerSpeed;
                case 60:
                    return WiredBoxType.EffectRegenerateMaps;
                case 61:
                    return WiredBoxType.EffectGiveUserBadge;
                case 62:
                    return WiredBoxType.EffectAddScore;
                case 63:
                    return WiredBoxType.TriggerLongRepeat;
                case 64:
                    return WiredBoxType.HighscoreClassicAlltime;
                case 65:
                    return WiredBoxType.EffectEnableUserBox;
                case 66:
                    return WiredBoxType.EffectDanceUserBox;
                case 67:
                    return WiredBoxType.EffectGiveUserCreditsBox;
                case 68:
                    return WiredBoxType.EffectGiveUserDucketsBox;
                case 69:
                    return WiredBoxType.EffectHandUserItemBox;
                case 70:
                    return WiredBoxType.EffectFreezeUserBox;
                case 71:
                    return WiredBoxType.EffectFixRoomBox;
                case 72:
                    return WiredBoxType.EffectFastWalkUserBox;
                case 73:
                    return WiredBoxType.EffectGiveUserDiamondsBox;
            }
        }

        public static int GetWiredId(WiredBoxType Type)
        {
            switch (Type)
            {
                case WiredBoxType.TriggerUserSays:
                case WiredBoxType.TriggerUserSaysCommand:
                case WiredBoxType.ConditionMatchStateAndPosition:
                    return 0;
                case WiredBoxType.TriggerWalkOnFurni:
                case WiredBoxType.TriggerWalkOffFurni:
                case WiredBoxType.ConditionFurniHasUsers:
                case WiredBoxType.ConditionFurniHasFurni:
                case WiredBoxType.ConditionTriggererOnFurni:
                    return 1;
                case WiredBoxType.EffectMatchPosition:
                    return 3;
                case WiredBoxType.EffectMoveAndRotate:
                case WiredBoxType.TriggerStateChanges:
                    return 4;
                case WiredBoxType.ConditionUserCountInRoom:
                    return 5;
                case WiredBoxType.ConditionActorIsInTeamBox:
                case WiredBoxType.TriggerRepeat:
                case WiredBoxType.TriggerLongRepeat:
                case WiredBoxType.EffectAddScore:
                    return 6;
                case WiredBoxType.TriggerRoomEnter:
                case WiredBoxType.EffectShowMessage:
                case WiredBoxType.EffectEnableUserBox:
                case WiredBoxType.EffectDanceUserBox:
                case WiredBoxType.EffectGiveUserCreditsBox:
                case WiredBoxType.EffectGiveUserDucketsBox:
                case WiredBoxType.EffectGiveUserDiamondsBox:
                case WiredBoxType.EffectHandUserItemBox:
                    return 7;
                case WiredBoxType.TriggerGameStarts:
                case WiredBoxType.TriggerGameEnds:
                case WiredBoxType.EffectTeleportToFurni:
                case WiredBoxType.EffectToggleFurniState:
                case WiredBoxType.ConditionFurniTypeMatches:
                    return 8;
                case WiredBoxType.EffectGiveUserBadge:
                case WiredBoxType.EffectRegenerateMaps:
                case WiredBoxType.EffectKickUser:
                case WiredBoxType.EffectSetRollerSpeed:
                    return 7;
                case WiredBoxType.EffectAddActorToTeam:
                    return 9;
                case WiredBoxType.EffectRemoveActorFromTeam:
                case WiredBoxType.ConditionIsGroupMember:
                case WiredBoxType.EffectFreezeUserBox:
                case WiredBoxType.EffectFixRoomBox:
                    return 10;
                case WiredBoxType.TriggerUserFurniCollision:
                case WiredBoxType.ConditionIsWearingBadge:
                case WiredBoxType.EffectMoveFurniToNearestUser:
                    return 11;
                case WiredBoxType.ConditionIsWearingFX:
                case WiredBoxType.EffectMoveFurniFromNearestUser:
                    return 12;
                case WiredBoxType.ConditionFurniHasNoUsers:
                    return 14;
                case WiredBoxType.ConditionTriggererNotOnFurni:
                    return 15;
                case WiredBoxType.ConditionUserCountDoesntInRoom:
                    return 16;
                case WiredBoxType.EffectGiveReward:
                    return 17;
                case WiredBoxType.EffectExecuteWiredStacks:
                case WiredBoxType.ConditionFurniHasNoFurni:
                    return 18;
                case WiredBoxType.ConditionFurniTypeDoesntMatch:
                    return 19;
                case WiredBoxType.EffectMuteTriggerer:
                    return 20;
                case WiredBoxType.ConditionIsNotGroupMember:
                case WiredBoxType.EffectTeleportBotToFurniBox:
                    return 21;
                case WiredBoxType.ConditionIsNotWearingBadge:
                case WiredBoxType.EffectBotMovesToFurniBox:
                    return 22;
                case WiredBoxType.ConditionIsNotWearingFX:
                case WiredBoxType.EffectBotCommunicatesToAllBox:
                    return 23;
                case WiredBoxType.EffectBotGivesHanditemBox:
                    return 24;
                case WiredBoxType.EffectBotFollowsUserBox:
                case WiredBoxType.ConditionActorHasHandItemBox:
                    return 25;
                case WiredBoxType.EffectBotChangesClothesBox:
                    return 26;
                case WiredBoxType.EffectBotCommunicatesToUserBox:
                    return 27;
            }
            return 0;
        }

        public static List<int> ContainsBlockedTrigger(IWiredItem Box, ICollection<IWiredItem> Triggers)
        {
            List<int> BlockedItems = new List<int>();

            if (Box.Type != WiredBoxType.EffectShowMessage && Box.Type != WiredBoxType.EffectMuteTriggerer && Box.Type != WiredBoxType.EffectTeleportToFurni && Box.Type != WiredBoxType.EffectKickUser && Box.Type != WiredBoxType.ConditionTriggererOnFurni)
                return BlockedItems;

            foreach (IWiredItem Item in Triggers)
            {
                if (Item.Type == WiredBoxType.TriggerRepeat || Item.Type == WiredBoxType.TriggerLongRepeat)
                {
                    if (!BlockedItems.Contains(Item.Item.GetBaseItem().SpriteId))
                        BlockedItems.Add(Item.Item.GetBaseItem().SpriteId);
                    else continue;
                }
                else continue;
            }

            return BlockedItems;
        }

        public static List<int> ContainsBlockedEffect(IWiredItem Box, ICollection<IWiredItem> Effects)
        {
            List<int> BlockedItems = new List<int>();

            if (Box.Type != WiredBoxType.TriggerRepeat || Box.Type != WiredBoxType.TriggerLongRepeat)
                return BlockedItems;

            bool HasMoveRotate = Effects.Where(x => x.Type == WiredBoxType.EffectMoveAndRotate).ToList().Count > 0;
            bool HasMoveNear = Effects.Where(x => x.Type == WiredBoxType.EffectMoveFurniToNearestUser).ToList().Count > 0;

            foreach (IWiredItem Item in Effects)
            {
                if (Item.Type == WiredBoxType.EffectKickUser || Item.Type == WiredBoxType.EffectMuteTriggerer || Item.Type == WiredBoxType.EffectShowMessage || Item.Type == WiredBoxType.EffectTeleportToFurni || Item.Type == WiredBoxType.EffectBotFollowsUserBox)
                {
                    if (!BlockedItems.Contains(Item.Item.GetBaseItem().SpriteId))
                        BlockedItems.Add(Item.Item.GetBaseItem().SpriteId);
                    else continue;
                }
                else if ((Item.Type == WiredBoxType.EffectMoveFurniToNearestUser && HasMoveRotate) || (Item.Type == WiredBoxType.EffectMoveAndRotate && HasMoveNear))
                {
                    if (!BlockedItems.Contains(Item.Item.GetBaseItem().SpriteId))
                        BlockedItems.Add(Item.Item.GetBaseItem().SpriteId);
                    else continue;
                }
            }

            return BlockedItems;
        }
    }
}