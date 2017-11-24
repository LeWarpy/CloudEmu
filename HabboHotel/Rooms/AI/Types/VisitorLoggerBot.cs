using System;
using System.Drawing;
using Cloud.HabboHotel.GameClients;
using System.Data;
using Cloud.HabboHotel.Rooms.AI.Speech;
using Cloud.Database.Interfaces;

namespace Cloud.HabboHotel.Rooms.AI.Types
{
    class VisitorLogger : BotAI
    {
        private int VirtualId;
        private static readonly Random Random = new Random();
        private int ActionTimer = 0;
        private int SpeechTimer = 0;

        public VisitorLogger(int VirtualId)
        {
            this.VirtualId = VirtualId;
        }

        public override void OnSelfEnterRoom()
        {
        }

        public override void OnSelfLeaveRoom(bool Kicked)
        {
        }

        public override void OnUserEnterRoom(RoomUser User)
        {
            if (GetBotData() == null)
                return;

            RoomUser Bot = GetRoomUser();

            if (User.GetClient().GetHabbo().CurrentRoom.OwnerId == User.GetClient().GetHabbo().Id)
            {
                DataTable getUsername;
                using (IQueryAdapter query = CloudServer.GetDatabaseManager().GetQueryReactor())
                {
                    query.SetQuery("SELECT username FROM room_visits WHERE roomid = @id");
                    query.AddParameter("id", User.RoomId);
                    getUsername = query.getTable();
                }

                foreach (DataRow Row in getUsername.Rows)
                {
                    Bot.Chat("¡Me alegro de verlo Señor! Diga 'Si', si desea saber quien ha visitado la sala en su ausencia.", false);
                    return;
                }
                Bot.Chat("He estado muy atento y te afirmo que nadie visitó esta sala mientras tú no estabas.", false);
            }
            else
            {
                Bot.Chat("Hola " + User.GetClient().GetHabbo().Username + ", le hablare de ti al dueño.", false);

                using (IQueryAdapter query = CloudServer.GetDatabaseManager().GetQueryReactor())
                {
                    query.SetQuery("INSERT INTO room_visits (roomid, username, gone) VALUE (@roomid, @username, @gone)");
                    query.AddParameter("roomid", User.RoomId);
                    query.AddParameter("username", User.GetClient().GetHabbo().Username);
                    query.AddParameter("gone", "todavía está en la sala.");
                    query.RunQuery();
                }
                return;
            }
        }


        public override void OnUserLeaveRoom(GameClient Client)
        {
            if (GetBotData() == null)
                return;

            RoomUser Bot = GetRoomUser();

            if (Client.GetHabbo().CurrentRoom.OwnerId == Client.GetHabbo().Id)
            {
                DataTable getRoom;

                using (IQueryAdapter query = CloudServer.GetDatabaseManager().GetQueryReactor())
                {
                    query.SetQuery("DELETE FROM room_visits WHERE roomid = @id");
                    query.AddParameter("id", Client.GetHabbo().CurrentRoom.RoomId);
                    getRoom = query.getTable();
                }
            }
            DataTable getUpdate;

            using (IQueryAdapter query = CloudServer.GetDatabaseManager().GetQueryReactor())
            {
                query.SetQuery("UPDATE room_visits SET gone = @gone WHERE roomid = @id AND username = @username");
                query.AddParameter("gone", "se ha ido");
                query.AddParameter("id", Client.GetHabbo().CurrentRoom.RoomId);
                query.AddParameter("username", Client.GetHabbo().Username);
                getUpdate = query.getTable();
            }
        }

        public override void OnUserSay(RoomUser User, string Message)
        {
            if (User == null || User.GetClient() == null || User.GetClient().GetHabbo() == null)
                return;

            if (Gamemap.TileDistance(GetRoomUser().X, GetRoomUser().Y, User.X, User.Y) > 8)
                return;

            switch (Message.ToLower())
            {
                case "si":
                case "yes":
                    if (GetBotData() == null)
                        return;

                    if (User.GetClient().GetHabbo().CurrentRoom.OwnerId == User.GetClient().GetHabbo().Id)
                    {
                        DataTable getRoomVisit;

                        using (IQueryAdapter query = CloudServer.GetDatabaseManager().GetQueryReactor())
                        {
                            query.SetQuery("SELECT username, gone FROM room_visits WHERE roomid = @id");
                            query.AddParameter("id", User.RoomId);
                            getRoomVisit = query.getTable();
                        }

                        foreach (DataRow Row in getRoomVisit.Rows)
                        {
                            var gone = Convert.ToString(Row["gone"]);
                            var username = Convert.ToString(Row["username"]);

                            GetRoomUser().Chat(username + " " + gone, false);
                        }
                        using (IQueryAdapter query = CloudServer.GetDatabaseManager().GetQueryReactor())
                        {
                            query.SetQuery("DELETE FROM room_visits WHERE roomid = @id");
                            query.AddParameter("id", User.RoomId);
                            getRoomVisit = query.getTable();
                        }
                        return;
                    }
                    break;
            }
        }

        public override void OnUserShout(RoomUser User, string Message)
        {
        }

        public override void OnTimerTick()
        {
            if (GetBotData() == null)
                return;

            if (SpeechTimer <= 0)
            {
                if (GetBotData().RandomSpeech.Count > 0)
                {
                    if (GetBotData().AutomaticChat == false)
                        return;

                    RandomSpeech Speech = GetBotData().GetRandomSpeech();

					string String = CloudServer.GetGame().GetChatManager().GetFilter().IsUnnaceptableWord(Speech.Message, out string word) ? "Spam" : Speech.Message;
					if (String.Contains("<img src") || String.Contains("<font ") || String.Contains("</font>") || String.Contains("</a>") || String.Contains("<i>"))
                        String = "I really shouldn't be using HTML within bot speeches.";
                    GetRoomUser().Chat(String, false, GetBotData().ChatBubble);
                }
                SpeechTimer = GetBotData().SpeakingInterval;
            }
            else
                SpeechTimer--;

            if (ActionTimer <= 0)
            {
                Point nextCoord;
                switch (GetBotData().WalkingMode.ToLower())
                {
                    default:
                    case "stand":
                        // (8) Why is my life so boring?
                        break;

                    case "freeroam":
                        if (GetBotData().ForcedMovement)
                        {
                            if (GetRoomUser().Coordinate == GetBotData().TargetCoordinate)
                            {
                                GetBotData().ForcedMovement = false;
                                GetBotData().TargetCoordinate = new Point();

                                GetRoomUser().MoveTo(GetBotData().TargetCoordinate.X, GetBotData().TargetCoordinate.Y);
                            }
                        }
                        else if (GetBotData().ForcedUserTargetMovement > 0)
                        {
                            RoomUser Target = GetRoom().GetRoomUserManager().GetRoomUserByHabbo(GetBotData().ForcedUserTargetMovement);
                            if (Target == null)
                            {
                                GetBotData().ForcedUserTargetMovement = 0;
                                GetRoomUser().ClearMovement(true);
                            }
                            else
                            {
                                var Sq = new Point(Target.X, Target.Y);

                                if (Target.RotBody == 0)
                                {
                                    Sq.Y--;
                                }
                                else if (Target.RotBody == 2)
                                {
                                    Sq.X++;
                                }
                                else if (Target.RotBody == 4)
                                {
                                    Sq.Y++;
                                }
                                else if (Target.RotBody == 6)
                                {
                                    Sq.X--;
                                }


                                GetRoomUser().MoveTo(Sq);
                            }
                        }
                        else if (GetBotData().TargetUser == 0)
                        {
                            nextCoord = GetRoom().GetGameMap().GetRandomWalkableSquare();
                            GetRoomUser().MoveTo(nextCoord.X, nextCoord.Y);
                        }
                        break;

                    case "specified_range":

                        break;
                }

                ActionTimer = new Random(DateTime.Now.Millisecond + this.VirtualId ^ 2).Next(5, 15);
            }
            else
                ActionTimer--;
        }
    }
}