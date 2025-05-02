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
	//*	StyleInfoCollection																											*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of StyleInfoItem Items.
	/// </summary>
	public class StyleInfoCollection : List<StyleInfoItem>
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
	//*	StyleInfoItem																														*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Definition of information about a single style.
	/// </summary>
	public class StyleInfoItem
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
		//*	BackgroundColor																												*
		//*-----------------------------------------------------------------------*
		private Color mBackgroundColor = Color.Empty;
		/// <summary>
		/// Get/Set the background color.
		/// </summary>
		public Color BackgroundColor
		{
			get { return mBackgroundColor; }
			set { mBackgroundColor = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	FontColor																															*
		//*-----------------------------------------------------------------------*
		private Color mFontColor = Color.Empty;
		/// <summary>
		/// Get/Set the color of the font.
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
		/// Get/Set the name of the font.
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
		private double mFontSize = 0d;
		/// <summary>
		/// Get/Set the size of the font.
		/// </summary>
		public double FontSize
		{
			get { return mFontSize; }
			set { mFontSize = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Name																																	*
		//*-----------------------------------------------------------------------*
		private string mName = "";
		/// <summary>
		/// Get/Set the name of the style.
		/// </summary>
		public string Name
		{
			get { return mName; }
			set { mName = value; }
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

}
