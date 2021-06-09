using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;
using Starter.Services;

namespace Starter
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VideoPage2 : ContentPage
    {
        //private string youtubeLink;
        private readonly YouTubeService _youTubeService;

        public VideoPage2(string youtubeLink)
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

                string youtubeurl = GetYouTubeUrl(videoId);
                if (!string.IsNullOrEmpty(youtubeurl))
                {
                    MyMediaElement.Source = youtubeurl;
                }

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
            MyMediaElement.Stop();
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

            // это 2
            //using (var client = new HttpClient())
            //{
            //    var videoPageContent = client.GetStringAsync(videoInfoUrl).Result;
            //    var videoParameters = ParseQueryString(videoPageContent);
            //    var encodedStreamsDelimited = WebUtility.HtmlDecode(videoParameters["url_encoded_fmt_stream_map"]);
            //    var encodedStreams = encodedStreamsDelimited.Split(',');
            //    var streams = encodedStreams.Select(ParseQueryString);

            //    var stream = streams
            //        .OrderBy(s =>
            //        {
            //            var type = s["type"];
            //            if (type.Contains("video/mp4")) return 10;
            //            if (type.Contains("video/3gpp")) return 20;
            //            if (type.Contains("video/x-flv")) return 30;
            //            if (type.Contains("video/webm")) return 40;
            //            return int.MaxValue;
            //        })
            //        .ThenBy(s =>
            //        {
            //            var quality = s["quality"];

            //            switch (Device.Idiom)
            //            {
            //                case TargetIdiom.Phone:
            //                    return Array.IndexOf(new[] { "medium", "high", "small" }, quality);
            //                default:
            //                    return Array.IndexOf(new[] { "high", "medium", "small" }, quality);
            //            }
            //        })
            //        .FirstOrDefault();

            //    return stream["url"];
            //}
        }
        

        public void MediaElement_MediaOpened(System.Object sender, System.EventArgs e)
        {
            MyActivityIndicator.IsVisible = false;
        }

        public void MediaElement_MediaFailed(System.Object sender, System.EventArgs e)
        {            
            MyActivityIndicator.IsVisible = false;
            MyMediaElement.Stop();
            MyGrid.Children.Remove(MyMediaElement);
            Navigation.PopToRootAsync();
        }



        private Dictionary<string, string> ParseQueryString(string query)
        {
            return ParseQueryString(query, Encoding.UTF8);
        }

        private Dictionary<string, string> ParseQueryString(string query, Encoding encoding)
        {
            if (query == null)
                throw new ArgumentNullException("query");
            if (encoding == null)
                throw new ArgumentNullException("encoding");
            if (query.Length == 0 || (query.Length == 1 && query[0] == '?'))
                return new Dictionary<string, string>();
            if (query[0] == '?')
                query = query.Substring(1);

            var result = new Dictionary<string, string>();
            ParseQueryString(query, encoding, result);
            return result;
        }

        private void ParseQueryString(string query, Encoding encoding, Dictionary<string, string> result)
        {
            if (query.Length == 0)
                return;

            string decoded = System.Net.WebUtility.HtmlDecode(query);
            int decodedLength = decoded.Length;
            int namePos = 0;
            bool first = true;
            while (namePos <= decodedLength)
            {
                int valuePos = -1, valueEnd = -1;
                for (int q = namePos; q < decodedLength; q++)
                {
                    if (valuePos == -1 && decoded[q] == '=')
                    {
                        valuePos = q + 1;
                    }
                    else if (decoded[q] == '&')
                    {
                        valueEnd = q;
                        break;
                    }
                }

                if (first)
                {
                    first = false;
                    if (decoded[namePos] == '?')
                        namePos++;
                }

                string name, value;
                if (valuePos == -1)
                {
                    name = null;
                    valuePos = namePos;
                }
                else
                {
                    name = System.Net.WebUtility.UrlDecode(decoded.Substring(namePos, valuePos - namePos - 1));
                }
                if (valueEnd < 0)
                {
                    namePos = -1;
                    valueEnd = decoded.Length;
                }
                else
                {
                    namePos = valueEnd + 1;
                }
                value = System.Net.WebUtility.UrlDecode(decoded.Substring(valuePos, valueEnd - valuePos));

                result.Add(name, value);
                if (namePos == -1)
                    break;
            }
        }

    }

}