using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Umbrella.Utilities.Data.Filtering;
using Umbrella.AspNetCore.Blazor.Extensions;
using Umbrella.AppFramework.Security.Abstractions;
using System.Security.Claims;

namespace Umbrella.AspNetCore.Blazor.Components.Grid
{
	public partial class UmbrellaColumn
	{
		[Inject]
		private IAuthorizationService AuthorizationService { get; set; } = null!;

		[Inject]
		private IAppAuthHelper AuthHelper { get; set; } = null!;

		/// <summary>
		/// Gets or sets the <see cref="IUmbrellaGrid"/> instance that contains this column.
		/// </summary>
		[CascadingParameter]
		protected IUmbrellaGrid UmbrellaGridInstance { get; set; } = null!;

		[CascadingParameter(Name = nameof(ScanMode))]
		private bool ScanMode { get; set; }

		// TODO NET6 - Deprecate this and replace it with an Expression<Func<T, object>> and then get the member name from that for the property name.
		// Mark as obsolete so we can migrate gradually in target projects.
		[Parameter]
		public string? PropertyName { get; set; }

		[Parameter]
		public string? Heading { get; set; }

		/// <summary>
		/// Gets or sets the percentage width.
		/// </summary>
		[Parameter]
		public int? PercentageWidth { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="UmbrellaColumn"/> is sortable.
		/// </summary>
		[Parameter]
		public bool Sortable { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="UmbrellaColumn"/> is filterable.
		/// </summary>
		[Parameter]
		public bool Filterable { get; set; }

		/// <summary>
		/// Gets or sets the type of the filter control.
		/// </summary>
		[Parameter]
		public UmbrellaColumnFilterType FilterControlType { get; set; }

		/// <summary>
		/// Gets or sets the type of the filter match. Defaults to <see cref="FilterType.Contains"/>.
		/// </summary>
		[Parameter]
		public FilterType FilterMatchType { get; set; } = FilterType.Contains;

		[Parameter]
		public IReadOnlyCollection<object> FilterOptions { get; set; } = Array.Empty<object>();

		[Parameter]
		public UmbrellaColumnFilterOptionsType? FilterOptionsType { get; set; }

		[Parameter]
		public Func<object, string>? FilterOptionDisplayNameSelector { get; set; }

		[Parameter]
		public bool IsActions { get; set; }

		[Parameter]
		public RenderFragment? ChildContent { get; set; }

		[Parameter(CaptureUnmatchedValues = true)]
		public IReadOnlyDictionary<string, object> AdditionalAttributes { get; set; } = null!;

		/// <summary>
		/// Gets or sets the comma-delimited list of roles for which this column should be displayed.
		/// </summary>
		/// <remarks>
		/// Used in conjunction with the <see cref="Policy"/> property.
		/// </remarks>
		[Parameter]
		public string? Roles { get; set; }

		/// <summary>
		/// Gets or sets the authorization policy for which this column should be displayed.
		/// </summary>
		/// <remarks>
		/// Used in conjunction with the <see cref="Roles"/> property.
		/// </remarks>
		[Parameter]
		public string? Policy { get; set; }

		/// <summary>
		/// Gets a value indicating whether this column is visible.
		/// </summary>
		[Parameter]
		public bool IsVisible { get; set; } = true;

		/// <summary>
		/// Gets a value indicating whether this is the first column in the grid.
		/// </summary>
		public bool IsFirstColumn => UmbrellaGridInstance.FirstColumnPropertyName?.Equals(PropertyName) == true;

		/// <inheritdoc />
		protected override async Task OnInitializedAsync()
		{
			ClaimsPrincipal claimsPrincipal = await AuthHelper.GetCurrentClaimsPrincipalAsync();

			if (!await AuthorizationService.AuthorizeRolesAndPolicyAsync(claimsPrincipal, Roles, Policy))
				IsVisible = false;

			if (ScanMode)
			{
				if (IsVisible)
				{
					var definition = new UmbrellaColumnDefinition(Heading, PercentageWidth, Sortable, Filterable, FilterOptions, FilterOptionDisplayNameSelector, AdditionalAttributes, FilterControlType, FilterMatchType, FilterOptionsType, PropertyName);
					UmbrellaGridInstance.AddColumnDefinition(definition);
				}
			}
			else
			{
				UmbrellaGridInstance.SetColumnScanCompleted();
			}
		}
	}
}