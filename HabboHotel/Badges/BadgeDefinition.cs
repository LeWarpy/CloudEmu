namespace Cloud.HabboHotel.Badges
{
    public class BadgeDefinition
    {
        private string _code;
        private string _requiredRight;

        public BadgeDefinition(string Code, string RequiredRight)
        {
			_code = Code;
			_requiredRight = RequiredRight;
        }

        public string Code
        {
            get { return _code; }
            set { _code = value; }
        }

        public string RequiredRight
        {
            get { return _requiredRight; }
            set { _requiredRight = value; }
        }
    }
}