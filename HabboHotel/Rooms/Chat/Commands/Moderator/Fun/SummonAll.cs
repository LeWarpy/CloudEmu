using System.Linq;
using Cloud.HabboHotel.GameClients;
using Cloud.Communication.Packets.Outgoing.Rooms.Session;

namespace Cloud.HabboHotel.Rooms.Chat.Commands.Moderator.Fun
{
    class SummonAll : IChatCommand
    {
        public string PermissionRequired => "command_summon";
        public string Parameters => "";
        public string Description => "Trae a todos los usuarios.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {

            foreach (GameClient Client in CloudServer.GetGame().GetClientManager().GetClients.ToList())
            {
                if (Client == null || Client.GetHabbo() == null || Client.GetHabbo().Username == Session.GetHabbo().Username)
                    continue;

                Client.SendNotification("¡Acabas de ser atraído por " + Session.GetHabbo().Username + "!");
                if (!Client.GetHabbo().InRoom)
                    Client.SendMessage(new RoomForwardComposer(Session.GetHabbo().CurrentRoomId));
                else if (Client.GetHabbo().InRoom)
                    Client.SendMessage(new RoomForwardComposer(Session.GetHabbo().CurrentRoomId));
            }

            Session.SendWhisper("Acabas de atraer a todo el puto hotel men.");

            }
        }
    }
