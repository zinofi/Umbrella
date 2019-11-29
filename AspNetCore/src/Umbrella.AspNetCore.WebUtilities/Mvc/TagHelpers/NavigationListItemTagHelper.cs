using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Umbrella.AspNetCore.WebUtilities.Mvc.TagHelpers
{
	// TODO: Remove this - shouldn't automatically apply this behaviour to tags.
	// Replace with the tag helper in the BCC project.
    [HtmlTargetElement("li", Attributes = ActionAttributeName, ParentTag = "ul")]
    [HtmlTargetElement("li", Attributes = ControllerAttributeName, ParentTag = "ul")]
    public class NavigationListItemTagHelper : AnchorTagHelper
    {
        #region Private Constants
        private const string ActionAttributeName = "asp-action";
        private const string ControllerAttributeName = "asp-controller";
        #endregion

        #region Public Properties
        public string ActiveClass { get; set; } = "active";
        #endregion

        #region Constructors
        public NavigationListItemTagHelper(IHtmlGenerator generator)
            : base(generator)
        {
        }
        #endregion

        #region Overridden Methods
        public async override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            await base.ProcessAsync(context, output);

            var childContent = await output.GetChildContentAsync();
            var content = childContent.GetContent();
            output.TagName = "li";

            var href = output.Attributes.FirstOrDefault(a => a.Name == "href");
            if (href != null)
            {
                var tagBuilder = new TagBuilder("a");
                tagBuilder.Attributes.Add("href", href.Value.ToString());
                tagBuilder.InnerHtml.AppendHtml(content);

                output.Content.SetHtmlContent(tagBuilder);
                output.Attributes.Remove(href);
            }
            else
            {
                output.Content.SetHtmlContent(content);
            }

            if (ShouldBeActive())
            {
                MakeActive(output);
            }
        }
        #endregion

        #region Private Methods
        private bool ShouldBeActive()
        {
            var routeData = ViewContext.RouteData.Values;
            var currentController = routeData["controller"] as string;
            var currentAction = routeData["action"] as string;
            var result = false;

            if (!string.IsNullOrWhiteSpace(Controller) && !string.IsNullOrWhiteSpace(Action))
            {
                result = string.Equals(Action, currentAction, StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(Controller, currentController, StringComparison.OrdinalIgnoreCase);
            }
            else if (!string.IsNullOrWhiteSpace(Action))
            {
                result = string.Equals(Action, currentAction, StringComparison.OrdinalIgnoreCase);
            }
            else if (!string.IsNullOrWhiteSpace(Controller))
            {
                result = string.Equals(Controller, currentController, StringComparison.OrdinalIgnoreCase);
            }

            return result;
        }

        private void MakeActive(TagHelperOutput output)
        {
            if (output.Attributes.TryGetAttribute("class", out TagHelperAttribute classAttribute))
            {
                output.Attributes.SetAttribute("class", $"{classAttribute.Value} {ActiveClass}");
            }
            else
            {
                output.Attributes.Add(new TagHelperAttribute("class", ActiveClass));
            }
        } 
        #endregion
    }
}