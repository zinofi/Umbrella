using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Umbrella.AspNetCore.WebUtilities.Mvc;

/// <summary>
/// Represents a validation problem details response that includes additional metadata for error handling and
/// diagnostics.
/// </summary>
/// <remarks>This class extends <see cref="ValidationProblemDetails"/> to provide extra properties such as an
/// error code and trace identifier. It is typically used in web APIs to convey detailed validation errors to clients,
/// enabling more precise error handling and improved traceability in distributed systems.</remarks>
public class UmbrellaValidationProblemDetails : ValidationProblemDetails
{
	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaValidationProblemDetails"/> class.
	/// </summary>
	public UmbrellaValidationProblemDetails()
	{
	}

	/// <summary>
	/// Initializes a new instance of the UmbrellaValidationProblemDetails class using the specified model state
	/// dictionary.
	/// </summary>
	/// <param name="modelState">The ModelStateDictionary containing the validation errors to be included in the problem details. Cannot be null.</param>
	public UmbrellaValidationProblemDetails(ModelStateDictionary modelState) : base(modelState)
	{
	}

	/// <summary>
	/// Initializes a new instance of the UmbrellaValidationProblemDetails class using the specified validation errors.
	/// </summary>
	/// <param name="errors">A dictionary containing validation errors, where each key is the name of the field and the value is an array of
	/// error messages associated with that field. Cannot be null.</param>
	public UmbrellaValidationProblemDetails(IDictionary<string, string[]> errors) : base(errors)
	{
	}

	/// <summary>
	/// Gets or sets a specific error code which can be used by the client
	/// to handle the error more precisely.
	/// </summary>
	public string? Code { get; set; }

	/// <summary>
	/// Gets or sets the trace id that can be used to identify the details
	/// associated with this problem in logs.
	/// </summary>
	public string? TraceId { get; set; }
}