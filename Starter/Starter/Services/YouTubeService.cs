using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Common;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;
using Starter.Models;
using System.Web;

namespace Starter.Services
{
    public class YouTubeService
    {
        private static readonly YoutubeClient _youtubeClient = new YoutubeClient();

        public static void Initialize()
        {
            // Make a call to YouTube for DNS cache initialization, i.e. faster subsequent requests...
            //_youtubeClient.Videos.Streams.GetManifestAsync(VideoId.Parse("_GuOjXYl5ew")).FireAndForget();
        }

        public async Task<YouTubeVideo> GetVideoFromUrlAsync(string url)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentException($"'{nameof(url)}' cannot be null or empty.", nameof(url));

            if (!IsYoutubeUrl(url))
                throw new ArgumentException($"'{nameof(url)}' is not a valid YouTube url.", nameof(url));


            var uri = new Uri(url);
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

            //var videoId = VideoId.Parse(url);


            var manifest = await _youtubeClient.Videos.Streams.GetManifestAsync(videoId);

            var stream = GetVideoStream(manifest);

            return new YouTubeVideo
            {
                Url = url,
                VideoStreamUrl = stream.Url
            };
        }

        private static Thumbnail GetVideoThumbnail(Video video)
        {
            if (video is null)
                throw new ArgumentNullException(nameof(video));

            var thumbnail = video.Thumbnails.OrderBy(x => x.Resolution.Height).FirstOrDefault(x => x.Resolution.Height >= 265) ?? video.Thumbnails.FirstOrDefault();

            return thumbnail;
        }

        private static IVideoStreamInfo GetVideoStream(StreamManifest manifest)
        {
            if (manifest is null)
                throw new ArgumentNullException(nameof(manifest));

            var allVideos = manifest.GetMuxedStreams();

            var stream = allVideos.OrderBy(x => x.VideoQuality.MaxHeight).FirstOrDefault(x => x.VideoQuality.IsHighDefinition) ?? allVideos.FirstOrDefault();

            if (stream == null)
                throw new Exception("Cannot find a playable video stream");

            return stream;
        }

        private static bool IsYoutubeUrl(string url)
        {
            var host = new Uri(url).Host;

            if (host == "www.youtube.com" || host == "youtube.com" || host == "youtu.be" || host == "www.youtube.be")
                return true;

            return false;
        }
    }
}
