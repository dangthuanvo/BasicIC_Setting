using BasicIC_Setting.Models.Common;
using BasicIC_Setting.Models.Main;
using Common.Commons;
using Common.Params.Base;
using Repository.CustomModel;
using System.Data.Entity;
using System.Threading.Tasks;

namespace BasicIC_Setting.Services.Interfaces
{
    public interface IBaseCRUDService<T, V> where T : BaseModel
    {
        //
        // Summary:
        //     Get all records of type T based on param passed in
        //
        // Parameters:
        //   param:
        //     Paging param with search
        //   dbContext:
        //     Context passed in to preserve transaction
        //
        // Returns:
        //     Response Service with result from query
        Task<ResponseService<ListResult<T>>> GetAll(PagingParam param, DbContext dbContext = null);
        Task<ResponseService<ListResult<T>>> GetAllNoTenantId(PagingParam param, DbContext dbContext = null);

        //
        // Summary:
        //     Create record on db with type T
        //
        // Parameters:
        //   obj:
        //     Object to be created on db
        //   dbContext:
        //     Context passed in to preserve transaction
        //   autoLog:
        //     True then the method will push a kafka message to log create data
        //
        // Returns:
        //     Response Service with item of type T created on db
        Task<ResponseService<T>> Create(T param, DbContext dbContext = null, bool autoLog = true);

        //
        // Summary:
        //     Get record from id passed in
        //
        // Parameters:
        //   obj:
        //     ItemModel with id to get
        //   dbContext:
        //     Context passed in to preserve transaction
        //
        // Returns:
        //     Response Service with an item of type T with passed in id.
        Task<ResponseService<T>> GetById(ItemModel param, DbContext dbContext = null);

        //
        // Summary:
        //     Update a record on db
        //
        // Parameters:
        //   obj:
        //     Object of type T to be updated on db
        //   dbContext:
        //     Context passed in to preserve transaction
        //   autoLog:
        //     True then the method will push a kafka message to log update data
        //
        // Returns:
        //     Tuple with
        //          Item 1 is Response Service with an item of type T with passed in id.
        //          Item 2 is object type T that is the original record from db before update.
        Task<(ResponseService<T>, T)> Update(T param, DbContext dbContext = null, bool autoLog = true);

        //
        // Summary:
        //     Delete a record from id passed in
        //
        // Parameters:
        //   obj:
        //     ItemModel with id of item to delete
        //   dbContext:
        //     Context passed in to preserve transaction
        //   autoLog:
        //     True then the method will push a kafka message to log deletet data
        //
        // Returns:
        //     Response Service with true if delete successful and false if otherwise.
        Task<ResponseService<bool>> Delete(ItemModel param, DbContext dbContext = null, bool autoLog = true);
        Task<ResponseService<ListResult<T>>> GetAllGlobalSearch(PagingParamGlobalSearch param, DbContext dbContext = null);
    }
}