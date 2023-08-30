using BasicIC_Setting.Models.Main;
using System;

namespace BasicIC_Setting.Models.KafkaModels
{
    public class KafkaUserLogModel : BaseModel
    {
        public Nullable<bool> status { get; set; }
        public string log_type { get; set; }
        public string message { get; set; }
        public string exception { get; set; }
        public Nullable<Guid> interaction_id { get; set; }
        public string username_dest { get; set; }
        public string username_source { get; set; }
        public Nullable<int> duration { get; set; }
        public string object_name { get; set; }
        public string service { get; set; }
        public Nullable<Guid> reference_id { get; set; }
        public string original_data { get; set; }
        public string update_data { get; set; }
    }
}