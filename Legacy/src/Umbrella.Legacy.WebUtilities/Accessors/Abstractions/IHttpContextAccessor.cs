using System.Web;

namespace Umbrella.Legacy.WebUtilities.Accessors.Abstractions
{
    public interface IHttpContextAccessor
    {
        HttpContext HttpContext { get; set; }
    }
}