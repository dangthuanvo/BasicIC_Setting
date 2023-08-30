namespace BasicIC_Setting.Models.Main
{
    public class PermissionRequest
    {
        public string username { get; set; }
        public string permission_name { get; set; }
        public string permission_type { get; set; }

        public PermissionRequest(string username, string permission_name, string permission_type)
        {
            this.username = username;
            this.permission_name = permission_name;
            this.permission_type = permission_type;
        }
    }
}