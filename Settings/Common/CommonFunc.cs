using BasicIC_Setting.App_Start;
using BasicIC_Setting.KafkaComponents;
using BasicIC_Setting.Models.Common;
using BasicIC_Setting.Models.KafkaModels;
using BasicIC_Setting.Models.Main;
using Common;
using Common.Interfaces;
using Common.Params.Base;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Ninject;
using Repository.CustomModel;
using Repository.EF;
using Repository.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace BasicIC_Setting.Common
{
    public static class CommonFunc
    {
        private static ILogger _logger = CreateInstanceDJ<ILogger>();
        private static List<M02_DefaultCommonSetting> ListDefaultCommonSettingData;

        /// <summary>
        /// Type: Common method
        /// Description: Method to create Depenency Injection instance
        /// </summary>
        /// <returns></returns>
        public static T CreateInstanceDJ<T>()
        {
            return NinjectWebCommon.kernel.Get<T>();
        }

        /// <summary>
        /// Type: Common method
        /// Description: Method to validate Bearer Token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        //public static async Task<bool> ValidateToken(string token)
        //{
        //    try
        //    {
        //        AuthenAPI _authenAPI = new AuthenAPI(token);
        //        _logger = new Logger();

        //        ResponseService<InfoAuthenModel> response = await _authenAPI.ValidateToken();
        //        if (response.status)
        //        {
        //            SessionStore.Set(Constants.KEY_SESSION_TENANT_ID, response.data.tenant_id);
        //            SessionStore.Set(Constants.KEY_SESSION_EMAIL, response.data.username);
        //            SessionStore.Set(Constants.KEY_SESSION_IS_ROOT, response.data.is_rootuser.ToString());
        //            SessionStore.Set(Constants.KEY_SESSION_TOKEN, token);
        //            SessionStore.Set(Constants.KEY_SESSION_CUSTOMER_TYPE, response.data.customer_type);

        //            return true;
        //        }
        //        else
        //        {
        //            _logger.LogError(response.message);
        //            return false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex);
        //        return false;
        //    }
        //}

        /// <summary>
        /// Type: Extension Method
        /// Description: Hàm kiểm tra access token 
        /// Owner: phipt
        /// Create log: 15.06.2023 - phipt 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static bool ValidateTokenV2(string token)
        {
            try
            {
                var principal = GetPrincipalFromToken(token);
                if (principal.claimsPrincipal == null)
                    return false;
                switch (principal.signatureAlgorithm)
                {
                    case "RS256":
                        var claimModel = new ClaimModel();
                        var strValue = principal.claimsPrincipal.Claims.FirstOrDefault(claim => claim.Type == "ic_claims")?.Value ?? "";
                        claimModel = JsonConvert.DeserializeObject<ClaimModel>(strValue);
                        SessionStore.Set(Constants.KEY_SESSION_TOKEN, token);
                        SessionStore.Set(Constants.KEY_SESSION_EMAIL, claimModel?.user_name);
                        SessionStore.Set(Constants.KEY_SESSION_TENANT_ID, claimModel?.tenant_id);
                        SessionStore.Set(Constants.KEY_SESSION_IS_ADMIN, claimModel?.is_administrator);
                        SessionStore.Set(Constants.KEY_SESSION_IS_ROOT, claimModel?.is_rootuser);
                        SessionStore.Set(Constants.KEY_SESSION_IS_SUPERVISOR, claimModel?.is_supervisor);
                        SessionStore.Set(Constants.KEY_SESSION_IS_AGENT, claimModel?.is_agent);
                        SessionStore.Set(Constants.KEY_SESSION_CUSTOMER_TYPE, "advance");
                        break;
                    case "HS256":
                        ClaimsIdentity identity = null;
                        identity = (ClaimsIdentity)principal.claimsPrincipal.Identity;
                        SessionStore.Set(Constants.KEY_SESSION_TOKEN, token);
                        SessionStore.Set(Constants.KEY_SESSION_EMAIL, principal.claimsPrincipal.Claims.First(claim => claim.Type == "username").Value);
                        SessionStore.Set(Constants.KEY_SESSION_TENANT_ID, principal.claimsPrincipal.Claims.First(claim => claim.Type == "tenant_id").Value);
                        SessionStore.Set(Constants.KEY_SESSION_IS_ADMIN, principal.claimsPrincipal.Claims.First(claim => claim.Type == "is_administrator").Value);
                        SessionStore.Set(Constants.KEY_SESSION_IS_ROOT, principal.claimsPrincipal.Claims.First(claim => claim.Type == "is_rootuser").Value);
                        SessionStore.Set(Constants.KEY_SESSION_IS_SUPERVISOR, principal.claimsPrincipal.Claims.First(claim => claim.Type == "is_supervisor").Value);
                        SessionStore.Set(Constants.KEY_SESSION_IS_AGENT, principal.claimsPrincipal.Claims.First(claim => claim.Type == "is_agent").Value);
                        SessionStore.Set(Constants.KEY_SESSION_CUSTOMER_TYPE, principal.claimsPrincipal.Claims.First(claim => claim.Type == "customer_type").Value);
                        break;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return false;
            }
        }

        /// <summary>
        /// Type: Extension Method
        /// Description: Hàm xử lý decrypt access token 
        /// Owner: phipt
        /// Create log: 15.06.2023 - phipt 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static GetPrincipalModel GetPrincipalFromToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenS = tokenHandler.ReadJwtToken(token);
                var signatureAlgorithm = tokenS.SignatureAlgorithm;
                var exp = tokenS.Claims.First(claim => claim.Type == "exp").Value;
                switch (signatureAlgorithm)
                {
                    case "RS256":
                        // handle SSO token
                        var tokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateAudience = false,
                            ValidateIssuer = false,
                            ValidateIssuerSigningKey = true,
                            ValidateLifetime = false,
                            IssuerSigningKeys = ConfigAuth.openIdConfig.SigningKeys //Key giải mã token  
                        };
                        SecurityToken securityToken;
                        long currentDate = ConvertToTimestamp(DateTime.UtcNow);
                        if (long.Parse(exp) >= currentDate)
                        {
                            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
                            return new GetPrincipalModel(principal, "RS256");
                        }
                        break;
                    case "HS256":
                        // handle local token
                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(WebConfigurationManager.AppSettings["SECRET_KEY"]));
                        var tokenValidationParametersLocal = new TokenValidationParameters
                        {
                            ValidateAudience = false,
                            ValidateIssuer = false,
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = key,
                            ValidateLifetime = false
                        };
                        SecurityToken securityTokenLocal;
                        var tokenSLocal = tokenHandler.ReadToken(token) as JwtSecurityToken;
                        var expLocal = tokenSLocal.Claims.First(claim => claim.Type == "exp").Value;
                        long currentDateLocal = ConvertToTimestamp(DateTime.UtcNow);
                        if (long.Parse(expLocal) >= currentDateLocal)
                        {
                            var principal = tokenHandler.ValidateToken(token, tokenValidationParametersLocal, out securityTokenLocal);
                            return new GetPrincipalModel(principal, "HS256");
                        }
                        break;
                }
                return new GetPrincipalModel(null, "");
            }
            catch
            {
                return new GetPrincipalModel(null, "");
            }
        }

        public static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Type: Extension Method
        /// Description: Hàm xử lý convert datetime to timestamp
        /// Owner: phipt
        /// Create log: 16.11.2022 - phipt 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static long ConvertToTimestamp(DateTime value)
        {
            TimeSpan elapsedTime = value - Epoch;
            return (long)elapsedTime.TotalSeconds;
        }


        /// <summary>
        /// Type: Common method
        /// Description: Method to get method name of current stacktrace
        /// </summary>
        /// <param name="stackTrace"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Type: Common method
        /// Description: Method to log exception to Kafka
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static async Task LogErrorToKafka(string topic, Exception ex)
        {
            //send log kafka from service handle kafka
            LogSystemErrorModel logMess = new LogSystemErrorModel(ex);
            ProducerWrapper<LogSystemErrorModel> _producer = new ProducerWrapper<LogSystemErrorModel>();
            await _producer.CreateMess(Topic.LOG_ERROR_SYSTEM, logMess, topic);
        }

        /// <summary>
        /// Type: Common method
        /// Description: Method to log error message to Kafka
        /// </summary>
        /// <param name="mess"></param>
        /// <returns></returns>
        public static async Task LogErrorToKafka(string mess)
        {
            //send log kafka from rest api
            LogSystemErrorModel logMess = new LogSystemErrorModel(mess);
            ProducerWrapper<LogSystemErrorModel> _producer = new ProducerWrapper<LogSystemErrorModel>();
            await _producer.CreateMess(Topic.LOG_ERROR_SYSTEM, logMess);
        }

        /// <summary>
        /// Type: Common method
        /// Description: Method to get all default common setting stored in DB
        /// </summary>           
        /// <returns></returns>
        public static async Task GetAllDefaultCommonSetting()
        {
            PagingParam param = new PagingParam();
            SettingsRepository<M02_DefaultCommonSetting> _defaultCommonSettingRepo = new SettingsRepository<M02_DefaultCommonSetting>();
            ListResult<M02_DefaultCommonSetting> responseCommonSettings = await _defaultCommonSettingRepo.GetAll(param);

            ListDefaultCommonSettingData = responseCommonSettings.items;
        }

        /// <summary>
        /// Type: Common method
        /// Description: Method to get default common setting stored in memory
        /// </summary>
        /// <param name="key"></param>             
        /// <returns></returns>
        public static string GetValueDefaultCommonSetting(string key)
        {
            if (ListDefaultCommonSettingData == null)
                Task.WaitAll(GetAllDefaultCommonSetting());

            var value = ListDefaultCommonSettingData.FirstOrDefault(q => q.key.Equals(key))?.value;
            if (string.IsNullOrEmpty(value))
                _logger.LogError("Default common setting error: key missing " + key);
            return value;
        }

        /// <summary>
        /// Type: Common method
        /// Description: Method to send kafka message for changes
        /// </summary>
        /// <param name="key"></param>             
        /// <returns></returns>        
        public static async Task CreateKafkaLog(Guid idReference, string logType, string objectName, string originalDataJson, string updateData = null)
        {
            // Don't log if using secret key
            bool isSecretKey = SessionStore.Get(Constants.KEY_SESSION_IS_SECRET_KEY);
            if (isSecretKey)
                return;

            // Create new model
            KafkaUserLogModel kafkaUserLog = new KafkaUserLogModel();
            kafkaUserLog.AddInfo();
            kafkaUserLog.message = "";

            // Set updateData to empty if null
            if (updateData == null)
                updateData = "";

            // Map data to model
            kafkaUserLog.status = true;
            kafkaUserLog.log_type = logType;
            kafkaUserLog.object_name = objectName;
            kafkaUserLog.service = "BasicIC_Setting";
            kafkaUserLog.reference_id = idReference;
            kafkaUserLog.original_data = originalDataJson;
            kafkaUserLog.update_data = updateData;

            // create producer
            ProducerWrapper<object> _producer = new ProducerWrapper<object>();
            await _producer.CreateMess(Topic.LOG_CRUD_SETTINGS, kafkaUserLog);
        }

        public static string ToDescription(this Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            if (attributes != null && attributes.Any())
            {
                return attributes.First().Description;
            }

            return value.ToString();
        }
    }
}