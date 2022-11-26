using System;

namespace Umbrella.AspNetCore.WebUtilities.Mvc.ModelBinding.Binders.DataExpression.Sorting;

/// <summary>
/// A model binder provider for the <see cref="SortExpressionModelBinder"/>.
/// </summary>
public class SortExpressionModelBinderProvider : DataExpressionModelBinderProvider<SortExpressionModelBinder>
{
	/// <inheritdoc />
	protected override Type DataExpressionType => DataExpressionModelBinderHelper.SortExpressionType;
}