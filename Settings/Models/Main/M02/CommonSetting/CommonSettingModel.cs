using Common.CustomAttributes;

namespace BasicIC_Setting.Models.Main.M02
{
    public class CommonSettingModel : DefaultBaseModel
    {
        public string key { get; set; }
        public string value { get; set; }
        [OneOf(new string[] { "string", "number", "boolean", "password" })]
        public string common_type { get; set; }
        [OneOf(new string[] { "common", "auto_service", "size_file", "email_service", "social_config" })]
        public string setting_for { get; set; }
    }
}