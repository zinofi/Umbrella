using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Utilities.Extensions;
using Umbrella.Xamarin.Extensions;
using Xamarin.Forms;

namespace Umbrella.Xamarin.Controls
{
	public class ToggleButton : Button
	{
		public event EventHandler<ToggledEventArgs>? Toggled;

		public static BindableProperty LabelledByProperty = BindableProperty.Create(nameof(LabelledBy), typeof(Label), typeof(ToggleButton));
		public static BindableProperty IsToggledProperty = BindableProperty.Create(nameof(IsToggled), typeof(bool), typeof(ToggleButton), false, BindingMode.TwoWay, propertyChanged: OnIsToggledChanged);
		public static BindableProperty GroupNameProperty = BindableProperty.Create(nameof(GroupName), typeof(string), typeof(ToggleButton));

		/// <summary>
		/// Initializes a new instance of the <see cref="ToggleButton"/> class.
		/// </summary>
		public ToggleButton()
		{
			Clicked += (sender, args) => ToggleState();
		}

		public Label? LabelledBy
		{
			set => SetValue(LabelledByProperty, value);
			get => (Label?)GetValue(LabelledByProperty);
		}

		public bool IsToggled
		{
			set => SetValue(IsToggledProperty, value);
			get => (bool)GetValue(IsToggledProperty);
		}

		public string? GroupName
		{
			get => (string?)GetValue(GroupNameProperty);
			set => SetValue(GroupNameProperty, value);
		}

		/// <inheritdoc />
		protected override void OnParentSet()
		{
			base.OnParentSet();

			VisualStateManager.GoToState(this, "ToggledOff");

			if (LabelledBy != null)
			{
				var grTap = new TapGestureRecognizer
				{
					Command = new Command(ToggleState)
				};

				LabelledBy.GestureRecognizers.Add(grTap);
			}
		}

		private void ToggleState()
		{
			if (!string.IsNullOrEmpty(GroupName))
			{
				IReadOnlyCollection<ToggleButton> lstToggleButton = this.FindPageControls<ToggleButton>(x => x.GroupName == GroupName);

				bool newValue = !IsToggled;

				if (newValue)
				{
					// Ensure others in the group are deselected
					lstToggleButton.Where(x => x != this).ForEach(x => x.IsToggled = false);
					IsToggled = newValue;
				}
				else
				{
					// Never allow deselection for grouped items as we need to ensure 1 is always selected
					// once an initial selection has been made.
				}
			}
			else
			{
				IsToggled = !IsToggled;
			}
		}

		private static void OnIsToggledChanged(BindableObject bindable, object oldValue, object newValue)
		{
			var toggleButton = (ToggleButton)bindable;
			bool isToggled = (bool)newValue;

			// Fire event
			toggleButton.Toggled?.Invoke(toggleButton, new ToggledEventArgs(isToggled));

			// Set the visual state
			VisualStateManager.GoToState(toggleButton, isToggled ? "ToggledOn" : "ToggledOff");
		}
	}
}