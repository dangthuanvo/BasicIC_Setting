using Common;
using Common.Commons;
using Common.Interfaces;
using Common.Params.Base;
using Consul;
using BasicIC_Setting.Config;
using BasicIC_Setting.KafkaComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BasicIC_Setting.Common
{
    public class ConsultClient
    {
        private static ConsulClient client = new ConsulClient();
        private static string projectName = HttpContext.Current.ApplicationInstance.GetType().BaseType.Assembly.GetName().Name;
        private static string address = ConfigManager.StaticGet(Constants.CONF_ADDRESS_SERVICE);
        private static string port = ConfigManager.StaticGet(Constants.CONF_PORT_SERVICE);
        private static string healthCheckService = ConfigManager.StaticGet(Constants.CONF_HEALTH_CHECK_SERVICE);
        private static string protocolService = ConfigManager.StaticGet(Constants.CONF_PROTOCOL_SERVICE);
        private static string sourceFabioService = ConfigManager.StaticGet(Constants.CONF_SOURCE_FABIO_SERVICE);
        private static string stateSource = ConfigManager.StaticGet(Constants.CONF_STATE_SOURCE);
        private static ILogger _logger = CommonFunc.CreateInstanceDJ<ILogger>();

        public static async Task<ResponseService<bool>> RegisterService(bool rootRegister = false)
        {
            try
            {
                _logger.LogInfo(CommonFunc.GetMethodName(new System.Diagnostics.StackTrace()));

                if (stateSource == Constants.STATE_SOURCE_PRODUCTION && !rootRegister) return new ResponseService<bool>("This is state production!").BadRequest();
                List<string> listAddressConsul = ConfigManager.StaticGet(Constants.CONF_ADDRESS_DISCOVERY_SERVICE).Split(',').ToList();

                foreach (string addressConsul in listAddressConsul)
                {
                    client.Config.Address = new Uri(addressConsul);
                    var registration = new AgentServiceRegistration()
                    {
                        ID = $"{projectName}-{address}:{port}",
                        Name = projectName,
                        Address = address,
                        Port = int.Parse(port),
                        Tags = new[] { $"urlprefix-/{sourceFabioService} strip=/{sourceFabioService}" },
                        Check = new AgentServiceCheck()
                        {
                            HTTP = $"{protocolService}://{address}:{port}/api/consult/health-check",
                            Timeout = TimeSpan.FromSeconds(100),
                            Interval = TimeSpan.FromSeconds(10)
                        }
                    };
                    await client.Agent.ServiceRegister(registration);
                }

                return new ResponseService<bool>(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ResponseService<bool>(ex);
            }
        }

        public static async Task<ResponseService<bool>> UnRegisterService(string id_service = null)
        {
            try
            {
                _logger.LogInfo(CommonFunc.GetMethodName(new System.Diagnostics.StackTrace()));

                if (stateSource == Constants.STATE_SOURCE_PRODUCTION) return new ResponseService<bool>("This is state production!").BadRequest();

                List<string> listAddressConsul = ConfigManager.StaticGet(Constants.CONF_ADDRESS_DISCOVERY_SERVICE).Split(',').ToList();

                foreach (string addressConsul in listAddressConsul)
                {
                    client.Config.Address = new Uri(addressConsul);
                    id_service = string.IsNullOrEmpty(id_service) ? $"{projectName}-{address}:{port}" : id_service;
                    await client.Agent.ServiceDeregister(id_service);
                }

                return new ResponseService<bool>(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ResponseService<bool>(ex);
            }
        }

        public static async Task<ResponseService<bool>> SendTopic(TopicParam param)
        {
            try
            {
                _logger.LogInfo(CommonFunc.GetMethodName(new System.Diagnostics.StackTrace()));

                if (stateSource == Constants.STATE_SOURCE_PRODUCTION) return new ResponseService<bool>("This is state production!").BadRequest();

                ProducerWrapper<Object> _producer = new ProducerWrapper<Object>();
                await _producer.CreateMess(param.topic, param.data);

                return new ResponseService<bool>(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ResponseService<bool>(ex);
            }
        }

        public static async Task<ResponseService<string>> GetInfoServiceConsult(string nameService)
        {
            try
            {
                _logger.LogInfo(CommonFunc.GetMethodName(new System.Diagnostics.StackTrace()));

                // infor service from consult
                client.Config.Address = new Uri(ConfigManager.StaticGet(Constants.CONF_ADDRESS_DISCOVERY_SERVICE));
                var services = await ConsultClient.client.Agent.Services();
                var services_health = await ConsultClient.client.Agent.Checks();

                foreach (var service in services.Response)
                {
                    // get status service
                    var healthService = services_health.Response.Where(item => item.Value.ServiceName == service.Value.Service).FirstOrDefault();
                    string statusService = healthService.Value == null ? "" : healthService.Value.Status.Status;
                    statusService = healthCheckService != "true" ? Constants.SERVICE_CONSULT_GOOD_HEALTH : statusService;

                    // get url service
                    if (service.Value.Service.Equals(nameService) && statusService.Equals(Constants.SERVICE_CONSULT_GOOD_HEALTH))
                    {
                        string baseUrl = $"{service.Value.Address}:{service.Value.Port}";
                        return new ResponseService<string> { data = baseUrl, status = true };
                    }
                }

                return new ResponseService<string>("No info service");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return new ResponseService<string>(ex);
            }
        }
    }
}
