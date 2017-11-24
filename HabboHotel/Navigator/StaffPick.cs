using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cloud.HabboHotel.Navigator
{
    public class StaffPick
    {
        public int RoomId { get; set; }
        public string Image { get; set; }

        public StaffPick(int roomId, string image)
        {
            this.RoomId = roomId;
            this.Image = image;
        }
    }
}
