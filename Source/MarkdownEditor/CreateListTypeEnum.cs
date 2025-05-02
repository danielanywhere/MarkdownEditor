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
	//*	CreateListTypeEnum																											*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Enumeration of list types to be created.
	/// </summary>
	public enum CreateListTypeEnum
	{
		/// <summary>
		/// Don't create a list, use the existing list, or unknown.
		/// </summary>
		None = 0,
		/// <summary>
		/// A list is currently active. A new list section will increase the
		/// list level.
		/// </summary>
		Active,
		/// <summary>
		/// Create a bulleted list.
		/// </summary>
		Bulleted,
		/// <summary>
		/// Create a numbered list.
		/// </summary>
		Numbered
	}
	//*-------------------------------------------------------------------------*
}
