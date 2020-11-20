using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Umbrella.Utilities;
using Umbrella.Utilities.Extensions;

namespace Umbrella.AspNetCore.Blazor.Components.Form
{
	public class DisplayNameText : ComponentBase
	{
		private bool _hasRendered = false;

		[Parameter]
		public Expression<Func<object>> ForTarget { get; set; } = null!;

		[Parameter]
		public RenderFragment? Content { get; set; }

		[Parameter(CaptureUnmatchedValues = true)]
		public IReadOnlyDictionary<string, object> AdditionalAttributes { get; set; } = null!;

		/// <inheritdoc />
		protected override void OnParametersSet()
		{
			Guard.ArgumentNotNull(ForTarget, nameof(ForTarget));

			if (!(ForTarget.Body is MemberExpression) && !(ForTarget.Body is UnaryExpression))
				throw new ArgumentException("A MemberExpression must be provided.", nameof(ForTarget));
		}

		/// <inheritdoc />
		protected override void BuildRenderTree(RenderTreeBuilder builder)
		{
			base.BuildRenderTree(builder);

			var expressionBody = ForTarget.Body;

			if (expressionBody is UnaryExpression expression && expression.NodeType == ExpressionType.Convert)
			{
				expressionBody = expression.Operand;
			}

			var me = (MemberExpression)expressionBody;

			builder.OpenElement(0, "span");
			builder.AddMultipleAttributes(1, AdditionalAttributes);

			if (Content is null)
			{
				builder.AddContent(2, ForTarget.GetDisplayText());
			}
			else
			{
				Content(builder);
			}

			builder.CloseElement();

			_hasRendered = true;
		}

		/// <inheritdoc />
		protected override bool ShouldRender() => !_hasRendered;
	}
}