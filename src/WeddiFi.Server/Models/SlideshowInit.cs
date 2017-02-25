using System;

namespace WeddiFi.Server.Models
{
    public class SlideshowInit
    {
        public SlideshowInit(string file, double rotationMs)
        {
            File = file;
            RotationMs = (int)Math.Ceiling(rotationMs);
        }

        public string File { get; }

        public int RotationMs { get; set; }
    }
}