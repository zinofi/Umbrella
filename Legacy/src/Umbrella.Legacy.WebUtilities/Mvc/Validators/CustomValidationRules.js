(function ($)
{
	jQuery.validator.addMethod("datemin", function (value, element)
	{
		return this.optional(element) || ValidateDateMin($(element).val(), $(element).data("valDateminMin"));
	});

	jQuery.validator.unobtrusive.adapters.add("datemin", ["min"], function (options)
	{
		options.rules["datemin"] = function ()
		{
			return ValidateDateMin($(options.element).val(), options.params.min);
		};

		options.messages["datemin"] = options.message;
	});

	function ValidateDateMin(value, minDate)
	{
		var dateVal = ParseDate(value);
		var minVal = ParseDate(minDate);

		return dateVal >= minVal;
	}

	function ParseDate(dateString)
	{
		var parts = dateString.split("/");
		return new Date(parts[2], parts[1] - 1, parts[0]);
	}
}(jQuery));