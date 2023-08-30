using System.Web;

namespace BasicIC_Setting.Common
{
    public class SessionStore
    {
        public static string InitTenantId;
        private static string InitUserId;

        public static dynamic Get(string key)
        {
            if (HttpContext.Current != null && HttpContext.Current.Session != null && HttpContext.Current.Session[key] != null)
            {
                return HttpContext.Current.Session[key];
            }
            return null;
        }

        public static void Set(string key, object value)
        {
            if (HttpContext.Current != null && HttpContext.Current.Session != null)
            {
                HttpContext.Current.Session[key] = value;
            }
        }
    }
}