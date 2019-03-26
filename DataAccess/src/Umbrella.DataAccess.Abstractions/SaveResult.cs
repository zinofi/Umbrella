using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.DataAccess.Abstractions
{
	// TODO: V3 - Make use of this with EF repos.
	public class SaveResult<T>
	{
		public SaveResult()
		{
		}

		public SaveResult(bool success, T item, IReadOnlyCollection<ValidationResult> validationResults, bool? alreadyExists = null)
		{
			Success = success;
			Item = item;
			ValidationResults = validationResults;
			AlreadyExists = alreadyExists;
		}

		/// <summary>
		/// Specifies whether or not the attempt to save the item was successful.
		/// </summary>
		public bool Success { get; set; }

		/// <summary>
		///  Determine whether or not the item being saved already exists. This may be null where this check has not been carried out.
		/// </summary>
		public bool? AlreadyExists { get; set; }

		/// <summary>
		/// The item that has either been successfully saved or was attempted to be saved where saving failed.
		/// </summary>
		public T Item { get; set; }

		/// <summary>
		/// A list of validation results that contain messages detailing why it might be the case that the item couldn't be saved.
		/// </summary>
		public IReadOnlyCollection<ValidationResult> ValidationResults { get; set; }
	}
}