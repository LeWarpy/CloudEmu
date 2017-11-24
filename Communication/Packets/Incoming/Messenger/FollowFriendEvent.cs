﻿using Cloud.HabboHotel.GameClients;

using Cloud.Communication.Packets.Outgoing.Messenger;
using Cloud.Communication.Packets.Outgoing.Rooms.Session;

namespace Cloud.Communication.Packets.Incoming.Messenger
{
    class FollowFriendEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null || Session.GetHabbo().GetMessenger() == null)
                return;

            int BuddyId = Packet.PopInt();
            if (BuddyId == 0 || BuddyId == Session.GetHabbo().Id)
                return;

            GameClient Client = CloudServer.GetGame().GetClientManager().GetClientByUserID(BuddyId);
            if (Client == null || Client.GetHabbo() == null)
                return;

            if (!Client.GetHabbo().InRoom)
            {
                Session.SendMessage(new FollowFriendFailedComposer(2));
                Session.GetHabbo().GetMessenger().UpdateFriend(Client.GetHabbo().Id, Client, true);
                return;
            }
            else if (Session.GetHabbo().CurrentRoom != null && Client.GetHabbo().CurrentRoom != null)
            {
                if (Session.GetHabbo().CurrentRoom.RoomId == Client.GetHabbo().CurrentRoom.RoomId)
                    return;
            }

            Session.SendMessage(new RoomForwardComposer(Client.GetHabbo().CurrentRoomId));
        }
    }
}
