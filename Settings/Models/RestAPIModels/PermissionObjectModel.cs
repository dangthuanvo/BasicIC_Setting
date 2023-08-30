using BasicIC_Setting.Models.Main;

namespace BasicIC_Setting.Models.RestAPIModels
{
    public class PermissionObjectModel : BaseModel
    {
        public string object_name { get; set; }
        public string module_id { get; set; }
        public string description { get; set; }
        public bool is_active { get; set; }
    }
}