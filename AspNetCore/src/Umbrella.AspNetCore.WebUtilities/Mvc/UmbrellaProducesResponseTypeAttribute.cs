using Microsoft.AspNetCore.Mvc;

namespace Umbrella.AspNetCore.WebUtilities.Mvc;

/// <summary>
/// An attribute that specifies the response type for a controller action.
/// </summary>
public sealed class UmbrellaProducesResponseTypeAttribute : ProducesResponseTypeAttribute
{
	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaProducesResponseTypeAttribute"/> class.
	/// </summary>
	/// <param name="statusCode">The status code.</param>
	public UmbrellaProducesResponseTypeAttribute(int statusCode)
		: base(statusCode)
	{
		Type = statusCode == 400 ? typeof(UmbrellaValidationProblemDetails) : typeof(UmbrellaProblemDetails);
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaProducesResponseTypeAttribute"/> class.
	/// </summary>
	/// <param name="type">The type.</param>
	/// <param name="statusCode">The status code.</param>
	public UmbrellaProducesResponseTypeAttribute(Type type, int statusCode)
		: base(type, statusCode)
	{
		Type = statusCode == 400 ? typeof(UmbrellaValidationProblemDetails) : typeof(UmbrellaProblemDetails);
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaProducesResponseTypeAttribute"/> class.
	/// </summary>
	/// <param name="type">The type.</param>
	/// <param name="statusCode">The status code.</param>
	/// <param name="contentType">The content type.</param>
	/// <param name="additionalContentTypes">Additional content types.</param>
	public UmbrellaProducesResponseTypeAttribute(Type type, int statusCode, string contentType, params string[] additionalContentTypes)
		: base(type, statusCode, contentType, additionalContentTypes)
	{
		Type = statusCode == 400 ? typeof(UmbrellaValidationProblemDetails) : typeof(UmbrellaProblemDetails);
		ContentType = contentType;
		AdditionalContentTypes = additionalContentTypes;
	}

	/// <summary>
	/// Gets the media type of the content associated with the current instance.
	/// </summary>
	public string? ContentType { get; }

	/// <summary>
	/// Gets additional content types associated with the current instance.
	/// </summary>
	public IReadOnlyCollection<string> AdditionalContentTypes { get; } = [];
}