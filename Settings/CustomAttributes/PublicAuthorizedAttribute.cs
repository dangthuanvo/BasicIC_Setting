using Common;
using Common.Commons;
using Common.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using BasicIC_Setting.Common;
using BasicIC_Setting.Config;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace BasicIC_Setting.CustomAttributes
{
    public class PublicAuthorizedAttribute : FilterAttribute, IAuthorizationFilter
    {
        private static ILogger _logger = new Logger();

        public async Task<HttpResponseMessage> ExecuteAuthorizationFilterAsync(HttpActionContext actionContext, CancellationToken cancellationToken, Func<Task<HttpResponseMessage>> continuation)
        {
            if (await PublicAuthorize(actionContext))
            {
                return actionContext.Response ?? await continuation();
            }

            // handle unauthorized
            ResponseService<String> response = new ResponseService<string>("User is invalid!");
            return new HttpResponseMessage(HttpStatusCode.Unauthorized)
            {
                Content = new StringContent(JsonConvert.SerializeObject(response), Encoding.UTF8, "application/json")
            };
        }

        private async Task<bool> PublicAuthorize(HttpActionContext actionContext)
        {
            try
            {
                // Validate using public key OR secret key
                string public_key = null;
                string secret_key = null;
                try
                {
                    public_key = actionContext.Request.Headers.GetValues("PUBLIC_KEY").FirstOrDefault();
                }
                catch { }
                try
                {
                    secret_key = actionContext.Request.Headers.GetValues("API_SECRET_KEY").FirstOrDefault();
                }
                catch { }
                if ((public_key != null && public_key == ConfigManager.StaticGet("PUBLIC_KEY")) ||
                    (secret_key != null && secret_key == ConfigManager.StaticGet("API_SECRET_KEY")))
                {
                    string content = actionContext.Request.Content.ReadAsStringAsync().Result;
                    if (content != null)
                    {
                        try
                        {
                            JObject body = JsonConvert.DeserializeObject<JObject>(content);
                            if (body["tenant_id"] != null)
                            {
                                string tenant_id = body["tenant_id"].ToString();

                                if (tenant_id != null)
                                {
                                    SessionStore.Set(Constants.KEY_SESSION_TENANT_ID, tenant_id);
                                }
                            }
                        }
                        catch
                        {
                            SessionStore.Set(Constants.KEY_SESSION_TENANT_ID, null);
                        }
                    }

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return false;
            }
        }
    }
}