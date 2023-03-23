using System.Globalization;
using Umbrella.DataAnnotations.BaseClasses;

namespace Umbrella.DataAnnotations;

/// <summary>
/// A validation attribute used to specify a maximum permitted <see cref="DateTime"/> to the end of the day, i.e 1 second to midnight, in UTC.
/// </summary>
/// <seealso cref="ModelAwareValidationAttribute" />
[AttributeUsage(AttributeTargets.Property)]
public class MaxDateAttribute : ModelAwareValidationAttribute
{
	private readonly DateTime? _maxDate;
	private readonly int? _offSetDays;

	/// <summary>
	/// Maximum date defaults to the current date
	/// </summary>
	public MaxDateAttribute()
	{
		var utcNow = DateTime.UtcNow;

		_maxDate = new DateTime(utcNow.Year, utcNow.Month, utcNow.Day, 23, 59, 59, DateTimeKind.Utc);
	}

	/// <summary>
	/// Maximum date based on the offset days from the current date
	/// </summary>
	/// <param name="offsetDays"></param>
	public MaxDateAttribute(int offsetDays)
	{
		_offSetDays = offsetDays;
	}

	/// <summary>
	/// Maximum date based on the exact date specified at the end of the day, i.e. 23:59:59
	/// </summary>
	/// <param name="year"></param>
	/// <param name="month"></param>
	/// <param name="day"></param>
	public MaxDateAttribute(int year, int month, int day)
	{
		_maxDate = new DateTime(year, month, day, 23, 59, 59, DateTimeKind.Utc);
	}

	/// <summary>
	/// Gets the client validation parameters.
	/// </summary>
	/// <returns>A collection of validation parameters.</returns>
	protected override IEnumerable<KeyValuePair<string, object>> GetClientValidationParameters() => base.GetClientValidationParameters()
			.Union(new[] { new KeyValuePair<string, object>("Max", GetMaxDate().ToString("d", CultureInfo.CurrentCulture)) });

	/// <summary>
	/// Returns true if property is valid.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <param name="model">The model.</param>
	/// <returns>
	///   <c>true</c> if the specified value is valid; otherwise, <c>false</c>.
	/// </returns>
	public override bool IsValid(object value, object model)
	{
		// TODO: Consider enforcing the value to be specified as DateTimeKind.Utc to avoid issues.

		DateTime maxDate = GetMaxDate();

		if (value is DateTime result)
		{
			if (result > maxDate)
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
				if (DateTime.TryParse(strDateTime, out result) && result > maxDate)
				{
					return false;
				}
			}
		}

		// Assume success
		return true;
	}

	private DateTime GetMaxDate()
	{
		DateTime maxDate;

		var now = DateTime.UtcNow;

		if (_maxDate.HasValue)
			maxDate = _maxDate.Value;
		else if (_offSetDays.HasValue)
			maxDate = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59, DateTimeKind.Utc).AddDays(_offSetDays.Value);
		else
			maxDate = now;

		return maxDate;
	}
}