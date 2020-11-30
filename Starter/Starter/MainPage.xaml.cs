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

            var test = "sdfsdf";
            //StringBuilder htmlStr = new StringBuilder("");
            //htmlStr.Append("<html><body>");
            //htmlStr.Append("<head>");
            //string l = "<link rel=\"stylesheet\" href=\"https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/css/bootstrap.min.css\" integrity=\"sha384-ggOyR0iXCbMQv3Xipma34MD+dH/1fQ784/j6cY/iJTQUOhcWr7x9JvoRxT2MZw1T\" crossorigin=\"anonymous\">";
            //htmlStr.Append(l); htmlStr.Append("</head>");
            //htmlStr.Append("<div>");
            //htmlStr.Append("<p style='line-height: 1; 'align='right'>;ijpoij арное ено  окн оно</p><p style='line-height: 1;' align='left'> ноен ооен о нено еноен оasdfasdfasdfгшщгг</p> <p style='line-height: 1;' align='left'>ншшн нгш нгшшнгш </p><p style='line-height: 1; ' align='left'>нгшнгшнгш нгшнг шнгш нш <br>ншн ыфаыфлафыа ывафы ыаф <br></p><p><br></p>");
            //string l2 = "<p align =\"justify\">шарфы книги колеса укшугзц уклоуоу&nbsp; жыаофыоа фжа ыал&nbsp; ылаоф аыа фжафы авао а вф<font color=\"#E76363\">ао фыаофы фы афыа офыа&nbsp;</font> </p><p align=\"center\">фжав<br></p><p></p><p style=\"line-height: 1;\" align=\"center\"><span style=\"background-color: rgb(255, 0, 0);\"><font color=\"#0000FF\">Этаж 34</font></span><br></p><p style=\"line-height: 1;\" align=\"center\">Телефон<font color=\"#083139\"> <font color=\"#21104A\">8 <font color=\"#CE0000\">888 888 88 88</font>,</font> 8 111 111 11 11</font><br></p><p style=\"line-height: 1;\" align=\"center\"><span style=\"background-color: transparent;\"><font color=\"#CE0000\">Бутик </font></span>3523<br></p><p></p>";
            //    htmlStr.Append(l2);
            //htmlStr.Append("</div></body></html>");

            //string text = "<b><p style='align: right;'>sdfafasf</p></b> sladfjsaf <u>trrgrtgr</u>";



            //webView.Source = new HtmlWebViewSource { Html = htmlStr.ToString() };


            //this.Content = new StackLayout { Children = { webView } };
            //test();
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
            //webView.Source = new UrlWebViewSource { Url = urlEntry.Text };
            // или так
            // webView.Source = urlEntry.Text;
            test();
        }

        public void RefreshText() 
        {

            Device.BeginInvokeOnMainThread(() =>
            {
                // UI interaction here
                try
                {
                    viewSource.Html = cycle.advText;
                    //viewSource = new HtmlWebViewSource() { Html = cycle.advText };
                    webView.Source = viewSource;
                }
                catch (Exception e)
                {
                    var t = e;
                    viewSource.Html = e.Message;
                    //var htmlSource = new HtmlWebViewSource() { Html = e.Message };
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
                //var htmlSource = new HtmlWebViewSource() { Html = text };
                //htmlSource.Html = text;
                //webView.Source = htmlSource;
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
