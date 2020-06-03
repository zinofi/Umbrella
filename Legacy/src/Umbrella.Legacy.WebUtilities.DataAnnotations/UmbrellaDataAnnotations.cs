using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Umbrella.DataAnnotations;

namespace Umbrella.Legacy.WebUtilities.DataAnnotations
{
    public sealed class UmbrellaDataAnnotations : UmbrellaDataAnnotations<UmbrellaValidator>
    {
    }

    public abstract class UmbrellaDataAnnotations<T> where T : UmbrellaValidator
    {
        public static void Attribute<TAttribute>()
        {
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(TAttribute), typeof(T));
        }

        public static void RegisterAll()
        {
            Attribute<IsAttribute>();
            Attribute<EqualToAttribute>();
            Attribute<NotEqualToAttribute>();
            Attribute<GreaterThanAttribute>();
            Attribute<LessThanAttribute>();
            Attribute<GreaterThanOrEqualToAttribute>();
            Attribute<LessThanOrEqualToAttribute>();
            Attribute<RequiredIfAttribute>();
            Attribute<RequiredIfTrueAttribute>();
            Attribute<RequiredIfFalseAttribute>();
            Attribute<RequiredIfEmptyAttribute>();
            Attribute<RequiredIfNotEmptyAttribute>();
            Attribute<RequiredIfNotAttribute>();
            Attribute<RegularExpressionIfAttribute>();
            Attribute<RequiredIfRegExMatchAttribute>();
            Attribute<RequiredIfNotRegExMatchAttribute>();
            Attribute<MinDateAttribute>();
            Attribute<MaxDateAttribute>();
            Attribute<UmbrellaPostcodeAttribute>();
            Attribute<UmbrellaPhoneAttribute>();
        }
    }
}