﻿using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Cloud.HabboHotel.Items.Data.Moodlight;

namespace Cloud.Communication.Packets.Outgoing.Rooms.Furni.Moodlight
{
    class MoodlightConfigComposer : ServerPacket
    {
        public MoodlightConfigComposer(MoodlightData MoodlightData)
            : base(ServerPacketHeader.MoodlightConfigMessageComposer)
        {
            base.WriteInteger(MoodlightData.Presets.Count);
            base.WriteInteger(MoodlightData.CurrentPreset);

            int i = 1;
            foreach (MoodlightPreset Preset in MoodlightData.Presets)
            {
                base.WriteInteger(i);
                base.WriteInteger(Preset.BackgroundOnly ? 2 : 1);
               base.WriteString(Preset.ColorCode);
                base.WriteInteger(Preset.ColorIntensity);
                i++;
            }
        }
    }
}