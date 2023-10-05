using System.Globalization;
using Umbrella.DataAnnotations.BaseClasses;

namespace Umbrella.DataAnnotations;

/// <summary>
/// A validation attribute used to specify a minimum permitted <see cref="DateTime"/> from the start of the day, i.e midnight, in UTC.
/// </summary>
/// <seealso cref="ModelAwareValidationAttribute" />
[AttributeUsage(AttributeTargets.Property)]
public class MinDateAttribute : ModelAwareValidationAttribute
{
	private readonly DateTime? _minDate;
	private readonly int? _offSetDays;

	/// <summary>
	/// Minimum date defaults to the current date
	/// </summary>
	public MinDateAttribute()
	{
		_minDate = DateTime.UtcNow.Date;
	}

	/// <summary>
	/// Minimum date based on the offset days from the current date
	/// </summary>
	/// <param name="offsetDays"></param>
	public MinDateAttribute(int offsetDays)
	{
		_offSetDays = offsetDays;
	}

	/// <summary>
	/// Minimum date based on the exact date specified
	/// </summary>
	/// <param name="year"></param>
	/// <param name="month"></param>
	/// <param name="day"></param>
	public MinDateAttribute(int year, int month, int day)
	{
		_minDate = new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc);
	}

	/// <summary>
	/// Gets the client validation parameters.
	/// </summary>
	/// <returns>A collection of validation parameters.</returns>
	protected override IEnumerable<KeyValuePair<string, object>> GetClientValidationParameters() => base.GetClientValidationParameters()
			.Union(new[] { new KeyValuePair<string, object>("Min", GetMinDate().ToString("d", CultureInfo.CurrentCulture)) });

	/// <summary>
	/// Returns true if the property is valid.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <param name="model">The model.</param>
	/// <returns>
	///   <c>true</c> if the specified value is valid; otherwise, <c>false</c>.
	/// </returns>
	public override bool IsValid(object value, object model)
	{
		// TODO: Consider enforcing the value to be specified as DateTimeKind.Utc to avoid issues.

		DateTime minDate = GetMinDate();

		if (value is DateTime result)
		{
			if (result.Date < minDate)
			{
				return false;
			}
		}
		else
		{
			string? strDateTime = value as string;

			if (!string.IsNullOrEmpty(strDateTime))
			{
				// We have a value - try and parse it to a datetime
				if (DateTime.TryParse(strDateTime, out result) && result.Date < minDate)
				{
					return false;
				}
			}
		}

		// Assume success
		return true;
	}

	private DateTime GetMinDate()
	{
		DateTime minDate;

		if (_minDate.HasValue)
			minDate = _minDate.Value;
		else if (_offSetDays.HasValue)
			minDate = DateTime.UtcNow.Date.AddDays(_offSetDays.Value);
		else
			minDate = DateTime.UtcNow.Date;

		return minDate;
	}
}