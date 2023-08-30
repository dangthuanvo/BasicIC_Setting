using BasicIC_Setting.Models.Common;
using BasicIC_Setting.Models.Main;
using Common.Commons;
using Common.Params.Base;
using Repository.CustomModel;
using System.Data.Entity;
using System.Threading.Tasks;

namespace BasicIC_Setting.Services.Interfaces
{
    public interface IDefaultBaseCRUDService<T, V> where T : DefaultBaseModel
    {
        Task<ResponseService<ListResult<T>>> GetAll(PagingParam param, DbContext dbContext = null);
        Task<ResponseService<T>> Create(T obj, DbContext dbContext = null, bool autoLog = true);
        Task<ResponseService<T>> GetById(ItemModel obj, DbContext dbContext = null);
        Task<(ResponseService<T>, T)> Update(T obj, DbContext dbContext = null, bool autoLog = true);
        Task<ResponseService<bool>> Delete(ItemModel obj, DbContext dbContext = null, bool autoLog = true);
        Task<ResponseService<ListResult<T>>> GetAllGlobalSearch(PagingParamGlobalSearch param, DbContext dbContext = null);
    }
}