using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Xam.Forms.VideoPlayer;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace Starter
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VideoPage3 : ContentPage
    {
        //private string youtubeLink;
        
        public VideoPage3(string youtubeLink)
        {            
            InitializeComponent();
            try
            {
                //GetVideoContent(youtubeLink);

                var uri = new Uri(youtubeLink);

                // you can check host here => uri.Host <= "www.youtube.com"

                var query = HttpUtility.ParseQueryString(uri.Query);

                var videoId = string.Empty;

                if (query.AllKeys.Contains("v"))
                {
                    videoId = query["v"];
                }
                else
                {
                    videoId = uri.Segments.Last();
                }

                

                UriVideoSource uriVideoSurce = new UriVideoSource()
                {
                    Uri = GetYouTubeUrl(videoId)
                };
                videoPlayer.Source = uriVideoSurce;
            }
            catch (Exception ex)
            {
                Navigation.PopToRootAsync();
            }            
        }

        //public void SetYoutubeLink(string youtubeLink)
        //{
        //    this.youtubeLink = youtubeLink;
        //}
        
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            
        }               

        public string GetYouTubeUrl(string videoId)
        {
            var videoInfoUrl = $"https://www.youtube.com/get_video_info?video_id={videoId}";

            // два метода либо 1 либо 2 
            // Это 1 
            using (var client = new HttpClient())
            {
                var videoPageContent = client.GetStringAsync(videoInfoUrl).Result;
                var videoParameters = HttpUtility.ParseQueryString(videoPageContent);
                var encodedStreamsDelimited1 = WebUtility.HtmlDecode(videoParameters["player_response"]);
                JObject jObject = JObject.Parse(encodedStreamsDelimited1);
                string url = (string)jObject["streamingData"]["formats"][0]["url"];
                return url;
            }
        }
    }
}