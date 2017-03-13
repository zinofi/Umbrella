using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbrella.DynamicImage.Abstractions;

namespace Umbrella.AspNetCore.DynamicImage.Configuration
{
    public class DynamicImageOptions
    {
        public bool Enabled { get; set; }
        public List<DynamicImageMapping> Mappings { get; set; } = new List<DynamicImageMapping>();
    }
}