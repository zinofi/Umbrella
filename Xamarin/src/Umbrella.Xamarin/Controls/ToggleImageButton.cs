using System.Diagnostics.CodeAnalysis;
using Umbrella.Utilities.Extensions;
using Umbrella.Xamarin.Extensions;
using Xamarin.Forms;

namespace Umbrella.Xamarin.Controls;

/// <summary>
/// A control that extends the <see cref="ImageButton"/> control to provide checkbox and radiobutton behaviour.
/// </summary>
/// <seealso cref="ImageButton" />
[SuppressMessage("Usage", "CA2211:Non-constant fields should not be visible", Justification = "Required by Xamarin's conventions.")]
public class ToggleImageButton : ImageButton
{
	/// <summary>
	/// Occurs when the toggle state of the button changes.
	/// </summary>
	public event EventHandler<ToggledEventArgs>? Toggled;

	/// <summary>
	/// The labelled by property
	/// </summary>
	public static BindableProperty LabelledByProperty = BindableProperty.Create(nameof(LabelledBy), typeof(Label), typeof(ToggleButton));

	/// <summary>
	/// The is toggled property
	/// </summary>
	public static BindableProperty IsToggledProperty = BindableProperty.Create(nameof(IsToggled), typeof(bool), typeof(ToggleButton), false, BindingMode.TwoWay, propertyChanged: OnIsToggledChanged);

	/// <summary>
	/// The group name property
	/// </summary>
	public static BindableProperty GroupNameProperty = BindableProperty.Create(nameof(GroupName), typeof(string), typeof(ToggleButton));

	/// <summary>
	/// Initializes a new instance of the <see cref="ToggleImageButton"/> class.
	/// </summary>
	public ToggleImageButton()
	{
		Clicked += (sender, args) => ToggleState();
	}

	/// <summary>
	/// Gets or sets the <see cref="Label"/> that is associated with this control.
	/// </summary>
	public Label? LabelledBy
	{
		set => SetValue(LabelledByProperty, value);
		get => (Label?)GetValue(LabelledByProperty);
	}

	/// <summary>
	/// Gets or sets a value indicating whether this instance is toggled.
	/// </summary>
	public bool IsToggled
	{
		set => SetValue(IsToggledProperty, value);
		get => (bool)GetValue(IsToggledProperty);
	}

	/// <summary>
	/// Gets or sets the name of the group. This is used to provide radio button behaviour.
	/// </summary>
	public string? GroupName
	{
		get => (string?)GetValue(GroupNameProperty);
		set => SetValue(GroupNameProperty, value);
	}

	/// <inheritdoc />
	protected override void OnParentSet()
	{
		base.OnParentSet();
		_ = VisualStateManager.GoToState(this, "ToggledOff");

		if (LabelledBy is not null)
		{
			_ = VisualStateManager.GoToState(LabelledBy, "ToggledOff");

			var grTap = new TapGestureRecognizer
			{
				Command = new Command(ToggleState)
			};

			LabelledBy.GestureRecognizers.Add(grTap);
			AutomationProperties.SetLabeledBy(this, LabelledBy);
		}
	}

	private void ToggleState()
	{
		if (!string.IsNullOrEmpty(GroupName))
		{
			IReadOnlyCollection<ToggleImageButton> lstToggleButton = this.FindPageControls<ToggleImageButton>(x => x.GroupName == GroupName);

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

	[SuppressMessage("Usage", "VSTHRD100:Avoid async void methods", Justification = "This is an event handler.")]
	private static async void OnIsToggledChanged(BindableObject bindable, object oldValue, object newValue)
	{
		var toggleButton = (ToggleImageButton)bindable;
		bool isToggled = (bool)newValue;

		// Fire event
		toggleButton.Toggled?.Invoke(toggleButton, new ToggledEventArgs(isToggled));

		// Small delay to ensure the visual state change is picked up properly.
		await Task.Delay(50);

		// Set the visual state
		_ = VisualStateManager.GoToState(toggleButton, isToggled ? "ToggledOn" : "ToggledOff");

		if (toggleButton.LabelledBy is not null)
			_ = VisualStateManager.GoToState(toggleButton.LabelledBy, isToggled ? "ToggledOn" : "ToggledOff");
	}
}