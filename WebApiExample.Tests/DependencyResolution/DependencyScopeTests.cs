using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Web.Http.Hosting;
using NUnit.Framework;
using StructureMap;
using StructureMap.Graph;
using WebApiExample.DependencyResolution;

namespace WebApiExample.Tests.DependencyResolution
{
    namespace WebApiContrib.IoC.StructureMap.Tests
    {
        [TestFixture]
        public class DependencyScopeTests
        {
            [Test]
            public void Should_register_nested_container_for_disposal_when_created_with_default_activator()
            {
                var request = CreateRequestWithConfiguration();

                new DefaultHttpControllerActivator()
                    .Create(request,
                    new HttpControllerDescriptor(),
                    typeof(ContactsController));

                var resourcesToDispose = (List<IDisposable>)request.Properties[HttpPropertyKeys.DisposableRequestResourcesKey];
                Assert.NotNull(resourcesToDispose.SingleOrDefault(x => x is StructureMapDependencyScope));
            }


            [Test]
            public void Scope_registered_for_disposal_should_not_use_same_container_as_global_container()
            {
                var request = CreateRequestWithConfiguration();

                new DefaultHttpControllerActivator().Create(request,
                    new HttpControllerDescriptor(),
                    typeof(ContactsController));

                // Get service calls container.GetInstance. The container Returns itself
                var nestedContainer = request.GetDependencyScope().GetService(typeof(IContainer));
                Assert.AreNotSame(nestedContainer, ParentContainer);
            }

            private static HttpRequestMessage CreateRequestWithConfiguration()
            {
                var request = new HttpRequestMessage();
                var webApiConfiguration = new HttpConfiguration
                {
                    DependencyResolver = new StructureMapDependencyResolver(ParentContainer)
                };
                request.Properties[HttpPropertyKeys.HttpConfigurationKey] = webApiConfiguration;
                return request;
            }

            #region Set up Container
            private static readonly Lazy<IContainer> LazyContainer = new Lazy<IContainer>(() =>
            {
                var container = new Container();
                container.Configure(x =>
                {
                    x.For<IContactRepository>().Use<InMemoryContactRepository>();
                    x.Scan(y =>
                    {
                        y.TheCallingAssembly();
                        y.AddAllTypesOf<ApiController>();
                    }
                        );
                });
                return container;
            });


            private class ContactsController : ApiController
            {

            }

            private interface IContactRepository
            {
                
            }

            private class InMemoryContactRepository : IContactRepository
            {
                
            }

            private static IContainer ParentContainer { get { return LazyContainer.Value; } }

            #endregion


        }
    }
}