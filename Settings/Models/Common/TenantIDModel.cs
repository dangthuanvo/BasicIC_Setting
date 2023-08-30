using System;
using System.ComponentModel.DataAnnotations;

namespace BasicIC_Setting.Models.Common
{
    public class TenantIDModel
    {
        [Required]
        public string tenant_id { get; set; }

        public TenantIDModel()
        {
            this.tenant_id = null;
        }

        public TenantIDModel(string id)
        {
            this.tenant_id = id;
        }

        public TenantIDModel(Guid id)
        {
            this.tenant_id = id.ToString();
        }
    }
}