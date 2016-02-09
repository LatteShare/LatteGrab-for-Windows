using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
