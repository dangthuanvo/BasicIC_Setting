using Common.Commons;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Configuration;

namespace Common.ApiHelper
{
    public class RestfulApi
    {
        public HttpClient client;
        private readonly string hostFabio = WebConfigurationManager.AppSettings["HOST_FABIO_SERVICE"];


        public RestfulApi() { }

        private void DefaultSetting()
        {
            // ServicePointManager.ServerCertificateValidationCallback = (send  er, cert, chain, sslPolicyErrors) => true;
            var handler = new HttpClientHandler { UseDefaultCredentials = true };
            client = new HttpClient(handler);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.
                Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        // rest api by consult
        public RestfulApi(ResponseService<string>
    confBaseUrl, string token = "", string preDomainApi = "/api/")
        {
            DefaultSetting();
            client.BaseAddress = new Uri(confBaseUrl.data + preDomainApi);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        // rest api by fabio
        public RestfulApi ToFabio(string sourceFabio, string token = "", string preDomainApi = "/api/")
        {
            DefaultSetting();
            client.BaseAddress = new Uri(hostFabio + sourceFabio + preDomainApi);
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return this;
        }

        public RestfulApi FullUri(string confBaseUrl, string token = "")
        {
            DefaultSetting();
            client.BaseAddress = new Uri(confBaseUrl);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return this;
        }
    }
}
