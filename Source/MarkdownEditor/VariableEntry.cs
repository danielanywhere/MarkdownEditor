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
using System.Formats.Tar;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using static MarkdownEditor.MarkdownEditorUtil;

namespace MarkdownEditor
{
	//*-------------------------------------------------------------------------*
	//*	VariableEntryCollection																									*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of VariableEntryItem Items.
	/// </summary>
	public class VariableEntryCollection :
		ObservableCollection<VariableEntryItem>
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
					foreach(VariableEntryItem entryItem in e.NewItems)
					{
						entryItem.PropertyChanged += Item_PropertyChanged;
					}
					break;
				case NotifyCollectionChangedAction.Move:
					//	The item still exists in the collection.
					break;
				case NotifyCollectionChangedAction.Remove:
					//	Items were removed.
					foreach(VariableEntryItem entryItem in e.OldItems)
					{
						entryItem.PropertyChanged -= Item_PropertyChanged;
					}
					break;
				case NotifyCollectionChangedAction.Replace:
					//	Items were replaced in the collection.
					foreach(VariableEntryItem entryItem in e.OldItems)
					{
						entryItem.PropertyChanged -= Item_PropertyChanged;
					}
					foreach(VariableEntryItem entryItem in e.NewItems)
					{
						entryItem.PropertyChanged += Item_PropertyChanged;
					}
					break;
				case NotifyCollectionChangedAction.Reset:
					//	A major change has occurred on the collection.
					foreach(VariableEntryItem entryItem in e.OldItems)
					{
						entryItem.PropertyChanged -= Item_PropertyChanged;
					}
					foreach(VariableEntryItem entryItem in e.NewItems)
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
		public static void ApplyChanges(BindingList<VariableEntryItem> source,
			ObservableCollection<VariableEntryItem> target)
		{
			int count = 0;
			VariableEntryItem entrySource = null;
			VariableEntryItem entryTarget = null;
			int index = 0;

			if(source != null && target != null)
			{
				//	Check for new and changed items.
				foreach(VariableEntryItem entryItem in source)
				{
					entryTarget = target.FirstOrDefault(x => x.Name == entryItem.Name);
					if(entryTarget == null)
					{
						//	New.
						target.Add(entryItem);
					}
					else if(entryTarget.Value != entryItem.Value)
					{
						//	Updated.
						entryTarget.Value = entryItem.Value;
					}
				}
				//	Check for deleted items.
				count = target.Count;
				for(index = 0; index < count; index ++)
				{
					entryTarget = target[index];
					entrySource = source.FirstOrDefault(x => x.Name == entryTarget.Name);
					if(entrySource == null)
					{
						target.RemoveAt(index);
						index--;	//	Delist.
						count--;	//	Discount.
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
		public static void Clone(ObservableCollection<VariableEntryItem> source,
			BindingList<VariableEntryItem> target)
		{
			if(source?.Count > 0 && target != null)
			{
				foreach(VariableEntryItem entryItem in source)
				{
					target.Add(new VariableEntryItem()
					{
						Name = entryItem.Name,
						Value = entryItem.Value
					});
				}
			}
		}
		/// <summary>
		/// Clone the items of the source into the target list.
		/// </summary>
		/// <param name="source">
		/// Reference to the source list containing items to clone.
		/// </param>
		/// <param name="target">
		/// Reference to the target list to receive the items.
		/// </param>
		public static void Clone(ObservableCollection<VariableEntryItem> source,
			List<VariableEntryItem> target)
		{
			if(source?.Count > 0 && target != null)
			{
				foreach(VariableEntryItem entryItem in source)
				{
					target.Add(new VariableEntryItem()
					{
						Name = entryItem.Name,
						Value = entryItem.Value
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

		//*-----------------------------------------------------------------------*
		//* ResolveValue																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Resolve the caller's interpolated variable from values within the
		/// collection and return the resolved value.
		/// </summary>
		/// <param name="entries">
		/// Reference to the collection of variables containing the values to
		/// search.
		/// </param>
		/// <param name="variable">
		/// Interpolated string variable to resolve, using curly brace syntax.
		/// </param>
		/// <returns>
		/// The value of the resolved variable, if found. Otherwise, an empty
		/// string.
		/// </returns>
		public static string ResolveValue(VariableEntryCollection entries,
			string variable)
		{
			VariableEntryItem entry = null;
			string result = "";
			string text = "";

			if(entries.Count > 0 && variable?.Length > 0)
			{
				text = GetValue(variable,
					ResourceMain.rxInterpolatedExpression, "variable");
				entry = entries.FirstOrDefault(x => x.Name == text);
				if(entry != null)
				{
					result = entry.Value;
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ResolveValues																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Resolve the values in the specified collection.
		/// </summary>
		/// <param name="entries">
		/// Reference to the collection of entries to be resolved.
		/// </param>
		public static void ResolveValues(VariableEntryCollection entries)
		{
			MatchCollection matches = null;
			int maxPassCount = 20;
			int passIndex = 1;
			FindReplaceCollection replacements = new FindReplaceCollection();
			bool unresolved = true;
			VariableEntryItem variableEntry = null;
			string variableName = "";

			if(entries?.Count > 0)
			{
				while(unresolved && passIndex <= maxPassCount)
				{
					unresolved = false;
					foreach(VariableEntryItem entryItem in entries)
					{
						if(Regex.IsMatch(entryItem.Value,
							ResourceMain.rxInterpolatedExpression))
						{
							replacements.Clear();
							matches = Regex.Matches(entryItem.Value,
								ResourceMain.rxInterpolatedExpression);
							foreach(Match matchItem in matches)
							{
								variableName = GetValue(matchItem, "variable");
								variableEntry = entries.FirstOrDefault(x =>
									x.Name == variableName &&
										!Regex.IsMatch(x.Value,
											ResourceMain.rxInterpolatedExpression));
								if(variableEntry != null)
								{
									//	A full value was found for this variable.
									replacements.Add(new FindReplaceItem()
									{
										Find = matchItem.Value,
										Replace = variableEntry.Value
									});
								}
								else
								{
									unresolved = true;
								}
							}
							foreach(FindReplaceItem replacementItem in replacements)
							{
								entryItem.Value = entryItem.Value.Replace(
									replacementItem.Find, replacementItem.Replace);
							}
						}
					}
					passIndex++;
				}
			}

		}
		//*-----------------------------------------------------------------------*


	}
	//*-------------------------------------------------------------------------*

	//*-------------------------------------------------------------------------*
	//*	VariableEntryItem																												*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Individual variable entry.
	/// </summary>
	public class VariableEntryItem : INotifyPropertyChanged
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
		//*	Name																																	*
		//*-----------------------------------------------------------------------*
		private string mName = "";
		/// <summary>
		/// Get/Set the name of the variable.
		/// </summary>
		public string Name
		{
			get { return mName; }
			set
			{
				bool changed = (mName != value);

				mName = value;
				if(changed)
				{
					OnPropertyChanged();
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* PropertyChanged																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Fired when a property value on this item has changed.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Value																																	*
		//*-----------------------------------------------------------------------*
		private string mValue = "";
		/// <summary>
		/// Get/Set the value of the entry.
		/// </summary>
		public string Value
		{
			get { return mValue; }
			set
			{
				bool changed = (mValue != value);

				mValue = value;
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
