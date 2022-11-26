using System.Collections.Generic;

namespace Umbrella.Legacy.WebUtilities.WebApi.Validators;

/// <summary>
/// The response to be sent as part of the <see cref="StatusCodeValidationAttribute"/> HTTP response.
/// </summary>
public class ValidationResponse
{
	/// <summary>
	/// Initializes a new instance of the <see cref="ValidationResponse"/> class.
	/// </summary>
	/// <param name="message">The message.</param>
	/// <param name="modelState">State of the model.</param>
	public ValidationResponse(string message, Dictionary<string, List<string>> modelState)
	{
		Message = message;
		ModelState = modelState;
	}

	/// <summary>
	/// Gets the message.
	/// </summary>
	public string Message { get; }

	/// <summary>
	/// Gets the state of the model.
	/// </summary>
	public Dictionary<string, List<string>> ModelState { get; }
}