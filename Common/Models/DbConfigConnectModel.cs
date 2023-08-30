using System;

namespace Common.Models
{
    public class DbConfigConnectModel
    {
        public string ConnectionString { get; set; }
        public string NameDatabase { get; set; } // tenant id
        public string secret_key { get; set; }

        public DbConfigConnectModel(string connectionString, string nameDatabase)
        {
            if (!string.IsNullOrEmpty(connectionString) && !string.IsNullOrEmpty(nameDatabase))
            {
                this.ConnectionString = connectionString;
                this.NameDatabase = nameDatabase;
            }
            else
            {
                throw new Exception("Wrong token!(missing information of tenant id)");
            }
        }
    }
}
