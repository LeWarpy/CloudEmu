namespace Cloud.HabboHotel.Permissions
{
    class Permission
    {
        public int Id { get; set; }
        public string PermissionName { get; set; }
        public string Description { get; set; }

        public Permission(int Id, string Name, string Description)
        {
            this.Id = Id;
            PermissionName = Name;
            this.Description = Description;
        }
    }
}
