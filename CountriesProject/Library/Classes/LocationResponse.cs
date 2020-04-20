using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Classes
{
    public class LocationResponse
    {
        public string AuthenticationResultCode { get; set; }
        public string BrandLogoUri { get; set; }
        public string Copyright { get; set; }
        public List<ResourceSet> ResourceSets { get; set; }
        public string StatusCode { get; set; }
        public string StatusDescription { get; set; }
        public string TraceId { get; set; }
    }
}
