using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Umbrella.Legacy.WebUtilities.Mvc.Helpers
{
    public static class ValidationMessageHelpers
    {
        public static MvcHtmlString ValidationCalloutFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string additionalCssClass = "")
        {
            string name = ExpressionHelper.GetExpressionText(expression);

            return ValidationCallout(htmlHelper, name, additionalCssClass);
        }

        public static MvcHtmlString ValidationCallout(this HtmlHelper htmlHelper, string name, string additionalCssClass = "")
        {
            string validationCssClass = "validate-error field-validation-valid";

            TagBuilder outerBuilder = new TagBuilder("div");
            outerBuilder.MergeAttribute("data-valmsg-for", name);
            outerBuilder.MergeAttribute("data-valmsg-replace", "true");

            ModelState modelState;
            if (htmlHelper.ViewData.ModelState.TryGetValue(name, out modelState))
            {
                if (modelState.Errors.Count > 0)
                {
                    validationCssClass = "validate-error";
                    outerBuilder.InnerHtml = modelState.Errors[0].ErrorMessage;
                }
            }

            outerBuilder.MergeAttribute("class", validationCssClass + " " + additionalCssClass);

            return new MvcHtmlString(outerBuilder.ToString(TagRenderMode.Normal));
        }
    }
}
