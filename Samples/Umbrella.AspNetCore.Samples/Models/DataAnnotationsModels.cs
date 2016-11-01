using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Umbrella.DataAnnotations;

namespace Umbrella.AspNetCore.Samples.Models
{
    public class DataAnnotationsModel
    {
        [Required(ErrorMessage = "Please enter a value")]
        public string Required { get; set; }

        [UmbrellaPostcode(ErrorMessage = "Please enter a valid umbrella postcode")]
        public string UmbrellaPostcode { get; set; }

        [UmbrellaPhone(ErrorMessage = "Please enter a valid umbrella telephone")]
        public string UmbrellaPhone { get; set; }
    }
}