using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Activation;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Caliburn.Micro;
using Microsoft.EntityFrameworkCore;
using TodoistDemo.Core.Communication;
using TodoistDemo.Core.Services;
using TodoistDemo.Core.Storage.Database;
using TodoistDemo.ViewModels;
using TodoistDemo.Views;

namespace TodoistDemo
{
    public sealed partial class App
    {
        private WinRTContainer _container;

        public App()
        {
            InitializeComponent();
            UnhandledException += AppUnhandledException;
            using (var db = new TodoistContext())
            {
                db.Database.Migrate();
            }
        }

        private async void AppUnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            await new MessageDialog("A aparut o eroare, va rugam reincercati " + e.Exception.Message).ShowAsync();
        }

        protected override void Configure()
        {
            _container = new WinRTContainer();

            _container.RegisterWinRTServices();

            _container.RegisterPerRequest(typeof(IRestClient), "RestClient", typeof(RestClient));
            _container.RegisterPerRequest(typeof(IAccountManager), "AccountManager", typeof(AccountManager));

            _container.PerRequest<ItemsViewModel>();
        }

        protected override void PrepareViewFirst(Frame rootFrame)
        {
            _container.RegisterNavigationService(rootFrame);
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            DisplayRootView<ItemsView>();
        }

        protected override object GetInstance(Type service, string key)
        {
            return _container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }
    }
}