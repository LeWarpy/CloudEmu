namespace Cloud.HabboHotel.Catalog.Pets
{
    public class PetRace
    {
        private int _raceId;
        private int _primaryColour;
        private int _secondaryColour;
        public bool _hasPrimaryColour;
        public bool _hasSecondaryColour;

        public PetRace(int RaceId, int PrimaryColour, int SecondaryColour, bool HasPrimaryColour, bool HasSecondaryColour)
        {
            _raceId = RaceId;
            _primaryColour = PrimaryColour;
            _secondaryColour = SecondaryColour;
            _hasPrimaryColour = HasPrimaryColour;
            _hasSecondaryColour = HasSecondaryColour;
        }

        public int RaceId
        {
            get { return _raceId; }
            set { _raceId = value; }
        }

        public int PrimaryColour
        {
            get { return _primaryColour; }
            set { _primaryColour = value; }
        }

        public int SecondaryColour
        {
            get { return _secondaryColour; }
            set { _secondaryColour = value; }
        }

        public bool HasPrimaryColour
        {
            get { return _hasPrimaryColour; }
            set { _hasPrimaryColour = value; }
        }

        public bool HasSecondaryColour
        {
            get { return _hasSecondaryColour; }
            set { _hasSecondaryColour = value; }
        }
    }
}
