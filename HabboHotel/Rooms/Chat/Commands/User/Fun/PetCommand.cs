using Cloud.Communication.Packets.Outgoing.Rooms.Engine;
using System.Text;
using Cloud.Communication.Packets.Outgoing.Notifications;

namespace Cloud.HabboHotel.Rooms.Chat.Commands.User.Fun 
{
    class PetCommand : IChatCommand
    {
        public string PermissionRequired => "command_pet";
        public string Parameters => "";
        public string Description => "Se transforma em um PET.";

        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)
        {
            

            RoomUser RoomUser = Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (RoomUser == null)
                return;


            if (Params.Length == 1)
            {
                StringBuilder List = new StringBuilder("");
                Session.SendMessage(new MOTDNotificationComposer("Lista de Mascotes disponiveis! "+CloudServer.HotelName+"\n" +
                "···········································\n" +
                ":pet habbo » Volta ao normal\n" +
                "····································\n" +
                ":pet dog » Transformar-se em um Cachorro\n" +
                "····································\n" +
                ":pet cat » Transformar-se em um Gato \n" +
                "····································\n" +
                ":pet terrier » Transformar-se em um Cachorro fofo\n" +
                "····································\n" +
				":pet croc » Transformar-se em um Crocodilo\n" +
                "····································\n" +
				":pet bear » Transformar-se em um Urso\n" +
                "····································\n" +
				":pet pig » Transformar-se em um Porco\n" +
                "····································\n" +
				":pet lion » Transformar-se em um Leão\n" +
                "····································\n" +
				":pet rhino » Transformar-se em um Rinoceronte\n" +
                "····································\n" +
				":pet spider » Transformar-se em uma Aranha\n" +
                "····································\n" +
				":pet turtle » Transformar-se em uma Tartaruga\n" +
                "····································\n" +
				":pet chick » Transformar-se em uma Galinha\n" +
                "····································\n" +
				":pet frog » Transformar-se em um Sapo\n" +
                "····································\n" +
				":pet drag » Transformar-se em um Dragão\n" +
                "····································\n" +
				":pet monkey » Transformar-se em um Macaco\n" +
                "····································\n" +
				":pet horse » Transformar-se em um Cavalo\n" +
                "····································\n" +
				":pet bunny » Transformar-se em um Coelinho\n" +
                "····································\n" +
				":pet pigeon » Transformar-se em um Pintinho\n" +
                "····································\n" +
				":pet demon » Transformar-se em um Satánas\n" +
                "····································\n" +
				":pet gnome » Transformar-se em um Gnomo\n" +
                "····································\n" +
				":pet raptor » Transformar-se em um Dinossauro\n" +
                "····································\n" +
				":pet pterodactyl » Transformar-se em um Pterodáctilo\n" +
                "····································\n" +
				":pet elefante » Transformar-se em um Elefante\n" +
                "····································\n" +
				":pet supermario » Transformar-se em um SuperMario\n" +
                "····································\n" +
				":pet lobo » Transformar-se em um Lobo\n" +
                "····································\n" +
				":pet pikachu »Transformar-se em um Pikachu\n"));
                return;
            }

            int TargetPetId = GetPetIdByString(Params[1].ToString());
            if (TargetPetId == 0)
            {
                Session.SendWhisper("Ops!Não existe um pet com esse nome!");
                return;
            }

            //Change the users Pet Id.
            Session.GetHabbo().PetId = (TargetPetId == -1 ? 0 : TargetPetId);

            //Quickly remove the old user instance.
            Room.SendMessage(new UserRemoveComposer(RoomUser.VirtualId));

            //Add the new one, they won't even notice a thing!!11 8-)
            Room.SendMessage(new UsersComposer(RoomUser));

            //Tell them a quick message.
            if (Session.GetHabbo().PetId > 0)
                Session.SendWhisper("Usar ':pet habbo' para voltar ao normal!");
        }

        private int GetPetIdByString(string Pet)
        {
            switch (Pet.ToLower())
            {
                default:
                    return 0;
                case "habbo":
                    return -1;
                case "perro":
                    return 60;//This should be 0.
                case "gato":
                case "1":
                    return 1;
                case "terrier":
                case "2":
                    return 2;
                case "croc":
                case "croco":
                case "3":
                    return 3;
                case "oso":
                case "4":
                    return 4;
                case "liz":
                case "cerdo":
                case "kill":
                case "5":
                    return 5;
                case "leon":
                case "rawr":
                case "6":
                    return 6;
                case "rhino":
                case "7":
                    return 7;
                case "spider":
                case "arana":
                case "araña":
                case "8":
                    return 8;
                case "tortuga":
                case "9":
                    return 9;
                case "chick":
                case "chicken":
                case "pollo":
                case "10":
                    return 10;
                case "frog":
                case "rana":
                case "11":
                    return 11;
                case "drag":
                case "dragon":
                case "12":
                    return 12;
                case "monkey":
                case "mono":
                case "14":
                    return 14;
                case "horse":
                case "caballo":
                case "15":
                    return 15;
                case "bunny":
                case "conejo":
                case "17":
                    return 17;
                case "pigeon":
                case "pajaro":
                case "21":
                    return 21;
                case "demon":
                case "demonio":
                case "23":
                    return 23;
                case "babybear":
                case "bebeoso":
                case "24":
                    return 24;
                case "babyterrier":
                case "bebeterrier":
                case "25":
                    return 25;
                case "gnome":
                case "gnomo":
                case "26":
                    return 26;
                case "kitten":
                case "gatito":
                case "28":
                    return 28;
                case "puppy":
                case "perrito":
                case "29":
                    return 29;
                case "piglet":
                case "cerdito":
                case "30":
                    return 30;
                case "haloompa":
                case "31":
                    return 31;
                case "rock":
                case "piedra":
                case "32":
                    return 32;
                case "pterodactyl":
                case "pterosaur":
                case "33":
                    return 33;
                case "raptor":
                case "velociraptor":
                case "34":
                    return 34;
                case "vaca":
                case "35":
                    return 35;
                case "pinguino":
                case "36":
                    return 36;
                case "elefante":
                case "37":
                    return 37;
                case "bebeguapo":
                case "38":
                    return 38;
                case "bebefeo":
                case "39":
                    return 39;
                case "supermario":
                case "mario":
                case "40":
                    return 40;
                case "pikachu":
                case "41":
                    return 41;
                case "lobo":
                case "42":
                    return 42;
            }
        }
    }
}