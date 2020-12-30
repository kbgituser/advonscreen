using Android.Webkit;
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
        public int PointId = 2;
        public Cycle cycle;
        public bool Connected { get; set; }
        //string baseUri = "https://adv.kenseler.kz";
        string baseUri = "https://192.168.0.107:45455";
        //string baseUri = "https://10.0.2.2:45455";


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

            //uri = "https://adv.kenseler.kz/adsapi/";

            string uri = baseUri+ "/adsapi/";
            //uri = "https://192.168.0.106:45455/adsapi/";
            //uri = "https://adv.kenseler.kz/adsapi/";

            //uri = "https://10.0.2.2:44379/adsapi/";

            cycle = new Cycle(PointId, uri);
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

                    string uri = "https://192.168.0.105:45455/adsapi/";
                    string imgUri = "https://192.168.0.106:45455";

                    //CookieManager.Instance.RemoveAllCookie();
                    var html = "";
                    html += "<style>";
                    html += "html, body {margin:0px;padding:0px;}";

                    html += ".PointImage {";
                    html += "background-position: center center; ";
                    html += "max-width: 100%; max-height: 100vh; min-height: 100vh; min-width: 100%;";
                    html += "background-repeat: no-repeat; ";
                    html += "object-fit: contain; ";
                    
                    html += "vertical-align: middle";
                    

                    html += "}";

                    html += ".PointScreen {" +

                    //"height: 100vh;" +
                    "height: 100%;"+


                    //"height: " + cycle.CurrentAdvertisement.Point.Height + "px;" +
                    //"width: " + cycle.CurrentAdvertisement.Point.Width + "px;" +
                    //"min-height: " + cycle.CurrentAdvertisement.Point.Height + "px;" +
                    //"min-width: " + cycle.CurrentAdvertisement.Point.Width + "px;" +
                    //"display: inline-block;" +
                    "overflow: -moz-hidden-unscrollable;" +
                    "overflow: hidden;" +
                    "transform-origin: 0 0;" +
                    //"border: 2px solid;" +
                    "white-space: normal;" +
                    //"line-height: 0.8;" +                    
                    //"line-height: 1;" +                    
                    "background-color: " + cycle.CurrentAdvertisement.BackgroundColor +";"+

                    //"background-image: linear-gradient(to bottom,rgba(240, 255, 40, 1) 0%,"+
                    //    "rgba(240, 255, 40, 1) 100%),linear - gradient(to bottom,rgba(240, 40, 40, 1) 0%,rgba(240, 40, 40, 1) 100%);"+
                    //"background-clip: content-box, padding-box;"+

                    "font-size: " + cycle.CurrentAdvertisement.FontSize + "px;}"
                    + "p {line-height: 1; margin-bottom: 0; margin-top: 0; margin: 0;}"

                    ;

                    html += "</style>";
                    
                    //html += "Ширина - "+webView.Width + "; Высота - " + webView.Height+";";
                    if (cycle.CurrentAdvertisement.AdvertisementType == AdvertisementType.Text)
                    {
                        html += "<div class='PointScreen'>";
                        html += cycle.CurrentAdvertisement.Text;
                        html += "</div>";
                    }
                    else if (cycle.CurrentAdvertisement.AdvertisementType == AdvertisementType.Photo)
                    {
                        //html += "<div class='PointImage'>";
                        html += "<img src=\""+ baseUri
                        + cycle.CurrentAdvertisement.ImagePath 
                        + "\" " +
                        "class='PointImage' " +
                        "/>";
                        //html += "</div>";
                    }
                    else if (cycle.CurrentAdvertisement.AdvertisementType == AdvertisementType.Video)
                    {
                        html += cycle.CurrentAdvertisement.Video ;
                    }



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
