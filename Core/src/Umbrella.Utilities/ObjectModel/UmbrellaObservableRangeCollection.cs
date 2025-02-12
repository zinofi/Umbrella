// Based on the code at https://github.com/xamarin/XamarinCommunityToolkit/blob/main/src/CommunityToolkit/Xamarin.CommunityToolkit/ObjectModel/ObservableRangeCollection.shared.cs

// Copyright(c).NET Foundation and Contributors
// All Rights Reserved
// Licensed under the MIT License (MIT)

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using CommunityToolkit.Diagnostics;

namespace Umbrella.Utilities.ObjectModel;

/// <summary>
/// An extension of the <see cref="ObservableCollection{T}"/> type that adds support for manipulating the collection
/// using ranges, i.e. adding, removing, and replacing multiple items at the same time instead of individually.
/// </summary>
/// <typeparam name="T">The type stored by the collection.</typeparam>
/// <seealso cref="ObservableCollection{T}" />
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
		Guard.IsNotNull(collection);

		if (notificationMode is not NotifyCollectionChangedAction.Add and not NotifyCollectionChangedAction.Reset)
			throw new ArgumentException("Mode must be either Add or Reset for AddRange.", nameof(notificationMode));

		CheckReentrancy();

		int startIndex = Count;

		bool itemsAdded = AddArrangeCore(collection);

		if (!itemsAdded)
			return;

		if (notificationMode is NotifyCollectionChangedAction.Reset)
		{
			RaiseChangeNotificationEvents(NotifyCollectionChangedAction.Reset);
			return;
		}

		var changedItems = collection is List<T> list ? list : [.. collection];

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
		Guard.IsNotNull(collection);

		if (notificationMode is not NotifyCollectionChangedAction.Remove and not NotifyCollectionChangedAction.Reset)
			throw new ArgumentException("Mode must be either Remove or Reset for RemoveRange.", nameof(notificationMode));

		CheckReentrancy();

		if (notificationMode is NotifyCollectionChangedAction.Reset)
		{
			bool raiseEvents = false;

			foreach (var item in collection)
			{
				_ = Items.Remove(item);
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
		Clear();
		AddRange(collection);
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