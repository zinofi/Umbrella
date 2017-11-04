using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Umbrella.AspNetCore.WebUtilities.Mvc;

namespace Umbrella.AspNetCore.Samples.Controllers
{
    public class DynamicImageController : UmbrellaController
    {
        public DynamicImageController(ILogger<DynamicImageController> logger)
            : base(logger)
        {
        }

        public IActionResult Index()
        {
            try
            {
                return View();
            }
            catch (Exception exc) when (Log.WriteError(exc))
            {
                throw;
            }
        }
    }
}