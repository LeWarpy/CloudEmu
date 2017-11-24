namespace Cloud.HabboHotel.Navigator
{
    public class FeaturedRoom
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public string Caption { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public int CategoryId { get; set; }

        public FeaturedRoom(int id, int roomId, string caption, string description, string image, int categoryId)
        {
            this.Id = id;
            this.RoomId = roomId;
            this.Caption = caption;
            this.Description = description;
            this.Image = image;
            this.CategoryId = categoryId;
        }
    }
}