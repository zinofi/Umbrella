using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Umbrella.Utilities.Extensions;

namespace Umbrella.DataAccess.Abstractions;

/// <summary>
/// A map of property expressions used to load related data of an entity, e.g. load the orders for a customer, as part of the same database query.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
public class IncludeMap<TEntity>
{
	#region Public Properties
	/// <summary>
	/// Gets the includes as expressions.
	/// </summary>
	public HashSet<Expression<Func<TEntity, object>>> Includes { get; } = new HashSet<Expression<Func<TEntity, object>>>();

	/// <summary>
	/// Gets the includes as string paths, e.g. Parent -> Child -> Name will be Parent.Child.Name.
	/// </summary>
	public HashSet<string> PropertyPaths { get; } = new HashSet<string>();
	#endregion

	#region Constructors		
	/// <summary>
	/// Initializes a new instance of the <see cref="IncludeMap{TEntity}"/> class.
	/// </summary>
	/// <param name="paths">The property paths.</param>
	public IncludeMap(params Expression<Func<TEntity, object>>[] paths)
	{
		foreach (var path in paths)
		{
			Includes.Add(path);

			string propertyPath = path.GetMemberPath();

			if (!string.IsNullOrEmpty(propertyPath))
				PropertyPaths.Add(propertyPath);
		}
	}
	#endregion

	#region Overridden Methods
	/// <inheritdoc />
	public override string ToString() => string.Join(", ", PropertyPaths);
	#endregion
}