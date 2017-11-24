using System;
using System.Collections.Generic;

namespace Cloud.Communication.Packets.Outgoing.Sound
{
    class SoundSettingsComposer : ServerPacket
    {
        public SoundSettingsComposer(ICollection<int> ClientVolumes, Boolean ChatPreference, Boolean InvitesStatus, Boolean FocusPreference, int FriendBarState)
            : base(ServerPacketHeader.SoundSettingsMessageComposer)
        {
            foreach (int VolumeValue in ClientVolumes)
            {
				WriteInteger(VolumeValue);
            }

			WriteBoolean(ChatPreference);
			WriteBoolean(InvitesStatus);
			WriteBoolean(FocusPreference);
			WriteInteger(FriendBarState);
			WriteInteger(0);
			WriteInteger(0);
        }
    }
}