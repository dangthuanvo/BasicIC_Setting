using Common;
using Common.Commons;
using Newtonsoft.Json;
using BasicIC_Setting.Common;
using BasicIC_Setting.Config;
using BasicIC_Setting.Models.Main;
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
    public class PermissionAttributeFilter : FilterAttribute, IActionFilter
    {
        private string permisson_name = "";
        private string permission_type = "";
        private readonly string PATH_PRE_API = "user/api/permission/get-status-permission-by-type-and-name/";

        public PermissionAttributeFilter(string permisson_name, string permission_type)
        {
            this.permisson_name = permisson_name;
            this.permission_type = permission_type;
        }

        public async Task<HttpResponseMessage> ExecuteActionFilterAsync(HttpActionContext actionContext, CancellationToken cancellationToken, Func<Task<HttpResponseMessage>> continuation)
        {
            var result = await GetStatusPermissionTypeAObjectByUser(SessionStore.Get(Constants.KEY_SESSION_EMAIL), permisson_name, permission_type, actionContext);
            if (!result)
            {
                ResponseService<String> response = new ResponseService<string>("Access denied");
                return new HttpResponseMessage(HttpStatusCode.Forbidden)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(response), Encoding.UTF8, "application/json")
                };
            }
            return actionContext.Response ?? await continuation();
        }

        // lấy quyền của người dùng ở 1 loại quyền cụ thể của 1 đối tượng phân quyền.
        public virtual async Task<bool> GetStatusPermissionTypeAObjectByUser(string username, string permission_name, string permission_type, HttpActionContext actionContext)
        {
            bool response = false;
            BaseResponse<bool> resPermission = new BaseResponse<bool>();
            RequestAPI _requestAPI = new RequestAPI(Constants.CONF_HOST_FABIO_SERVICE, SessionStore.Get(Constants.KEY_SESSION_TOKEN));
            PermissionRequest permission = new PermissionRequest(username, permission_name, permission_type);
            var result = await _requestAPI.client.PostAsJsonAsync(PATH_PRE_API, permission);
            if (result.IsSuccessStatusCode)
            {
                resPermission = await result.Content.ReadAsAsync<BaseResponse<bool>>();
                response = resPermission.data;

                return response;
            }
            else
            {
                string secret_key = actionContext.Request.Headers.GetValues("API_SECRET_KEY").FirstOrDefault();
                if (secret_key != null && secret_key == ConfigManager.StaticGet("API_SECRET_KEY"))
                {
                    return true;
                }
            }

            return false;
        }
    }
}