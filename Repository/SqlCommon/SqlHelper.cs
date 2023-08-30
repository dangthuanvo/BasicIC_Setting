using Repository.EF;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Threading.Tasks;

namespace Repository.SqlCommon
{
    public static class SqlHelper
    {
        public static object ExecuteStoredProcedureScalar(string storedProcedureName, SqlParameter[] parameters)
        {
            object returnValue;
            using (var dbcontext = new M02_BasicEntities())
            {
                dbcontext.Database.ExecuteSqlCommand("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED");
                using (var cmd = dbcontext.Database.Connection.CreateCommand())
                {
                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();
                    cmd.CommandText = storedProcedureName;
                    cmd.CommandType = CommandType.StoredProcedure;
                    foreach (var parameter in parameters)
                    {
                        cmd.Parameters.Add(parameter);
                    }

                    var returnParameter = cmd.CreateParameter();
                    returnParameter.ParameterName = "@return_value";
                    returnParameter.Direction = ParameterDirection.Output;
                    returnParameter.Size = -1;
                    cmd.Parameters.Add(returnParameter);

                    cmd.ExecuteScalar();
                    cmd.Connection.Close();

                    returnValue = returnParameter.Value;
                }
            }
            return returnValue;
        }

        public static async Task<DataSet> ExecuteStoredProcedure(string storedProcedureName, SqlParameter[] parameters)
        {
            DataSet dataSet = new DataSet();
            using (var dbcontext = new M02_BasicEntities())
            {
                dbcontext.Database.ExecuteSqlCommand("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED");
                using (var cmd = dbcontext.Database.Connection.CreateCommand())
                {
                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();
                    cmd.CommandText = storedProcedureName;
                    cmd.CommandType = CommandType.StoredProcedure;
                    foreach (var parameter in parameters)
                    {
                        cmd.Parameters.Add(parameter);
                    }

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        do
                        {
                            DataTable dt = new DataTable();
                            dt.Load(reader);
                            dataSet.Tables.Add(dt);
                        }
                        while (!reader.IsClosed);
                    }
                }
            }
            return dataSet;
        }

        public static async Task<DataSet> ExecuteQuery(string query)
        {
            DataSet dataSet = new DataSet();
            using (var dbcontext = new M02_BasicEntities())
            {
                dbcontext.Database.ExecuteSqlCommand("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED");
                using (var cmd = dbcontext.Database.Connection.CreateCommand())
                {
                    if (cmd.Connection.State != ConnectionState.Open)
                        cmd.Connection.Open();
                    cmd.CommandText = query;
                    cmd.CommandType = CommandType.Text;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        do
                        {
                            DataTable dt = new DataTable();
                            dt.Load(reader);
                            dataSet.Tables.Add(dt);
                        }
                        while (!reader.IsClosed);
                    }
                }
            }
            return dataSet;
        }

        /// <summary>
        /// Hàm convert data table to list dynamic
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<dynamic> ToDynamic(this DataTable dt)
        {
            var dynamicDt = new List<dynamic>();
            foreach (DataRow row in dt.Rows)
            {
                dynamic dyn = new ExpandoObject();
                dynamicDt.Add(dyn);
                foreach (DataColumn column in dt.Columns)
                {
                    var dic = (IDictionary<string, object>)dyn;
                    dic[column.ColumnName] = string.IsNullOrEmpty(row[column].ToString()) ? "" : row[column];
                }
            }
            return dynamicDt;
        }
    }
}
