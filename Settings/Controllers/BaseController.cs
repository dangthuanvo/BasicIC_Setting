using BasicIC_Setting.Common;
using Common.Commons;
using System.Threading.Tasks;
using System.Web.Http;

namespace BasicIC_Setting.Controllers
{
    public class BaseController : ApiController
    {
        public BaseController() : base() { }
    }

    [RoutePrefix("api/consult")]
    public class ConsultController : ApiController
    {
        public ConsultController() : base() { }
        [Route("health-check")]
        [HttpGet]
        public IHttpActionResult HealthCheck()
        {
            return Ok();
        }
        [Route("register")]
        [HttpPost]
        public async Task<IHttpActionResult> Register()
        {
            ResponseService<bool> response = await ConsultClient.RegisterService();
            if (response.status)
            {
                return Ok(response);
            }
            else
            {
                return new ResponseFail<bool>().Error(response);
            }
        }

        [Route("unregister")]
        [HttpPost]
        public async Task<IHttpActionResult> UnRegister()
        {
            ResponseService<bool> response = await ConsultClient.UnRegisterService();
            if (response.status)
            {
                return Ok(response);
            }
            else
            {
                return new ResponseFail<bool>().Error(response);
            }
        }
    }

}