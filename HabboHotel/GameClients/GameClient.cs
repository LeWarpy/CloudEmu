using System;
using Cloud.Communication.ConnectionManager;
using Cloud.Communication;
using Cloud.Core;
using System.Linq;
using System.Collections.Generic;
using Cloud.Communication.Packets.Incoming;
using Cloud.HabboHotel.Rooms;
using Cloud.Database.Interfaces;
using Cloud.HabboHotel.Users;
using Cloud.HabboHotel.Catalog;
using Cloud.Communication.Interfaces;
using Cloud.HabboHotel.Users.UserData;
using Cloud.Communication.Packets.Outgoing.Sound;
using Cloud.Communication.Packets.Outgoing.Rooms.Chat;
using Cloud.Communication.Packets.Outgoing.Handshake;
using Cloud.Communication.Packets.Outgoing.Navigator;
using Cloud.Communication.Packets.Outgoing.Moderation;
using Cloud.Communication.Packets.Outgoing.Inventory.AvatarEffects;
using Cloud.Communication.Packets.Outgoing.Inventory.Achievements;
using Cloud.Communication.Encryption.Crypto.Prng;
using Cloud.HabboHotel.Users.Messenger.FriendBar;
using Cloud.Communication.Packets.Outgoing.BuildersClub;
using Cloud.HabboHotel.Moderation;
using Cloud.Communication.Packets.Outgoing.Rooms.Notifications;
using Cloud.HabboHotel.Users.Messenger;
using Cloud.HabboHotel.Permissions;
using Cloud.Communication.Packets.Outgoing.Notifications;

namespace Cloud.HabboHotel.GameClients
{
    public class GameClient
    {
        private readonly int _id;
        private Habbo _habbo;
        public string MachineId;
        private bool _disconnected;
        public ARC4 RC4Client = null;
        private GamePacketParser _packetParser;
        private ConnectionInformation _connection;
        public int PingCount { get; set; }
        internal int CurrentRoomUserId;
        internal DateTime TimePingedReceived;

        public GameClient(int ClientId, ConnectionInformation pConnection)
        {
            _id = ClientId;
            _connection = pConnection;
            _packetParser = new GamePacketParser(this);

            PingCount = 0;
        }

        private void SwitchParserRequest()
        {
            _packetParser.SetConnection(_connection);
            _packetParser.OnNewPacket += Parser_OnNewPacket;
            byte[] data = (_connection.parser as InitialPacketParser).currentData;
            _connection.parser.Dispose();
            _connection.parser = _packetParser;
            _connection.parser.handlePacketData(data);
        }

        private void Parser_OnNewPacket(ClientPacket Message)
        {
            try
            {
                CloudServer.GetGame().GetPacketManager().TryExecutePacket(this, Message);
            }
            catch (Exception e)
            {

				ExceptionLogger.LogException(e);
			}
        }

        private void PolicyRequest()
        {
            _connection.SendData(CloudServer.GetDefaultEncoding().GetBytes("<?xml version=\"1.0\"?>\r\n" +
                   "<!DOCTYPE cross-domain-policy SYSTEM \"/xml/dtds/cross-domain-policy.dtd\">\r\n" +
                   "<cross-domain-policy>\r\n" +
                   "<allow-access-from domain=\"*\" to-ports=\"1-31111\" />\r\n" +
                   "</cross-domain-policy>\x0"));
        }


        public void StartConnection()
        {
            if (_connection == null)
                return;

            PingCount = 0;

            (_connection.parser as InitialPacketParser).PolicyRequest += PolicyRequest;
            (_connection.parser as InitialPacketParser).SwitchParserRequest += SwitchParserRequest;
            _connection.startPacketProcessing();
        }

        public bool TryAuthenticate(string AuthTicket)
        {
            try
            {
				UserData userData = UserDataFactory.GetUserData(AuthTicket, out byte errorCode);
				if (errorCode == 1 || errorCode == 2)
                {
                    Disconnect();
                    return false;
                }

                #region Ban Checking
                //Let's have a quick search for a ban before we successfully authenticate..
                ModerationBan BanRecord = null;
                if (!string.IsNullOrEmpty(MachineId))
                {
                    if (CloudServer.GetGame().GetModerationManager().IsBanned(MachineId, out BanRecord))
                    {
                        if (CloudServer.GetGame().GetModerationManager().MachineBanCheck(MachineId))
                        {
                            Disconnect();
                            return false;
                        }
                    }
                }

                if (userData.user != null)
                {
                    //Now let us check for a username ban record..
                    BanRecord = null;
                    if (CloudServer.GetGame().GetModerationManager().IsBanned(userData.user.Username, out BanRecord))
                    {
                        if (CloudServer.GetGame().GetModerationManager().UsernameBanCheck(userData.user.Username))
                        {
                            Disconnect();
                            return false;
                        }
                    }
                }
                #endregion

                CloudServer.GetGame().GetClientManager().RegisterClient(this, userData.userID, userData.user.Username);
                _habbo = userData.user;
                _habbo.ssoTicket = AuthTicket;
                if (_habbo != null)
                {
                    userData.user.Init(this, userData);

                    SendMessage(new AuthenticationOKComposer());
                    SendMessage(new AvatarEffectsComposer(_habbo.Effects().GetAllEffects));
                    SendMessage(new NavigatorSettingsComposer(_habbo.HomeRoom));
                    SendMessage(new FavouritesComposer(userData.user.FavoriteRooms));
                    SendMessage(new FigureSetIdsComposer(_habbo.GetClothing().GetClothingParts));
                    SendMessage(new UserRightsComposer(_habbo));
                    SendMessage(new AvailabilityStatusComposer());
                    SendMessage(new AchievementScoreComposer(_habbo.GetStats().AchievementPoints));
                    SendMessage(new BuildersClubMembershipComposer());
                    SendMessage(new CfhTopicsInitComposer(CloudServer.GetGame().GetModerationManager().UserActionPresets));
                    SendMessage(new BadgeDefinitionsComposer(CloudServer.GetGame().GetAchievementManager()._achievements));
                    SendMessage(new SoundSettingsComposer(_habbo.ClientVolume, _habbo.ChatPreference, _habbo.AllowMessengerInvites, _habbo.FocusPreference, FriendBarStateUtility.GetInt(_habbo.FriendbarState)));

                    if (GetHabbo().GetMessenger() != null)
                        GetHabbo().GetMessenger().OnStatusChanged(true);

                    if (!string.IsNullOrEmpty(MachineId))
                    {
                        if (_habbo.MachineId != MachineId)
                        {
                            using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
                            {
                                dbClient.SetQuery("UPDATE `users` SET `machine_id` = @MachineId WHERE `id` = @id LIMIT 1");
                                dbClient.AddParameter("MachineId", MachineId);
                                dbClient.AddParameter("id", _habbo.Id);
                                dbClient.RunQuery();
                            }
                        }

                        _habbo.MachineId = MachineId;
                    }

					if (CloudServer.GetGame().GetPermissionManager().TryGetGroup(_habbo.Rank, out PermissionGroup PermissionGroup))
					{
						if (!String.IsNullOrEmpty(PermissionGroup.Badge))
							if (!_habbo.GetBadgeComponent().HasBadge(PermissionGroup.Badge))
								_habbo.GetBadgeComponent().GiveBadge(PermissionGroup.Badge, true, this);
					}

					if (!CloudServer.GetGame().GetCacheManager().ContainsUser(_habbo.Id))
                        CloudServer.GetGame().GetCacheManager().GenerateUser(_habbo.Id);

                    _habbo.InitProcess();

                    if (userData.user.GetPermissions().HasRight("mod_tickets"))
                    {
                        SendMessage(new ModeratorInitComposer(
                         CloudServer.GetGame().GetModerationManager().UserMessagePresets,
                         CloudServer.GetGame().GetModerationManager().RoomMessagePresets,
                         CloudServer.GetGame().GetModerationManager().GetTickets));
                    }

                    if (CloudServer.GetGame().GetSettingsManager().TryGetValue("user.login.message.enabled") == "1")
                        SendMessage(new MOTDNotificationComposer(CloudServer.GetGame().GetLanguageManager().TryGetValue("user.login.message")));

                    if (ExtraSettings.WELCOME_MESSAGE_ENABLED)
                    {
                        SendMessage(new MOTDNotificationComposer(ExtraSettings.WelcomeMessage.Replace("%username%", GetHabbo().Username)));
                    }


                   

                    if (ExtraSettings.TARGETED_OFFERS_ENABLED)
                    {
                        if (CloudServer.GetGame().GetTargetedOffersManager().TargetedOffer != null)
                        {
                            CloudServer.GetGame().GetTargetedOffersManager().Initialize(CloudServer.GetDatabaseManager().GetQueryReactor());
                            TargetedOffers TargetedOffer = CloudServer.GetGame().GetTargetedOffersManager().TargetedOffer;

                            if (TargetedOffer.Expire > CloudServer.GetIUnixTimestamp())
                            {

                                if (TargetedOffer.Limit != GetHabbo()._TargetedBuy)
                                {

                                    SendMessage(CloudServer.GetGame().GetTargetedOffersManager().TargetedOffer.Serialize());
                                }
                            }
                            else
                            {
                                using (var dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
                                    dbClient.runFastQuery("UPDATE targeted_offers SET active = 'false'");
                                using (var dbClient2 = CloudServer.GetDatabaseManager().GetQueryReactor())
                                    dbClient2.runFastQuery("UPDATE users SET targeted_buy = '0' WHERE targeted_buy > 0");
                            }
                        }
                    }

                    //SendMessage(new HCGiftsAlertComposer());

                    

                    /*var nuxStatuss = new ServerPacket(ServerPacketHeader.NuxSuggestFreeGiftsMessageComposer);
                    SendMessage(nuxStatuss);*/

                    CloudServer.GetGame().GetRewardManager().CheckRewards(this);
                    CloudServer.GetGame().GetAchievementManager().TryProgressHabboClubAchievements(this);
                    CloudServer.GetGame().GetAchievementManager().TryProgressRegistrationAchievements(this);
                    CloudServer.GetGame().GetAchievementManager().TryProgressLoginAchievements(this);
					ICollection<MessengerBuddy> Friends = new List<MessengerBuddy>();
					foreach (MessengerBuddy Buddy in GetHabbo().GetMessenger().GetFriends().ToList())
					{
						if (Buddy == null)
							continue;

						GameClient Friend = CloudServer.GetGame().GetClientManager().GetClientByUserID(Buddy.Id);
						if (Friend == null)
							continue;
						string figure = GetHabbo().Look;
				
					}
					return true;
				}
			}
			catch (Exception e)
			{
				ExceptionLogger.LogException(e);
			}
			return false;
		}

		public void SendWhisper(string Message, int Colour = 0)
        {
            if (GetHabbo() == null || GetHabbo().CurrentRoom == null)
                return;

            RoomUser User = GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(GetHabbo().Username);
            if (User == null)
                return;

            SendMessage(new WhisperComposer(User.VirtualId, Message, 0, (Colour == 0 ? User.LastBubble : Colour)));
        }

        public void SendNotification(string Message)
        {
            SendMessage(new BroadcastMessageAlertComposer(Message));
        }

        public void LogsNotif(string Message, string Key)
        {
            SendMessage(new RoomNotificationComposer(Message, Key));
        }

        public void SendMessage(IServerPacket Message)
        {
            byte[] bytes = Message.GetBytes();

            if (Message == null)
                return;

            if (GetConnection() == null)
                return;

            GetConnection().SendData(bytes);
        }

        public void SendShout(string Message, int Colour = 0)
        {
            if (this == null || GetHabbo() == null || GetHabbo().CurrentRoom == null)
                return;

            RoomUser User = GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(GetHabbo().Username);
            if (User == null)
                return;

            SendMessage(new ShoutComposer(User.VirtualId, Message, 0, (Colour == 0 ? User.LastBubble : Colour)));
        }


        public int ConnectionID
        {
            get { return _id; }
        }

        public ConnectionInformation GetConnection()
        {
            return _connection;
        }

        public Habbo GetHabbo()
        {
            return _habbo;
        }

        public void Disconnect()
        {
            try
            {
                if (GetHabbo() != null)
                {
                    ICollection<MessengerBuddy> Friends = new List<MessengerBuddy>();
                    foreach (MessengerBuddy Buddy in GetHabbo().GetMessenger().GetFriends().ToList())
                    {
                        if (Buddy == null)
                            continue;

                        GameClient Friend = CloudServer.GetGame().GetClientManager().GetClientByUserID(Buddy.Id);
                        if (Friend == null)
                            continue;
                        string figure = GetHabbo().Look;

                    }
                    using (IQueryAdapter dbClient = CloudServer.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.runFastQuery(GetHabbo().GetQueryString);
                    }

                    GetHabbo().OnDisconnect();
                    CloudServer.GetGame().GetClientManager().DispatchEventDisconnect(this);
                    
                }
            }
            catch (Exception e)
            {
                ExceptionLogger.LogException(e);
            }


            if (!_disconnected)
            {
                if (_connection != null)
                    _connection.Dispose();
                _disconnected = true;
            }
        }

        public void Dispose()
        {
            CurrentRoomUserId = -1;

            if (GetHabbo() != null)
                GetHabbo().OnDisconnect();

            MachineId = string.Empty;
            _disconnected = true;
            _habbo = null;
            _connection = null;
            RC4Client = null;
            _packetParser = null;
        }
    }
}