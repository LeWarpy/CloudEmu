using Cloud.HabboHotel.Items;


namespace Cloud.HabboHotel.Catalog
{
	public class CatalogItemValue
	{
		public int Id
		{
			get;
			set;
		}

		public ItemData Base
		{
			get;
			set;
		}

		public int Amount
		{
			get;
			set;
		}

		public string ExtraData
		{
			get;
			set;
		}

		public int LimitedEditionStack
		{
			get;
			set;
		}

		public int LimitedEditionSells
		{
			get;
			set;
		}

		public CatalogItem Parent
		{
			get;
			set;
		}

		public bool IsLimited
		{
			get
			{
				return LimitedEditionStack > 0;
			}
		}

		public char Type
		{
			get
			{
				return Base.Type;
			}
		}

		public int SpriteId
		{
			get
			{
				return Base.SpriteId;
			}
		}

		public CatalogItemValue(CatalogItem Parent, int id, ItemData baseitem, int amount, string extradata, int limitedstack = 0, int limitedsells = 0)
		{
			Id = id;
			Base = baseitem;
			Amount = amount;
			ExtraData = extradata;
			LimitedEditionStack = limitedstack;
			LimitedEditionSells = limitedsells;
			this.Parent = Parent;
		}
	}
}
