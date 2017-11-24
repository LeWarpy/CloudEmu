using System;
using System.Linq;
using System.Collections.Generic;

using Cloud.HabboHotel.GameClients;
using Cloud.Communication.Packets.Outgoing.Inventory.Purse;
using Cloud.Communication.Packets.Outgoing.Inventory.Achievements;

using Cloud.Database.Interfaces;
using log4net;


namespace Cloud.HabboHotel.Achievements
{
    public class AchievementManager
    {
        private static readonly ILog log = LogManager.GetLogger("Cloud.HabboHotel.Achievements.AchievementManager");

        public Dictionary<string, Achievement> _achievements;

        public AchievementManager()
        {
            this._achievements = new Dictionary<string, Achievement>();
            this.LoadAchievements();

            log.Info("» Achievement Manager -> CARGADO");
        }

        public void LoadAchievements()
        {
            AchievementLevelFactory.GetAchievementLevels(out _achievements);
        }

        public void TryProgressLoginAchievements(GameClient Session)
        {
            if (Session.GetHabbo() == null)
                return;
            UserAchievement loginACH = Session.GetHabbo().GetAchievementData("ACH_Login");
            if (loginACH == null)
            {
                ProgressAchievement(Session, "ACH_Login", 1, true);
                return;
            }

            Double daysBtwLastLogin = CloudServer.GetUnixTimestamp() - Session.GetHabbo().LastOnline;

            if (daysBtwLastLogin >= 51840 && daysBtwLastLogin <= 112320)
                ProgressAchievement(Session, "ACH_Login", 1, true);
        }

        public void TryProgressRegistrationAchievements(GameClient Session)
        {
            if (Session.GetHabbo() == null)
                return;

            UserAchievement regACH = Session.GetHabbo().GetAchievementData("ACH_RegistrationDuration");
            if (regACH == null)
            {
                ProgressAchievement(Session, "ACH_RegistrationDuration", 1, true);
                return;
            }

            if (regACH.Level == 5)
                return;

            double sinceMember = CloudServer.GetUnixTimestamp() - (int)Session.GetHabbo().AccountCreated;
            int daysSinceMember = Convert.ToInt32(Math.Round(sinceMember / 86400));
            if (daysSinceMember == regACH.Progress)
                return;

            int dais = daysSinceMember - regACH.Progress;
            if (dais < 1)
                return;

            ProgressAchievement(Session, "ACH_RegistrationDuration", dais, false);
        }

        public void TryProgressHabboClubAchievements(GameClient Session)
        {
            if (Session.GetHabbo() == null || !Session.GetHabbo().GetClubManager().HasSubscription("habbo_vip"))
                return;

            UserAchievement ClubACH = Session.GetHabbo().GetAchievementData("ACH_VipHC");
            if (ClubACH == null)
            {
                ProgressAchievement(Session, "ACH_VipHC", 1, true);
                ProgressAchievement(Session, "ACH_BasicClub", 1, true);
                return;
            }

            if (ClubACH.Level == 5)
                return;

            var Subscription = Session.GetHabbo().GetClubManager().GetSubscription("habbo_vip");
            Double SinceActivation = CloudServer.GetUnixTimestamp() - Subscription.ExpireTime;

            if (SinceActivation < 31556926)
                return;
            if (SinceActivation >= 31556926)
            {
                ProgressAchievement(Session, "ACH_VipHC", 1, false);
                ProgressAchievement(Session, "ACH_BasicClub", 1, false);
            }
            if (SinceActivation >= 63113851)
            {
                ProgressAchievement(Session, "ACH_VipHC", 1, false);
                ProgressAchievement(Session, "ACH_BasicClub", 1, false);
            }
            if (SinceActivation >= 94670777)
            {
                ProgressAchievement(Session, "ACH_VipHC", 1, false);
                ProgressAchievement(Session, "ACH_BasicClub", 1, false);
            }
            if (SinceActivation >= 126227704)
            {
                ProgressAchievement(Session, "ACH_VipHC", 1, false);
                ProgressAchievement(Session, "ACH_BasicClub", 1, false);
            }
            if (SinceActivation >= 157784630)
            {
                ProgressAchievement(Session, "ACH_VipHC", 1, false);
                ProgressAchievement(Session, "ACH_BasicClub", 1, false);
            }
        }


        public bool ProgressAchievement(GameClient Session, string AchievementGroup, int ProgressAmount, bool FromZero = false)
        {
            if (!_achievements.ContainsKey(AchievementGroup) || Session == null)
                return false;

            Achievement AchievementData = null;
            AchievementData = _achievements[AchievementGroup];
            UserAchievement UserData = Session.GetHabbo().GetAchievementData(AchievementGroup);

            if (UserData == null)
            {
                UserData = new UserAchievement(AchievementGroup, 0, 0);
                Session.GetHabbo().Achievements.TryAdd(AchievementGroup, UserData);
            }

            int TotalLevels = AchievementData.Levels.Count;
            if (UserData != null && UserData.Level == TotalLevels)
                return false; // done, no more.

            int TargetLevel = (UserData != null ? UserData.Level + 1 : 1);

            if (TargetLevel > TotalLevels)
                TargetLevel = TotalLevels;

            AchievementLevel TargetLevelData = AchievementData.Levels[TargetLevel];
            int NewProgress = 0;
            if (FromZero)
                NewProgress = ProgressAmount;
            else
                NewProgress = (UserData != null ? UserData.Progress + ProgressAmount : ProgressAmount);
            int NewLevel = (UserData != null ? UserData.Level : 0);
            int NewTarget = NewLevel + 1;

            if (NewTarget > TotalLevels)
                NewTarget = TotalLevels;

            if (NewProgress >= TargetLevelData.Requirement)
            {
                NewLevel++;
                NewTarget++;
                NewProgress = 0;

                if (TargetLevel == 1)
                    Session.GetHabbo().GetBadgeComponent().GiveBadge(string.Format("{0}{1}", AchievementGroup, TargetLevel), true, Session);
                else
                    Session.GetHabbo().GetBadgeComponent().RemoveBadge(Convert.ToString(string.Format("{0}{1}", AchievementGroup, TargetLevel - 1)));
                    Session.GetHabbo().GetBadgeComponent().GiveBadge(string.Format("{0}{1}", AchievementGroup, TargetLevel), true, Session);

                if (NewTarget > TotalLevels)
                    NewTarget = TotalLevels;

                Session.SendMessage(new AchievementUnlockedComposer(AchievementData, TargetLevel, TargetLevelData.RewardPoints, TargetLevelData.RewardPixels));
                Session.GetHabbo().GetMessenger().BroadcastAchievement(Session.GetHabbo().Id, Users.Messenger.MessengerEventTypes.ACHIEVEMENT_UNLOCKED, AchievementGroup + TargetLevel);

                using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery(string.Concat("REPLACE INTO `user_achievements` VALUES ('" + Session.GetHabbo().Id + "', @group, '" + NewLevel + "', '" + NewProgress + "')"));
                    dbClient.AddParameter("group", AchievementGroup);
                    dbClient.RunQuery();
                }

                UserData.Level = NewLevel;
                UserData.Progress = NewProgress;

                Session.GetHabbo().Duckets += TargetLevelData.RewardPixels;
                Session.GetHabbo().GetStats().AchievementPoints += TargetLevelData.RewardPoints;
                Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Duckets, TargetLevelData.RewardPixels));
                Session.SendMessage(new AchievementScoreComposer(Session.GetHabbo().GetStats().AchievementPoints));

                AchievementLevel NewLevelData = AchievementData.Levels[NewTarget];
                Session.SendMessage(new AchievementProgressedComposer(AchievementData, NewTarget, NewLevelData, TotalLevels, Session.GetHabbo().GetAchievementData(AchievementGroup)));

                // Set Talent
                Talent talent = null;
                if (CloudServer.GetGame().GetTalentManager().TryGetTalent(AchievementGroup, out talent))
                    CloudServer.GetGame().GetTalentManager().CompleteUserTalent(Session, talent);
                return true;
            }
            else
            {
                UserData.Level = NewLevel;
                UserData.Progress = NewProgress;
                using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("REPLACE INTO `user_achievements` VALUES ('" + Session.GetHabbo().Id + "', @group, '" + NewLevel + "', '" + NewProgress + "')");
                    dbClient.AddParameter("group", AchievementGroup);
                    dbClient.RunQuery();
                }

                Session.SendMessage(new AchievementProgressedComposer(AchievementData, TargetLevel, TargetLevelData, TotalLevels, Session.GetHabbo().GetAchievementData(AchievementGroup)));
            }
            return false;
        }

        public ICollection<Achievement> GetGameAchievements(int GameId)
        {
            List<Achievement> GameAchievements = new List<Achievement>();
            foreach (Achievement Achievement in _achievements.Values.ToList())
            {
                if (Achievement.Category == "games" && Achievement.GameId == GameId)
                    GameAchievements.Add(Achievement);
            }
            return GameAchievements;
        }

        public Achievement GetAchievement(string achievementGroup)
            => _achievements.ContainsKey(achievementGroup) ? _achievements[achievementGroup] : null;
    }
}
