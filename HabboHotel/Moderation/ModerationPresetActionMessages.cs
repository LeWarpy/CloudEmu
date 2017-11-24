﻿namespace Cloud.HabboHotel.Moderation
{
    public class ModerationPresetActionMessages
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public string Caption { get; set; }
        public string MessageText { get; set; }
        public int MuteTime { get; set; }
        public int BanTime { get; set; }
        public int IPBanTime { get; set; }
        public int TradeLockTime { get; set; }
        public string Notice { get; set; }

        public ModerationPresetActionMessages(int Id, int ParentId, string Caption, string MessageText, int MuteTime, int BanTime, int IPBanTime, int TradeLockTime, string Notice)
        {
            this.Id = Id;
            this.ParentId = ParentId;
            this.Caption = Caption;
            this.MessageText = MessageText;
            this.MuteTime = MuteTime;
            this.BanTime = BanTime;
            this.IPBanTime = IPBanTime;
            this.TradeLockTime = TradeLockTime;
            this.Notice = Notice;
        }
    }
}
