using System;

namespace Umbrella.AspNetCore.WebUtilities.Mvc.ModelBinding.Binders.DataExpression.Filtering;

/// <summary>
/// A model binder provider for the <see cref="FilterExpressionModelBinder"/>.
/// </summary>
public class FilterExpressionModelBinderProvider : DataExpressionModelBinderProvider<FilterExpressionModelBinder>
{
	/// <inheritdoc />
	protected override Type DataExpressionType => DataExpressionModelBinderHelper.FilterExpressionType;
}