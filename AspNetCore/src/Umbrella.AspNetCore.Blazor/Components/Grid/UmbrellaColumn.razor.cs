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

		[CascadingParameter]
		protected IUmbrellaGrid UmbrellaGridInstance { get; set; } = null!;

		[CascadingParameter(Name = nameof(ScanMode))]
		private bool ScanMode { get; set; }

		[Parameter]
		public string? PropertyName { get; set; }

		[Parameter]
		public string? Heading { get; set; }

		/// <summary>
		/// Gets or sets the percentage width.
		/// </summary>
		[Parameter]
		public int? PercentageWidth { get; set; }

		[Parameter]
		public bool Sortable { get; set; }

		[Parameter]
		public bool Filterable { get; set; }

		[Parameter]
		public UmbrellaColumnFilterType FilterControlType { get; set; }

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

		[Parameter]
		public string? Roles { get; set; }

		[Parameter]
		public string? Policy { get; set; }

		public bool IsVisible { get; private set; } = true;

		public bool IsFirstColumn => UmbrellaGridInstance.FirstColumnPropertyName?.Equals(PropertyName) == true;

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