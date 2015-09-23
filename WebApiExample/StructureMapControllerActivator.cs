using System;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using StructureMap;

namespace WebApiExample
{
    public class StructureMapControllerActivator : IHttpControllerActivator
    {
        public IHttpController Create(
           HttpRequestMessage request,
           HttpControllerDescriptor controllerDescriptor,
           Type controllerType)
        {
            var scope = request.GetDependencyScope();
            // GetService calls IContainer.GetInstance<>(); 
            // Container.GetInstance returns itself.
            var container = (IContainer)scope.GetService(typeof(IContainer));
            //Do something with the container that needs the request or descriptor
            return GetControllerFromContainer(container, controllerType);
        }

        private IHttpController GetControllerFromContainer(IContainer container, Type type)
        {
            IHttpController controller;
            // TryGetInstance does not return contcete types, 
            // ApiControllers are routinely registered without interfaces
            try
            {
                controller = container.GetInstance(type) as IHttpController;
            }
            catch (StructureMapException exe)
            {
                return null;
            }
            return controller;
        }
    }

}