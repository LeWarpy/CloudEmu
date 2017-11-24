using System;

namespace Cloud.HabboHotel.Users.UserData
{
    public class UserDataNotFoundException : Exception
    {
        public UserDataNotFoundException(string reason)
            : base(reason)
        {
        }
    }
}