// Based on the code at https://github.com/xamarin/XamarinCommunityToolkit/blob/main/src/CommunityToolkit/Xamarin.CommunityToolkit/ObjectModel/ObservableRangeCollection.shared.cs

// Copyright(c).NET Foundation and Contributors
// All Rights Reserved
// Licensed under the MIT License (MIT)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace Umbrella.Utilities.ObjectModel
{
	/// <summary>
	/// An extension of the <see cref="ObservableCollection{T}"/> type that adds support for manipulating the collection
	/// using ranges, i.e. adding, removing, and replacing multiple items at the same time instead of individually.
	/// </summary>
	/// <typeparam name="T">The type stored by the collection.</typeparam>
	/// <seealso cref="System.Collections.ObjectModel.ObservableCollection{T}" />
	/// <remarks>
	/// This is based on the ObservableRangeCollection from the Xamarin Community Toolkit. It has been cloned and modified here
	/// to allow its use in non-Xamarin projects and has been renamed to prevent namespace clashes.
	/// </remarks>
	public class UmbrellaObservableRangeCollection<T> : ObservableCollection<T>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UmbrellaObservableRangeCollection{T}"/> class.
		/// </summary>
		public UmbrellaObservableRangeCollection()
			: base()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UmbrellaObservableRangeCollection{T}"/> class.
		/// </summary>
		/// <param name="collection">The collection from which the elements are copied.</param>
		public UmbrellaObservableRangeCollection(IEnumerable<T> collection)
			: base(collection)
		{
		}

		/// <summary>
		/// Adds the range.
		/// </summary>
		/// <param name="collection">The collection.</param>
		/// <param name="notificationMode">The notification mode.</param>
		/// <exception cref="ArgumentException">Mode must be either Add or Reset for AddRange. - notificationMode</exception>
		/// <exception cref="ArgumentNullException">collection</exception>
		public void AddRange(IEnumerable<T> collection, NotifyCollectionChangedAction notificationMode = NotifyCollectionChangedAction.Add)
		{
			if (notificationMode != NotifyCollectionChangedAction.Add && notificationMode != NotifyCollectionChangedAction.Reset)
				throw new ArgumentException("Mode must be either Add or Reset for AddRange.", nameof(notificationMode));

			if (collection is null)
				throw new ArgumentNullException(nameof(collection));

			CheckReentrancy();

			int startIndex = Count;

			bool itemsAdded = AddArrangeCore(collection);

			if (!itemsAdded)
				return;

			if (notificationMode == NotifyCollectionChangedAction.Reset)
			{
				RaiseChangeNotificationEvents(NotifyCollectionChangedAction.Reset);
				return;
			}

			var changedItems = collection is List<T> list ? list : new List<T>(collection);

			RaiseChangeNotificationEvents(NotifyCollectionChangedAction.Add, changedItems, startIndex);
		}

		/// <summary>
		/// Removes the range.
		/// </summary>
		/// <param name="collection">The collection.</param>
		/// <param name="notificationMode">The notification mode.</param>
		/// <exception cref="ArgumentException">Mode must be either Remove or Reset for RemoveRange. - notificationMode</exception>
		/// <exception cref="ArgumentNullException">collection</exception>
		public void RemoveRange(IEnumerable<T> collection, NotifyCollectionChangedAction notificationMode = NotifyCollectionChangedAction.Remove)
		{
			if (notificationMode != NotifyCollectionChangedAction.Remove && notificationMode != NotifyCollectionChangedAction.Reset)
				throw new ArgumentException("Mode must be either Remove or Reset for RemoveRange.", nameof(notificationMode));

			if (collection is null)
				throw new ArgumentNullException(nameof(collection));

			CheckReentrancy();

			if (notificationMode == NotifyCollectionChangedAction.Reset)
			{
				bool raiseEvents = false;

				foreach (var item in collection)
				{
					Items.Remove(item);
					raiseEvents = true;
				}

				if (raiseEvents)
					RaiseChangeNotificationEvents(NotifyCollectionChangedAction.Reset);

				return;
			}

			var changedItems = new List<T>(collection);

			for (int i = 0; i < changedItems.Count; i++)
			{
				if (!Items.Remove(changedItems[i]))
				{
					changedItems.RemoveAt(i--);
				}
			}

			if (changedItems.Count == 0)
				return;

			RaiseChangeNotificationEvents(NotifyCollectionChangedAction.Remove, changedItems);
		}

		/// <summary>
		/// Replaces the current collection with the specified item.
		/// </summary>
		/// <param name="item">The item.</param>
		public void Replace(T item) => ReplaceRange(new T[] { item });

		/// <summary>
		/// Replaces the current collection with the specified <paramref name="collection"/>.
		/// </summary>
		/// <param name="collection">The collection.</param>
		/// <exception cref="ArgumentNullException">collection</exception>
		public void ReplaceRange(IEnumerable<T> collection)
		{
			if (collection is null)
				throw new ArgumentNullException(nameof(collection));

			CheckReentrancy();

			bool previouslyEmpty = Items.Count == 0;

			var oldItems = Items.ToList();

			Items.Clear();

			AddArrangeCore(collection);

			bool currentlyEmpty = Items.Count == 0;

			if (previouslyEmpty && currentlyEmpty)
				return;

			RaisePropertyChangedEvents();
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, collection.ToList(), oldItems));
		}

		/// <inheritdoc />
		protected override void ClearItems()
		{
			CheckReentrancy();
			Items.Clear();
			RaiseChangeNotificationEvents(NotifyCollectionChangedAction.Reset);
		}

		private bool AddArrangeCore(IEnumerable<T> collection)
		{
			bool itemAdded = false;

			foreach (var item in collection)
			{
				Items.Add(item);
				itemAdded = true;
			}

			return itemAdded;
		}

		private void RaisePropertyChangedEvents()
		{
			OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
			OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
		}

		private void RaiseChangeNotificationEvents(NotifyCollectionChangedAction action, List<T>? changedItems = null, int startingIndex = -1)
		{
			RaisePropertyChangedEvents();

			if (changedItems is null)
				OnCollectionChanged(new NotifyCollectionChangedEventArgs(action));
			else
				OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, changedItems, startingIndex));
		}
	}
}