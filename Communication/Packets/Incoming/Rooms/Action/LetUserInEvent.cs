
using Cloud.HabboHotel.Rooms;
using Cloud.HabboHotel.GameClients;
using Cloud.Communication.Packets.Outgoing.Navigator;
using Cloud.Communication.Packets.Outgoing.Rooms.Session;

namespace Cloud.Communication.Packets.Incoming.Rooms.Action
{
    class LetUserInEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Room Room;

            if (!CloudServer.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room))
                return;

            if (!Room.CheckRights(Session))
                return;

            string Name = Packet.PopString();
            bool Accepted = Packet.PopBoolean();

            GameClient Client = CloudServer.GetGame().GetClientManager().GetClientByUsername(Name);
            if (Client == null)
                return;

            if (Accepted)
            {
                Client.GetHabbo().RoomAuthOk = true;
                Client.SendMessage(new FlatAccessibleComposer(""));
                Room.SendMessage(new FlatAccessibleComposer(Client.GetHabbo().Username), true);
            }
            else
            {
                Client.SendMessage(new FlatAccessDeniedComposer(""));
                Room.SendMessage(new FlatAccessDeniedComposer(Client.GetHabbo().Username), true);
            }
        }
    }
}
