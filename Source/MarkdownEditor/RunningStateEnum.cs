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
	//*	RunningStateEnum																												*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Enumeration of recognized running states.
	/// </summary>
	public enum RunningStateEnum
	{
		/// <summary>
		/// Form is not running or the state is unknown.
		/// </summary>
		None = 0,
		/// <summary>
		/// The form is running.
		/// </summary>
		Running,
		/// <summary>
		/// The form is in pre-closing state.
		/// </summary>
		Preclose,
		/// <summary>
		/// The form can close when ready.
		/// </summary>
		CloseWhenReady
	}
	//*-------------------------------------------------------------------------*

}
