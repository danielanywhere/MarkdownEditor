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
using System.Threading.Tasks;

namespace MarkdownEditor
{
	//*-------------------------------------------------------------------------*
	//*	IndentLevelCollection																										*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of IndentLevelItem Items.
	/// </summary>
	public class IndentLevelCollection : List<IndentLevelItem>
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
	//*	IndentLevelItem																													*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Individual definition of a matched indent level and HTML level.
	/// </summary>
	public class IndentLevelItem
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
		////*-----------------------------------------------------------------------*
		////*	HtmlLevel																															*
		////*-----------------------------------------------------------------------*
		//private int mHtmlLevel = 0;
		///// <summary>
		///// Get/Set the HTML level for this entry.
		///// </summary>
		//public int HtmlLevel
		//{
		//	get { return mHtmlLevel; }
		//	set { mHtmlLevel = value; }
		//}
		////*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	IndentLevel																														*
		//*-----------------------------------------------------------------------*
		private int mIndentLevel = 0;
		/// <summary>
		/// Get/Set the indent level for this entry.
		/// </summary>
		public int IndentLevel
		{
			get { return mIndentLevel; }
			set { mIndentLevel = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ListType																															*
		//*-----------------------------------------------------------------------*
		private string mListType = "";
		/// <summary>
		/// Get/Set the list type at this indent level.
		/// </summary>
		public string ListType
		{
			get { return mListType; }
			set { mListType = value; }
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

}
