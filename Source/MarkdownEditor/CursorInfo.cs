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

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkdownEditor
{
	//*-------------------------------------------------------------------------*
	//*	CursorInfoCollection																										*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of CursorInfoItem Items.
	/// </summary>
	public class CursorInfoCollection : List<CursorInfoItem>
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
	//*	CursorInfoItem																													*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Individual cursor information.
	/// </summary>
	public class CursorInfoItem
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
		//*	Column																																*
		//*-----------------------------------------------------------------------*
		private int mColumn = 0;
		/// <summary>
		/// Get/Set the column upon which the cursor is located.
		/// </summary>
		[JsonProperty(Order = 2)]
		public int Column
		{
			get { return mColumn; }
			set { mColumn = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Line																																	*
		//*-----------------------------------------------------------------------*
		private int mLine = 0;
		/// <summary>
		/// Get/Set the line upon which the cursor is located.
		/// </summary>
		[JsonProperty(Order = 1)]
		public int Line
		{
			get { return mLine; }
			set { mLine = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Name																																	*
		//*-----------------------------------------------------------------------*
		private string mName = "";
		/// <summary>
		/// Get/Set the name of the cursor entry.
		/// </summary>
		[JsonProperty(Order = 0)]
		public string Name
		{
			get { return mName; }
			set { mName = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Relative																															*
		//*-----------------------------------------------------------------------*
		private int mRelative = 0;
		/// <summary>
		/// Get/Set a value indicating whether the cursor position is relative.
		/// </summary>
		[JsonProperty(Order = 3)]
		public int Relative
		{
			get { return mRelative; }
			set { mRelative = value; }
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

}
