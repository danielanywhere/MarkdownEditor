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
	//*	DocumentStyleTypeEnum																										*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Enumeration of defined document style types.
	/// </summary>
	public enum DocumentStyleTypeEnum
	{
		/// <summary>
		/// No document style defined or unknown.
		/// </summary>
		None = 0,
		/// <summary>
		/// Default heading font color.
		/// </summary>
		HeadingFontColor,
		/// <summary>
		/// Default heading font name.
		/// </summary>
		HeadingFontName,
		/// <summary>
		/// Default heading font size.
		/// </summary>
		HeadingFontSize,
		/// <summary>
		/// Default section background color.
		/// </summary>
		SectionBackgroundColor,
		/// <summary>
		/// Even numbered section background color.
		/// </summary>
		SectionBackgroundColorEven,
		/// <summary>
		/// Odd numbered section background color.
		/// </summary>
		SectionBackgroundColorOdd,
		/// <summary>
		/// Table header font color.
		/// </summary>
		TableHeaderFontColor,
		/// <summary>
		/// Table header font name.
		/// </summary>
		TableHeaderFontName,
		/// <summary>
		/// Table header font size.
		/// </summary>
		TableHeaderFontSize,
		/// <summary>
		/// Heading 1 font color.
		/// </summary>
		H1FontColor,
		/// <summary>
		/// Heading 2 font color.
		/// </summary>
		H2FontColor,
		/// <summary>
		/// Heading 3 font color.
		/// </summary>
		H3FontColor,
		/// <summary>
		/// Heading 4 font color.
		/// </summary>
		H4FontColor,
		/// <summary>
		/// Heading 5 font color.
		/// </summary>
		H5FontColor,
		/// <summary>
		/// Heading 6 font color.
		/// </summary>
		H6FontColor,
		/// <summary>
		/// Heading 7 font color.
		/// </summary>
		H7FontColor,
		/// <summary>
		/// Heading 8 font color.
		/// </summary>
		H8FontColor,
		/// <summary>
		/// Heading 9 font color.
		/// </summary>
		H9FontColor,
		/// <summary>
		/// Heading 1 font name.
		/// </summary>
		H1FontName,
		/// <summary>
		/// Heading 2 font name.
		/// </summary>
		H2FontName,
		/// <summary>
		/// Heading 3 font name.
		/// </summary>
		H3FontName,
		/// <summary>
		/// Heading 4 font name.
		/// </summary>
		H4FontName,
		/// <summary>
		/// Heading 5 font name.
		/// </summary>
		H5FontName,
		/// <summary>
		/// Heading 6 font name.
		/// </summary>
		H6FontName,
		/// <summary>
		/// Heading 7 font name.
		/// </summary>
		H7FontName,
		/// <summary>
		/// Heading 8 font name.
		/// </summary>
		H8FontName,
		/// <summary>
		/// Heading 9 font name.
		/// </summary>
		H9FontName,
		/// <summary>
		/// Heading 1 font size.
		/// </summary>
		H1FontSize,
		/// <summary>
		/// Heading 2 font size.
		/// </summary>
		H2FontSize,
		/// <summary>
		/// Heading 3 font size.
		/// </summary>
		H3FontSize,
		/// <summary>
		/// Heading 4 font size.
		/// </summary>
		H4FontSize,
		/// <summary>
		/// Heading 5 font size.
		/// </summary>
		H5FontSize,
		/// <summary>
		/// Heading 6 font size.
		/// </summary>
		H6FontSize,
		/// <summary>
		/// Heading 7 font size.
		/// </summary>
		H7FontSize,
		/// <summary>
		/// Heading 8 font size.
		/// </summary>
		H8FontSize,
		/// <summary>
		/// Heading 9 font size.
		/// </summary>
		H9FontSize
	}
	//*-------------------------------------------------------------------------*

}
