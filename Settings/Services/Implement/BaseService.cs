using AutoMapper;
using Common;
using Common.Interfaces;
using Common.Models;
using BasicIC_Setting.Common;
using BasicIC_Setting.Interfaces;
using System;
using System.Diagnostics;

namespace BasicIC_Setting.Services.Implement
{
    public class BaseService
    {
        protected readonly IConfigManager _config;
        protected readonly ILogger _logger;
        protected readonly IMapper _mapper;

        protected BaseService(IConfigManager config, ILogger logger, IMapper mapper)
        {
            _config = config;
            _logger = logger;
            _mapper = mapper;
        }

        protected DbConfigConnectModel GetConfigConnect()
        {
            string connectionString = _config.Get(Constants.CONF_HOST_MONGO_DB);
            string tenant_id = SessionStore.Get(Constants.KEY_SESSION_TENANT_ID) ?? SessionStore.InitTenantId;
            return new DbConfigConnectModel(connectionString, tenant_id);
        }

        protected DbConfigConnectModel GetConfigConnect(string tenantId, string secretKey)
        {
            string connectionString = _config.Get(Constants.CONF_HOST_MONGO_DB);
            string secretKeyConfig = _config.Get("");

            if (secretKey != secretKeyConfig)
            {
                throw new Exception("Wrong seret key!");
            }

            return new DbConfigConnectModel(connectionString, tenantId);
        }

        public static string GetMethodName(StackTrace stackTrace)
        {
            var method = stackTrace.GetFrame(0).GetMethod();

            string _methodName = method.DeclaringType.FullName;

            if (_methodName.Contains(">") || _methodName.Contains("<"))
            {
                _methodName = _methodName.Split('<', '>')[1];
            }
            else
            {
                _methodName = method.Name;
            }

            return _methodName;
        }
    }
}