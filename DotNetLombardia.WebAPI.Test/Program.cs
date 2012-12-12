using System;
using System.IO;
using System.Net;
using System.Web;
using DotNetLombardia.WebAPI.API.Code;

namespace DotNetLombardia.WebAPI.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            string consumerKey = "na";
            string consumerSecret = "j98byb78yg78n";

            string local = "http://localhost:1718";            
            string url, param;
            var oAuth = new OAuth();
            var ts = oAuth.GenerateTimeStamp();
            var uri = new Uri(string.Format("{0}/api/categories?limit=3&offset=2&cid={1}&cts={2}", local, consumerKey, ts));


            var signature = oAuth.GenerateSignature(uri, consumerKey, consumerSecret,
                string.Empty, string.Empty, "GET", ts, null, OAuth.SignatureTypes.HMACSHA1, out url, out param);
            url = string.Format("{0}&cs={1}", uri, HttpUtility.UrlEncode(signature));
            WebResponse webrespon = (WebResponse)WebRequest.Create(url).GetResponse();
            StreamReader stream = new StreamReader(webrespon.GetResponseStream());
            Console.WriteLine(stream.ReadToEnd());
            Console.Read();
        }
    }
}
