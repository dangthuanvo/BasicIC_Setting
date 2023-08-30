using BasicIC_Setting.Common;
using BasicIC_Setting.Models.Common;
using BasicIC_Setting.Services.Interfaces;
using Common.Commons;
using Common.Params.Base;
using Repository.CustomModel;
using Settings.Models.Main.M02;
using System.Threading.Tasks;
using System.Web.Http;

namespace BasicIC_Setting.Controllers
{
    [RoutePrefix("api/default-common-setting")]
    public class DefaultCommonSettingController : BaseController
    {
        private readonly IDefaultCommonSettingService _defaultCommonSettingService;

        public DefaultCommonSettingController(IDefaultCommonSettingService defaultCommonSettingService)
        {
            _defaultCommonSettingService = defaultCommonSettingService;
        }
        [Route("test")]
        [HttpPost]
        public async Task<bool> Test()
        {
            bool response = await _defaultCommonSettingService.Test();
            return true;
        }
        /// <summary>
        /// Type: API method
        /// Description: API for client to get default common setting (all)
        /// Request point: Client
        /// Authen: Yes, Bearer Token
        /// API document: https://docs.google.com/document/d/1xAZ6fG8ApHsajBu_ekNdcEghomp-ezqVqw3r13dlWXA/
        /// Owner: trint
        /// Create log:     15.11.2022 - trint        
        /// </summary>        
        /// <param name="param"></param>
        /// <returns></returns>
        [Route("get-all")]
        [HttpPost]
        public async Task<IHttpActionResult> GetAll(PagingParam param)
        {
            ResponseService<ListResult<DefaultCommonSettingModel>> response = await _defaultCommonSettingService.GetAllDecrypted(param);
            if (response.status)
                return Ok(response);

            return new ResponseFail<ListResult<DefaultCommonSettingModel>>().Error(response);
        }

        /// <summary>
        /// Type: API method
        /// Description: API for client to create default common setting
        /// Request point: Client
        /// Authen: Yes, Bearer Token
        /// API document: https://docs.google.com/document/d/1xAZ6fG8ApHsajBu_ekNdcEghomp-ezqVqw3r13dlWXA/
        /// Owner: trint
        /// Create log:     15.11.2022 - trint        
        /// </summary>        
        /// <param name="param"></param>
        /// <returns></returns>
        [Route("create")]
        [HttpPost]
        public async Task<IHttpActionResult> Add(DefaultCommonSettingModel param)
        {
            ResponseService<DefaultCommonSettingModel> response = await _defaultCommonSettingService.CreateCheckDuplicate(param);
            if (response.status)
                return Ok(response);

            return new ResponseFail<DefaultCommonSettingModel>().Error(response);
        }

        /// <summary>
        /// Type: API method
        /// Description: API for client to create default common setting (all tenants)
        /// Request point: Client
        /// Authen: Yes, Bearer Token
        /// API document: https://docs.google.com/document/d/1xAZ6fG8ApHsajBu_ekNdcEghomp-ezqVqw3r13dlWXA/
        /// Owner: trint
        /// Create log:     15.11.2022 - trint        
        /// </summary>                
        /// <returns></returns>

        /// <summary>
        /// Type: API method
        /// Description: API for client to update default common setting
        /// Request point: Client
        /// Authen: Yes, Bearer Token
        /// API document: https://docs.google.com/document/d/1xAZ6fG8ApHsajBu_ekNdcEghomp-ezqVqw3r13dlWXA/
        /// Owner: trint
        /// Create log:     15.11.2022 - trint        
        /// </summary>        
        /// <param name="param"></param>
        /// <returns></returns>
        [Route("update")]
        [HttpPost]
        public async Task<IHttpActionResult> Update(DefaultCommonSettingModel param)
        {
            ResponseService<DefaultCommonSettingModel> response = await _defaultCommonSettingService.UpdateCheckDuplicate(param);
            if (response.status)
                return Ok(response);

            return new ResponseFail<DefaultCommonSettingModel>().Error(response);
        }

        /// <summary>
        /// Type: API method
        /// Description: API for client to delete default common setting
        /// Request point: Client
        /// Authen: Yes, Bearer Token
        /// API document: https://docs.google.com/document/d/1xAZ6fG8ApHsajBu_ekNdcEghomp-ezqVqw3r13dlWXA/
        /// Owner: trint
        /// Create log:     15.11.2022 - trint        
        /// </summary>        
        /// <param name="param"></param>
        /// <returns></returns>
        [Route("delete")]
        [HttpPost]
        public async Task<IHttpActionResult> Remove([FromBody] ItemModel param)
        {
            ResponseService<bool> response = await _defaultCommonSettingService.Delete(param);
            if (response.status)
                return Ok(response);

            return new ResponseFail<bool>().Error(response);
        }

        /// <summary>
        /// Type: API method
        /// Description: API for client to get default common setting (by id)
        /// Request point: Client
        /// Authen: Yes, Bearer Token
        /// API document: https://docs.google.com/document/d/1xAZ6fG8ApHsajBu_ekNdcEghomp-ezqVqw3r13dlWXA/
        /// Owner: trint
        /// Create log:     15.11.2022 - trint        
        /// </summary>        
        /// <param name="param"></param>
        /// <returns></returns>
        [Route("get-item")]
        [HttpPost]
        public async Task<IHttpActionResult> GetById([FromBody] ItemModel param)
        {
            ResponseService<DefaultCommonSettingModel> response = await _defaultCommonSettingService.GetByIdDecrypted(param);
            if (response.status)
                return Ok(response);

            return new ResponseFail<DefaultCommonSettingModel>().Error(response);
        }

        /// <summary>
        /// Type: API method
        /// Description: API for client to add default data default common setting
        /// Request point: Client
        /// Authen: Yes, Bearer Token
        /// API document: https://docs.google.com/document/d/1xAZ6fG8ApHsajBu_ekNdcEghomp-ezqVqw3r13dlWXA/
        /// Owner: trint
        /// Create log:     15.11.2022 - trint        
        /// </summary>                
        /// <returns></returns>

        /// <summary>
        /// Type: API method
        /// Description: API for client to add more default data default common setting
        /// Request point: Client
        /// Authen: Yes, Bearer Token
        /// API document: https://docs.google.com/document/d/1xAZ6fG8ApHsajBu_ekNdcEghomp-ezqVqw3r13dlWXA/
        /// Owner: trint
        /// Create log:     15.11.2022 - trint        
        /// </summary>                
        /// <returns></returns>
    }
}
