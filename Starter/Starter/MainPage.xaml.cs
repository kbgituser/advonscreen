using Newtonsoft.Json;
using Starter.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Starter
{
    public partial class MainPage : ContentPage
    {
        //WebView webView;
        //Entry urlEntry;
        
        //public HtmlWebViewSource viewSource;
        public int PointId = 1;
        public Cycle cycle;
        public bool Connected { get; set; }

        public MainPage()
        {
            InitializeComponent();
            
            //Task.Run(() => cycle.StartShow());// работает
            //Task.Run(() => cycle.StartShowAsync());// тоже работает
            //cycle.StartShowAsync(); // тоже работает
            //cycle.StartShow();
            //Task.Factory.StartNew(() => cycle.StartShow());
            //cycle.StartShow2();
            //Task.Run(cycle.StartShow);
            //Task.Run(()=> cycle.StartShow());
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;
            Connected = Connectivity.NetworkAccess == NetworkAccess.Internet;
            string uri = "https://adv.kenseler.kz/adsapi/";            
            
            cycle = new Cycle(1, uri);
            cycle.onAdvChange += RefreshText;
            cycle.Init();
            cycle.InternetConnected = (Connectivity.NetworkAccess == NetworkAccess.Internet);
            cycle.StartShowAsync(); // тоже работает
        }

        void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            Connected = e.NetworkAccess == NetworkAccess.Internet;
            cycle.InternetConnected= e.NetworkAccess == NetworkAccess.Internet;
            if(cycle.InternetConnected) cycle.StartShowAsync();
        }

        void button_Clicked(object sender, EventArgs e)
        {            
            test();
        }

        public void RefreshText() 
        {

            Device.BeginInvokeOnMainThread(() =>
            {
                // UI interaction here
                try
                {
                    var html = "";
                    html += "<style>";
                    html += ".PointScreen {" +

                    //"height: 100vh;" +
                    //"width: 100vw;" +

                    //"height: " + cycle.CurrentAdvertisement.Point.Height + "px;" +
                    //"width: " + cycle.CurrentAdvertisement.Point.Width + "px;" +
                    //"min-height: " + cycle.CurrentAdvertisement.Point.Height + "px;" +
                    //"min-width: " + cycle.CurrentAdvertisement.Point.Width + "px;" +
                    //"display: inline-block;" +
                    //"overflow: hidden;" +
                    "font-size: " + cycle.CurrentAdvertisement.FontSize + "px;}";
                    html += "</style>";
                    html += "<div class='PointScreen'>";
                    //html += "Ширина - "+webView.Width + "; Высота - " + webView.Height+";";

                    html += cycle.CurrentAdvertisement.Text;
                    html += "</div>";
                    

                    viewSource.Html = html;
                    
                    webView.Source = viewSource;
                }
                catch (Exception e)
                {
                    var t = e;
                    viewSource.Html = e.Message;                    
                    webView.Source = viewSource;
                }
            });
        }

        public async void test()
        {                  
            string url = "https://adv.kenseler.kz/adsapi1/getadvertisement/1/1";
            Uri geturi = new Uri(url);

            var httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback =
            (message, cert, chain, errors) => { return true; };
            
            var client = new HttpClient(httpClientHandler);
            
            try
            {                
                HttpResponseMessage responseGet = await client.GetAsync(geturi);

                var json = await responseGet.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<Advertisement>(json);
                var text = result.Text;
                
                viewSource.Html = text;
                webView.Source = viewSource;
            }

            catch (TaskCanceledException e)
            {
                // Do something here ?
                var t = e;
                var htmlSource = new HtmlWebViewSource() { Html = e.Message };
                webView.Source = htmlSource;
            }

            catch (TimeoutException e)
            {
                // handle somehow                
                var t = e;
                var htmlSource = new HtmlWebViewSource() { Html = e.Message };
                webView.Source = htmlSource;
            }
            catch (HttpRequestException e)
            {
                //throw new HttpRequestException("Соединение с сетью недоступно. Проверьте соединение.");
                var t = e;
                var htmlSource = new HtmlWebViewSource() { Html = e.Message };
                webView.Source = htmlSource;
            }

            catch (Exception e)
            {
                var t = e;
                var htmlSource = new HtmlWebViewSource() { Html = e.Message };                
                webView.Source = htmlSource;
            }            
        }
    }
}
