using System;
using System.Collections.Generic;
using System.Text;

namespace Umbrella.DynamicImage.Abstractions
{
    public class DynamicImageConfigurationOptions
    {
        public bool Enabled { get; set; }
        public List<DynamicImageMapping> Mappings { get; set; } = new List<DynamicImageMapping>();
    }
}