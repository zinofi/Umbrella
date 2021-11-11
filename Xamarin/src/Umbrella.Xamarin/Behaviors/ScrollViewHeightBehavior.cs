using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Umbrella.Xamarin.Behaviors
{
	public class ScrollViewHeightBehavior : Behavior<ScrollView>
	{
		public static BindableProperty ParentLayoutProperty = BindableProperty.Create(nameof(ParentLayout), typeof(Layout), typeof(ScrollViewHeightBehavior), null, BindingMode.OneTime);

		private bool _hasHeightChanged;

		public Layout? ParentLayout
		{
			get => (Layout?)GetValue(ParentLayoutProperty);
			set => SetValue(ParentLayoutProperty, value);
		}

		public ScrollView? ScrollViewObject { get; private set; }

		/// <inheritdoc />
		protected override void OnAttachedTo(ScrollView bindable)
		{
			base.OnAttachedTo(bindable);
			ScrollViewObject = bindable;
			bindable.BindingContextChanged += Bindable_BindingContextChanged;
			bindable.SizeChanged += Bindable_SizeChanged;
		}

		/// <inheritdoc />
		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged();

			if (ScrollViewObject is null)
				return;

			BindingContext = ScrollViewObject.BindingContext;
		}

		/// <inheritdoc />
		protected override void OnDetachingFrom(ScrollView bindable)
		{
			base.OnDetachingFrom(bindable);

			bindable.BindingContextChanged -= Bindable_BindingContextChanged;
			bindable.SizeChanged -= Bindable_SizeChanged;
		}

		private async void Bindable_SizeChanged(object sender, EventArgs e)
		{
			if (ScrollViewObject is null)
				return;

			if (ParentLayout is null)
				return;

			if (!_hasHeightChanged)
			{
				double collectionHeight = ScrollViewObject.Height;

				if (collectionHeight > 0)
				{
					await Task.Delay(50);

					double newHeight = collectionHeight / 2 + 20;

					ScrollViewObject.HeightRequest = Math.Max(newHeight, ParentLayout.Height);

					_hasHeightChanged = true;
				}
			}
		}

		private void Bindable_BindingContextChanged(object sender, EventArgs e) => OnBindingContextChanged();
	}
}
