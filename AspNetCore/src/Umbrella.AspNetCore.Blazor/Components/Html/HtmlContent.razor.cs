namespace Umbrella.AspNetCore.Blazor.Components.Html;

/// <summary>
/// A component that can be used to render raw HTML content.
/// </summary>
/// <seealso cref="ComponentBase" />
public partial class HtmlContent
{
	/// <summary>
	/// Gets or sets the value of the HTML content to be displayed.
	/// </summary>
	[Parameter]
	public string? Value { get; set; }
}