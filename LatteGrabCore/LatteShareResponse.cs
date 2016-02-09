using System;

namespace LatteGrabCore
{
    class LatteShareResponse
    {
        public bool success { get; set; }
        public String message { get; set; }
        public String key { get; set; }
        public String url { get; set; }
        public String error { get; set; }
    }
}
