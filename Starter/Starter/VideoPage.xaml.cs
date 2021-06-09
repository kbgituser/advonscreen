using Starter.Services;
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

namespace Starter
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VideoPage : ContentPage
    {
        //private string youtubeLink;
        private readonly YouTubeService _youTubeService;
        public VideoPage(string youtubeLink)
        {            
            InitializeComponent();
            _youTubeService = new YouTubeService();
            try
            {
                //GetVideoContent(youtubeLink);
                LoadVideo(youtubeLink);
            }
            catch (Exception ex)
            {
                Navigation.PopToRootAsync();
            }
            finally
            {
                MyActivityIndicator.IsRunning = false;
            }
        }


        public async void LoadVideo(string videoUrl)
        {
            MyActivityIndicator.IsRunning = true;
            var video = await _youTubeService.GetVideoFromUrlAsync(videoUrl);
            Title = video.Title;
            MyMediaElement.Source = video.VideoStreamUrl;
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

        //private async Task GetVideoContent(string youtubeLink)
        //{
        //    MyActivityIndicator.IsVisible = true;
        //    var youtube = new YoutubeClient();            

        //    try
        //    {
        //        var streamManifest = await youtube.Videos.Streams.GetManifestAsync(youtubeLink);
        //        //var streamInfo = streamManifest.GetVideoOnly().Where(v=>v.VideoQuality == streamManifest.GetVideoOnly().GetAllVideoQualities().Min()).WithHighestVideoQuality();                
        //        var streamInfo = streamManifest.GetMuxed().WithHighestVideoQuality();

        //        if (streamInfo != null)
        //        {
        //            // Get the actual stream
        //            //var stream = await youtube.Videos.Streams.GetAsync(streamInfo);
        //            MyMediaElement.Source = streamInfo.Url;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex);
        //    }
        //}



        public void MediaElement_MediaOpened(System.Object sender, System.EventArgs e)
        {
            MyActivityIndicator.IsVisible = false;
        }

        public void MediaElement_MediaFailed(System.Object sender, System.EventArgs e)
        {            
            MyActivityIndicator.IsVisible = false;
            MyMediaElement.Stop();
            Navigation.PopToRootAsync();            
        }

        
    }

}