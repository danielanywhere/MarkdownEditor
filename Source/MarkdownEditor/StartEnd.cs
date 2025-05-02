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
	//*	StartEndCollection																											*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of StartEndItem Items.
	/// </summary>
	public class StartEndCollection : List<StartEndItem>
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
	//*	StartEndItem																														*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Individual start and end values.
	/// </summary>
	public class StartEndItem
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
		//*	End																																		*
		//*-----------------------------------------------------------------------*
		private string mEnd = "";
		/// <summary>
		/// Get/Set the end value.
		/// </summary>
		[JsonProperty(Order = 1)]
		public string End
		{
			get { return mEnd; }
			set { mEnd = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Start																																	*
		//*-----------------------------------------------------------------------*
		private string mStart = "";
		/// <summary>
		/// Get/Set the start value.
		/// </summary>
		[JsonProperty(Order = 0)]		
		public string Start
		{
			get { return mStart; }
			set { mStart = value; }
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

}
