using System;

namespace Common.Interfaces
{
    public interface ILogger
    {
        void LogDebug(string message);
        void LogError(string message);
        void LogInfo(string message);
        void LogWarn(string message);
        void LogError(Exception ex);

    }
}
