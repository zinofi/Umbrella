using System.Collections.Concurrent;
using System.Reflection;

namespace Umbrella.DataAnnotations.Helpers;

/// <summary>
/// Lightweight clone of the internal BCL ValidationAttributeStore used by <see cref="AsyncValidator"/>.
/// Caches validation attributes for types and properties to avoid repeated reflection.
/// </summary>
internal sealed class ValidationAttributeStore
{
	public static ValidationAttributeStore Instance { get; } = new ValidationAttributeStore();

	private readonly ConcurrentDictionary<(Type type, string propertyName), ValidationAttribute[]> _propertyAttributeCache = new();
	private readonly ConcurrentDictionary<Type, ValidationAttribute[]> _typeAttributeCache = new();
	private readonly ConcurrentDictionary<(Type type, string propertyName), Type> _propertyTypeCache = new();

	private ValidationAttributeStore() { }

	public Type GetPropertyType(ValidationContext context)
	{
		if (context == null)
			throw new ArgumentNullException(nameof(context));
		if (string.IsNullOrEmpty(context.MemberName))
			throw new ArgumentException("ValidationContext.MemberName must be set for property operations.", nameof(context));
		Type objectType = context.ObjectInstance?.GetType() ?? throw new ArgumentException("ValidationContext.ObjectInstance must not be null.", nameof(context));
		return _propertyTypeCache.GetOrAdd((objectType, context.MemberName), static key =>
		{
			var (type, name) = key;
			PropertyInfo? pi = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (pi == null)
				throw new ArgumentException($"Property '{name}' not found on type '{type.FullName}'.", nameof(context));
			return pi.PropertyType;
		});
	}

	public IEnumerable<ValidationAttribute> GetPropertyValidationAttributes(ValidationContext context)
	{
		if (context == null)
			throw new ArgumentNullException(nameof(context));
		if (string.IsNullOrEmpty(context.MemberName))
			return Array.Empty<ValidationAttribute>();
		Type objectType = context.ObjectInstance?.GetType() ?? throw new ArgumentException("ValidationContext.ObjectInstance must not be null.", nameof(context));
		var attrs = _propertyAttributeCache.GetOrAdd((objectType, context.MemberName), static key =>
		{
			var (type, name) = key;
			PropertyInfo? pi = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (pi == null)
				return [];
			return pi.GetCustomAttributes<ValidationAttribute>(true).ToArray();
		});
		return attrs;
	}

	public IEnumerable<ValidationAttribute> GetTypeValidationAttributes(ValidationContext context)
	{
		if (context == null)
			throw new ArgumentNullException(nameof(context));
		Type objectType = context.ObjectInstance?.GetType() ?? throw new ArgumentException("ValidationContext.ObjectInstance must not be null.", nameof(context));
		var attrs = _typeAttributeCache.GetOrAdd(objectType, static t => t.GetCustomAttributes<ValidationAttribute>(true).ToArray());
		return attrs;
	}
}