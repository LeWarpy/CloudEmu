using Cloud.Communication.Packets.Outgoing.Groups;

namespace Cloud.Communication.Packets.Incoming.Groups
{
    class GetBadgeEditorPartsEvent : IPacketEvent
    {
        public void Parse(HabboHotel.GameClients.GameClient Session, ClientPacket Packet)
        {
            Session.SendMessage(new BadgeEditorPartsComposer(
                CloudServer.GetGame().GetGroupManager().BadgeBases,
                CloudServer.GetGame().GetGroupManager().BadgeSymbols,
                CloudServer.GetGame().GetGroupManager().BadgeBaseColours,
                CloudServer.GetGame().GetGroupManager().BadgeSymbolColours,
                CloudServer.GetGame().GetGroupManager().BadgeBackColours));
        }
    }
}
