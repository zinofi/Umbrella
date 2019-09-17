using System.Web;

namespace Umbrella.Legacy.WebUtilities.Accessors.Abstractions
{
	// TODO v3.x
	internal interface IHttpContextAccessor
    {
        HttpContext HttpContext { get; set; }
    }
}