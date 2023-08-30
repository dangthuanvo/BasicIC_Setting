using System;
using System.Reflection;

namespace BasicIC_Setting.Models.Main
{
    public class LogMessModel
    {
        public string topic { get; set; }
        public string mess { get; set; }
        public string service { get; set; }
        public DateTime create_time { get; set; }

        public LogMessModel(string topic, string mess)
        {
            this.topic = topic;
            this.mess = mess;
            this.service = Assembly.GetCallingAssembly().GetName().Name;
            this.create_time = DateTime.Now;
        }
    }

    public class LogSystemErrorModel
    {
        public string mess { get; set; }
        public string service { get; set; }
        public DateTime create_time { get; set; }
        public Exception ex { get; set; }

        public LogSystemErrorModel(Exception ex)
        {
            this.mess = ex.Message;
            this.service = Assembly.GetCallingAssembly().GetName().Name;
            this.create_time = DateTime.Now;
            this.ex = ex;
        }

        public LogSystemErrorModel(string mess)
        {
            this.mess = mess;
            this.service = Assembly.GetCallingAssembly().GetName().Name;
            this.create_time = DateTime.Now;
        }
    }
}