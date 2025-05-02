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
using System.Windows.Forms;

namespace MarkdownEditor
{
	//*-------------------------------------------------------------------------*
	//*	MnemonicToolStripRenderer																								*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Renders a toolstrip with mnemonics always activated.
	/// </summary>
	public class MnemonicToolStripRenderer : ToolStripProfessionalRenderer
	{
		//*************************************************************************
		//*	Private																																*
		//*************************************************************************
		//*************************************************************************
		//*	Protected																															*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//* OnRenderItemText																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Raises the RenderItemText event when the item text is going to be
		/// rendered.
		/// </summary>
		/// <param name="e">
		/// Tool strip item text render event arguments.
		/// </param>
		protected override void OnRenderItemText(
			ToolStripItemTextRenderEventArgs e)
		{
			e.TextFormat &= ~TextFormatFlags.NoPrefix;
			e.TextFormat &= ~TextFormatFlags.HidePrefix;
			base.OnRenderItemText(e);
		}
		//*-----------------------------------------------------------------------*

		//*************************************************************************
		//*	Public																																*
		//*************************************************************************
	}
	//*-------------------------------------------------------------------------*

}
