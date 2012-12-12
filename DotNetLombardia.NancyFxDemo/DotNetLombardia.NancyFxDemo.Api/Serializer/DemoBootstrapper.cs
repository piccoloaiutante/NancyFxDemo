using Nancy.Bootstrapper;
using Nancy.Hosting.Aspnet;

namespace DotNetLombardia.NancyFxDemo.Api.Serializer
{
    public class DemoBootstrapper : DefaultNancyAspNetBootstrapper
    {
        protected override NancyInternalConfiguration InternalConfiguration
        {
            get
            {
                return NancyInternalConfiguration.WithOverrides(c => c.Serializers.Insert(0, typeof(JsonNetSerializer)));
            }
        }
    }
}