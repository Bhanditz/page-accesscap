using System.Web;

namespace PageAccessCap.Utils
{
    public class IPManager
    {
        public static string GetClientIP()
        {
            return GetClientIP(new HttpRequestWrapper(HttpContext.Current.Request));
        }
        public static string GetClientIP(HttpRequestBase requestContext)
        {
            string ipList = requestContext.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipList))
            {
                return ipList.Split(',')[0].Trim();
            }

            return requestContext.ServerVariables["REMOTE_ADDR"].Trim();
        }
    }
}