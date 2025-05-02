/*
 * Copyright (c). 2024 - 2025 Daniel Patterson, MCSD (danielanywhere).
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 * 
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MarkdownEditor
{
	//*-------------------------------------------------------------------------*
	//*	VariableStyleCollection																									*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of VariableStyleItem Items.
	/// </summary>
	public class VariableStyleCollection :
		ObservableCollection<VariableStyleItem>
	{
		//*************************************************************************
		//*	Private																																*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//* Item_PropertyChanged																									*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// A property value has changed on an item.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Property changed event arguments.
		/// </param>
		private void Item_PropertyChanged(object sender,
			PropertyChangedEventArgs e)
		{
			if(ItemPropertyChanged != null)
			{
				ItemPropertyChanged.Invoke(sender, e);
			}
		}
		//*-----------------------------------------------------------------------*

		//*************************************************************************
		//*	Protected																															*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//* OnCollectionChanged																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Raises the CollectionChanged event when items have been added to or
		/// removed from the collection, and when the collection has been cleared.
		/// </summary>
		/// <param name="e">
		/// Notify collection changed event arguments.
		/// </param>
		protected override void OnCollectionChanged(
			NotifyCollectionChangedEventArgs e)
		{
			base.OnCollectionChanged(e);

			switch(e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					foreach(VariableStyleItem entryItem in e.NewItems)
					{
						entryItem.PropertyChanged += Item_PropertyChanged;
					}
					break;
				case NotifyCollectionChangedAction.Move:
					//	The item still exists in the collection.
					break;
				case NotifyCollectionChangedAction.Remove:
					//	Items were removed.
					foreach(VariableStyleItem entryItem in e.OldItems)
					{
						entryItem.PropertyChanged -= Item_PropertyChanged;
					}
					break;
				case NotifyCollectionChangedAction.Replace:
					//	Items were replaced in the collection.
					foreach(VariableStyleItem entryItem in e.OldItems)
					{
						entryItem.PropertyChanged -= Item_PropertyChanged;
					}
					foreach(VariableStyleItem entryItem in e.NewItems)
					{
						entryItem.PropertyChanged += Item_PropertyChanged;
					}
					break;
				case NotifyCollectionChangedAction.Reset:
					//	A major change has occurred on the collection.
					foreach(VariableStyleItem entryItem in e.OldItems)
					{
						entryItem.PropertyChanged -= Item_PropertyChanged;
					}
					foreach(VariableStyleItem entryItem in e.NewItems)
					{
						entryItem.PropertyChanged += Item_PropertyChanged;
					}
					break;
			}
		}
		//*-----------------------------------------------------------------------*

		//*************************************************************************
		//*	Public																																*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//* ApplyChanges																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Apply changes in a source list to a target.
		/// </summary>
		/// <param name="source">
		/// Reference to the caller's source list.
		/// </param>
		/// <param name="target">
		/// Reference to the caller's target list.
		/// </param>
		public static void ApplyChanges(BindingList<VariableStyleItem> source,
			ObservableCollection<VariableStyleItem> target)
		{
			int count = 0;
			VariableStyleItem entrySource = null;
			VariableStyleItem entryTarget = null;
			int index = 0;

			if(source != null && target != null)
			{
				//	Check for new and changed items.
				foreach(VariableStyleItem entryItem in source)
				{
					entryTarget = target.FirstOrDefault(x =>
						x.StyleName == entryItem.StyleName);
					if(entryTarget == null)
					{
						//	New.
						target.Add(entryItem);
					}
					else if(entryTarget.StyleValue != entryItem.StyleValue)
					{
						//	Updated.
						entryTarget.StyleValue = entryItem.StyleValue;
					}
				}
				//	Check for deleted items.
				count = target.Count;
				for(index = 0; index < count; index++)
				{
					entryTarget = target[index];
					entrySource = source.FirstOrDefault(x =>
						x.StyleName == entryTarget.StyleName);
					if(entrySource == null)
					{
						target.RemoveAt(index);
						index--;  //	Delist.
						count--;  //	Discount.
					}
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* Clone																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Clone the items of the source into the target list.
		/// </summary>
		/// <param name="source">
		/// Reference to the source list containing items to clone.
		/// </param>
		/// <param name="target">
		/// Reference to the target list to receive the items.
		/// </param>
		public static void Clone(ObservableCollection<VariableStyleItem> source,
			BindingList<VariableStyleItem> target)
		{
			if(source?.Count > 0 && target != null)
			{
				foreach(VariableStyleItem styleItem in source)
				{
					target.Add(new VariableStyleItem()
					{
						StyleName = styleItem.StyleName,
						StyleValue = styleItem.StyleValue
					});
				}
			}
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Clone the items of the source into the target list.
		/// </summary>
		/// <param name="source">
		/// Reference to the source list containing items to clone.
		/// </param>
		/// <param name="target">
		/// Reference to the target list to receive the items.
		/// </param>
		public static void Clone(ObservableCollection<VariableStyleItem> source,
			List<VariableStyleItem> target)
		{
			if(source?.Count > 0 && target != null)
			{
				foreach(VariableStyleItem styleItem in source)
				{
					target.Add(new VariableStyleItem()
					{
						StyleName = styleItem.StyleName,
						StyleValue = styleItem.StyleValue
					});
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ItemPropertyChanged																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Fired when the property value of an item in the collection has changed.
		/// </summary>
		public event PropertyChangedEventHandler ItemPropertyChanged;
		//*-----------------------------------------------------------------------*


	}
	//*-------------------------------------------------------------------------*

	//*-------------------------------------------------------------------------*
	//*	VariableStyleItem																												*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Individual variable style entry.
	/// </summary>
	public class VariableStyleItem : INotifyPropertyChanged
	{
		//*************************************************************************
		//*	Private																																*
		//*************************************************************************
		//*************************************************************************
		//*	Protected																															*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//* OnPropertyChanged																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Raises the PropertyChanged event when the value of a property has been
		/// changed.
		/// </summary>
		/// <param name="name">
		/// Name of the property. If called from within a property, this parameter
		/// will be auto-filled with the member's name.
		/// </param>
		protected void OnPropertyChanged([CallerMemberName] string name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
		//*-----------------------------------------------------------------------*

		//*************************************************************************
		//*	Public																																*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//* PropertyChanged																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Fired when the property value on this item has changed.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	StyleName																															*
		//*-----------------------------------------------------------------------*
		private string mStyleName = "";
		/// <summary>
		/// Get/Set the name of the style.
		/// </summary>
		[DisplayName("Style Name")]
		public string StyleName
		{
			get { return mStyleName; }
			set
			{
				bool changed = (mStyleName != value);

				mStyleName = value;
				if(changed)
				{
					OnPropertyChanged();
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	StyleValue																														*
		//*-----------------------------------------------------------------------*
		private string mStyleValue = "";
		/// <summary>
		/// Get/Set the style value.
		/// </summary>
		[DisplayName("Value")]
		public string StyleValue
		{
			get { return mStyleValue; }
			set
			{
				bool changed = (mStyleValue != value);

				mStyleValue = value;
				if(changed)
				{
					OnPropertyChanged();
				}
			}
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

}
