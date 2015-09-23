using System;
using System.Web;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using StructureMap;
using StructureMap.Graph;
using WebApiExample.DependencyResolution;

namespace WebApiExample
{
    public class WebApiApplication : HttpApplication
    {
        public static IContainer Container { get; private set; }
        
        protected void Application_Start()
        {
            //Application_Start is called once so no need to lock.
            Container = new Container();
            Container.Configure(x =>
            {
                //Register Dependencies
                x.Scan(y =>
                {
                    y.TheCallingAssembly();
                    y.AddAllTypesOf<ApiController>();
                });
                //Optionally Register IControllerActivator
                //For<IControllerActivator>.Use<StructureMapControllerActivator>();
            });
            GlobalConfiguration.Configure(WebApiConfig.Register);
            GlobalConfiguration.Configuration.DependencyResolver = new StructureMapDependencyResolver(Container);
            //If you are not implementing the dependency resolver at all but wish to use your own ControllerActivator
            //You can register it thusly:
            //GlobalConfiguration.Configuration.Services.Replace(typeof(IHttpControllerActivator), new StructureMapControllerActivator());
        }

        protected void Application_End()
        {
            //Dispose of containers to make sure Singleton Scoped dependencies get disposed 
            Container.Dispose();
        }
    }

}