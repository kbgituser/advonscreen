using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ScreenChecker
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();            
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            viewSource.Html =  "Ширина - "+webView.Width + "; Высота - " + webView.Height+";";;
            webView.Source = viewSource;
            WaitAndExecute();
            //Task.Run(()=> WaitAndExecute());
        }

        protected async Task WaitAndExecute()
        {
            await Task.Delay(3000);
            viewSource.Html = "Ширина - " + webView.Width + "; Высота - " + webView.Height + ";"; ;
            webView.Source = viewSource;
        }
    }
}
