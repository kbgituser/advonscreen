using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Starter
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();            
        }
        public bool Connected { get; set; }


        protected override void OnStart()
        {
            //Device.OpenUri(new Uri("http://scr.kenseler.kz"));
            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
            Connected = Connectivity.NetworkAccess == NetworkAccess.Internet;
        }

        protected override void OnSleep()
        {
            //System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow();
        }

        protected override void OnResume()
        {
        }

        void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            Connected = e.NetworkAccess == NetworkAccess.Internet;            
        }
    }
}
