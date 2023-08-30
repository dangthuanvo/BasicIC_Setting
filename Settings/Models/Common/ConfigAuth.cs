using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicIC_Setting.Models.Common
{
    public static class ConfigAuth
    {
        public static OpenIdConnectConfiguration openIdConfig { get; set; }
    }
}
