using BasicIC_Setting.CustomAttributes;
using System.Web.Http;

namespace BasicIC_Setting
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }

            );

            config.Filters.Add(new ValidateModelAttribute());

        }
    }
}
