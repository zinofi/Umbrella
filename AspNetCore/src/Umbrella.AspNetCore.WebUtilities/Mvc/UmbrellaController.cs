﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Umbrella.AspNetCore.WebUtilities.Mvc;

/// <summary>
/// Serves as the base class for all MVC controllers.
/// </summary>
public abstract class UmbrellaController : Controller
{
	#region Protected Properties		
	/// <summary>
	/// Gets the logger.
	/// </summary>
	protected ILogger Logger { get; }
	#endregion

	#region Constructors		
	/// <summary>
	/// Initializes a new instance of the <see cref="UmbrellaController"/> class.
	/// </summary>
	/// <param name="logger">The logger.</param>
	protected UmbrellaController(ILogger logger)
	{
		Logger = logger;
	}
	#endregion
}