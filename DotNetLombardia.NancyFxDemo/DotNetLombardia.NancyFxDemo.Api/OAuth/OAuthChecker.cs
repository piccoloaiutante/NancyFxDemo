using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using DotNetLombardia.NancyFxDemo.Api.Models;
using Nancy;
using Nancy.Helpers;

namespace DotNetLombardia.NancyFxDemo.Api.OAuth
{
    internal class OAuthChecker
    {
        internal static dynamic CheckOAuth( NancyContext ctx)
        {
            try
            {
                string query = ctx.Request.Url.Query;
                var qs = HttpUtility.ParseQueryString(query);

                var auth = new OAuth();

                string clientID = qs.Get("cid");
                string clientSignature = qs.Get("cs");
                string clientTimestamp = qs.Get("cts");

                if (string.IsNullOrEmpty(clientID) || string.IsNullOrEmpty(clientSignature) || string.IsNullOrEmpty(clientTimestamp))
                    throw new Exception("You must supply valid authentication arguments to access method!");

                qs.Remove("cs");
                var original = ctx.Request.Url.SiteBase + ctx.Request.Url.Path + ConstructQueryString(qs);

                int ts = int.Parse(clientTimestamp);
                int now = int.Parse(auth.GenerateTimeStamp());
                if ((now - ts) > 60) throw new Exception("Invalid Timestamp");

                using (var db = new NorthwindEntities())
                {
                    var clientSecret = "j98byb78yg78n";

                    string normalized;
                    string normalizedParams;
                    var signature = auth.GenerateSignature(new Uri(original), clientID, clientSecret,
                        null, null, ctx.Request.Method, clientTimestamp, null, Api.OAuth.OAuth.SignatureTypes.HMACSHA1, out normalized, out normalizedParams);

                    if (!signature.Equals(clientSignature)) throw new Exception("Authentication failed!");
                }

                return ctx.Response;
            }
            catch (Exception ex)
            {
                return HttpStatusCode.Unauthorized;
            }
        }

        private static String ConstructQueryString(NameValueCollection parameters)
        {
            List<string> items = new List<string>();

            foreach (String name in parameters)
                items.Add(String.Concat(name, "=", System.Web.HttpUtility.UrlEncode(parameters[name])));

            return "?" + String.Join("&", items.ToArray());
        }
    }
}