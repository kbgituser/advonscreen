using System;
using System.Collections.Generic;
using System.Text;

namespace Starter.Models
{
    public class YouTubeVideo
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Url { get; set; }

        public string PreviewPictureUrl { get; set; }

        public string VideoStreamUrl { get; set; }
    }
}
