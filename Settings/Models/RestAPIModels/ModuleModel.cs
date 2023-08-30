using BasicIC_Setting.Models.Main;
using System;

namespace BasicIC_Setting.Models.RestAPIModels
{
    public class ModuleModel : BaseModel
    {
        public string module_name { get; set; }
        public string display_name { get; set; }
        public Nullable<int> position { get; set; }
        public string description { get; set; }
        public bool is_active { get; set; }
    }
}