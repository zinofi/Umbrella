using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.Extensions.Localization;
using Umbrella.DataAnnotations;

namespace Umbrella.AspNetCore.DataAnnotations
{
	/// <summary>
	/// A custom validation adapter provider to allow custom <see cref="ValidationAttribute"/>s to be registered with the ASP.NET Core infrastructure. The primary purpose of this
	/// is to enable client-side validation using the default validation mechanism built into ASP.NET Core.
	/// </summary>
	/// <seealso cref="IValidationAttributeAdapterProvider" />
	internal class UmbrellaValidationAttributeAdapterProvider : IValidationAttributeAdapterProvider
	{
		#region Private Members
		private readonly IValidationAttributeAdapterProvider m_MvcFrameworkImplementation = new ValidationAttributeAdapterProvider();
		#endregion

		#region IValidationAttributeAdapterProvider Members
		/// <summary>
		/// Returns the <see cref="T:Microsoft.AspNetCore.Mvc.DataAnnotations.IAttributeAdapter" /> for the given <see cref="T:System.ComponentModel.DataAnnotations.ValidationAttribute" />.
		/// </summary>
		/// <param name="attribute">The <see cref="T:System.ComponentModel.DataAnnotations.ValidationAttribute" /> to create an <see cref="T:Microsoft.AspNetCore.Mvc.DataAnnotations.IAttributeAdapter" />
		/// for.</param>
		/// <param name="stringLocalizer">The <see cref="T:Microsoft.Extensions.Localization.IStringLocalizer" /> which will be used to create messages.</param>
		/// <returns>
		/// An <see cref="T:Microsoft.AspNetCore.Mvc.DataAnnotations.IAttributeAdapter" /> for the given <paramref name="attribute" />.
		/// </returns>
		public IAttributeAdapter GetAttributeAdapter(ValidationAttribute attribute, IStringLocalizer stringLocalizer)
		{
			IAttributeAdapter adapter = m_MvcFrameworkImplementation.GetAttributeAdapter(attribute, stringLocalizer);

			if (adapter == null)
			{
				Type type = attribute.GetType();

				if (type == typeof(UmbrellaPostcodeAttribute) || type == typeof(UmbrellaPhoneAttribute))
				{
					// TODO: Is this still needed? Or will it just work?
					// adapter = new RegularExpressionAttributeAdapter((RegularExpressionAttribute)attribute, stringLocalizer);
				}
				else
				{
					// Need to create adapter implementations of all the other attributes in the Umbrella.DataAnnotations library in order
					// for them to work properly with jQuery unobtrusive validation as they do currently in MVC 5 using the Umbrella.Legacy.WebUtilities.DataAnnotations library.
				}
			}

			return adapter;
		}
		#endregion
	}
}