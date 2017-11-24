﻿namespace Cloud.Core.FigureData.Types
{
    class Part
    {
        public int Id { get; set; }
        public SetType SetType { get; set; }
        public bool Colorable { get; set; }
        public int Index { get; set; }
        public int ColorIndex { get; set; }
        public Part(int id, SetType setType, bool colorable, int index, int colorIndex)
        {
            this.Id = id;
            this.SetType = setType;
            this.Colorable = colorable;
            this.Index = index;
            this.ColorIndex = colorIndex;
        }
    }
}