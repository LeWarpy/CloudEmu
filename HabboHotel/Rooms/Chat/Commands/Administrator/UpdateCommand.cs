using System.Linq;
using Cloud.Communication.Packets.Outgoing.Rooms.Notifications;
using Cloud.Communication.Packets.Outgoing.Catalog;
using Cloud.Core;
using Cloud.HabboHotel.GameClients;
using Cloud.Communication.Packets.Outgoing.Notifications;
using System.Text;
using Cloud.Communication.Packets.Incoming.LandingView;
using Cloud.HabboHotel.LandingView;
using Cloud.HabboHotel.Rooms.TraxMachine;

namespace Cloud.HabboHotel.Rooms.Chat.Commands.Administrator
{
    class UpdateCommand : IChatCommand
    {
        public string PermissionRequired => "command_update";
        public string Parameters => "[VARIABLE]";
        public string Description => "Atualizar catalago.";

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                StringBuilder List = new StringBuilder();
                List.Append("- LISTA DE COMANDOS STAFF -\n\n");
                List.Append(":update catalogue - Atualiza o catalogo.\n········································································\n");
                List.Append(":update items - Atualiza os items.\n········································································\n");
                List.Append(":update jukebox - Atualiza as musicas.\n········································································\n");
                List.Append(":update wordfilter - Atualiza o filtro do hotel.\n········································································\n");
                List.Append(":update models - Atualiza o filtro del hotel.\n········································································\n");
                List.Append(":update promotions - Atualiza as promoções.\n········································································\n");
                List.Append(":update halloffame - Atualiza pontos de fama.\n········································································\n");
                List.Append(":update youtube - Atualiza os videos TV's.\n········································································\n");
                List.Append(":update permissions - Atualiza as permissões de rank.\n········································································\n");
                List.Append(":update settings - Atualiza las configurações do hotel.\n········································································\n");
                List.Append(":update bans - Atualiza os banidos do hotel.\n········································································\n");
                List.Append(":update quests - Atualiza os Quests do hotel.\n········································································\n");
                List.Append(":update achievements - Atualiza is logs de usuarios.\n········································································\n");
                List.Append(":update bots - Atualiza os bots do hotel.\n········································································\n");
                List.Append(":update achievements - Atualiza os logros de usuarios.\n········································································\n");
                Session.SendMessage(new MOTDNotificationComposer(List.ToString()));
                return;
            }

            string UpdateVariable = Params[1];
            switch (UpdateVariable.ToLower())
            {
                case "cata":
                case "catalog":
                case "catalogue":
                    if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_catalog"))
                    {
                        Session.LogsNotif("Ups, Você não tem permissão  'command_update_catalog' permiso", "advice");
                        break;
                    }

                    CloudServer.GetGame().GetCatalog().Init(CloudServer.GetGame().GetItemManager());
                    CloudServer.GetGame().GetClientManager().SendMessage(new CatalogUpdatedComposer());
                    Session.LogsNotif("Catalogo actualizado correctamente", "catalogue");
                    break;

                case "discos":
                case "songs":
                case "jukebox":
                case "canciones":
                    if (!Session.GetHabbo().GetPermissions().HasCommand("command_songsdata"))
                    {
                        Session.LogsNotif("Ups, Você não tem permissão 'command_songsdata", "advice");
                        break;
                    }
                    int count = TraxSoundManager.Songs.Count;
                    TraxSoundManager.Init();
                    Session.LogsNotif("Música recarregadas com sucesso, diferença de comprimento: " + checked(count - TraxSoundManager.Songs.Count), "advice");
                    break;

                case "wordfilter":
                    if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_filter"))
                    {
                        Session.LogsNotif("Ups, Você não tem permissão 'command_update_filter' permiso", "advice");
                        break;
                    }

                    CloudServer.GetGame().GetChatManager().GetFilter().InitWords();
                    CloudServer.GetGame().GetChatManager().GetFilter().InitCharacters();
                    Session.LogsNotif("Filtro atualizado corretamente", "advice");
                    break;

                case "items":
                case "furni":
                case "furniture":
                    if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_furni"))
                    {
                        Session.LogsNotif("Ups, Você não tem permissão 'command_update_furni' permiso", "advice");
                        break;
                    }

                    CloudServer.GetGame().GetItemManager().Init();
                    Session.LogsNotif("Items acualizados corretamente", "advice");
                    break;

                case "models":
                    if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_models"))
                    {
                        Session.LogsNotif("Ups, Você não tem permissão 'command_update_models' permiso", "advice");
                        break;
                    }

                    CloudServer.GetGame().GetRoomManager().LoadModels();
                    Session.LogsNotif("Salas atualizadas corretamente.", "advice");
                    break;

                case "promotions":
                    if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_promotions"))
                    {
                        Session.LogsNotif("Ups, Você não tem permissão 'command_update_promotions' permiso.", "advice");
                        break;
                    }

                    CloudServer.GetGame().GetLandingManager().LoadPromotions();
                    Session.LogsNotif("Promoçoes atualizadas corretamente.", "advice");
                    break;

                case "halloffame":
                case "salondelafama":
                    if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_halloffame"))
                    {
                        Session.LogsNotif("Ups, Você não tem permissão 'command_update_halloffame' permiso.", "advice");
                        break;
                    }

                    GetHallOfFame.GetInstance().Load();
                    Session.LogsNotif("Hall of Fame atualizado com sucesso.", "advice");
                    break;

                case "youtube":
                    if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_youtube"))
                    {
                        Session.LogsNotif("Ups, Você não tem permissão 'command_update_youtube' permiso.", "advice");
                        break;
                    }

                    CloudServer.GetGame().GetTelevisionManager().Init();
                    Session.LogsNotif("TV's atualizados.", "advice");
                    break;

                case "navigator":
                    if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_navigator"))
                    {
                        Session.LogsNotif("Ups, Você não tem permissão 'command_update_navigator'.", "advice");
                        break;
                    }

                    CloudServer.GetGame().GetNavigator().Init();
                    Session.LogsNotif("Navegador de salas atualizado.", "advice");
                    break;

                case "ranks":
                case "rights":
                case "permissions":
                    if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_rights"))
                    {
                        Session.LogsNotif("Ups, Você não tem permissão 'command_update_rights'.", "advice");
                        break;
                    }

                    CloudServer.GetGame().GetPermissionManager().Init();

                    foreach (GameClient Client in CloudServer.GetGame().GetClientManager().GetClients.ToList())
                    {
                        if (Client == null || Client.GetHabbo() == null || Client.GetHabbo().GetPermissions() == null)
                            continue;

                        Client.GetHabbo().GetPermissions().Init(Client.GetHabbo());
                    }

                    Session.LogsNotif("Permissoes atualizadas.", "advice");
                    break;
                case "pinatas":
                    if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_catalog"))
                    {
                        Session.SendWhisper("Oops, Você não tem permissão para atualizar prêmios pinatas.");
                        break;
                    }

                    CloudServer.GetGame().GetPinataManager().Initialize(CloudServer.GetDatabaseManager().GetQueryReactor());
                    CloudServer.GetGame().GetClientManager().SendMessage(RoomNotificationComposer.SendBubble("catalogue", "Premios Actualizados", ""));
                    break;
                case "crafting":
                    if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_furni"))
                    {
                        Session.SendWhisper("Oops, Você não tem permissão para atualizar .");
                        break;
                    }

                    CloudServer.GetGame().GetCraftingManager().Init();
                    Session.SendWhisper("Crafting actualizado correctamente.");
                    break;
                case "crackable":
                case "ecotron":
                case "pinata":
                case "piñata":
                    CloudServer.GetGame().GetPinataManager().Initialize(CloudServer.GetDatabaseManager().GetQueryReactor());
                    CloudServer.GetGame().GetFurniMaticRewardsMnager().Initialize(CloudServer.GetDatabaseManager().GetQueryReactor());
                    CloudServer.GetGame().GetTargetedOffersManager().Initialize(CloudServer.GetDatabaseManager().GetQueryReactor());
                    break;

                case "relampago":
                case "targeted":
                case "targetedoffers":
                    CloudServer.GetGame().GetTargetedOffersManager().Initialize(CloudServer.GetDatabaseManager().GetQueryReactor());
                    break;

                case "config":
                case "settings":
                    if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_configuration"))
                    {
                        Session.LogsNotif("Ups,Você não tem permissão 'command_update_configuration'.", "advice"); ;
                        break;
                    }

                    CloudServer.GetGame().GetSettingsManager().Init();
                    ExtraSettings.RunExtraSettings();
                    CatalogSettings.RunCatalogSettings();
                    NotificationSettings.RunNotiSettings();
                    CloudServer.GetGame().GetTargetedOffersManager().Initialize(CloudServer.GetDatabaseManager().GetQueryReactor());
                    Session.LogsNotif("Configuraçoes atualizadas.", "advice");
                    break;

                case "bans":
                    if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_bans"))
                    {
                        Session.LogsNotif("Ups, Você não tem'command_update_bans' permiso.", "advice");
                        break;
                    }

                    CloudServer.GetGame().GetModerationManager().ReCacheBans();
                    Session.LogsNotif("Cache Ban re-cargado.", "advice");
                    break;

                case "quests":
                    if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_quests"))
                    {
                        Session.LogsNotif("Ups, Você não tem permissão 'command_update_quests' permiso.", "advice");
                        break;
                    }

                    CloudServer.GetGame().GetQuestManager().Init();
                    Session.LogsNotif("Quests atualizadas.", "advice");
                    break;

                case "achievements":
                    if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_achievements"))
                    {
                        Session.LogsNotif("Ups, Você não tem permissão 'command_update_achievements' permiso.", "advice");
                        break;
                    }

                    CloudServer.GetGame().GetAchievementManager().LoadAchievements();
                    Session.LogsNotif("Achievements atualizados.", "advice");
                    break;

                case "moderation":
                    if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_moderation"))
                    {
                        Session.LogsNotif("Ups, Você não tem permissão 'command_update_moderation' permiso.", "advice"); ;
                        break;
                    }

                    CloudServer.GetGame().GetModerationManager().Init();
                    CloudServer.GetGame().GetClientManager().ModAlert("Presets de moderación se han actualizado.Por favor, vuelva a cargar el cliente para ver los nuevos presets.");

                    Session.LogsNotif("Configurações dos moderadores atualizadas.", "advice");
                    break;

                case "vouchers":
                    if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_vouchers"))
                    {
                        Session.LogsNotif("Ups, Você não tem permissão'command_update_vouchers.", "advice");
                        break;
                    }

                    CloudServer.GetGame().GetCatalog().GetVoucherManager().Init();
                    Session.LogsNotif("O catálogo cache atualizado.", "advice");
                    break;

                case "gc":
                case "games":
                case "gamecenter":
                    if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_game_center"))
                    {
                        Session.LogsNotif("Ups, Você não tem permissão 'command_update_game_center'.", "advice");
                        break;
                    }

                    CloudServer.GetGame().GetGameDataManager().Init();
                    Session.LogsNotif("Cache Game Center foi atualizado com sucesso.", "advice");
                    break;

                case "pet_locale":
                    if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_pet_locale"))
                    {
                        Session.LogsNotif("Ups, Você não tem permissão 'command_update_pet_locale'.", "advice");
                        break;
                    }

                    CloudServer.GetGame().GetChatManager().GetPetLocale().Init();
                    CloudServer.GetGame().GetChatManager().GetPetCommands().Init();
                    Session.LogsNotif("Cache local Animais atualizado.", "advice");
                    break;

                case "locale":
                    if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_locale"))
                    {
                        Session.LogsNotif("Ups, Você não tem permissão 'command_update_locale'.", "advice");
                        break;
                    }

                    CloudServer.GetGame().GetLanguageManager().Init();
                    Session.LogsNotif("Locale caché acualizado corretamente.", "advice");
                    break;

                case "mutant":

                    if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_anti_mutant"))
                    {
                        Session.LogsNotif("Ups, Você não tem permissão 'command_update_anti_mutant'.", "advice");
                        break;
                    }

                    CloudServer.GetGame().GetFigureManager().Init();
                    Session.LogsNotif("FigureData manager recarregado com sucesso!", "advice");
                    break;

                case "bots":
                    if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_bots"))
                    {
                        Session.LogsNotif("Ups, Você não tem permissão 'command_update_bots'.", "advice");
                        break;
                    }

                    CloudServer.GetGame().GetBotManager().Init();
                    Session.LogsNotif("Bots actualizados.", "advice");
                    break;

                case "rewards":
                    if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_rewards"))
                    {
                        Session.LogsNotif("Ups, Você não tem permissão 'command_update_rewards'.", "advice");
                        break;
                    }

                    CloudServer.GetGame().GetRewardManager().Reload();
                    Session.LogsNotif("Gestor De Recompensas voltou a carregar com sucesso!", "advice");
                    break;

                case "chat_styles":
                    if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_chat_styles"))
                    {
                        Session.LogsNotif("Ups, Você não tem permissão 'command_update_chat_styles'.", "advice");
                        break;
                    }

                    CloudServer.GetGame().GetChatManager().GetChatStyles().Init();
                    Session.LogsNotif("estilos de chat recarregado com sucesso!", "advice");
                    break;

                case "definitions":
                case "badge_definitions":
                    if (!Session.GetHabbo().GetPermissions().HasCommand("command_update_badge_definitions"))
                    {
                        Session.LogsNotif("Ups, Você não tem permissão 'command_update_badge_definitions'.", "advice");
                        break;
                    }

                    CloudServer.GetGame().GetBadgeManager().Init();
                    Session.LogsNotif("Definições placas recarregado com sucesso!", "advice");
                    break;

                default:
                    Session.LogsNotif("'" + UpdateVariable + "' não é uma coisa válida para recarregar.", "advice");
                    break;
            }
        }
    }
}
