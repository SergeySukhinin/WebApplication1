
using System.Text.RegularExpressions;

namespace WebApplication1.CustomConstraints
{
    public class CustomClientTypeConstraint : IRouteConstraint
    {
        public bool Match(HttpContext? httpContext, IRouter? route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
        {
            //check wether the value exists
            if (!values.ContainsKey(routeKey))
            { //client-type
                return false;
            }
            Regex reg = new Regex("^(physical|business|corporation)$");
            string? clientType = Convert.ToString(values[routeKey]);
            if (clientType!=null && reg.IsMatch(clientType))
            {
                return true;
            }
            return false;
        }
    }
}
