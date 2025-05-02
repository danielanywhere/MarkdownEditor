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
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkdownEditor
{
	//*-------------------------------------------------------------------------*
	//*	StyleTrackerCollection																									*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of StyleTrackerElement Items.
	/// </summary>
	public class StyleTrackerCollection : List<StyleTrackerElement>
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


	}
	//*-------------------------------------------------------------------------*

	//*-------------------------------------------------------------------------*
	//*	StyleTrackerElement																											*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Individual style tracking element.
	/// </summary>
	public class StyleTrackerElement
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
		//*	Bold																																	*
		//*-----------------------------------------------------------------------*
		private bool mBold = false;
		/// <summary>
		/// Get/Set a value indicating whether the current style is bold.
		/// </summary>
		public bool Bold
		{
			get { return mBold; }
			set { mBold = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	FontColor																															*
		//*-----------------------------------------------------------------------*
		private Color mFontColor = Color.Transparent;
		/// <summary>
		/// Get/Set the font color for this element.
		/// </summary>
		public Color FontColor
		{
			get { return mFontColor; }
			set { mFontColor = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	FontName																															*
		//*-----------------------------------------------------------------------*
		private string mFontName = "";
		/// <summary>
		/// Get/Set the name of the font for this style.
		/// </summary>
		public string FontName
		{
			get { return mFontName; }
			set { mFontName = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	FontSize																															*
		//*-----------------------------------------------------------------------*
		private double mFontSize = 12d;
		/// <summary>
		/// Get/Set the font size for this style.
		/// </summary>
		public double FontSize
		{
			get { return mFontSize; }
			set { mFontSize = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Italic																																*
		//*-----------------------------------------------------------------------*
		private bool mItalic = false;
		/// <summary>
		/// Get/Set a value indicating whether the current text is italic.
		/// </summary>
		public bool Italic
		{
			get { return mItalic; }
			set { mItalic = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Underline																															*
		//*-----------------------------------------------------------------------*
		private bool mUnderline = false;
		/// <summary>
		/// Get/Set a value indicating whether the current text is underlined.
		/// </summary>
		public bool Underline
		{
			get { return mUnderline; }
			set { mUnderline = value; }
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*


	//*-------------------------------------------------------------------------*
	//*	StyleTrackerItem																												*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Individual style tracking instance.
	/// </summary>
	public class StyleTrackerItem
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
		/// Create a new instance of the StyleTrackerItem Item.
		/// </summary>
		public StyleTrackerItem()
		{
			mStyles = new StyleTrackerCollection();
			mStyles.Add(new StyleTrackerElement());
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Bold																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Get/Set a value indicating whether the current style is bold.
		/// </summary>
		public bool Bold
		{
			get { return mStyles[^1].Bold; }
			set { mStyles[^1].Bold = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	FontColor																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Get/Set the font color for this element.
		/// </summary>
		public Color FontColor
		{
			get { return mStyles[^1].FontColor; }
			set { mStyles[^1].FontColor = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	FontName																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Get/Set the name of the font for this style.
		/// </summary>
		public string FontName
		{
			get { return mStyles[^1].FontName; }
			set { mStyles[^1].FontName = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	FontSize																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Get/Set the font size for this style.
		/// </summary>
		public double FontSize
		{
			get { return mStyles[^1].FontSize; }
			set { mStyles[^1].FontSize = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Italic																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Get/Set a value indicating whether the current text is italic.
		/// </summary>
		public bool Italic
		{
			get { return mStyles[^1].Italic; }
			set { mStyles[^1].Italic = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Pop																																		*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Pop a style element from the stack and return it to the caller.
		/// </summary>
		/// <returns>
		/// A reference to the topmost item on the stack, if found. Otherwise,
		/// null.
		/// </returns>
		public StyleTrackerElement Pop()
		{
			StyleTrackerElement result = null;

			if(mStyles.Count > 1)
			{
				result = mStyles[^1];
				mStyles.RemoveAt(mStyles.Count - 1);
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Push																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Push the current element values to the stack and create a new working
		/// set.
		/// </summary>
		public void Push()
		{
			StyleTrackerElement newItem = new StyleTrackerElement()
			{
				Bold = Bold,
				FontColor = FontColor,
				FontName = FontName,
				FontSize = FontSize,
				Italic = Italic,
				Underline = Underline
			};
			mStyles.Add(newItem);
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Styles																																*
		//*-----------------------------------------------------------------------*
		private StyleTrackerCollection mStyles = null;
		/// <summary>
		/// Get a reference to the collection of styles registered in this
		/// instance.
		/// </summary>
		public StyleTrackerCollection Styles
		{
			get { return mStyles; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Underline																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Get/Set a value indicating whether the current text is underlined.
		/// </summary>
		public bool Underline
		{
			get { return mStyles[^1].Underline; }
			set { mStyles[^1].Underline = value; }
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

}
