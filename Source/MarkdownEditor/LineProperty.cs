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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using static MarkdownEditor.MarkdownEditorUtil;

namespace MarkdownEditor
{
	//*-------------------------------------------------------------------------*
	//*	LinePropertyCollection																									*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of LinePropertyItem Items.
	/// </summary>
	public class LinePropertyCollection : List<LinePropertyItem>
	{
		//*************************************************************************
		//*	Private																																*
		//*************************************************************************
		//*************************************************************************
		//*	Protected																															*
		//*************************************************************************
		//*************************************************************************
		//*	Public																																*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//*	_Constructor																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Create a new instance of the LinePropertyCollection Item.
		/// </summary>
		public LinePropertyCollection()
		{
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Create a new instance of the LinePropertyCollection Item.
		/// </summary>
		public LinePropertyCollection(string source)
		{
			MatchCollection matches = null;

			if(source?.Length > 0)
			{
				matches = Regex.Matches(source, ResourceMain.rxLine);
				foreach(Match matchItem in matches)
				{
					this.Add(new LinePropertyItem()
					{
						Line = GetValue(matchItem, "line"),
						Index = GetIndex(matchItem, "line")
					});
				}
			}
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

	//*-------------------------------------------------------------------------*
	//*	LinePropertyItem																												*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Individual item having a line and properties.
	/// </summary>
	public class LinePropertyItem
	{
		//*************************************************************************
		//*	Private																																*
		//*************************************************************************
		//*************************************************************************
		//*	Protected																															*
		//*************************************************************************
		//*************************************************************************
		//*	Public																																*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//*	Index																																	*
		//*-----------------------------------------------------------------------*
		private int mIndex = 0;
		/// <summary>
		/// Get/Set the index of the start of this line within the source.
		/// </summary>
		public int Index
		{
			get { return mIndex; }
			set { mIndex = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Line																																	*
		//*-----------------------------------------------------------------------*
		private string mLine = "";
		/// <summary>
		/// Get/Set the line of this item.
		/// </summary>
		public string Line
		{
			get { return mLine; }
			set { mLine = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	LineType																															*
		//*-----------------------------------------------------------------------*
		private TextLineTypeEnum mLineType = TextLineTypeEnum.Text;
		/// <summary>
		/// Get/Set the type of line associated with this item.
		/// </summary>
		public TextLineTypeEnum LineType
		{
			get { return mLineType; }
			set { mLineType = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Properties																														*
		//*-----------------------------------------------------------------------*
		private PropertyCollection mProperties = new PropertyCollection();
		/// <summary>
		/// Get a reference to the collection of properties on this item.
		/// </summary>
		public PropertyCollection Properties
		{
			get { return mProperties; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	SetProperty																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Set the value of the specified property, creating it if it doesn't
		/// already exist.
		/// </summary>
		/// <param name="name">
		/// Name of the property to set.
		/// </param>
		/// <param name="value">
		/// Value to place on the property.
		/// </param>
		public void SetProperty(string name, object value)
		{
			mProperties.SetValue(name, value);
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

}
