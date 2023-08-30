using BasicIC_Setting.Models.Main;
using Common.CustomAttributes;

namespace Settings.Models.Main.M02
{
    public class DefaultCommonSettingModel : DefaultBaseModel
    {
        public string key { get; set; }
        public string value { get; set; }
        [OneOf(new string[] { "string", "number", "boolean", "password" })]
        public string common_type { get; set; }
        [OneOf(new string[] { "email_service" })]
        public string setting_for { get; set; }
    }
}