using System;

namespace BasicIC_Setting.Models.RestAPIModels
{
    public class UserModel
    {
        public string username { get; set; }
        public string fullname { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string description { get; set; }
        public Guid role_id { get; set; }
        public bool? is_administrator { get; set; }
        public bool? is_rootuser { get; set; }
        public bool? is_active { get; set; }
        public DateTime? create_time { get; set; }
        public string create_by { get; set; }
        public DateTime? modify_time { get; set; }
        public string modify_by { get; set; }
        public Guid? tenant_id { get; set; }
        public string report_to { get; set; }
    }
}