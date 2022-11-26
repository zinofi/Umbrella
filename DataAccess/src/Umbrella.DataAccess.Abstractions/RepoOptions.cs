﻿namespace Umbrella.DataAccess.Abstractions;

/// <summary>
/// A default RepoOptions type that contains virtual properties for <see cref="SanitizeEntity"/> and <see cref="ValidateEntity"/> with default values of <see langword="true"/>,
/// and another property <see cref="ProcessChildren"/> defaulted to <see langword="false" />.
/// </summary>
public class RepoOptions
{
	/// <summary>
	/// Initializes a new instance of the <see cref="RepoOptions"/> class.
	/// </summary>
	public RepoOptions()
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="RepoOptions"/> class.
	/// </summary>
	/// <param name="sanitizeEntity">if set to <c>true</c> sanitizes the entity.</param>
	/// <param name="validateEntity">if set to <c>true</c> validates the entity.</param>
	/// <param name="processChildren">if set to <c>true</c> processes the children of the entity, i.e. child collections.</param>
	public RepoOptions(bool sanitizeEntity = true, bool validateEntity = true, bool processChildren = false)
	{
		SanitizeEntity = sanitizeEntity;
		ValidateEntity = validateEntity;
		ProcessChildren = processChildren;
	}

	/// <summary>
	/// Gets or sets a value indicating whether to sanitize the entity. Defaults to <see langword="true" />.
	/// </summary>
	public virtual bool SanitizeEntity { get; set; } = true;

	/// <summary>
	/// Gets or sets a value indicating whether to validate the entity. Defaults to <see langword="true" />.
	/// </summary>
	public virtual bool ValidateEntity { get; set; } = true;

	/// <summary>
	/// Gets or sets a value indicating whether children of the entity will be processed. Defaults to <see langword="false" />.
	/// </summary>
	public virtual bool ProcessChildren { get; set; }
}