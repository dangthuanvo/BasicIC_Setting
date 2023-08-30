using Common.Params.Base;
using Repository.CustomModel;
using System;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IRepositorySql<T> : IDisposable where T : class
    {
        Task<T> Create(T obj, DbContext dbContext = null);
        Task<T> GetById(object obj, DbContext dbContext = null);
        Task<ListResult<T>> GetAll(PagingParam parram, DbContext dbContext = null, bool isPrivate = false);
        Task<T> Update(T obj, DbContext dbContext = null);
        Task<T> Delete(object obj, DbContext dbContext = null);
        Task<ListResult<T>> GetAllGlobalSearch(PagingParamGlobalSearch param, DbContext dbContext = null);
    }
}
