using BasicIC_Setting.Models.Main.M02;
using Common.Commons;
using Repository.EF;
using System.Threading.Tasks;

namespace BasicIC_Setting.Services.Interfaces
{
    /// <summary>
    /// Description: Build-up interfaces to handle features regarding email setting
    /// </summary>
    public interface IEmailSettingService
    {
        Task<ResponseService<EmailSettingModel>> Create(EmailSettingModel param, M02_BasicEntities dbContext = null);
        Task<ResponseService<EmailSettingModel>> Get(M02_BasicEntities dbContext = null);
        Task<ResponseService<EmailSettingModel>> Update(EmailSettingModel obj, M02_BasicEntities dbContext = null);
        Task<ResponseService<bool>> Delete(M02_BasicEntities dbContext = null);

    }
}