
using System.Collections.Generic;

namespace GreenLightTracker.Src
{
    class Payload
    {
        public string type { get; set; }
        public string data { get; set; }
    }

    class PathAWSDTO
    {
        public string route_id { get; set; }
        public string ts { get; set; }
        public string payload { get; set; }
    }
}