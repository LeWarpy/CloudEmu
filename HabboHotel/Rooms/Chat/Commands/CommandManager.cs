using System.Linq;
using System.Text;
using System.Collections.Generic;
using Cloud.HabboHotel.GameClients;
using Cloud.HabboHotel.Rooms.Chat.Commands.User;
using Cloud.HabboHotel.Rooms.Chat.Commands.User.Fun;
using Cloud.HabboHotel.Rooms.Chat.Commands.Moderator;
using Cloud.HabboHotel.Rooms.Chat.Commands.Moderator.Fun;
using Cloud.HabboHotel.Rooms.Chat.Commands.Administrator;
using Cloud.Communication.Packets.Outgoing.Notifications;
using Cloud.Database.Interfaces;
using Cloud.HabboHotel.Rooms.Chat.Commands.Events;
using Cloud.HabboHotel.Items.Wired;



namespace Cloud.HabboHotel.Rooms.Chat.Commands
{
    public class CommandManager
    {
        private string _prefix = ":";
        private readonly Dictionary<string, IChatCommand> _commands;
        public CommandManager(string Prefix)
        {
            _prefix = Prefix;
            _commands = new Dictionary<string, IChatCommand>();
            RegisterVIP();
            RegisterUser();
            RegisterEvents();
            RegisterModerator();
            RegisterAdministrator();
        }

        public bool Parse(GameClient Session, string Message)
        {
            if (Session == null || Session.GetHabbo() == null || Session.GetHabbo().CurrentRoom == null)
                return false;

            if (!Message.StartsWith(_prefix))
                return false;

            if (Message == _prefix + "commands")
            {
                StringBuilder List = new StringBuilder();
                List.Append("- LISTA DE COMANDOS DISPONIVEIS -\n\n");
                foreach (var CmdList in _commands.ToList())
                {
                    if (!string.IsNullOrEmpty(CmdList.Value.PermissionRequired))
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand(CmdList.Value.PermissionRequired))
                            continue;
                    }

                    List.Append(":" + CmdList.Key + " " + CmdList.Value.Parameters + " >> " + CmdList.Value.Description + "\n········································································\n");
                }
               
                Session.SendMessage(new MOTDNotificationComposer(List.ToString()));
                return true;
            }

            Message = Message.Substring(1);
            string[] Split = Message.Split(' ');

            if (Split.Length == 0)
                return false;

			if (Session.GetHabbo().Rank == 1)
			{
				LogCommand(Session.GetHabbo().Id, Message, Session.GetHabbo().MachineId);
			}
			if (_commands.TryGetValue(Split[0].ToLower(), out IChatCommand Cmd))
            {

                if (Session.GetHabbo().GetPermissions().HasRight("mod_tool"))
                    LogCommandStaff(Session.GetHabbo().Id, Message, Session.GetHabbo().MachineId, Session.GetHabbo().Username);

                if (!string.IsNullOrEmpty(Cmd.PermissionRequired))
                {
                    if (!Session.GetHabbo().GetPermissions().HasCommand(Cmd.PermissionRequired))
                        return false;
                }


                Session.GetHabbo().IChatCommand = Cmd;
                Session.GetHabbo().CurrentRoom.GetWired().TriggerEvent(WiredBoxType.TriggerUserSaysCommand, Session.GetHabbo(), this);

                Cmd.Execute(Session, Session.GetHabbo().CurrentRoom, Split);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Registers the VIP set of commands.
        /// </summary>
        private void RegisterVIP()
        {
            Register("spull", new SuperPullCommand());
        }

        /// <summary>
        /// Registers the Events set of commands.
        /// </summary>
        private void RegisterEvents()
        {
            Register("eha", new EventAlertCommand());
            Register("eventha", new EventAlertCommand());
            Register("pha", new PublicityAlertCommand());
            Register("da2", new DiceAlertCommand());
        }

        /// <summary>
        /// Registers the default set of commands.
        /// </summary>
        private void RegisterUser()
        {
            Register("groupchat", new GroupChatCommand()); // test
            Register("about", new InfoCommand());
            Register("builder", new Builder());
            Register("info", new InfoCommand());
            Register("pickall", new PickAllCommand());
            Register("ejectall", new EjectAllCommand());
            Register("lay", new LayCommand());
            Register("sit", new SitCommand());
            Register("help", new HelpCommand());
            Register("stand", new StandCommand());
            Register("kiss", new KissCommand());
            Register("mutepets", new MutePetsCommand());
            Register("mutebots", new MuteBotsCommand());
            Register("beijar", new KissCommand());
            Register("bater", new  BaterCommand());
            Register("curar", new CurarCommand());
			Register("color", new ColourCommand());
			Register("sexo", new SexCommand());
			Register("fumar", new WeedCommand());

			Register("mimic", new MimicCommand());
            Register("dance", new DanceCommand());
            Register("push", new PushCommand());
            Register("pull", new PullCommand());
            Register("enable", new EnableCommand());
            Register("follow", new FollowCommand());
            Register("faceless", new FacelessCommand());
            Register("moonwalk", new MoonwalkCommand());

            Register("unload", new UnloadCommand());
            Register("reload", new UnloadCommand(true));
            Register("fixroom", new RegenMaps());
            Register("empty", new EmptyItems());
            Register("setmax", new SetMaxCommand());
            Register("setspeed", new SetSpeedCommand());
            Register("disablefriends", new DisableFriendsCommand());
            Register("enablefriends", new EnableFriendsCommand());
            Register("disablediagonal", new DisableDiagonalCommand());
            Register("flagme", new FlagMeCommand());
            Register("stats", new StatsCommand());
            Register("kickpets", new KickPetsCommand());
            Register("kickbots", new KickBotsCommand());

            Register("dnd", new DNDCommand());
            Register("matar", new MatarCommand());
            Register("disablegifts", new DisableGiftsCommand());
            Register("convertcredits", new ConvertCreditsCommand());
            Register("convertduckets", new ConvertDucketsCommand());
            Register("disablewhispers", new DisableWhispersCommand());
            Register("disablemimic", new DisableMimicCommand()); ;
            Register("pet", new PetCommand());
            Register("spush", new SuperPushCommand());
            Register("superpush", new SuperPushCommand());
            Register("Roubar", new RoubarCommand());
            Register("Fuzil", new UziCPCommand());

        }

        /// <summary>
        /// Registers the moderator set of commands.
        /// </summary>
        private void RegisterModerator()
        {
            Register("ban", new BanCommand());
            Register("unban", new UnBanCommand());
            Register("mip", new MIPCommand());
            Register("ipban", new IPBanCommand());
            Register("bpu", new BanPubliCommand());
            Register("prefixname", new PrefixNameCommand());
            Register("pcolor", new ColourPrefixCommand());
            Register("ui", new UserInfoCommand());
            Register("userinfo", new UserInfoCommand());
            Register("roomcredits", new GiveRoom());
            Register("sa", new StaffAlertCommand());
            Register("ga", new GuideAlertCommand());
            Register("roomunmute", new RoomUnmuteCommand());
            Register("roommute", new RoomMuteCommand());
            Register("roombadge", new RoomBadgeCommand());
            Register("roomalert", new RoomAlertCommand());
            Register("roomkick", new RoomKickCommand());
            Register("mute", new MuteCommand());
			Register("unmute", new UnmuteCommand());
			Register("massbadge", new MassBadgeCommand());
            Register("massgive", new MassGiveCommand());
            Register("globalgive", new GlobalGiveCommand());
            Register("kick", new KickCommand());
            Register("skick", new KickCommand());
            Register("ha", new HotelAlertCommand());
            Register("hal", new HALCommand());
            Register("give", new GiveCommand());
            Register("givebadge", new GiveBadgeCommand());
            Register("rbadge", new TakeUserBadgeCommand());
            Register("dc", new DisconnectCommand());
            Register("disconnect", new DisconnectCommand());
            Register("alert", new AlertCommand());

            Register("tradeban", new TradeBanCommand());
            Register("poll", new PollCommand());
            Register("quizz", new IdolQuizCommand());
            Register("lastmsg", new LastMessagesCommand());
            Register("lastconsolemsg", new LastConsoleMessagesCommand());

            Register("teleport", new TeleportCommand());
            Register("summon", new SummonCommand());
            Register("senduser", new SendUserCommand());
            Register("override", new OverrideCommand());
            Register("massenable", new MassEnableCommand());
            Register("massdance", new MassDanceCommand());
            Register("freeze", new FreezeCommand());
            Register("unfreeze", new UnFreezeCommand());
            Register("fastwalk", new FastwalkCommand());
            Register("superfastwalk", new SuperFastwalkCommand());
            Register("coords", new CoordsCommand());
            Register("alleyesonme", new AllEyesOnMeCommand());
            Register("allaroundme", new AllAroundMeCommand());
            Register("forcesit", new ForceSitCommand());

            Register("ignorewhispers", new IgnoreWhispersCommand());
            Register("forced_effects", new DisableForcedFXCommand());

            Register("makesay", new MakeSayCommand());
            Register("flaguser", new FlagUserCommand());
            Register("addblackword", new FilterCommand());
            Register("usermsj", new UserMessageCommand());
            Register("globalmsj", new GlobalMessageCommand());
            Register("userson", new ViewOnlineCommand());
            Register("makepublic", new MakePublicCommand());
            Register("makeprivate", new MakePrivateCommand());
			Register("premiar", new PremiarCommand());
		}

        /// <summary>
        /// Registers the administrator set of commands.
        /// </summary>
        private void RegisterAdministrator()
        {
            Register("addpredesigned", new AddPredesignedCommand());
            Register("removepredesigned", new RemovePredesignedCommand());
            Register("bubble", new BubbleCommand());
            Register("staffson", new StaffInfo());
            Register("staffons", new StaffInfo());
            Register("bubblebot", new BubbleBotCommand());
            Register("update", new UpdateCommand());
            Register("emptyuser", new EmptyUser());
            Register("deletegroup", new DeleteGroupCommand());
            Register("handitem", new CarryCommand());
            Register("goto", new GOTOCommand());
            Register("dj", new DJAlert());
            Register("summonall", new SummonAll());
            Register("djalert", new DJAlert());
            Register("catup", new CatalogUpdateAlert());
        }

        /// <summary>
        /// Registers a Chat Command.
        /// </summary>
        /// <param name="CommandText">Text to type for this command.</param>
        /// <param name="Command">The command to execute.</param>
        public void Register(string CommandText, IChatCommand Command)
        {
            _commands.Add(CommandText, Command);
        }

        public static string MergeParams(string[] Params, int Start)
        {
            var Merged = new StringBuilder();
            for (int i = Start; i < Params.Length; i++)
            {
                if (i > Start)
                    Merged.Append(" ");
                Merged.Append(Params[i]);
            }

            return Merged.ToString();
        }

        public void LogCommand(int UserId, string Data, string MachineId)
        {
            using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("INSERT INTO `logs_client_user` (`user_id`,`data_string`,`machine_id`, `timestamp`) VALUES (@UserId,@Data,@MachineId,@Timestamp)");
                dbClient.AddParameter("UserId", UserId);
                dbClient.AddParameter("Data", Data);
                dbClient.AddParameter("MachineId", MachineId);
                dbClient.AddParameter("Timestamp", CloudServer.GetUnixTimestamp());
                dbClient.RunQuery();
            }
        }

        public void LogCommandStaff(int UserId, string Data, string MachineId, string Username)
        {
            using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("INSERT INTO `logs_client_staff` (`user_id`,`data_string`,`machine_id`, `timestamp`) VALUES (@UserId,@Data,@MachineId,@Timestamp)");
                dbClient.AddParameter("UserId", UserId);
                dbClient.AddParameter("Data", Data);
                dbClient.AddParameter("MachineId", MachineId);
                dbClient.AddParameter("Timestamp", CloudServer.GetUnixTimestamp());
                dbClient.RunQuery();
            }

        }

        public bool TryGetCommand(string Command, out IChatCommand IChatCommand)
        {
            return _commands.TryGetValue(Command, out IChatCommand);
        }
    }
}