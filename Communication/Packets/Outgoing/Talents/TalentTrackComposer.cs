using System;
using System.Collections.Generic;
using Cloud.HabboHotel.Achievements;
using Cloud.HabboHotel.GameClients;

namespace Cloud.Communication.Packets.Outgoing.Talents
{
    class TalentTrackComposer : ServerPacket
    {
        public TalentTrackComposer(GameClient session, string trackType, List<Talent> talents)
            : base(ServerPacketHeader.TalentTrackMessageComposer)
        {
			WriteString(trackType);
			WriteInteger(talents.Count);

            int failLevel = -1;

            foreach (Talent current in talents)
            {
				WriteInteger(current.Level);
                int nm = failLevel == -1 ? 1 : 0; // TODO What does this mean?
				WriteInteger(nm);

                List<Talent> children = CloudServer.GetGame().GetTalentManager().GetTalents(trackType, current.Id);

				WriteInteger(children.Count);

                foreach (Talent child in children)
                {
                    UserAchievement achievment = session.GetHabbo().GetAchievementData(child.AchievementGroup);
                    if (child.GetAchievement() == null)
                        throw new NullReferenceException(
                            string.Format("The following talent achievement can't be found: {0}",
                                child.AchievementGroup));

                    // TODO Refactor What does num mean?!
                    var num = (failLevel != -1 && failLevel < child.Level)
                       ? 0
                       : (session.GetHabbo().GetAchievementData(child.AchievementGroup) == null)
                           ? 1
                           : (session.GetHabbo().GetAchievementData(child.AchievementGroup).Level >=
                              child.AchievementLevel)
                               ? 2
                               : 1;

					WriteInteger(child.GetAchievement().Id);
					WriteInteger(0); // TODO Magic constant

					WriteString(child.AchievementGroup + child.AchievementLevel);
					WriteInteger(num);

					WriteInteger(achievment != null ? achievment.Progress : 0);
					WriteInteger(child.GetAchievement() == null ? 0
                        : child.GetAchievement().Levels[child.AchievementLevel].Requirement);

                    if (num != 2 && failLevel == -1)
                        failLevel = child.Level;
                }

				WriteInteger(0); // TODO Magic constant

                // TODO Type should be enum?
                if (current.Type == "citizenship" && current.Level == 4)
                {
					WriteInteger(2);
					WriteString("HABBO_CLUB_VIP_7_DAYS");
					WriteInteger(7);
					WriteString(current.Prize); // TODO Hardcoded stuff
					WriteInteger(0);
                }
                else
                {
					WriteInteger(1);
					WriteString(current.Prize);
					WriteInteger(0);
                }
            }
        }
    }
}
