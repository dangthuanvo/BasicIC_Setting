using BasicIC_Setting.Models.Common;
using BasicIC_Setting.Models.RestAPIModels;
using Common.Commons;
using Common.Params.Base;
using Repository.CustomModel;
using Repository.EF;
using Settings.Models.Main.M02;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BasicIC_Setting.Services.Interfaces
{
    /// <summary>
    /// Description: Build-up interfaces to handle features regarding default common setting
    /// </summary>
    public interface IDefaultCommonSettingService : IDefaultBaseCRUDService<DefaultCommonSettingModel, M02_DefaultCommonSetting>
    {
        Task<ResponseService<DefaultCommonSettingModel>> CreateCheckDuplicate(DefaultCommonSettingModel obj, List<TenantModel> tenantList = null, M02_BasicEntities dbContext = null);
        Task<ResponseService<ListResult<DefaultCommonSettingModel>>> GetAllDecrypted(PagingParam param, M02_BasicEntities dbContext = null);
        Task<ResponseService<DefaultCommonSettingModel>> GetByIdDecrypted(ItemModel obj, M02_BasicEntities dbContext = null);
        Task<ResponseService<DefaultCommonSettingModel>> UpdateCheckDuplicate(DefaultCommonSettingModel obj, M02_BasicEntities dbContext = null);
        Task<bool> Test();
    }
}