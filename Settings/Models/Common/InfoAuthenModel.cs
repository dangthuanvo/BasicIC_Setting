namespace BasicIC_Setting.Models.Common
{
    public class InfoAuthenModel
    {
        public string username { get; set; }
        public string tenant_id { get; set; }
        public bool is_rootuser { get; set; }
        public string customer_type { get; set; }
    }
}