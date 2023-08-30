using Common;
using Common.Commons;
using Common.Params.Base;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Threading.Tasks;

namespace Repository.Queries
{
    public class Query<T>
    {
        private DbContext dbContext { get; set; }

        // Outer select statement
        private string select { get; set; } = $"SELECT ";

        // Outer condition statement for join tables
        private string conditions { get; set; } = "";

        // Because of paging, primary table's OFFSET, ROWS FETCH NEXT, and WHERE clause has to be put
        // in a query inside the actual query
        // Example:
        //  SELECT *
        //      FROM (
        //          SELECT* FROM tableA [WHERE HERE] OFFSET 0 ROWS FETCH NEXT 10 ROWS ONLY
        //      ) q
        //      JOIN tableB on foo = bar [WHERE HERE]
        private string innerSelect { get; set; } = $"\tWHERE [{typeof(T).Name}].[id] in\n" +
                                                        $"\t\t(SELECT [id] FROM\n" +
                                                        $"\t\t\t(SELECT DISTINCT ";

        private string from { get; set; } = $"FROM [{"dbo"}].[{typeof(T).Name}] ";

        // Order By statement
        private string sort { get; set; } = "";

        // Join [tableName] ON statement
        private string include { get; set; } = "";

        // Save tenant_id
        private Guid tenant_id;

        // Paging
        private int page { get; set; } = 0;
        private int limit { get; set; } = 0;

        // All joining table types
        private List<Type> includeTypes { get; set; } = new List<Type>();

        // All properties name and type to dynamically create class. Used for receiving join model from query
        private List<string> propertyNames { get; set; } = new List<string>();
        private List<Type> propertyTypes { get; set; } = new List<Type>();

        // When joining tables and searching joined table, only get records from the join table that match
        // value search
        public bool is_inner_join { get; set; } = false;

        private class IncludeType
        {
            public Type type { get; set; }
            public Type parentType { get; set; }
        }

        //public Query()
        //{
        //    baseQueryString = $"SELECT * FROM [{"dbo"}].[{typeof(T).Name}] ";
        //}

        //public Query(M02_SettingsEntities dbContext)
        //{
        //    this.dbContext = dbContext;
        //}

        public Query(PagingParam param, DbContext dbContext, bool isPrivate)
        {
            this.dbContext = dbContext;
            //baseQueryString = $"SELECT * FROM [{"dbo"}].[{typeof(T).Name}] ";
            this.SelectBuilder(typeof(T), typeof(T).GetProperties().Where(m => m.GetAccessors()[0].IsFinal ||
                                                                        !m.GetAccessors()[0].IsVirtual).ToArray());
            this.page = param.page;
            this.limit = param.limit;
            this.is_inner_join = param.is_inner_join;

            // Search by tenant_id
            if (!String.IsNullOrEmpty(param.tenant_id))
            {
                this.tenant_id = Guid.Parse(param.tenant_id);
            }

            // Join tables
            if (param.join_list != null && param.join_list.Count > 0)
                foreach (JoinParam joinParam in param.join_list)
                    this.Include(joinParam);

            // Search accurate
            if (param.field_get_list != null && param.field_get_list.Count > 0)
                foreach (SearchParam searchParam in param.field_get_list)
                    this.Where(searchParam);

            // Search like
            if (param.search_list != null && param.search_list.Count > 0)
                foreach (SearchParam searchParam in param.search_list)
                    if (isPrivate)
                        this.WhereLikeAllowValueNull(searchParam);
                    else
                        this.WhereLike(searchParam);

            // Search table
            if (param.table_search_list != null && param.table_search_list.Length > 0)
                foreach (string searchParam in param.table_search_list)
                    this.WhereTable(searchParam);

            // Sort
            if (param.sorts != null && param.sorts.Count > 0)
                foreach (SortParam sort in param.sorts)
                    this.OrderBy(sort);

            // Add custom query
            if (param.custom_query_list != null && param.custom_query_list.Count > 0)
                foreach (CustomQueryString customQuery in param.custom_query_list)
                    this.AddQueryString(customQuery);
        }

        // Search accurate
        public Query<T> Where(SearchParam param)
        {
            //if (String.IsNullOrEmpty(param.value_search.ToString()))
            //    return this;

            Type type;

            // Default to type of T if search type not passed in
            if (String.IsNullOrEmpty(param.type))
                type = typeof(T);
            else
                type = Type.GetType($"Repository.EF.{param.type}");

            // Check if name_field is valid
            PropertyInfo property = type.GetProperty(param.name_field);
            if (property == null)
                throw new KeyNotFoundException(param.name_field + " not found");

            // Add conjunction to query string
            if (String.IsNullOrEmpty(conditions))
                conditions = "WHERE ( ";
            else
                conditions += $"{param.conjunction} ";

            // Replace Boolean by 1 or 0
            if (param.value_search != null && (property.PropertyType == typeof(bool)
                || property.PropertyType == typeof(bool?)))
                if (param.value_search.ToString().ToLower() == "true")
                    param.value_search = 1;
                else if (param.value_search.ToString().ToLower() == "false")
                    param.value_search = 0;

            // Replace ' (single quote) in search value by '' (2 single quotes))
            if (param.value_search != null)
                param.value_search = param.value_search.ToString().Replace("--", "").Replace("'", "").Replace("/*", "").Replace(";", "");
            if (param.upper_bound != null)
                param.upper_bound = param.upper_bound.ToString().Replace("--", "").Replace("'", "").Replace("/*", "").Replace(";", "");

            // Handle open parenthesis
            if (param.is_open_parenthesis > 0)
                for (int i = 0; i < param.is_open_parenthesis; i++)
                    conditions += "(";

            // Handle when no upper_bound is passed in
            if (param.upper_bound == null)
            {
                // if null then search is or is not null
                if (param.value_search == null)
                    conditions += $" [{type.Name}].[{param.name_field}] IS {(param.logical_operator == "!=" ? "NOT" : "")} NULL ";
                // else if type is string then add N before value search to search for nvarchar
                else if (property.PropertyType == typeof(string))
                    conditions += $" [{type.Name}].[{param.name_field}] {(param.logical_operator)} N'{param.value_search}' ";
                // else search normally
                else
                    conditions += $" [{type.Name}].[{param.name_field}] {(param.logical_operator)} '{param.value_search}' ";
            }
            // upper_bounded is passed in then check for logical operator to search less than upper_bound or between
            else
            {
                if (param.logical_operator == "<")
                    conditions += $" ([{type.Name}].[{param.name_field}] < '{param.upper_bound}') ";
                else
                    conditions += $" ([{type.Name}].[{param.name_field}] > '{param.value_search}' AND [{type.Name}].[{param.name_field}] < '{param.upper_bound}') ";

            }
            // Handle closing parenthesis
            if (param.is_close_parenthesis > 0)
                for (int i = 0; i < param.is_close_parenthesis; i++)
                    conditions += ")";

            return this;
        }

        // Search LIKE
        public Query<T> WhereLike(SearchParam param)
        {
            if (String.IsNullOrEmpty(param.value_search?.ToString()))
                return this;

            Type type;

            // Default to type of T if search type not passed in
            if (String.IsNullOrEmpty(param.type))
                type = typeof(T);
            else
                type = Type.GetType($"Repository.EF.{param.type}");

            // Check if name_field is valid
            PropertyInfo property = type.GetProperty(param.name_field);
            if (property == null)
                throw new KeyNotFoundException(param.name_field + " not found");

            // Replace Boolean by 1 or 0
            if (property.PropertyType == typeof(bool)
                || property.PropertyType == typeof(bool?))
                if (param.value_search.ToString().ToLower() == "true")
                    param.value_search = 1;
                else if (param.value_search.ToString().ToLower() == "false")
                    param.value_search = 0;

            // Add conjunction to query string
            if (String.IsNullOrEmpty(conditions))
                conditions = "WHERE ( ";
            else
                conditions += $"{param.conjunction} ";

            string valueSearch;
            //bool isUnicode;
            if (param.value_search != null)
            {
                // Check if necessary to convert
                //valueSearch = param.value_search.ToString();
                //isUnicode = CommonFuncMain.IsUnicode(valueSearch);

                valueSearch = param.value_search.ToString().Replace("_", "[_]").Replace("--", "").Replace("'", "''").Replace("/*", "").Replace("%", "[%]");
            }
            else
            {
                valueSearch = null;
                //isUnicode = false;
            }

            // Handle open parenthesis
            if (param.is_open_parenthesis > 0)
                for (int i = 0; i < param.is_open_parenthesis; i++)
                    conditions += "(";

            // convert datetime then search
            if (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?))
                conditions += $" [dbo].[ufn_convertDatetime]([{type.Name}].[{param.name_field}]) LIKE '%{valueSearch}%' ";
            // else if null then search is or is not null depending on logical operator
            else if (valueSearch == null)
                conditions += $" [{type.Name}].[{param.name_field}] IS {(param.logical_operator == "!=" ? "NOT" : "")} NULL ";
            // else convert value from db then search unaccented
            else if (property.PropertyType == typeof(Guid) || property.PropertyType == typeof(Guid?))
                //conditions += $" CONVERT(VARCHAR(max), dbo.ufn_removeMark([{type.Name}].[{param.name_field}]), 103) {(param.logical_operator == "!=" ? "NOT" : "")} LIKE '%{valueSearch}%' ";
                conditions += $"[{type.Name}].[{param.name_field}] {(param.logical_operator == "!=" ? "NOT" : "")} LIKE '%{valueSearch}%' ";
            else
                conditions += $" [dbo].[fn_removeCharacters]([{type.Name}].[{param.name_field}]) {(param.logical_operator == "!=" ? "NOT" : "")} LIKE N'%{valueSearch}%' ";

            // Handle closing parenthesis
            if (param.is_close_parenthesis > 0)
                for (int i = 0; i < param.is_close_parenthesis; i++)
                    conditions += ")";

            return this;
        }

        public Query<T> WhereLikeAllowValueNull(SearchParam param)
        {
            Type type;

            // Default to type of T if search type not passed in
            if (String.IsNullOrEmpty(param.type))
                type = typeof(T);
            else
                type = Type.GetType($"Repository.EF.{param.type}");

            // Check if name_field is valid
            PropertyInfo property = type.GetProperty(param.name_field);
            if (property == null)
                throw new KeyNotFoundException(param.name_field + " not found");

            // Replace Boolean by 1 or 0
            if (property.PropertyType == typeof(bool)
                || property.PropertyType == typeof(bool?))
                if (param.value_search.ToString().ToLower() == "true")
                    param.value_search = 1;
                else if (param.value_search.ToString().ToLower() == "false")
                    param.value_search = 0;

            // Add conjunction to query string
            if (String.IsNullOrEmpty(conditions))
                conditions = "WHERE ( ";
            else
                conditions += $"{param.conjunction} ";

            string valueSearch;
            //bool isUnicode;
            if (param.value_search != null)
            {
                // Check if necessary to convert
                //valueSearch = param.value_search.ToString();
                //isUnicode = CommonFuncMain.IsUnicode(valueSearch);

                valueSearch = param.value_search.ToString().Replace("_", "[_]").Replace("--", "").Replace("'", "").Replace("/*", "").Replace("%", "[%]").Replace(";", "");
            }
            else
            {
                valueSearch = null;
                //isUnicode = false;
            }

            // Handle open parenthesis
            if (param.is_open_parenthesis > 0)
                for (int i = 0; i < param.is_open_parenthesis; i++)
                    conditions += "(";

            // convert datetime then search
            if (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?))
                conditions += $" [dbo].[ufn_convertDatetime]([{type.Name}].[{param.name_field}]) LIKE '%{valueSearch}%' ";
            // else if null then search is or is not null depending on logical operator
            else if (valueSearch == null)
                conditions += $" [{type.Name}].[{param.name_field}] IS {(param.logical_operator == "!=" ? "NOT" : "")} NULL ";
            // else convert value from db then search unaccented
            else if (property.PropertyType == typeof(Guid) || property.PropertyType == typeof(Guid?))
                //conditions += $" CONVERT(VARCHAR(max), dbo.ufn_removeMark([{type.Name}].[{param.name_field}]), 103) {(param.logical_operator == "!=" ? "NOT" : "")} LIKE '%{valueSearch}%' ";
                conditions += $"[{type.Name}].[{param.name_field}] {(param.logical_operator == "!=" ? "NOT" : "")} LIKE '%{valueSearch}%' ";
            else
                conditions += $" [dbo].[fn_removeCharacters]([{type.Name}].[{param.name_field}]) {(param.logical_operator == "!=" ? "NOT" : "")} LIKE N'%{valueSearch}%' ";

            // Handle closing parenthesis
            if (param.is_close_parenthesis > 0)
                for (int i = 0; i < param.is_close_parenthesis; i++)
                    conditions += ")";

            return this;
        }

        // Search table
        public Query<T> WhereTable(string param, Type type = default)
        {
            if (String.IsNullOrEmpty(param))
                return this;

            // Default to type of T if search type not passed in
            if (type == default)
                type = typeof(T);

            // Get all properties that is not virtual (not mapped to other tables)
            PropertyInfo[] properties = type.GetProperties().Where(m => m.GetAccessors()[0].IsFinal ||
                                                                        !m.GetAccessors()[0].IsVirtual).ToArray();

            // Check if necessary to convert
            //bool isUnicode = CommonFuncMain.IsUnicode(param);

            param = param.Replace("_", "[_]").Replace("--", "").Replace("'", "").Replace("/*", "").Replace("%", "[%]").Replace(";", "");

            if (properties.Length > 0)
            {
                // Add conjunction to query string
                if (String.IsNullOrEmpty(conditions))
                    conditions = "WHERE ((";
                else
                    conditions += $"AND (";

                foreach (PropertyInfo property in properties)
                {
                    // convert datetime then search
                    if (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?))
                        conditions += $"[dbo].[ufn_convertDatetime]([{type.Name}].[{property.Name}]) LIKE '%{param}%' OR ";
                    // else convert value from db then search unaccented
                    else if (property.PropertyType == typeof(Guid) || property.PropertyType == typeof(Guid?))
                        //conditions += $"CONVERT(VARCHAR(max), dbo.ufn_removeMark([{type.Name}].[{property.Name}]), 103) LIKE '%{param}%' OR ";
                        conditions += $"[{type.Name}].[{property.Name}] LIKE '%{param}%' OR ";
                    else
                        conditions += $" [dbo].[fn_removeCharacters]([{type.Name}].[{property.Name}]) LIKE N'%{param}%' OR ";
                }

                // Remove last " OR " and add closing parenthesis
                conditions = conditions.Remove(conditions.Length - 4);
                conditions += ") ";
            }

            return this;
        }

        // Add custom query string
        public Query<T> AddQueryString(CustomQueryString param)
        {
            string value = this.GetType().GetProperty(param.fieldName, BindingFlags.NonPublic | BindingFlags.Instance).GetValue(this).ToString();
            value = value += param.queryString;

            this.GetType().GetProperty(param.fieldName, BindingFlags.NonPublic | BindingFlags.Instance).SetValue(this, value);

            return this;
        }

        // Handle Include
        public Query<T> Include(JoinParam param)
        {
            Type type;

            // Default to type of T if search type not passed in
            if (String.IsNullOrEmpty(param.joinTable))
                type = typeof(T);
            else
                type = Type.GetType($"Repository.EF.{param.joinTable}");

            // Get all properties that is virtual (mapped to other tables)
            PropertyInfo[] properties = type.GetProperties().Where(m => m.GetAccessors()[0].IsFinal ||
                                                                        !m.GetAccessors()[0].IsVirtual).ToArray();

            // Add JOIN statement
            if (String.IsNullOrEmpty(this.include))
                this.include = $"{param.joinType} [{type.Name}] ON {typeof(T).Name}.{param.primaryColumn} = {type.Name}.{param.foreignColumn} ";
            else
                this.include += $"{param.joinType} [{type.Name}] ON {typeof(T).Name}.{param.primaryColumn} = {type.Name}.{param.foreignColumn} ";
            //if (String.IsNullOrEmpty(this.include))
            //    this.include = $"{param.joinType} [{type.Name}] ON {param.joinFromTable ?? typeof(T).Name}.{param.primaryColumn} = {type.Name}.{param.foreignColumn} ";
            //else
            //    this.include += $"{param.joinType} [{type.Name}] ON {param.joinFromTable ?? typeof(T).Name}.{param.primaryColumn} = {type.Name}.{param.foreignColumn} ";

            // Build SELECT statement (add SELECT nameField as newNameField to map to runtime class) 
            this.SelectBuilder(type, properties);

            return this;
        }

        // Handle sort
        public Query<T> OrderBy(SortParam param, Type type = default)
        {
            // Default to type of T if search type not passed in
            if (type == default)
                type = typeof(T);

            // Check if name_field is valid
            PropertyInfo property = type.GetProperty(param.name_field);
            if (property == null)
                throw new KeyNotFoundException(param.name_field + " not found");

            if (String.IsNullOrEmpty(sort))
                sort = "ORDER BY ";
            else
                sort += ", ";

            sort += $"[{type.Name}].[{param.name_field}] {param.type_sort} ";

            return this;
        }

        // Return query string
        public override string ToString()
        {
            // Check if create_time is a valid field
            PropertyInfo property = typeof(T).GetProperty("modify_time");

            // Add sorting by create_time if type T has create_time property and
            // sort doesn't contain create_time
            string sortToString = sort;
            if (property != null)
            {
                // Example:
                // sortToString = "ORDER BY [M02_DistributionRule].modify_time DESC"
                if (String.IsNullOrEmpty(sortToString))
                    sortToString = $"ORDER BY [{typeof(T).Name}].[modify_time] DESC ";
                else if (!sortToString.Contains($"[{typeof(T).Name}].[modify_time]"))
                    sortToString += $", [{typeof(T).Name}].[modify_time] DESC ";
            }

            // Remove ", \n\t\t\t" from innerSelect
            // Example:
            // FROM ( SELECT DISTINCT [M02_DistributionRule].[id] AS 'id',
            // [M02_DistributionRule].[rule_name] AS 'rule_name',
            // ...
            string innerQuery = this.innerSelect.Remove(this.innerSelect.LastIndexOf(", \n\t\t\t"), 6);
            innerQuery += "\n";

            // We need to use OFFSET and FETCH NEXT, and they requires ORDER BY
            // Adding CURRENT_TIMESTAMP will act as a dummy ORDER BY
            if (String.IsNullOrEmpty(sortToString))
            {
                // Example:
                // FROM ( SELECT DISTINCT [M02_DistributionRule].[id] AS 'id',
                // [M02_DistributionRule].[rule_name] AS 'rule_name',
                // ...
                // , CURRENT_TIMESTAMP as 'CURRENT_TIMESTAMP'
                innerQuery += ", CURRENT_TIMESTAMP as 'CURRENT_TIMESTAMP' ";
                // Example:
                // ORDER BY CURRENT_TIMESTAMP 
                sortToString = "ORDER BY CURRENT_TIMESTAMP ";
            }

            // Add query by tenant_id at the last of conditions
            string conditionToString = conditions;
            if (!this.tenant_id.Equals(Guid.Empty))
            {
                if (!String.IsNullOrEmpty(conditions))
                    conditionToString += $") AND [{typeof(T).Name}].[tenant_id] = '{this.tenant_id}' ";
                else
                    conditionToString = $"WHERE [{typeof(T).Name}].[tenant_id] = '{this.tenant_id}' ";
            }
            else if (!String.IsNullOrEmpty(conditions))
                conditionToString += ") ";

            // Add paging
            if (page > 0 && limit > 0)
                // Example:
                // FROM [{ "dbo"}].[{ typeof(T).Name}] WHERE [id] in
                //  (SELECT [id] 
                //      FROM ( SELECT DISTINCT [M02_DistributionRule].[id] AS 'id',
                //      [M02_DistributionRule].[rule_name] AS 'rule_name',
                //      ...
                //      FROM [dbo].[M02_DistributionRule]
                //          LEFT JOIN [M02_DistributionRule_Channel] ON M02_DistributionRule.id = M02_DistributionRule_Channel.rule_id
                //          LEFT JOIN [M02_DistributionRule_Location] ON M02_DistributionRule.id = M02_DistributionRule_Location.rule_id
                //      WHERE [M02_DistributionRule].[tenant_id] = '1229B2AB-93C2-427A-88F3-B737FAFACA93'
                //          AND [M02_DistributionRule_Channel].[channel_type] = 'facebook_comment'
                //      ORDER BY [M02_DistributionRule].modify_time DESC
                //          OFFSET 0 ROWS FETCH NEXT 2 ROWS ONLY
                //  ) AS M02_DistributionRule 
                innerQuery += $"\t\t\t{from}\n" +
                              $"{(String.IsNullOrEmpty(include) ? "" : $"\t\t\t{include}\n")}" +
                              $"\t\t\t\t{conditionToString}\n" +
                              $"\t\t\t\t{sortToString}\n" +
                              $"\t\t\t\tOFFSET {(page - 1) * limit} ROWS FETCH NEXT {limit} ROWS ONLY )\n" +
                              $"\t\tAS {typeof(T).Name}) ";
            else
                // Example:
                // FROM [M02_DistributionRule] WHERE [id] in
                //  (SELECT [id] 
                //      FROM ( SELECT DISTINCT [M02_DistributionRule].[id] AS 'id',
                //      [M02_DistributionRule].[rule_name] AS 'rule_name',
                //      ...
                //      FROM [dbo].[M02_DistributionRule]
                //          LEFT JOIN [M02_DistributionRule_Channel] ON M02_DistributionRule.id = M02_DistributionRule_Channel.rule_id
                //          LEFT JOIN [M02_DistributionRule_Location] ON M02_DistributionRule.id = M02_DistributionRule_Location.rule_id
                //      WHERE [M02_DistributionRule].[tenant_id] = '1229B2AB-93C2-427A-88F3-B737FAFACA93'
                //          AND [M02_DistributionRule_Channel].[channel_type] = 'facebook_comment'
                //      ORDER BY [M02_DistributionRule].modify_time DESC
                //          OFFSET 0 ROWS
                //  ) AS M02_DistributionRule 
                innerQuery += $"\t\t\t{from}\n" +
                              $"{(String.IsNullOrEmpty(include) ? "" : $"\t\t\t{include}\n")}" +
                              $"\t\t\t\t{conditionToString}\n" +
                              $"\t\t\t\t{sortToString}\n" +
                              $"\t\t\t\tOFFSET 0 ROWS)\n" +
                              $"\t\tAS {typeof(T).Name}) ";

            // Remove ", \n\t" from select
            // Example:
            // FROM ( SELECT DISTINCT [M02_DistributionRule].[id] AS 'id',
            // [M02_DistributionRule].[rule_name] AS 'rule_name',
            // ...
            // [M02_DistributionRule_Channel].[id] AS 'M02_DistributionRule_Channel_id',
            // [M02_DistributionRule_Channel].[rule_id] AS 'M02_DistributionRule_Channel_rule_id',
            // ...
            string selectToString = this.select.Remove(this.select.LastIndexOf(", \n\t"), 4);

            // Example:
            // include = "LEFT JOIN [M02_DistributionRule_Channel] ON M02_DistributionRule.id = M02_DistributionRule_Channel.rule_id
            //              LEFT JOIN [M02_DistributionRule_Location] ON M02_DistributionRule.id = M02_DistributionRule_Location.rule_id "
            // conditionToString = "WHERE [M02_DistributionRule_Channel].[channel_type] = 'facebook_comment'
            //                            AND [M02_DistributionRule].[tenant_id] = '1229B2AB-93C2-427A-88F3-B737FAFACA93' "
            // sortToString = "ORDER BY [M02_DistributionRule].modify_time DESC "
            if (this.is_inner_join)
                return "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED\n" +
                      $"\t{selectToString}\n" +
                      $"\t{this.from}\n" +
                      $"{(String.IsNullOrEmpty(this.include) ? "" : $"\t{this.include}\n")}" +
                      $"{innerQuery}\n" +
                      $"\t{conditionToString}\n" +
                      $"\t{sortToString}";
            else
                return "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED\n" +
                      $"\t{selectToString}\n" +
                      $"\t{this.from}\n" +
                      $"{(String.IsNullOrEmpty(this.include) ? "" : $"\t{this.include}\n")}" +
                      $"{innerQuery}\n" +
                      $"\t{sortToString}";
        }

        // Return list from query string
        public async Task<List<T>> ToListAsync()
        {
            string queryString = ToString();

            if (includeTypes.Count > 0)
            {
                Type type;

                // Create key to check if dynamic class is created
                string key = $"{typeof(T).Name} | ";
                foreach (Type includeType in includeTypes)
                {
                    key += $"{includeType.Name} + ";
                }

                // Value doesn't exist means dynamic class is not created, go ahead and create new dynamic class
                if (!Constants.CREATED_DYNAMIC_TYPE.TryGetValue(key, out type))
                {
                    // Build dynamic class to accept query
                    string assemblyName = "dynamicClass_" + Guid.NewGuid().ToString() + DateTime.Now.ToString("ddMMyyyyHms");
                    RuntimeClassBuilder MCB = new RuntimeClassBuilder(assemblyName);
                    type = MCB.CreateType(propertyNames, propertyTypes);

                    //  Add to dictionary
                    Constants.CREATED_DYNAMIC_TYPE.Add(key, type);
                }

                // Query
                DbRawSqlQuery query = dbContext.Database.SqlQuery(type, queryString, new string[0] { });

                var list = await query.ToListAsync();

                List<T> listResult = this.ParseList(list);

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                return listResult;
            }
            else
            {
                DbRawSqlQuery<T> query = dbContext.Database.SqlQuery<T>(queryString);

                return await query.ToListAsync();
            }
        }

        // Return count from query string
        public async Task<int> CountAsync()
        {
            // Remove "," from select
            string selectToString = $"SELECT COUNT(*)";

            // Remove ", \n\t\t\t" from innerSelect
            string innerQuery = this.innerSelect.Remove(this.innerSelect.LastIndexOf(", \n\t\t\t"), 6);
            innerQuery += "\n";

            // Add query by tenant_id at the last of conditions
            string conditionToString = conditions;
            if (!this.tenant_id.Equals(Guid.Empty))
            {
                if (!String.IsNullOrEmpty(conditions))
                    conditionToString += $") AND [{typeof(T).Name}].[tenant_id] = '{this.tenant_id}' ";
                else
                    conditionToString = $"WHERE [{typeof(T).Name}].[tenant_id] = '{this.tenant_id}' ";
            }
            else if (!String.IsNullOrEmpty(conditions))
                conditionToString += ") ";

            innerQuery += $"\t\t\t{this.from}\n" +
                $"{(String.IsNullOrEmpty(this.include) ? "" : $"\t\t\t{include}\n")}" +
                $"\t\t\t{conditionToString})\n" +
                $"\t\tAS {typeof(T).Name})";

            string queryString = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED\n" +
                                $"\t{selectToString}\n" +
                                $"\t{this.from}\n" +
                                innerQuery;
            DbRawSqlQuery<int> query = dbContext.Database.SqlQuery<int>(queryString);

            return (await query.ToListAsync()).FirstOrDefault();
        }

        private class totalClass
        {
            public int total { get; set; }
        }

        #region Helper func
        private void SelectBuilder(Type tableType, PropertyInfo[] properties)
        {
            if (properties.Length > 0)
            {
                // Handle when selecting properties from foreign (joined) table
                if (tableType != typeof(T))
                {
                    // Add all properties to SELECT statement
                    foreach (PropertyInfo property in properties)
                    {
                        this.select += $"[{tableType.Name}].[{property.Name}] AS '{tableType.Name}_{property.Name}', \n\t";

                        // Add to property list for dynamically creating class
                        this.propertyNames.Add($"{tableType.Name}_{property.Name}");
                        if (Nullable.GetUnderlyingType(property.PropertyType) == null && property.PropertyType != typeof(string))
                            this.propertyTypes.Add(typeof(Nullable<>).MakeGenericType(property.PropertyType));
                        else
                            this.propertyTypes.Add(property.PropertyType);
                    }

                    includeTypes.Add(tableType);
                }
                // Handle when selecting properties the primary table
                else
                {
                    // Add all properties to SELECT statement
                    foreach (PropertyInfo property in properties)
                    {
                        this.select += $"[{tableType.Name}].[{property.Name}] AS '{property.Name}', \n\t";
                        this.innerSelect += $"[{tableType.Name}].[{property.Name}] AS '{property.Name}', \n\t\t\t";

                        // Add to property list for dynamically creating class
                        this.propertyNames.Add($"{property.Name}");
                        this.propertyTypes.Add(property.PropertyType);
                    }
                }
            }
        }

        private List<T> ParseList(List<object> list)
        {
            if (list.Count == 0)
            {
                return new List<T>();
            }
            else
            {
                List<T> listResult = new List<T>();

                // Get properties of T
                // Properties that is not mapped to other tables
                List<PropertyInfo> tProperties = typeof(T).GetProperties().Where(m => m.GetAccessors()[0].IsFinal ||
                                                                        !m.GetAccessors()[0].IsVirtual).ToList();
                // Properties mapped to other tables
                List<PropertyInfo> tListProperties = typeof(T).GetProperties().Where(m => !m.GetAccessors()[0].IsFinal &&
                                                                        m.GetAccessors()[0].IsVirtual).ToList();

                // Group all list by T
                string[] groupByT = tProperties.Select(m => m.Name).ToArray();
                var listGroup = list.GroupByMany(groupByT).ToList();
                for (int i = 0; i < listGroup.Count; i++)
                {
                    // Initialize T
                    T tObject = (T)Activator.CreateInstance(typeof(T));

                    // Set all fields' value to T
                    List<object> items = listGroup[i].Items.ToDynamicList();
                    var firstItem = items.First();
                    if (tProperties.Count > 0)
                        foreach (PropertyInfo property in tProperties)
                        {
                            var value = firstItem.GetType().GetProperty(property.Name).GetValue(firstItem);
                            property.SetValue(tObject, value);
                        }

                    // Map join table to corresponding list in T
                    var loop = Parallel.For(0, includeTypes.Count, j =>
                    {
                        // Get the include type's properties
                        List<PropertyInfo> properties = includeTypes[j].GetProperties().Where(m => m.GetAccessors()[0].IsFinal ||
                                                                !m.GetAccessors()[0].IsVirtual).ToList();
                        string[] groupByMany = (from m in properties select ($"{includeTypes[j].Name}_{m.Name}")).ToArray();

                        // Get all records that have the same values accross all properties of corresponding include type
                        var groupList = items.GroupByMany(groupByMany).ToList();

                        if (groupList.Count > 0)
                        {
                            // Create an instance of corresponding list
                            var listToSet = Activator.CreateInstance(typeof(List<>).MakeGenericType(includeTypes[j]));

                            // Get method AddList
                            MethodInfo method = GetType().GetMethod("AddList");
                            method = method.MakeGenericMethod(new Type[] { includeTypes[j] });

                            // Add each item into the list
                            foreach (var groupItem in groupList)
                            {
                                bool isEmpty = false;

                                // Initialize object with type includeType[j]
                                var addObj = Activator.CreateInstance(includeTypes[j]);

                                // Get first item from group of duplicate? items
                                var firstInnerItem = groupItem.Items.ToDynamicList().FirstOrDefault();

                                // Add values to object
                                foreach (PropertyInfo property in properties)
                                {
                                    var value = firstInnerItem.GetType().GetProperty($"{includeTypes[j].Name}_{property.Name}").GetValue(firstInnerItem, null);

                                    // If the rule is NOT Nullable but the value = null
                                    // then the record is empty
                                    if (value == null &&
                                        Nullable.GetUnderlyingType(property.PropertyType) == null &&
                                        property.PropertyType != typeof(string))
                                    {
                                        isEmpty = true;
                                        break;
                                    }

                                    property.SetValue(addObj, value);
                                }

                                // Call method to add object to list
                                if (!isEmpty)
                                    listToSet = method.Invoke(this, new object[] { listToSet, addObj });
                            }

                            // Set the list instance to T
                            PropertyInfo typeToSet = (from m in tListProperties
                                                      where m.PropertyType.GetGenericArguments().FirstOrDefault() == includeTypes[j]
                                                      select m)
                                                      .FirstOrDefault();

                            typeToSet.SetValue(tObject, listToSet);
                        }
                    });

                    while (!loop.IsCompleted) { }

                    listResult.Add(tObject);
                }

                return listResult;
            }
        }

        public IList<V> AddList<V>(List<V> list, V itemToAdd)
        {
            list.Add(itemToAdd);
            return list;
        }
        #endregion
    }
}