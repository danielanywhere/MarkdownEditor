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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using static MarkdownEditor.MarkdownEditorUtil;

namespace MarkdownEditor
{
	//*-------------------------------------------------------------------------*
	//*	frmProjectOptions																												*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Project options form.
	/// </summary>
	public partial class frmProjectOptions : Form
	{
		//*************************************************************************
		//*	Private																																*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//* btnCancel_Click																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// The Cancel button has been clicked.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Standard event arguments.
		/// </param>
		private void btnCancel_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* btnDocxFilename_Click																									*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// The DocX filename elipsis button has been clicked.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Standard event arguments.
		/// </param>
		private void btnDocxFilename_Click(object sender, EventArgs e)
		{
			string filename = OpenWordDialog("Set your Word Export Filename",
				GetFullFilename(mWorkingPath, txtDocxFilename.Text), false);

			if(filename.Length > 0)
			{
				txtDocxFilename.Text = GetRelativeFilename(mWorkingPath, filename);
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* btnDocxTemplateFilename_Click																					*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// The DocX template filename elipsis button has been clicked.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Standard event arguments.
		/// </param>
		private void btnDocxTemplateFilename_Click(object sender, EventArgs e)
		{
			string filename = OpenWordDialog("Set your Word Template Filename",
				GetFullFilename(mWorkingPath, txtDocxTemplateFilename.Text), false);

			if(filename.Length > 0)
			{
				txtDocxTemplateFilename.Text =
					GetRelativeFilename(mWorkingPath, filename);
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* btnHtmlFilename_Click																									*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// The Html filename elipsis button has been clicked.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Standard event arguments.
		/// </param>
		private void btnHtmlFilename_Click(object sender, EventArgs e)
		{
			string filename = OpenHtmlDialog("Set your HTML Export Filename",
				GetFullFilename(mWorkingPath, txtHtmlFilename.Text), false);

			if(filename.Length > 0)
			{
				txtHtmlFilename.Text = GetRelativeFilename(mWorkingPath, filename);
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* btnMarkdownFilename_Click																							*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// The Markdown filename elipsis button has been clicked.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Standard event arguments.
		/// </param>
		private void btnMarkdownFilename_Click(object sender, EventArgs e)
		{
			string filename = OpenMarkdownDialog("Set your active Markdown File",
				GetFullFilename(mWorkingPath, txtMarkdownFilename.Text), false);

			if(filename.Length > 0)
			{
				txtMarkdownFilename.Text = GetRelativeFilename(mWorkingPath, filename);
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* btnOK_Click																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// The OK button has been clicked.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Standard event arguments.
		/// </param>
		private void btnOK_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
			this.Close();
		}
		//*-----------------------------------------------------------------------*

		//*************************************************************************
		//*	Protected																															*
		//*************************************************************************
		//*************************************************************************
		//*	Public																																*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//*	_Constructor																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Create a new instance of the frmProjectOptions Item.
		/// </summary>
		public frmProjectOptions()
		{
			InitializeComponent();

			dgMappedColumns.DataSource = mMappedColumns;
			dgStyles.DataSource = mStyles;
			dgVariables.DataSource = mVariables;

			btnCancel.Click += btnCancel_Click;
			btnOK.Click += btnOK_Click;

			btnDocxFilename.Click += btnDocxFilename_Click;
			btnDocxTemplateFilename.Click += btnDocxTemplateFilename_Click;
			btnHtmlFilename.Click += btnHtmlFilename_Click;
			btnMarkdownFilename.Click += btnMarkdownFilename_Click;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	DocxFilename																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Get/Set the filename to use for Word Documents exported under this
		/// project.
		/// </summary>
		public string DocxFilename
		{
			get { return txtDocxFilename.Text; }
			set { txtDocxFilename.Text = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	DocxTemplateFilename																									*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Get/Set the filename to use for Word Documents templates referenced
		/// while creating new documents.
		/// </summary>
		public string DocxTemplateFilename
		{
			get { return txtDocxTemplateFilename.Text; }
			set { txtDocxTemplateFilename.Text = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	HtmlFilename																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Get/Set the filename of an HTML file to be exported from this project.
		/// </summary>
		public string HtmlFilename
		{
			get { return txtHtmlFilename.Text; }
			set { txtHtmlFilename.Text = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	MappedColumns																													*
		//*-----------------------------------------------------------------------*
		private BindingList<VariableEntryItem> mMappedColumns =
			new BindingList<VariableEntryItem>();
		/// <summary>
		/// Get a reference to the collection of Mapped columns.
		/// </summary>
		public BindingList<VariableEntryItem> MappedColumns
		{
			get { return mMappedColumns; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	MarkdownFilename																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Get/Set the filename to use for the markdown documents associated with
		/// this project.
		/// </summary>
		public string MarkdownFilename
		{
			get { return txtMarkdownFilename.Text; }
			set { txtMarkdownFilename.Text = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	OptionConvertToTable																									*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Get/Set a value indicating whether sections will be converted to
		/// tables.
		/// </summary>
		public bool OptionConvertToTable
		{
			get { return chkConvertToTable.Checked; }
			set { chkConvertToTable.Checked = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Styles																																*
		//*-----------------------------------------------------------------------*
		private BindingList<VariableStyleItem> mStyles =
			new BindingList<VariableStyleItem>();
		/// <summary>
		/// Get a reference to the collection of document styles.
		/// </summary>
		public BindingList<VariableStyleItem> Styles
		{
			get { return mStyles; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Variables																															*
		//*-----------------------------------------------------------------------*
		private BindingList<VariableEntryItem> mVariables =
			new BindingList<VariableEntryItem>();
		/// <summary>
		/// Get a reference to the collection of user variables.
		/// </summary>
		public BindingList<VariableEntryItem> Variables
		{
			get { return mVariables; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	WorkingPath																														*
		//*-----------------------------------------------------------------------*
		private string mWorkingPath = "";
		/// <summary>
		/// Get/Set the working base path for this project.
		/// </summary>
		public string WorkingPath
		{
			get { return mWorkingPath; }
			set { mWorkingPath = value; }
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

}
