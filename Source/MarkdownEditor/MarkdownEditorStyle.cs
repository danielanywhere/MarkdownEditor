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
	//*	MarkdownEditorStyleCollection																						*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of MarkdownEditorStyleItem Items.
	/// </summary>
	public class MarkdownEditorStyleCollection : List<MarkdownEditorStyleItem>
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
	//*	MarkdownEditorStyleItem																									*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Individual style theme for Markdown Editor.
	/// </summary>
	public class MarkdownEditorStyleItem
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
		//*	HtmlSectionFormatting																									*
		//*-----------------------------------------------------------------------*
		private List<string> mHtmlSectionFormatting = new List<string>();
		/// <summary>
		/// Get a reference to the series of text lines that define the way HTML
		/// sections should be formatted.
		/// </summary>
		public List<string> HtmlSectionFormatting
		{
			get { return mHtmlSectionFormatting; }
		}
		//*-----------------------------------------------------------------------*


	}
	//*-------------------------------------------------------------------------*

}
