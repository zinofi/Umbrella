using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Caching.Memory;
using Umbrella.DynamicImage.Abstractions;
using Umbrella.WebUtilities.Hosting;

namespace Umbrella.AspNetCore.WebUtilities.DynamicImage.Mvc.TagHelpers
{
	/// <summary>
	/// A tag helper used to generate source tags for picture elements that have the correct URLs for use with the DynamicImage infrastructure.
	/// </summary>
	/// <seealso cref="DynamicImageTagHelperBase" />
	[OutputElementHint("source")]
	[HtmlTargetElement("dynamic-source", Attributes = RequiredAttributeNames, ParentTag = "picture", TagStructure = TagStructure.WithoutEndTag)]
	public class DynamicImagePictureSourceTagHelper : DynamicImageTagHelperBase
	{
		/// <summary>
		/// Gets the name of the output tag.
		/// </summary>
		protected override string OutputTagName => "source";

		/// <summary>
		/// Initializes a new instance of the <see cref="DynamicImagePictureSourceTagHelper"/> class.
		/// </summary>
		/// <param name="memoryCache">The <see cref="IMemoryCache"/>.</param>
		/// <param name="dynamicImageUtility">The <see cref="IDynamicImageUtility"/>.</param>
		/// <param name="umbrellaHostingEnvironment">The <see cref="IUmbrellaWebHostingEnvironment"/>.</param>
		public DynamicImagePictureSourceTagHelper(
			IMemoryCache memoryCache,
			IDynamicImageUtility dynamicImageUtility,
			IUmbrellaWebHostingEnvironment umbrellaHostingEnvironment)
			: base(memoryCache, dynamicImageUtility, umbrellaHostingEnvironment)
		{
		}

		/// <summary>
		/// Asynchronously executes the <see cref="TagHelper"/> with the given <paramref name="context"/> and <paramref name="output"/>.
		/// </summary>
		/// <param name="context">Contains information associated with the current HTML tag.</param>
		/// <param name="output">A stateful HTML element used to generate an HTML tag.</param>
		/// <returns>A <see cref="Task"/> that on completion updates the output.</returns>
		public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			await base.ProcessAsync(context, output);

			output.Attributes.RemoveAll("alt");
			output.Attributes.RemoveAll("src");
		}
	}
}