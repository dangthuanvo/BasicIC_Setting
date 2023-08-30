using Common;
using Common.Commons;
using BasicIC_Setting.Config;
using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace BasicIC_Setting.Common
{
    public class RequestAPI
    {
        public HttpClient client;
        private readonly string hostFabio = ConfigManager.StaticGet(Constants.CONF_HOST_FABIO_SERVICE);

        public RequestAPI() { }

        private void DefaultSetting()
        {
            var handler = new HttpClientHandler { UseDefaultCredentials = true };
            client = new HttpClient(handler);

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.
                Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        // rest api by manual
        public RequestAPI(string confBaseUrl, string token = "")
        {
            DefaultSetting();
            client.BaseAddress = new Uri(ConfigManager.StaticGet(confBaseUrl));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        // rest api by consult
        public RequestAPI(ResponseService<string> confBaseUrl, string token = "", string preDomainApi = "/api/")
        {
            DefaultSetting();
            client.BaseAddress = new Uri(confBaseUrl.data + preDomainApi);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        // rest api by fabio
        public RequestAPI ToFabio(string sourceFabio, string token = "", string preDomainApi = "/api/")
        {
            DefaultSetting();
            client.BaseAddress = new Uri(hostFabio + sourceFabio + preDomainApi);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            if (string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Add("API_SECRET_KEY", ConfigManager.StaticGet("API_SECRET_KEY"));
            }
            return this;
        }
    }
}