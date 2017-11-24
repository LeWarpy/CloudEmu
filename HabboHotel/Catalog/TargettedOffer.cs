using System;
using System.Collections.Generic;
using Cloud.Database.Interfaces;
using Cloud.Communication.Packets.Outgoing;

namespace Cloud.HabboHotel.Catalog
{
    internal class TargetedOffersManager
     {
        
        internal TargetedOffers TargetedOffer;
        internal void Initialize(IQueryAdapter dbClient)
        {
            TargetedOffer = null;

            dbClient.SetQuery("SELECT * FROM targeted_offers WHERE active = 'true' LIMIT 1;");
            var row = dbClient.getRow();
           

            if (row == null)
                return;
            TargetedOffer = new TargetedOffers((int)row["id"], (int)row["limit"], Convert.ToInt32(CloudServer.GetUnixTimestamp() +
                (((int)row["time"] + 10) * 1)), (row["open"].ToString() == "show"), (row["active"].ToString() == "true"), (string)row["code"],
                (string)row["title"], (string)row["description"], (string)row["image"], (string)row["icon"],
                (string)row["money_type"], (string)row["items"], (string)row["price"]);
        }
    }

    internal class TargetedOffers
    {
        internal int Id, Limit, Time, Expire;
        internal bool Open, Active;
        internal string Code, Title, Description, Image, Icon, MoneyType;
        internal string[] Items, Price;
        internal List<TargetedItems> Products;

        internal TargetedOffers(int id, int limit, int time, bool open, bool active, string code, string title,
            string description, string image, string icon, string moneyType, string items, string price)
        {
            Id = id;
            Limit = limit;
            Time = time - CloudServer.GetIUnixTimestamp();
            Open = open;
            Active = active;
            Code = code;
            Title = title;
            Description = description;
            Image = image;
            Icon = icon;
            MoneyType = moneyType;
            Items = items.Split(';');
            Price = price.Split(';');
            Expire = time;

            Products = new List<TargetedItems>();
            foreach (var item in Items)
            {
                var itemType = item.Split(',')[0];
                var itemProduct = item.Split(',')[1];
                Products.Add(new TargetedItems(Id, itemType, itemProduct));
            }
        }

        internal int MoneyCode(string moneyType)
        {
            switch (moneyType)
            {
                case "duckets":
                    return 0;

                case "diamonds":
                    return 5;

                default:
                    return 0;
            }
        }

        internal ServerPacket Serialize()
        {
            var message = new ServerPacket(ServerPacketHeader.TargetOfferMessageComposer);
            message.WriteInteger(Open ? 4 : 1);
            message.WriteInteger(Id);
            message.WriteString(Code);
            message.WriteString(Code);
            message.WriteInteger(int.Parse(Price[0]));
            message.WriteInteger(int.Parse(Price[1]));
            message.WriteInteger(MoneyCode(MoneyType));
            message.WriteInteger(Limit);
            message.WriteInteger(Time);
            message.WriteString(Title);
            message.WriteString(Description);
            message.WriteString(Image);
            message.WriteString(Icon);
            message.WriteInteger(0);
            message.WriteInteger(Products.Count);
            foreach (var product in Products) message.WriteString(string.Empty);
            return message;
        }
    }

    internal class TargetedItems
    {
        internal int TargetedId;
        internal string ItemType, Item;

        internal TargetedItems(int targetedId, string itemType, string item)
        {
            TargetedId = targetedId;
            ItemType = itemType;
            Item = item;
        }
    }
}
