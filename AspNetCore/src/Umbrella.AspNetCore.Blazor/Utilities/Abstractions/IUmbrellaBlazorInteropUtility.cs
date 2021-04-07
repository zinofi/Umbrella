using System.Threading.Tasks;

namespace Umbrella.AspNetCore.Blazor.Utilities.Abstractions
{
	public interface IUmbrellaBlazorInteropUtility
	{
		event AwaitableBlazorEventHandler OnWindowScrolledTop;

		ValueTask AnimateScrollToAsync(int scrollY, int offset = 0);
		ValueTask AnimateScrollToAsync(string elementSelector, int offset = 0);
		ValueTask AnimateScrollToBottomAsync();
		ValueTask SetPageTitleAsync(string pageTitle);
	}
}