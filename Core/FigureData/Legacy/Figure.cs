namespace Cloud.Core.FigureData.Legacy
{
    class Figure
    {
        private string Part;
        private string PartId;
        public string Gender;
        private string Colorable;
        public Figure(string part, string partId, string gender, string colorable)
        {
            this.Part = part;
            this.PartId = partId;
            this.Gender = gender;
            this.Colorable = colorable;
        }
    }
}