namespace Umbrella.DataAccess.Abstractions;

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
	/// <param name="throwIfConcurrencyTokenMismatch">if set to <c>true</c> throws an exception if a concurrency token mismatch is detected before persisting changes to the database.</param>
	/// <param name="updateOriginalConcurrencyStamp">if set to <c>true</c> updates the original concurrency stamp during save operations.</param>
	public RepoOptions(bool sanitizeEntity = true, bool validateEntity = true, bool processChildren = false, bool throwIfConcurrencyTokenMismatch = true, bool updateOriginalConcurrencyStamp = true)
	{
		SanitizeEntity = sanitizeEntity;
		ValidateEntity = validateEntity;
		ProcessChildren = processChildren;
		ThrowIfConcurrencyTokenMismatch = throwIfConcurrencyTokenMismatch;
		UpdateOriginalConcurrencyStamp = updateOriginalConcurrencyStamp;
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

	/// <summary>
	/// Gets or sets a value indicating whether to throw an exception if a concurrency token mismatch is detected before persisting changes to the database. Defaults to <see langword="true" />.
	/// </summary>
	public virtual bool ThrowIfConcurrencyTokenMismatch { get; set; } = true;

	/// <summary>
	/// Gets or sets a value indicating whether to update the original concurrency token value during save operations. 
	/// Defaults to <see langword="true" />. Setting this to <see langword="false" /> may be useful when the same entity 
	/// is saved multiple times in a single unit of work.
	/// </summary>
	/// <remarks>
	/// <c>
	/// This could be highly dangerous if you are not careful with how you are handling your entities.
	/// Use at your own existential peril! No one is coming to save you.
	/// </c>
	/// </remarks>
	public virtual bool UpdateOriginalConcurrencyStamp { get; set; } = true;
}