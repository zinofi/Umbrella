// Copyright (c) Zinofi Digital Ltd. All Rights Reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Umbrella.Xamarin.Behaviors
{
	public class CollectionViewHeightBehavior : Behavior<CollectionView>
	{
		public static BindableProperty ParentLayoutProperty = BindableProperty.Create(nameof(ParentLayout), typeof(Layout), typeof(CollectionViewHeightBehavior), null, BindingMode.OneTime);

		private bool _hasHeightChanged;

		public Layout? ParentLayout
		{
			get => (Layout?)GetValue(ParentLayoutProperty);
			set => SetValue(ParentLayoutProperty, value);
		}

		public CollectionView? CollectionViewObject { get; private set; }

		/// <inheritdoc />
		protected override void OnAttachedTo(CollectionView bindable)
		{
			base.OnAttachedTo(bindable);
			CollectionViewObject = bindable;
			bindable.BindingContextChanged += Bindable_BindingContextChanged;
			bindable.SizeChanged += Bindable_SizeChanged;
		}

		private async void Bindable_SizeChanged(object sender, EventArgs e)
		{
			if (CollectionViewObject is null)
				return;

			if (ParentLayout is null)
				return;

			if (!_hasHeightChanged)
			{
				double collectionHeight = CollectionViewObject.Height;

				if (collectionHeight > 0)
				{
					await Task.Delay(50);

					double newHeight = collectionHeight / 2 + 20;

					CollectionViewObject.HeightRequest = Math.Max(newHeight, ParentLayout.Height);

					_hasHeightChanged = true;
				}
			}
		}

		private void Bindable_BindingContextChanged(object sender, EventArgs e) => OnBindingContextChanged();

		/// <inheritdoc />
		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged();

			if (CollectionViewObject is null)
				return;

			BindingContext = CollectionViewObject.BindingContext;
		}

		/// <inheritdoc />
		protected override void OnDetachingFrom(CollectionView bindable)
		{
			base.OnDetachingFrom(bindable);

			bindable.BindingContextChanged -= Bindable_BindingContextChanged;
			bindable.SizeChanged -= Bindable_SizeChanged;
		}
	}
}