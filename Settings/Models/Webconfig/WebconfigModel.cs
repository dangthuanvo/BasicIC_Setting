namespace BasicIC_Setting.Models.Webconfig
{
    public class DataAppSetting
    {
        public string key { get; set; }
        public string value { get; set; }

        public DataAppSetting() { }

        public DataAppSetting(string key, string value)
        {
            this.key = key;
            this.value = value;
        }
    }

    public class DataConnectionString
    {
        public string name { get; set; }
        public string connectionString { get; set; }

        public DataConnectionString() { }

        public DataConnectionString(string name, string connectionString)
        {
            this.name = name;
            this.connectionString = connectionString;
        }
    }
}