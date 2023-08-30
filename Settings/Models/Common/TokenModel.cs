
using System.Security.Claims;

namespace BasicIC_Setting.Models.Common
{
    public class GetPrincipalModel
    {
        public ClaimsPrincipal claimsPrincipal { get; set; }
        public string signatureAlgorithm { get; set; }

        public GetPrincipalModel(ClaimsPrincipal claimsPrincipal, string signatureAlgorithm)
        {
            this.claimsPrincipal = claimsPrincipal;
            this.signatureAlgorithm = signatureAlgorithm;
        }
    }

    public class ClaimModel
    {
        public string user_name { get; set; }
        public string tenant_id { get; set; }
        public string is_administrator { get; set; }
        public string is_rootuser { get; set; }
        public string is_supervisor { get; set; }
        public string is_agent { get; set; }
        public string customer_type { get; set; }
        public string is_third_party { get; set; }
    }
}
