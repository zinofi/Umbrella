using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbrella.WebUtilities.DynamicImage.Configuration;

namespace Umbrella.AspNetCore.WebUtilities.DynamicImage.Configuration
{
    public class DynamicImageOptions
    {
        public bool Enabled { get; set; }
        public List<DynamicImageMapping> Mappings { get; set; } = new List<DynamicImageMapping>();
    }
}