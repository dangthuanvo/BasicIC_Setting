using BasicIC_Setting.Common;
using BasicIC_Setting.CustomAttributes;
using BasicIC_Setting.Models.Main.M02;
using BasicIC_Setting.Services.Interfaces;
using Common.Commons;
using System.Threading.Tasks;
using System.Web.Http;

namespace BasicIC_Setting.Controllers
{
    [RoutePrefix("api/email-setting")]
    public class EmailSettingController : BaseController
    {
        private readonly IEmailSettingService _emailSettingService;

        public EmailSettingController(IEmailSettingService emailSettingService)
        {
            _emailSettingService = emailSettingService;
        }

        /// <summary>
        /// Type: API method
        /// Description: API for client to get email setting
        /// Request point: Client
        /// Authen: Yes, Bearer Token
        /// API document: https://docs.google.com/document/d/1xAZ6fG8ApHsajBu_ekNdcEghomp-ezqVqw3r13dlWXA/
        /// Owner: trint
        /// Create log:     15.11.2022 - trint        
        /// </summary>        
        /// <returns></returns>
        //[PermissionAttributeFilter("Email Setting", "access")]
        [Route("get")]
        [HttpPost]
        public async Task<IHttpActionResult> Get()
        {
            ResponseService<EmailSettingModel> response = await _emailSettingService.Get();
            if (response.status)
                return Ok(response);

            return new ResponseFail<EmailSettingModel>().Error(response);
        }

        /// <summary>
        /// Type: API method
        /// Description: API for client to create email setting
        /// Request point: Client
        /// Authen: Yes, Bearer Token
        /// API document: https://docs.google.com/document/d/1xAZ6fG8ApHsajBu_ekNdcEghomp-ezqVqw3r13dlWXA/
        /// Owner: trint
        /// Create log:     15.11.2022 - trint        
        /// </summary>        
        /// <param name="param"></param>
        /// <returns></returns>
        [PermissionAttributeFilter("Email Setting", "create")]
        [Route("create")]
        [HttpPost]
        public async Task<IHttpActionResult> Add(EmailSettingModel param)
        {
            ResponseService<EmailSettingModel> response = await _emailSettingService.Create(param);
            if (response.status)
                return Ok(response);

            return new ResponseFail<EmailSettingModel>().Error(response);
        }

        /// <summary>
        /// Type: API method
        /// Description: API for client to update email setting
        /// Request point: Client
        /// Authen: Yes, Bearer Token
        /// API document: https://docs.google.com/document/d/1xAZ6fG8ApHsajBu_ekNdcEghomp-ezqVqw3r13dlWXA/
        /// Owner: trint
        /// Create log:     15.11.2022 - trint        
        /// </summary>        
        /// <param name="param"></param>
        /// <returns></returns>
        [PermissionAttributeFilter("Email Setting", "edit")]
        [Route("update")]
        [ValidateModel]
        [HttpPost]
        public async Task<IHttpActionResult> Update(EmailSettingModel param)
        {
            ResponseService<EmailSettingModel> response = await _emailSettingService.Update(param);
            if (response.status)
                return Ok(response);

            return new ResponseFail<EmailSettingModel>().Error(response);
        }

        /// <summary>
        /// Type: API method
        /// Description: API for client to delete email setting
        /// Request point: Client
        /// Authen: Yes, Bearer Token
        /// API document: https://docs.google.com/document/d/1xAZ6fG8ApHsajBu_ekNdcEghomp-ezqVqw3r13dlWXA/
        /// Owner: trint
        /// Create log:     15.11.2022 - trint        
        /// </summary>        
        /// <returns></returns>
        [PermissionAttributeFilter("Email Setting", "delete")]
        [Route("delete")]
        [ValidateModel]
        [HttpPost]
        public async Task<IHttpActionResult> Remove()
        {
            ResponseService<bool> response = await _emailSettingService.Delete();
            if (response.status)
                return Ok(response);

            return new ResponseFail<bool>().Error(response);
        }
    }
}
