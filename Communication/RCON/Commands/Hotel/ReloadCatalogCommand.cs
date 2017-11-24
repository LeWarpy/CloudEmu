using Cloud.Communication.Packets.Outgoing.Catalog;

namespace Cloud.Communication.RCON.Commands.Hotel
{
    class ReloadCatalogCommand : IRCONCommand
    {
        public string Description => "Se utiliza para actualizar Catalogo";
        public string Parameters => "";

        public bool TryExecute(string[] parameters)
        {
            CloudServer.GetGame().GetCatalog().Init(CloudServer.GetGame().GetItemManager());
            CloudServer.GetGame().GetClientManager().SendMessage(new CatalogUpdatedComposer());
            return true;
        }
    }
}