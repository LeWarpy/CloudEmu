using System.Collections.Generic;
using Cloud.HabboHotel.Catalog.PredesignedRooms;

namespace Cloud.HabboHotel.Catalog
{
    public class CatalogPage
    {
        private int _id;
        private int _parentId;
        private string _caption;
        private string _pageLink;
        private int _icon;
        private int _minRank;
        private int _minVIP;
        private bool _visible;
        private bool _enabled;
        private string _template;

        private List<string> _pageStrings1;
        private List<string> _pageStrings2;

        private Dictionary<int, CatalogItem> _items;
        private Dictionary<int, CatalogDeal> _deals;
        private Dictionary<int, CatalogItem> _itemOffers;
        private PredesignedContent _predesignedItems;

        public CatalogPage(int Id, int ParentId, string Enabled, string Caption, string PageLink, int Icon, int MinRank, int MinVIP,
              string Visible, string Template, string PageStrings1, string PageStrings2, Dictionary<int, CatalogItem> Items,
              Dictionary<int, CatalogDeal> Deals, PredesignedContent PredesignedItems,
              ref Dictionary<int, int> flatOffers)
        {
            _id = Id;
            _parentId = ParentId;
            _enabled = Enabled.ToLower() == "1" ? true : false;
            _caption = Caption;
            _pageLink = PageLink;
            _icon = Icon;
            _minRank = MinRank;
            _minVIP = MinVIP;
            _visible = Visible.ToLower() == "1" ? true : false;
            _template = Template;

            foreach (string Str in PageStrings1.Split('|'))
            {
                if (_pageStrings1 == null) { _pageStrings1 = new List<string>(); }
                _pageStrings1.Add(Str);
            }

            foreach (string Str in PageStrings2.Split('|'))
            {
                if (_pageStrings2 == null) { _pageStrings2 = new List<string>(); }
                _pageStrings2.Add(Str);
            }

            _items = Items;
            _deals = Deals;
            _predesignedItems = PredesignedItems;

            _itemOffers = new Dictionary<int, CatalogItem>();
            foreach (int i in flatOffers.Keys)
            {
                if (flatOffers[i] == Id)
                {
                    foreach (CatalogItem item in _items.Values)
                    {
                        if (item.OfferId == i)
                        {
                            if (!_itemOffers.ContainsKey(i))
                                _itemOffers.Add(i, item);
                        }
                    }
                }
            }
        }

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public int ParentId
        {
            get { return _parentId; }
            set { _parentId = value; }
        }

        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

        public string Caption
        {
            get { return _caption; }
            set { _caption = value; }
        }

        public string PageLink
        {
            get { return _pageLink; }
            set { _pageLink = value; }
        }

        public int Icon
        {
            get { return _icon; }
            set { _icon = value; }
        }

        public int MinimumRank
        {
            get { return _minRank; }
            set { _minRank = value; }
        }

        public int MinimumVIP
        {
            get { return _minVIP; }
            set { _minVIP = value; }
        }

        public bool Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }

        public string Template
        {
            get { return _template; }
            set { _template = value; }
        }

        public List<string> PageStrings1
        {
            get { return _pageStrings1; }
            private set { _pageStrings1 = value; }
        }

        public List<string> PageStrings2
        {
            get { return _pageStrings2; }
            private set { _pageStrings2 = value; }
        }

        public Dictionary<int, CatalogDeal> Deals
        {
            get { return _deals; }
            private set { _deals = value; }
        }

        public PredesignedContent PredesignedItems
        {
            get { return _predesignedItems; }
            private set { _predesignedItems = value; }
        }

        public Dictionary<int, CatalogItem> Items
        {
            get { return _items; }
            private set { _items = value; }
        }

        public Dictionary<int, CatalogItem> ItemOffers
        {
            get { return _itemOffers; }
            private set { _itemOffers = value; }
        }

        public CatalogItem GetItem(int pId)
        {
            if (_items.ContainsKey(pId))
                return (CatalogItem)_items[pId];
            return null;
        }
    }
}