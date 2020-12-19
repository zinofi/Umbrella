using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices;

namespace Umbrella.DataAccess.Abstractions
{
	/// <summary>
	/// Encapsulates the result of a save operation on an item of type <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">The type of the item being saved.</typeparam>
	[StructLayout(LayoutKind.Auto)]
	public readonly struct SaveResult<T>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SaveResult{T}"/> struct.
		/// </summary>
		/// <param name="success">if set to <c>true</c> [success].</param>
		/// <param name="item">The item.</param>
		/// <param name="validationResults">The validation results.</param>
		/// <param name="alreadyExists">Specifies whether the item already exists.</param>
		public SaveResult(bool success, T item, IEnumerable<ValidationResult>? validationResults = null, bool? alreadyExists = null)
		{
			Success = success;
			Item = item;
			ValidationResults = validationResults?.ToArray() ?? Array.Empty<ValidationResult>();
			AlreadyExists = alreadyExists;
		}

		/// <summary>
		/// Specifies whether or not the attempt to save the item was successful.
		/// </summary>
		public bool Success { get; }

		/// <summary>
		/// Specifies whether or not the item being saved already exists. This may be null where this check has not been carried out or is not supported.
		/// </summary>
		public bool? AlreadyExists { get; }

		/// <summary>
		/// The item that has either been successfully saved or was attempted to be saved where saving failed.
		/// </summary>
		public T Item { get; }

		/// <summary>
		/// A list of validation results that contain messages detailing why it might be the case that the item couldn't be saved.
		/// </summary>
		public IReadOnlyCollection<ValidationResult> ValidationResults { get; }

		/// <summary>
		/// Gets the primary validation message which is the first message in the <see cref="ValidationResults"/> collection.
		/// </summary>
		public string? PrimaryValidationMessage => ValidationResults.FirstOrDefault()?.ErrorMessage;
	}
}