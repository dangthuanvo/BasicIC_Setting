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
    public class PrivateAuthorizedAttribute : FilterAttribute, IAuthorizationFilter
    {
        private static ILogger _logger = new Logger();

        public async Task<HttpResponseMessage> ExecuteAuthorizationFilterAsync(HttpActionContext actionContext, CancellationToken cancellationToken, Func<Task<HttpResponseMessage>> continuation)
        {
            if (Authorize(actionContext))
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

        private bool Authorize(HttpActionContext actionContext)
        {
            try
            {
                // Validate using only secret key
                string secret_key = null;
                try
                {
                    secret_key = actionContext.Request.Headers.GetValues("API_SECRET_KEY").FirstOrDefault();
                }
                catch { }
                if (secret_key != null && secret_key == ConfigManager.StaticGet("API_SECRET_KEY"))
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