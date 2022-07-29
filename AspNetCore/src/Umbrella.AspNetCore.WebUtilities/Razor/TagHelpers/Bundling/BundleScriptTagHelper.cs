using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Logging;
using Umbrella.AspNetCore.WebUtilities.Mvc.TagHelpers.Bundling;
using Umbrella.Utilities;
using Umbrella.Utilities.Extensions;
using Umbrella.WebUtilities.Bundling.Abstractions;
using Umbrella.WebUtilities.Exceptions;
using Umbrella.WebUtilities.Security;

namespace Umbrella.AspNetCore.WebUtilities.Razor.TagHelpers.Bundling
{
	/// <summary>
	/// A tag helper used to output script elements for named js bundles which either point to those bundles or render them inline.
	/// </summary>
	[HtmlTargetElement("bundle-script", Attributes = "name", TagStructure = TagStructure.WithoutEndTag)]
	public class BundleScriptTagHelper : BundleScriptTagHelper<IBundleUtility>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="BundleScriptTagHelper"/> class.
		/// </summary>
		/// <param name="logger">The logger.</param>
		/// <param name="bundleUtility">The bundle utility.</param>
		/// <param name="nonceContext">The nonce context.</param>
		public BundleScriptTagHelper(
			ILogger<WebpackScriptTagHelper> logger,
			IBundleUtility bundleUtility,
			Lazy<NonceContext> nonceContext)
			: base(logger, bundleUtility, nonceContext)
		{
		}
	}

	/// <summary>
	/// Serves as the base class for all bundling script tag helpers.
	/// </summary>
	/// <typeparam name="TBundleUtility">The type of the bundle utility.</typeparam>
	public abstract class BundleScriptTagHelper<TBundleUtility> : TagHelper
		where TBundleUtility : IBundleUtility
	{
		private readonly Lazy<NonceContext> _lazyNonceContext;

		/// <summary>
		/// Gets the logger.
		/// </summary>
		protected ILogger Logger { get; }

		/// <summary>
		/// Gets the bundle utility.
		/// </summary>
		protected TBundleUtility BundleUtility { get; }

		/// <summary>
		/// Gets the nonce context.
		/// </summary>
		protected NonceContext NonceContext => _lazyNonceContext.Value;

		/// <summary>
		/// Gets or sets the name of the bundle.
		/// </summary>
		public string Name { get; set; } = null!;

		/// <summary>
		/// Gets or sets a value indicating whether the contents of the bundle should be rendered inline in the HTML.
		/// </summary>
		public bool RenderInline { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="BundleScriptTagHelper{TBundleUtility}"/> class.
		/// </summary>
		/// <param name="logger">The logger.</param>
		/// <param name="bundleUtility">The bundle utility.</param>
		/// <param name="nonceContext">The nonce context.</param>
		public BundleScriptTagHelper(
			ILogger<WebpackScriptTagHelper> logger,
			TBundleUtility bundleUtility,
			Lazy<NonceContext> nonceContext)
		{
			Logger = logger;
			BundleUtility = bundleUtility;
			_lazyNonceContext = nonceContext;
		}

		/// <inheritdoc />
		public override void Init(TagHelperContext context)
		{
			Guard.ArgumentNotNullOrWhiteSpace(Name, nameof(Name));
			Name = Name.TrimToLowerInvariant();
			Guard.ArgumentNotNullOrWhiteSpace(Name, nameof(Name));

			base.Init(context);
		}

		/// <inheritdoc />
		public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			try
			{
				output.TagName = "script";
				output.TagMode = TagMode.StartTagAndEndTag;

				if (RenderInline)
				{
					string? content = await BundleUtility.GetScriptContentAsync(Name);

					if (string.IsNullOrWhiteSpace(content))
					{
						output.SuppressOutput();
						return;
					}

					if (!string.IsNullOrWhiteSpace(NonceContext.Current))
						output.Attributes.Add("nonce", NonceContext.Current);

					output.Content.SetHtmlContent(content);
				}
				else
				{
					string path = await BundleUtility.GetScriptPathAsync(Name);

					if (string.IsNullOrWhiteSpace(path))
					{
						output.SuppressOutput();
						return;
					}

					output.Attributes.Add("src", path);
				}
			}
			catch (Exception exc) when (Logger.WriteError(exc, new { Name, RenderInline }, returnValue: true))
			{
				throw new UmbrellaWebException($"There was a problem rendering the bundle script tag helper for named bundle: {Name}.", exc);
			}
		}
	}
}