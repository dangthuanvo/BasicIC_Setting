using Common.Interfaces;
using Newtonsoft.Json;
using System;

namespace Common.Commons
{
    public class Logger : ILogger
    {
        public static readonly log4net.ILog logger = log4net.LogManager.GetLogger
                                          (System.Reflection.MethodBase.
                                          GetCurrentMethod().DeclaringType);
        public void LogDebug(string message)
        {
            logger.Debug(message);
        }
        public void LogError(string message)
        {
            logger.Error(message);
        }
        public void LogInfo(string message)
        {
            logger.Info(message);
        }
        public void LogWarn(string message)
        {
            logger.Warn(message);
        }
        public void LogError(Exception ex)
        {
            logger.Error(JsonConvert.SerializeObject(ex));
        }
    }
}
