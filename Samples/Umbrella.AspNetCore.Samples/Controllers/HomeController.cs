using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Umbrella.AspNetCore.WebUtilities.Mvc;
using Umbrella.Utilities.Extensions;

namespace Umbrella.AspNetCore.Samples.Controllers
{
    public class HomeController : UmbrellaController
    {
        public HomeController(ILogger<HomeController> logger)
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

        public IActionResult Error()
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
