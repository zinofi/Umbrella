using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Umbrella.DataAnnotations;

namespace Umbrella.WebUtilities.Mvc.DataAnnotations
{
    public sealed class UmbrellaDataAnnotations : UmbrellaDataAnnotations<UmbrellaValidator>
    {
    }

    public abstract class UmbrellaDataAnnotations<T> where T : UmbrellaValidator
    {
        public static void Attribute(Type foolprooftAttributeType)
        {
            DataAnnotationsModelValidatorProvider.RegisterAdapter(foolprooftAttributeType, typeof(T));
        }

        public static void RegisterAll()
        {
            Attribute(typeof(IsAttribute));
            Attribute(typeof(EqualToAttribute));
            Attribute(typeof(NotEqualToAttribute));
            Attribute(typeof(GreaterThanAttribute));
            Attribute(typeof(LessThanAttribute));
            Attribute(typeof(GreaterThanOrEqualToAttribute));
            Attribute(typeof(LessThanOrEqualToAttribute));
            Attribute(typeof(RequiredIfAttribute));
            Attribute(typeof(RequiredIfTrueAttribute));
            Attribute(typeof(RequiredIfFalseAttribute));
            Attribute(typeof(RequiredIfEmptyAttribute));
            Attribute(typeof(RequiredIfNotEmptyAttribute));
            Attribute(typeof(RequiredIfNotAttribute));
            Attribute(typeof(RegularExpressionIfAttribute));
            Attribute(typeof(RequiredIfRegExMatchAttribute));
            Attribute(typeof(RequiredIfNotRegExMatchAttribute));
            Attribute(typeof(MinDateAttribute)); 
        }
    }
}
