using System.ComponentModel.DataAnnotations;

namespace Common.Params.Base
{
    public class ReportCampaignTaskSummaryParam
    {
        [Required]
        public string secret_key { get; set; }
        [Required]
        public string tenant_id { get; set; }
    }

}
