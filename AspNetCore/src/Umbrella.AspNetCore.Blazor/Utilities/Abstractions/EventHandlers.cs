using System.Threading.Tasks;

namespace Umbrella.AspNetCore.Blazor.Utilities.Abstractions;

/// <summary>
/// A delegate that defines the shape of an awaitable Blazor event handler.
/// </summary>
public delegate Task AwaitableBlazorEventHandler();