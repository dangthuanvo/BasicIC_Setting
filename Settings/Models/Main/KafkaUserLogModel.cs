using System;

namespace BasicIC_Setting.Models.Main
{
    public class KafkaLogActionModel : BaseModel
    {
        public string service { get; set; }
        public string log_type { get; set; }
        public string module { get; set; }
        public string description { get; set; }
        public Nullable<bool> status { get; set; }
        public string original_data { get; set; }
        public string update_data { get; set; }
    }
}