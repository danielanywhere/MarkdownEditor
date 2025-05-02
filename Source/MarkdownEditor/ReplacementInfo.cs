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
	//*	ReplacementInfoCollection																								*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of ReplacementInfoItem Items.
	/// </summary>
	public class ReplacementInfoCollection : List<ReplacementInfoItem>
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
		//*	Replace																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Replace all of the instances in the source with their associated
		/// replacement values.
		/// </summary>
		/// <param name="source">
		/// The caller's source string with the content to replace.
		/// </param>
		/// <param name="replacements">
		/// The replacements to apply to the string.
		/// </param>
		/// <returns>
		/// A version of the caller's string where all of the prescribed
		/// replacements have been made.
		/// </returns>
		public static string Replace(string source,
			ReplacementInfoCollection replacements)
		{
			StringBuilder builder = new StringBuilder();
			int index = 0;
			List<ReplacementInfoItem> items = null;
			int length = 0;

			if(source.Length > 0)
			{
				if(replacements?.Count > 0)
				{
					items = replacements.OrderBy(x => x.Index).ToList();
					foreach(ReplacementInfoItem replacementItem in items)
					{
						if(replacementItem.Index > index)
						{
							//	Some of the source string has been skipped.
							length = replacementItem.Index - index;
							builder.Append(source.Substring(index, length));
						}
						builder.Append(replacementItem.Text);
						index = replacementItem.Index + replacementItem.Length;
					}
					if(source.Length > index)
					{
						//	Paste the right side of the source.
						builder.Append(source.Substring(index));
					}
				}
				else
				{
					builder.Append(source);
				}
			}
			return builder.ToString();
		}
		//*-----------------------------------------------------------------------*


	}
	//*-------------------------------------------------------------------------*

	//*-------------------------------------------------------------------------*
	//*	ReplacementInfoItem																											*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Information about an individual replacement to make in the source string.
	/// </summary>
	public class ReplacementInfoItem
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
		/// Get/Set the source index at which the replacement will be made.
		/// </summary>
		public int Index
		{
			get { return mIndex; }
			set { mIndex = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Length																																*
		//*-----------------------------------------------------------------------*
		private int mLength = 0;
		/// <summary>
		/// Get/Set the length of the content to be replaced.
		/// </summary>
		public int Length
		{
			get { return mLength; }
			set { mLength = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Text																																	*
		//*-----------------------------------------------------------------------*
		private string mText = "";
		/// <summary>
		/// Get/Set the text to be placed into the target area.
		/// </summary>
		public string Text
		{
			get { return mText; }
			set { mText = value; }
		}
		//*-----------------------------------------------------------------------*


	}
	//*-------------------------------------------------------------------------*


}
