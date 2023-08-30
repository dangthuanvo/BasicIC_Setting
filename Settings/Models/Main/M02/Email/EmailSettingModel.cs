using Common.CustomAttributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace BasicIC_Setting.Models.Main.M02
{
    public class EmailSettingModel : BaseModel
    {
        [EmailValidation]
        public string address { get; set; }
        public string display_name { get; set; }
        public string pass { get; set; }
        public string smtp_host { get; set; }
        public Nullable<int> smtp_port { get; set; }
        [Required]
        public bool enable_ssl { get; set; }
        public Nullable<bool> is_active { get; set; }
    }
}