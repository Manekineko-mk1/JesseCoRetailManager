using Caliburn.Micro;
using JRMDesktopUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace JRMDesktopUI
{
    public class Bootstrapper : BootstrapperBase
    {
        // for dependency injection
        // This will handle the instantiation all the classes
        private SimpleContainer _container = new SimpleContainer();

        public Bootstrapper()
        {
            Initialize();
        }

        protected override void Configure()
        {
            // Whenever someone asks for a SimpleContainer instance, we will return the _container instance.
            _container.Instance(_container);

            // WindowManager : Bringing windows in and out
            // EventAggregator : We can pass event messages throughout our application, kinda like eventlistener
            _container
                .Singleton<IWindowManager, WindowManager>()
                .Singleton<IEventAggregator, EventAggregator>();

            // Use reflection on the current instance
            // From every types, filter to IsClass AND name = "ViewModel"
            // Return the result to a list
            // For each element in the result (viewModelType), use _container and call SimpleContainer.RegisterPerRequest()
            // RegisterPerRequest() => Register the class , so that a new instance is created for every request
            GetType().Assembly.GetTypes()
                .Where(type => type.IsClass)
                .Where(type => type.Name.EndsWith("ViewModel"))
                .ToList()
                .ForEach(viewModelType => _container.RegisterPerRequest(
                    viewModelType, viewModelType.ToString(), viewModelType));
        }

        // When the application starts, it will call the ShellViewModel
        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<ShellViewModel>();
        }

        // When we pass in a type and a name (key)
        // Caliburn will get us that instance
        protected override object GetInstance(Type service, string key)
        {
            return _container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.GetAllInstances(service);
        }

        // This is uses to construct things
        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }
    }
}
