using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Umbrella.AspNetCore.Blazor.Components.Breadcrumb;

/// <summary>
/// A message useď by the <see cref="UmbrellaBreadcrumb"/> component to notify the <see cref="UmbrellaBreadcrumbRenderer"/> component
/// that the breadcrumb items have changed.
/// </summary>
public class UmbrellaBreadcrumbStateChangedMessage : ValueChangedMessage<IEnumerable<UmbrellaBreadcrumbItem>>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaBreadcrumbStateChangedMessage"/> class.
	/// </summary>
	/// <param name="value">The value that has changed.</param>
	public UmbrellaBreadcrumbStateChangedMessage(IEnumerable<UmbrellaBreadcrumbItem> value) : base(value)
	{
	}
}