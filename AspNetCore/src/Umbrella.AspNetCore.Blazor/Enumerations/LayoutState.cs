namespace Umbrella.AspNetCore.Blazor.Enumerations;

/// <summary>
/// Represents the most common states that a component can be in during its lifecycle.
/// </summary>
public enum LayoutState
{
	/// <summary>
	/// A state that represents no content.
	/// </summary>
	None,

	/// <summary>
	/// The loading state. Primarily used to indicate progress during server communication.
	/// </summary>
	Loading,

	/// <summary>
	/// The saving state. Primarily used to prevent user interaction with the component during server communication.
	/// </summary>
	Saving,

	/// <summary>
	/// The success state. Primarily used the component has loaded successfully.
	/// </summary>
	Success,

	/// <summary>
	/// The error state. Primarily used when an error has occurred and a message needs to be displayed.
	/// </summary>
	Error,

	/// <summary>
	/// The empty state. Primarily used when there is no data to display, e.g. no results returned by a query.
	/// </summary>
	Empty
}