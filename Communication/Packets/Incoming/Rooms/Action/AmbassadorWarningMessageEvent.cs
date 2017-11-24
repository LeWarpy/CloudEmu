using Cloud.Communication.Packets.Outgoing.Moderation;
using Cloud.Communication.Packets.Outgoing.Rooms.Notifications;
using Cloud.HabboHotel.Rooms;


namespace Cloud.Communication.Packets.Incoming.Rooms.Action
{
    class AmbassadorWarningMessageEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {

            int UserId = Packet.PopInt();
            int RoomId = Packet.PopInt();
            int Time = Packet.PopInt();
            string HotelName = CloudServer.HotelName;

            Room Room = Session.GetHabbo().CurrentRoom;
            RoomUser Target = Room.GetRoomUserManager().GetRoomUserByHabbo(CloudServer.GetUsernameById(UserId));
            if (Target == null)
                return;

            long nowTime = CloudServer.CurrentTimeMillis();
            long timeBetween = nowTime - Session.GetHabbo()._lastTimeUsedHelpCommand;
            if (timeBetween < 60000)
            {
                Session.SendMessage(RoomNotificationComposer.SendBubble("Abuso", "Espere pelo menos 1 minuto para enviar um alerta de novo.", ""));
                return;
            }

            else
                CloudServer.GetGame().GetClientManager().StaffAlert(RoomNotificationComposer.SendBubble("advice", "" + Session.GetHabbo().Username + " acaba de mandar um alerta embaixador a " + Target.GetClient().GetHabbo().Username + ", clique aqui para ir.", "event:navigator/goto/" + Session.GetHabbo().CurrentRoomId));
            Target.GetClient().SendMessage(new BroadcastMessageAlertComposer("<b><font size='15px' color='#c40101'>Mensagem de embaixadores<br></font></b>embaixadores de " + HotelName + " considerar que o seu comportamento não é o melhor. Por favor, reconsidere a sua atitude, antes de um moderador tomar medidas."));

            Session.GetHabbo()._lastTimeUsedHelpCommand = nowTime;
        }
    }
}
