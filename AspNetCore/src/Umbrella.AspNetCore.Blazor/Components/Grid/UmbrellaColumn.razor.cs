using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Umbrella.Utilities.Data.Filtering;
using Umbrella.AspNetCore.Blazor.Extensions;
using Umbrella.AppFramework.Security.Abstractions;
using System.Security.Claims;
using Umbrella.Utilities.Data.Sorting;

namespace Umbrella.AspNetCore.Blazor.Components.Grid
{
	/// <summary>
	/// A column component for use with the <see cref="UmbrellaGrid{TItem}"/> component.
	/// </summary>
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

		/// <summary>
		/// Gets or sets the name of the property on the parent data item model that this column represents.
		/// </summary>
		[Parameter]
		public string? PropertyName { get; set; }

		/// <summary>
		/// Gets or sets the property path override used as the <see cref="FilterExpressionDescriptor.MemberPath"/> property value when
		/// creating filters.
		/// </summary>
		/// <remarks>
		/// If this value is <see langword="null"/>, the <see cref="PropertyName"/> will be used.
		/// </remarks>
		[Parameter]
		public string? FilterMemberPathOverride { get; set; }

		/// <summary>
		/// Gets or sets the property path override used as the value <see cref="SortExpressionDescriptor.MemberPath"/> property value when
		/// creating sorters.
		/// </summary>
		/// <remarks>
		/// If this value is <see langword="null"/>, the <see cref="PropertyName"/> will be used.
		/// </remarks>
		[Parameter]
		public string? SorterMemberPathOverride { get; set; }

		/// <summary>
		/// Gets or sets the text for the column heading.
		/// </summary>
		[Parameter]
		public string? Heading { get; set; }

		/// <summary>
		/// Gets the column short heading display text to be applied to the column heading only.
		/// </summary>
		/// <remarks>
		/// If not specified, <see cref="Heading"/> is used. The <see cref="Heading"/> property will always be used for the filter field
		/// name and will also be shown as a tooltip for the column heading where this property has a value.
		/// </remarks>
		public string? ShortHeading { get; set; }

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

		/// <summary>
		/// Gets or sets the filter options that appear in the drop down when <see cref="FilterControlType"/> is set to <see cref="UmbrellaColumnFilterType.Options"/>.
		/// </summary>
		[Parameter]
		public IReadOnlyCollection<object> FilterOptions { get; set; } = Array.Empty<object>();

		/// <summary>
		/// Gets or sets the type of the filter options.
		/// </summary>
		[Parameter]
		public UmbrellaColumnFilterOptionsType? FilterOptionsType { get; set; }

		/// <summary>
		/// Gets or sets the filter option display name selector used to convert a value in the <see cref="FilterOptions"/> collection to a friendly display name.
		/// </summary>
		/// <remarks>
		/// An example of where this might be required is when populating the <see cref="FilterOptions"/> collection with a list of enums. This selector can be used to
		/// convert enum names to friendlier formatted display names.
		/// </remarks>
		[Parameter]
		public Func<object, string>? FilterOptionDisplayNameSelector { get; set; }

		/// <summary>
		/// Gets or sets a value that specifies whether or not this column contains actions, e.g. buttons, links,
		/// that can be used to perform actions on the data item that this column is associated with.
		/// </summary>
		[Parameter]
		public bool IsActions { get; set; }

		/// <summary>
		/// Gets or sets the child content rendered by this column.
		/// </summary>
		[Parameter]
		public RenderFragment? ChildContent { get; set; }

		/// <summary>
		/// Gets or sets the uncaptured attributes that have been specified on this component. This dictionary is automatically populated by Blazor.
		/// </summary>
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
		/// Gets or sets a value which determines how this column will be rendered.
		/// </summary>
		/// <remarks>
		/// Defaults to <see cref="UmbrellaColumnDisplayMode.Full"/>.
		/// </remarks>
		[Parameter]
		public UmbrellaColumnDisplayMode DisplayMode { get; set; }

		/// <summary>
		/// Gets a value indicating whether this is the first column in the grid.
		/// </summary>
		public bool IsFirstColumn => UmbrellaGridInstance.FirstColumnPropertyName?.Equals(PropertyName) == true;

		/// <inheritdoc />
		protected override async Task OnInitializedAsync()
		{
			ClaimsPrincipal claimsPrincipal = await AuthHelper.GetCurrentClaimsPrincipalAsync();

			if (!await AuthorizationService.AuthorizeRolesAndPolicyAsync(claimsPrincipal, Roles, Policy))
				DisplayMode = UmbrellaColumnDisplayMode.None;

			if (ScanMode)
			{
				if (DisplayMode != UmbrellaColumnDisplayMode.None)
				{
					var definition = new UmbrellaColumnDefinition(Heading, ShortHeading, PercentageWidth, Sortable, Filterable, FilterOptions, FilterOptionDisplayNameSelector, AdditionalAttributes, FilterControlType, FilterMatchType, FilterOptionsType, PropertyName, FilterMemberPathOverride, SorterMemberPathOverride, DisplayMode);
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