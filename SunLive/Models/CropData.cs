using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SunLive.Models
{
    public class CropData
    {
        public string ImgId { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int W { get; set; }
        public int H { get; set; }

        public int imageWidth { get; set; }

        public int imageHeight { get; set; }
    }
}