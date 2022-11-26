using System.Web.Mvc;
using Umbrella.DataAnnotations;

namespace Umbrella.Legacy.WebUtilities.DataAnnotations;

/// <summary>
/// Used to register Umbrella validation attributes with the built-in <see cref="DataAnnotationsModelValidatorProvider"/>.
/// </summary>
/// <seealso cref="T:Umbrella.Legacy.WebUtilities.DataAnnotations.UmbrellaDataAnnotationsModelValidatorProvider{Umbrella.Legacy.WebUtilities.DataAnnotations.UmbrellaValidator}" />
public sealed class UmbrellaDataAnnotationsModelValidatorProvider : UmbrellaDataAnnotationsModelValidatorProvider<UmbrellaValidator>
{
}

/// <summary>
/// Serves as the base class for validator providers that use the <see cref="UmbrellaValidator"/> or types that extend it,
/// to register validation attributes with the built-in <see cref="DataAnnotationsModelValidatorProvider"/>.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <seealso cref="T:Umbrella.Legacy.WebUtilities.DataAnnotations.UmbrellaDataAnnotations{Umbrella.Legacy.WebUtilities.DataAnnotations.UmbrellaValidator}" />
public abstract class UmbrellaDataAnnotationsModelValidatorProvider<T> where T : UmbrellaValidator
{
	/// <summary>
	/// Registers the adapter.
	/// </summary>
	/// <typeparam name="TAttribute">The type of the attribute.</typeparam>
	public static void RegisterAdapter<TAttribute>() => DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(TAttribute), typeof(T));

	/// <summary>
	/// Registers all adapters.
	/// </summary>
	public static void RegisterAllAdapters()
	{
		RegisterAdapter<IsAttribute>();
		RegisterAdapter<EqualToAttribute>();
		RegisterAdapter<NotEqualToAttribute>();
		RegisterAdapter<GreaterThanAttribute>();
		RegisterAdapter<LessThanAttribute>();
		RegisterAdapter<GreaterThanOrEqualToAttribute>();
		RegisterAdapter<LessThanOrEqualToAttribute>();
		RegisterAdapter<RequiredIfAttribute>();
		RegisterAdapter<RequiredIfTrueAttribute>();
		RegisterAdapter<RequiredIfFalseAttribute>();
		RegisterAdapter<RequiredIfEmptyAttribute>();
		RegisterAdapter<RequiredIfNotEmptyAttribute>();
		RegisterAdapter<RequiredIfNotAttribute>();
		RegisterAdapter<RegularExpressionIfAttribute>();
		RegisterAdapter<RequiredIfRegExMatchAttribute>();
		RegisterAdapter<RequiredIfNotRegExMatchAttribute>();
		RegisterAdapter<MinDateAttribute>();
		RegisterAdapter<MaxDateAttribute>();
		RegisterAdapter<UmbrellaPostcodeAttribute>();
		RegisterAdapter<UmbrellaPhoneAttribute>();
	}
}