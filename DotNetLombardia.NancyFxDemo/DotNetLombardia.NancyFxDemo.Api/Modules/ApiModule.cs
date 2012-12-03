using System;
using System.Collections.Generic;
using System.Dynamic;
using Nancy;
using Nancy.Security;

namespace DotNetLombardia.NancyFxDemo.Api.Modules
{
    public class ApiModule : NancyModule
    {
        public ApiModule()
        {
            //this.RequiresAuthentication();

            Get[@"/api"] = r =>
            {
                IList<object> services = new List<object>();
                
                return Response.AsJson(services);
            };
        }

    }
}