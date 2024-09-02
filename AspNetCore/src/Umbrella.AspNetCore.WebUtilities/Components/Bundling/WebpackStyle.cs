using Umbrella.AspNetCore.WebUtilities.Components.Bundling.Abstractions;
using Umbrella.WebUtilities.Bundling.Abstractions;

namespace Umbrella.AspNetCore.WebUtilities.Components.Bundling;

/// <summary>
/// A component that renders a style tag for a Webpack bundle.
/// </summary>
public sealed class WebpackStyle : BundleStyleComponentBase<IWebpackBundleUtility>;