using Cloud.Communication.Packets.Outgoing.Rooms.Poll;
using Cloud.HabboHotel.GameClients;
using Cloud.HabboHotel.Items;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cloud.HabboHotel.Rooms.Chat.Commands.Moderator
{
    internal class IdolQuizCommand : IChatCommand
    {

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length == 0)
            {
                Session.SendWhisper("Por favor introduce la pregunta.");
            }
            else
            {

                string question = "< Voto negativo [ " + Params[1] + " ] Voto positivo >";
                if (Params[1] == "end")
                {
                    Item[] ReloadItems = Room.GetRoomItemHandler().GetFloor.ToArray();
                    foreach (Item Chair in ReloadItems.ToList())
                    {
                        //if (Chair == null || Room.GetRoomItemHandler().GetFloor.Contains(Chair))
                        //    continue;
                        if (Chair.GetBaseItem().InteractionType == InteractionType.idol_chair)
                        {
                            Chair.ExtraData = "0";
                            Chair.UpdateState();
                        }
                        if (Chair.GetBaseItem().InteractionType == InteractionType.idol_counter)
                        {
                            Chair.ExtraData = "0";
                            Chair.UpdateState();
                        }

                        Room.EndQuestion();
                    }
                }
                else
                {

                    Item[] Items = Room.GetRoomItemHandler().GetFloor.ToArray();
                    foreach (Item Chair in Items.ToList())
                    {
                        if (Chair.GetBaseItem().InteractionType == InteractionType.idol_chair)
                        {

                            bool HasUsers = false;

                            if (Room.GetGameMap().SquareHasUsers(Chair.GetX, Chair.GetY))
                                HasUsers = true;

                            if (!HasUsers)
                            {
                                Session.SendWhisper("No hay ningún juez en la silla de juzgado.");
                                return;
                            }
                            CloudServer.GetGame().GetClientManager().QuizzAlert(new QuickPollMessageComposer(question), Chair, Room);
                            Room.SetPoolQuestion(question);
                            Room.ClearPoolAnswers();
                            Console.WriteLine(question);
                        }
                    }
                }
            }
        }



        public string Description =>
			"Uma pesquisa rápida.";

        public string Parameters =>
            "[USUARIO]";

        public string PermissionRequired =>
            "command_give_badge";
    }
}