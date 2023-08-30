using Common.Commons;
using Common.Params.Base;
using EntityFramework.DynamicLinq;
using Repository.CustomModel;
using Repository.Interfaces;
using Repository.Queries;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public class BaseRepositorySql<T> : IRepositorySql<T> where T : class
    {
        public readonly DbContext _db;
        public BaseRepositorySql(DbContext dbContext)
        {
            _db = dbContext;
        }

        #region implement
        public virtual async Task<T> Create(T obj, DbContext dbContext = null)
        {
            if (dbContext == null)
                dbContext = _db;

            dbContext.Set<T>().Add(obj);
            await dbContext.SaveChangesAsync();

            return obj;
        }

        public virtual async Task<T> GetById(object obj, DbContext dbContext = null)
        {
            if (dbContext == null)
                dbContext = _db;

            return await dbContext.Set<T>().FindAsync(obj);
        }
        public async virtual Task<T> FindAsync(object obj, DbContext dbContext = null)
        {
            if (dbContext == null)
                dbContext = _db;

            var t = await dbContext.Set<T>().FindAsync(obj);

            return t;
        }
        public virtual async Task<T> GetByIdWithInclude(object obj, string[] inCludeTables, bool isCollection = false, DbContext dbContext = null)
        {
            if (dbContext == null)
                dbContext = _db;

            T dbObj = await dbContext.Set<T>().FindAsync(obj);
            if (dbObj == null)
                return dbObj;

            foreach (string inCludeTable in inCludeTables)
            {
                if (isCollection)
                    dbContext.Entry(dbObj).Collection(inCludeTable).Load();
                else
                    dbContext.Entry(dbObj).Reference(inCludeTable).Load();
            }
            return dbObj;
        }

        public virtual async Task<ListResult<T>> GetAll(PagingParam param, DbContext dbContext = null, bool isPrivate = false)
        {
            if (dbContext == null)
                dbContext = _db;

            param.ReplaceSpecialCharacterSearch();
            Query<T> query = new Query<T>(param, dbContext, isPrivate);

            List<T> datas = await query.ToListAsync();
            int total = await query.CountAsync();

            return new ListResult<T>(datas, total);
        }

        public virtual async Task<ListResult<T>> GetAllGlobalSearch(PagingParamGlobalSearch param, DbContext dbContext = null)
        {
            if (dbContext == null)
                dbContext = _db;

            var isunicode = CommonFuncMain.IsUnicode(param.value) ? "true" : "false";
            List<T> datas = new List<T>();
            int total = 0;
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@isUnicode", isunicode),
                new SqlParameter("@stringValue", param.value.ToLower()),
                new SqlParameter("@schema","dbo"),
                new SqlParameter("@table", typeof(T).Name),
                new SqlParameter("@tenant_id", new Guid(param.tenant_id))
            };
            if (param != null && param.page != 0 && param.limit != 0)
            {
                datas = dbContext.Database.SqlQuery<T>("global_search @isUnicode, @stringValue, @schema, @table, @tenant_id", parameters.ToArray()).Skip(((param.page - 1) * param.limit)).Take(param.limit).ToList();
            }
            else
            {
                datas = await dbContext.Database.SqlQuery<T>("global_search @isUnicode, @stringValue, @schema, @table, @tenant_id", parameters.ToArray()).ToListAsync();
            }
            total = datas.Count;
            return new ListResult<T>(datas, total);
        }



        public async virtual Task<T> Update(T obj, DbContext dbContext = null)
        {
            if (dbContext == null)
                dbContext = _db;

            dbContext.Set<T>().Attach(obj);

            dbContext.Entry(obj).State = EntityState.Modified;
            await dbContext.SaveChangesAsync();

            return obj;
        }

        public async virtual Task<T> UpdateFields(T obj, List<string> fields, DbContext dbContext = null)
        {
            if (dbContext == null)
                dbContext = _db;

            dbContext.Set<T>().Attach(obj);

            foreach (string field in fields)
                dbContext.Entry(obj).Property(field).IsModified = true;
            await dbContext.SaveChangesAsync();

            return obj;
        }

        public async virtual Task AddRange(List<T> listT, DbContext dbContext = null)
        {
            if (dbContext == null)
                dbContext = _db;

            if (listT != null && listT.Count > 0)
            {
                dbContext.Set<T>().AddRange(listT);
                await dbContext.SaveChangesAsync();
            }
        }

        public async virtual Task<T> Delete(object obj, DbContext dbContext = null)
        {
            if (dbContext == null)
                dbContext = _db;

            var itemDelete = await dbContext.Set<T>().FindAsync(obj);

            if (itemDelete == null)
                return null;

            dbContext.Set<T>().Remove(itemDelete);
            await dbContext.SaveChangesAsync();
            return itemDelete;
        }

        public async virtual Task<bool> Delete(T t, DbContext dbContext = null)
        {
            if (dbContext == null)
                dbContext = _db;

            if (t == default)
                return false;

            dbContext.Set<T>().Attach(t);

            dbContext.Set<T>().Remove(t);
            await dbContext.SaveChangesAsync();
            return true;
        }

        public async virtual Task<bool> DeleteRange(List<T> listT, DbContext dbContext = null)
        {
            if (dbContext == null)
                dbContext = _db;

            if (listT != null && listT.Count > 0)
            {
                foreach (T t in listT)
                    dbContext.Set<T>().Attach(t);

                dbContext.Set<T>().RemoveRange(listT);
                await dbContext.SaveChangesAsync();
            }

            return true;
        }
        public async virtual Task<bool> DeleteRange(List<Guid> listT, DbContext dbContext = null)
        {
            if (dbContext == null)
                dbContext = _db;

            if (listT != null && listT.Count > 0)
            {
                foreach (var item in listT)
                {
                    T entity = dbContext.Set<T>().Find(item);
                    dbContext.Set<T>().Remove(entity);
                }
                await dbContext.SaveChangesAsync();
            }

            return true;
        }

        //public async virtual Task<int> GetTotal()
        //{
        //    IQueryable<T> query = _db.Set<T>().AsNoTracking();

        //    return await query.CountAsync();
        //}

        public async virtual Task<int> GetTotal(DbContext dbContext = null)
        {
            if (dbContext == null)
                dbContext = _db;

            IQueryable<T> query = dbContext.Set<T>().AsNoTracking();

            return await query.CountAsync();
        }

        #region helper functions
        public IQueryable<T> addSort(List<SortParam> sorts, IQueryable<T> query)
        {
            Type type = null;
            string order = "";

            // Try to fetch create_time column
            try
            {
                Type nullableType = Nullable.GetUnderlyingType(typeof(T).GetProperty("create_time").PropertyType);
                type = nullableType != null ? nullableType : typeof(T).GetProperty("create_time").PropertyType;
            }
            catch { }

            if (sorts != null && sorts.Count > 0)
            {
                foreach (SortParam sortParam in sorts)
                {
                    if (sortParam.isAccessding)
                        order = order + sortParam.name_field + " " + "ASC, ";
                    else
                        order = order + sortParam.name_field + " " + "DESC, ";
                }
            }

            // Order by create_time
            if (type != null)
            {
                order = order + "create_time";
            }
            else if (order.Length > 2)
            {
                // Remove ", " from order string
                order = order.Remove(order.Length - 2, 2);
            }

            if (order.Length > 0)
            {
                try
                {
                    query = query.OrderBy(order);
                }
                catch
                {
                    throw new Exception("Sort field does not exist");
                }
            }


            return query;
        }

        public IQueryable<T> addSearchParams(List<SearchParam> search_list, IQueryable<T> query)
        {
            search_list.ForEach(searchParam =>
            {
                Type type = null;

                try
                {
                    // Get the type of the field on db
                    Type nullableType = Nullable.GetUnderlyingType(typeof(T).GetProperty(searchParam.name_field).PropertyType);
                    type = nullableType != null ? nullableType : typeof(T).GetProperty(searchParam.name_field).PropertyType;
                }
                catch
                {
                    throw new Exception("Field '" + searchParam.name_field + "' does not exist");
                }

                // null then the field does not exist on db
                if (type == null)
                {
                    throw new Exception("Field '" + searchParam.name_field + "' does not exist");
                }

                // Handle searching by Guid
                if (type == typeof(Guid))
                {
                    Guid searchValue = new Guid(searchParam.value_search.ToString());

                    string searchPhrase = searchParam.name_field + " == @0";
                    query = query.Where(searchPhrase, searchValue);
                }
                // Handle searching by DateTime
                else if (type == typeof(DateTime))
                {
                    DateTime under_bound;
                    if (searchParam.value_search.GetType() == typeof(DateTime))
                    {
                        under_bound = (DateTime)searchParam.value_search;
                    }
                    else
                    {
                        string[] formats = { "d/M/yyyy h:mm:ss tt", "d/M/yyyy HH:mm:ss", "d/M/yyyy h:mm:ss", "d/M/yyyy HH:mm",
                            "d/M/yyyy h:mm", "d/M/yyyy HH", "d/M/yyyy h", "d/M/yyyy", "d/M/yy"};
                        bool success = DateTime.TryParseExact(searchParam.value_search.ToString(),
                                            formats, null, DateTimeStyles.None, out under_bound);

                        if (!success)
                            throw new Exception("Error parsing value_search to DateTime");
                    }

                    // Handle searching if upper bound is given
                    if (searchParam.upper_bound != null)
                    {
                        DateTime upper_bound;
                        if (searchParam.value_search.GetType() == typeof(DateTime))
                        {
                            upper_bound = (DateTime)searchParam.value_search;
                        }
                        else
                        {
                            string[] formats = { "d/M/yyyy h:mm:ss tt", "d/M/yyyy HH:mm:ss", "d/M/yyyy h:mm:ss", "d/M/yyyy HH:mm",
                            "d/M/yyyy h:mm", "d/M/yyyy HH", "d/M/yyyy h", "d/M/yyyy", "d/M/yy"};
                            bool success = DateTime.TryParseExact(searchParam.value_search.ToString(),
                                                formats, null, DateTimeStyles.None, out upper_bound);

                            if (!success)
                                throw new Exception("Error parsing upper_bound to DateTime");
                        }

                        string searchPhrase = searchParam.name_field + " > @0 && " + searchParam.name_field + " < @1";
                        query = query.Where(searchPhrase, under_bound, upper_bound);
                    }
                    // Auto create upper bound
                    else
                    {
                        // If month is not given then search the entire year (i.e: 2021)
                        if (under_bound.Month == 0)
                        {
                            DateTime upper_bound = under_bound.AddYears(1);
                            string searchPhrase = searchParam.name_field + " > @0 && " + searchParam.name_field + " < @1";
                            query = query.Where(searchPhrase, under_bound, upper_bound);
                        }
                        // If day is not given then search the entire month (i.e: 04/2021)
                        else if (under_bound.Day == 0)
                        {
                            DateTime upper_bound = under_bound.AddMonths(1);
                            string searchPhrase = searchParam.name_field + " > @0 && " + searchParam.name_field + " < @1";
                            query = query.Where(searchPhrase, under_bound, upper_bound);
                        }
                        // If hour is not given then search the entire day (i.e: 16/04/2021)
                        else if (under_bound.Hour == 0 && searchParam.value_search.ToString().Length != 13)
                        {
                            DateTime upper_bound = under_bound.AddDays(1);
                            string searchPhrase = searchParam.name_field + " > @0 && " + searchParam.name_field + " < @1";
                            query = query.Where(searchPhrase, under_bound, upper_bound);
                        }
                        // If minute is not given then search for one hour (i.e: 16/04/2021 13)
                        else if (under_bound.Minute == 0 && searchParam.value_search.ToString().Length != 16)
                        {
                            DateTime upper_bound = under_bound.AddHours(1);
                            string searchPhrase = searchParam.name_field + " > @0 && " + searchParam.name_field + " < @1";
                            query = query.Where(searchPhrase, under_bound, upper_bound);
                        }
                        // If second is not given then search for one minute (i.e: 16/04/2021 13:52)
                        else if (under_bound.Second == 0 && searchParam.value_search.ToString().Length != 19)
                        {
                            DateTime upper_bound = under_bound.AddMinutes(1);
                            string searchPhrase = searchParam.name_field + " > @0 && " + searchParam.name_field + " < @1";
                            query = query.Where(searchPhrase, under_bound, upper_bound);
                        }
                        // If milisecon is not given then search for one second (i.e: 16/04/2021 13:52:50)
                        else if (under_bound.Millisecond == 0)
                        {
                            DateTime upper_bound = under_bound.AddSeconds(1);
                            string searchPhrase = searchParam.name_field + " > @0 && " + searchParam.name_field + " < @1";
                            query = query.Where(searchPhrase, under_bound, upper_bound);
                        }
                        // Exact search (i.e: 2021-04-14T13:42:50.913) (Please don't use this!!!)
                        else
                        {
                            string searchPhrase = searchParam.name_field + " == @0";
                            query = query.Where(searchPhrase, under_bound);
                        }
                    }
                }
                else
                {
                    if (searchParam.upper_bound != null && type != typeof(string))
                    {
                        var under_bound = Convert.ChangeType(searchParam.value_search.ToString(), type);
                        var upper_bound = Convert.ChangeType(searchParam.upper_bound.ToString(), type);

                        string searchPhrase = searchParam.name_field + " > @0 && " + searchParam.name_field + " < @1";
                        query = query.Where(searchPhrase, under_bound, upper_bound);
                    }
                    else
                    {
                        var searchValue = Convert.ChangeType(searchParam.value_search.ToString(), type);

                        string searchPhrase = searchParam.name_field + " == @0";
                        query = query.Where(searchPhrase, searchValue);
                    }
                }
            });

            return query;
        }


        protected class Property
        {
            public Type type { get; set; }
            public string name { get; set; }
        }

        protected class TypeList
        {
            public Type key { get; set; }
            public List<Property> props { get; set; }
        }

        public IQueryable<T> addGlobalSearchParam(string value_search, IQueryable<T> query)
        {
            // Return all records if value_search is empty
            if (value_search == null || value_search.Length == 0)
            {
                return query;
            }

            List<dynamic> searchValues = new List<dynamic>();
            string queryString = "WHERE ";


            // Get all properties and types of T
            var properties = typeof(T).GetProperties().Select(property =>
            {
                Type nullableType = Nullable.GetUnderlyingType(property.PropertyType);
                Type type = nullableType != null ? nullableType : property.PropertyType;

                return new Property { type = type, name = property.Name };
            }).ToList();

            foreach (var property in properties)
            {
                queryString += "CONVERT(VARCHAR(max)," + property.name + ", 103) LIKE " + "%" + value_search + "%" + " OR '";
            }

            //query = query.ToList().Select

            var initialTypes = properties.GroupBy(property => property.type, (key, property) => new TypeList { key = key, props = property.ToList() }).ToList();

            // Try to cast value_search to other types
            foreach (TypeList type in initialTypes)
            {
                try
                {
                    // Handle Guid type
                    if (type.key == typeof(Guid))
                    {
                        Guid searchValue = new Guid(value_search);

                        foreach (Property property in type.props)
                        {
                            if (queryString.Length == 0)
                                queryString += property.name + " == @" + searchValues.Count().ToString();
                            else
                                queryString += " || " + property.name + " == @" + searchValues.Count().ToString();
                        }

                        searchValues.Add(searchValue);
                    }
                    // Handle search by range in DateTime
                    else if (type.key == typeof(DateTime))
                    {
                        //DateTime underBound = (DateTime)Convert.ChangeType(value_search, typeof(DateTime));
                        //DateTime? upperBound = null;

                        //// If month is not given then search the entire year (i.e: 2021)
                        //if (underBound.Month == 0)
                        //{
                        //    upperBound = underBound.AddYears(1);
                        //}
                        //// If day is not given then search the entire month (i.e: 2021-04)
                        //else if (underBound.Day == 0)
                        //{
                        //    upperBound = underBound.AddMonths(1);
                        //}
                        //// If hour is not given then search the entire day (i.e: 2021-04-16)
                        //else if (underBound.Hour == 0 && value_search.Length != 13)
                        //{
                        //    upperBound = underBound.AddDays(1);
                        //}
                        //// If minute is not given then search for one hour (i.e: 2021-04-14T13)
                        //else if (underBound.Minute == 0 && value_search.Length != 16)
                        //{
                        //    upperBound = underBound.AddHours(1);
                        //}
                        //// If second is not given then search for one minute (i.e: 2021-04-14T13:52)
                        //else if (underBound.Second == 0 && value_search.Length != 19)
                        //{
                        //    upperBound = underBound.AddMinutes(1);
                        //}
                        //// If milisecon is not given then search for one second (i.e: 2021-04-14T13:52:50)
                        //else if (underBound.Millisecond == 0)
                        //{
                        //    upperBound = underBound.AddSeconds(1);
                        //}
                        //// Exact search (i.e: 2021-04-14T13:42:50.913) (Please don't use this!!!)
                        //else
                        //{
                        //}

                        //// Search DateTime in range
                        //if (upperBound != null)
                        //{
                        //    foreach (Property property in type.props)
                        //    {
                        //        if (queryString.Length == 0)
                        //            queryString += "(" + property.name + " > @" + searchValues.Count().ToString() + " && " + property.name + " < @" + (searchValues.Count() + 1).ToString() + ")";
                        //        else
                        //            queryString += " || (" + property.name + " > @" + searchValues.Count().ToString() + " && " + property.name + " < @" + (searchValues.Count() + 1).ToString() + ")";
                        //    }

                        //    searchValues.Add(underBound);
                        //    searchValues.Add(upperBound);
                        //}
                        //else
                        //{
                        //    foreach (Property property in type.props)
                        //    {
                        //        if (queryString.Length == 0)
                        //            queryString += "(" + property.name + " > @" + searchValues.Count().ToString() + " && " + property.name + " < @" + (searchValues.Count() + 1).ToString() + ")";
                        //        else
                        //            queryString += " || (" + property.name + " > @" + searchValues.Count().ToString() + " && " + property.name + " < @" + (searchValues.Count() + 1).ToString() + ")";
                        //    }

                        //    searchValues.Add(underBound);
                        //    searchValues.Add(upperBound);
                        //}
                    }
                    //// Handle type String and Int
                    //else if (type.key == typeof(String))
                    //{
                    //    //foreach (Property property in type.props)
                    //    //{
                    //    //    if (queryString.Length == 0)
                    //    //        queryString += property.name + ".ToString().Contains(@" + searchValues.Count().ToString() + ")";
                    //    //    else
                    //    //        queryString += " || " + property.name + ".ToString().Contains(@" + searchValues.Count().ToString() + ")";
                    //    //}

                    //    //searchValues.Add(value_search);

                    //    foreach (Property property in type.props)
                    //    {
                    //        if (queryString.Length == 0)
                    //            queryString += property.name + ".ToString().Contains(\"" + value_search + "\")";
                    //        else
                    //            queryString += " || " + property.name + ".ToString().Contains(\"" + value_search + "\")";
                    //    }
                    //}
                    //// Handle other types
                    //else
                    //{
                    //    //try
                    //    //{
                    //    var searchValue = Convert.ChangeType(value_search, type.key);

                    //    foreach (Property property in type.props)
                    //    {
                    //        if (queryString.Length == 0)
                    //            queryString += property.name + " == @" + searchValues.Count().ToString();
                    //        else
                    //            queryString += " || " + property.name + " == @" + searchValues.Count().ToString();
                    //    }

                    //    searchValues.Add(searchValue);
                    //    //}
                    //    //catch
                    //    //{
                    //    foreach (Property property in type.props)
                    //    {
                    //        if (queryString.Length == 0)nbnbnbnbnbnbnbbnbnbnbnbn
                    //            queryString += property.name + ".ToString().Contains(@" + searchValues.Count().ToString() + ")";
                    //        else
                    //            queryString += " || " + property.name + ".ToString().Contains(@" + searchValues.Count().ToString() + ")";
                    //    }

                    //    searchValues.Add(value_search);
                    //    //}
                    //}
                }
                catch (Exception)
                {
                }
            }

            if (queryString.Length != 0)
                query = query.Where(queryString, searchValues.Cast<object>().ToArray());

            return query;
        }

        #endregion


        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
