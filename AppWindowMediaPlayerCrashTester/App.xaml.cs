using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.WindowManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace AppWindowMediaPlayerCrashTester
{
    sealed partial class App : Application
    {
        public new static App Current => (App)Application.Current;
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();


            return services.BuildServiceProvider();
        }

        private void Initialize()
        {
            Frame mainFrame = Window.Current.Content as Frame;
            if (mainFrame == null)
            {
                mainFrame = new Frame();
                mainFrame.NavigationFailed += OnNavigationFailed;
                mainFrame.Navigate(typeof(MainPage), null);
            }
            Window.Current.Content = mainFrame;
            Window.Current.Activate();

            new Action(async () =>
            {
                var appWindow = await AppWindow.TryCreateAsync();
                appWindow.Title = "secondary";

                Frame appWindowFrame = new Frame();
                appWindowFrame.NavigationFailed += OnNavigationFailed;
                appWindowFrame.Navigate(typeof(SecondaryPage), null);

                ElementCompositionPreview.SetAppWindowContent(appWindow, appWindowFrame);

                await appWindow.TryShowAsync();
            })();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e) => this.Initialize();

        protected override void OnActivated(IActivatedEventArgs args) => this.Initialize();

        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        private void OnEnteredBackground(object sender, EnteredBackgroundEventArgs e)
        {
            var deferral = e.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}
