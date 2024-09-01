using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Umbrella.Utilities.Mapping.Abstractions;

namespace Umbrella.AspNetCore.WebUtilities.Components.Abstractions;

/// <summary>
/// A base component to be used with Blazor components which contain commonly used functionality.
/// </summary>
/// <seealso cref="ComponentBase" />
/// <seealso cref="IAsyncDisposable"/>
public abstract class UmbrellaComponentBase : ComponentBase
{
	[Inject]
	private ILoggerFactory LoggerFactory { get; set; } = null!;

	/// <summary>
	/// Gets the navigation manager.
	/// </summary>
	/// <remarks>
	/// Useful extension methods can be found inside <see cref="NavigationManagerExtensions"/>.
	/// </remarks>
	[Inject]
	protected NavigationManager Navigation { get; private set; } = null!;

	/// <summary>
	/// Gets the mapper.
	/// </summary>
	[Inject]
	protected IUmbrellaMapper Mapper { get; private set; } = null!;

	/// <summary>
	/// Gets the logger.
	/// </summary>
	protected ILogger Logger { get; private set; } = null!;

	/// <inheritdoc />
	protected override void OnInitialized()
	{
		base.OnInitialized();

		Logger = LoggerFactory.CreateLogger(GetType());
	}

	/// <summary>
	/// Reloads the component primarily in response to an error during the initial loading phase.
	/// </summary>
	/// <remarks>Defaults to <see cref="ComponentBase.OnInitializedAsync"/></remarks>
	/// <returns>An awaitable Task that completed when this operation has completed.</returns>
	protected virtual Task ReloadAsync() => OnInitializedAsync();
}