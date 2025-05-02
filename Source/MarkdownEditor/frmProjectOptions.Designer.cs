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

namespace MarkdownEditor
{
	partial class frmProjectOptions
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if(disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			grpDocuments = new System.Windows.Forms.GroupBox();
			btnHtmlFilename = new System.Windows.Forms.Button();
			txtHtmlFilename = new System.Windows.Forms.TextBox();
			btnDocxTemplateFilename = new System.Windows.Forms.Button();
			txtDocxTemplateFilename = new System.Windows.Forms.TextBox();
			lblHtmlFilename = new System.Windows.Forms.Label();
			btnMarkdownFilename = new System.Windows.Forms.Button();
			btnDocxFilename = new System.Windows.Forms.Button();
			lblDocxTemplateFilename = new System.Windows.Forms.Label();
			txtMarkdownFilename = new System.Windows.Forms.TextBox();
			lblMarkdownFilename = new System.Windows.Forms.Label();
			txtDocxFilename = new System.Windows.Forms.TextBox();
			lblDocxFilename = new System.Windows.Forms.Label();
			grpConversion = new System.Windows.Forms.GroupBox();
			chkConvertToTable = new System.Windows.Forms.CheckBox();
			tctlProjectOptions = new System.Windows.Forms.TabControl();
			tpgSettings = new System.Windows.Forms.TabPage();
			tpgColumnMap = new System.Windows.Forms.TabPage();
			dgMappedColumns = new System.Windows.Forms.DataGridView();
			tpgUserVariables = new System.Windows.Forms.TabPage();
			dgVariables = new System.Windows.Forms.DataGridView();
			tpgDocumentStyles = new System.Windows.Forms.TabPage();
			dgStyles = new System.Windows.Forms.DataGridView();
			btnOK = new System.Windows.Forms.Button();
			btnCancel = new System.Windows.Forms.Button();
			grpDocuments.SuspendLayout();
			grpConversion.SuspendLayout();
			tctlProjectOptions.SuspendLayout();
			tpgSettings.SuspendLayout();
			tpgColumnMap.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)dgMappedColumns).BeginInit();
			tpgUserVariables.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)dgVariables).BeginInit();
			tpgDocumentStyles.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)dgStyles).BeginInit();
			SuspendLayout();
			// 
			// grpDocuments
			// 
			grpDocuments.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			grpDocuments.Controls.Add(btnHtmlFilename);
			grpDocuments.Controls.Add(txtHtmlFilename);
			grpDocuments.Controls.Add(btnDocxTemplateFilename);
			grpDocuments.Controls.Add(txtDocxTemplateFilename);
			grpDocuments.Controls.Add(lblHtmlFilename);
			grpDocuments.Controls.Add(btnMarkdownFilename);
			grpDocuments.Controls.Add(btnDocxFilename);
			grpDocuments.Controls.Add(lblDocxTemplateFilename);
			grpDocuments.Controls.Add(txtMarkdownFilename);
			grpDocuments.Controls.Add(lblMarkdownFilename);
			grpDocuments.Controls.Add(txtDocxFilename);
			grpDocuments.Controls.Add(lblDocxFilename);
			grpDocuments.Location = new System.Drawing.Point(24, 20);
			grpDocuments.Name = "grpDocuments";
			grpDocuments.Size = new System.Drawing.Size(575, 174);
			grpDocuments.TabIndex = 0;
			grpDocuments.TabStop = false;
			grpDocuments.Text = "Documents";
			// 
			// btnHtmlFilename
			// 
			btnHtmlFilename.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
			btnHtmlFilename.Location = new System.Drawing.Point(507, 132);
			btnHtmlFilename.Name = "btnHtmlFilename";
			btnHtmlFilename.Size = new System.Drawing.Size(42, 29);
			btnHtmlFilename.TabIndex = 11;
			btnHtmlFilename.Text = "...";
			btnHtmlFilename.UseVisualStyleBackColor = true;
			// 
			// txtHtmlFilename
			// 
			txtHtmlFilename.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			txtHtmlFilename.Location = new System.Drawing.Point(149, 133);
			txtHtmlFilename.Name = "txtHtmlFilename";
			txtHtmlFilename.Size = new System.Drawing.Size(352, 27);
			txtHtmlFilename.TabIndex = 10;
			// 
			// btnDocxTemplateFilename
			// 
			btnDocxTemplateFilename.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
			btnDocxTemplateFilename.Location = new System.Drawing.Point(507, 97);
			btnDocxTemplateFilename.Name = "btnDocxTemplateFilename";
			btnDocxTemplateFilename.Size = new System.Drawing.Size(42, 29);
			btnDocxTemplateFilename.TabIndex = 8;
			btnDocxTemplateFilename.Text = "...";
			btnDocxTemplateFilename.UseVisualStyleBackColor = true;
			// 
			// txtDocxTemplateFilename
			// 
			txtDocxTemplateFilename.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			txtDocxTemplateFilename.Location = new System.Drawing.Point(149, 98);
			txtDocxTemplateFilename.Name = "txtDocxTemplateFilename";
			txtDocxTemplateFilename.Size = new System.Drawing.Size(352, 27);
			txtDocxTemplateFilename.TabIndex = 7;
			// 
			// lblHtmlFilename
			// 
			lblHtmlFilename.AutoSize = true;
			lblHtmlFilename.Location = new System.Drawing.Point(22, 136);
			lblHtmlFilename.Name = "lblHtmlFilename";
			lblHtmlFilename.Size = new System.Drawing.Size(115, 20);
			lblHtmlFilename.TabIndex = 9;
			lblHtmlFilename.Text = "HTML Filename:";
			// 
			// btnMarkdownFilename
			// 
			btnMarkdownFilename.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
			btnMarkdownFilename.Location = new System.Drawing.Point(507, 30);
			btnMarkdownFilename.Name = "btnMarkdownFilename";
			btnMarkdownFilename.Size = new System.Drawing.Size(42, 29);
			btnMarkdownFilename.TabIndex = 2;
			btnMarkdownFilename.Text = "...";
			btnMarkdownFilename.UseVisualStyleBackColor = true;
			// 
			// btnDocxFilename
			// 
			btnDocxFilename.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
			btnDocxFilename.Location = new System.Drawing.Point(507, 62);
			btnDocxFilename.Name = "btnDocxFilename";
			btnDocxFilename.Size = new System.Drawing.Size(42, 29);
			btnDocxFilename.TabIndex = 5;
			btnDocxFilename.Text = "...";
			btnDocxFilename.UseVisualStyleBackColor = true;
			// 
			// lblDocxTemplateFilename
			// 
			lblDocxTemplateFilename.AutoSize = true;
			lblDocxTemplateFilename.Location = new System.Drawing.Point(22, 101);
			lblDocxTemplateFilename.Name = "lblDocxTemplateFilename";
			lblDocxTemplateFilename.Size = new System.Drawing.Size(114, 20);
			lblDocxTemplateFilename.TabIndex = 6;
			lblDocxTemplateFilename.Text = "Word Template:";
			// 
			// txtMarkdownFilename
			// 
			txtMarkdownFilename.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			txtMarkdownFilename.Location = new System.Drawing.Point(149, 31);
			txtMarkdownFilename.Name = "txtMarkdownFilename";
			txtMarkdownFilename.Size = new System.Drawing.Size(352, 27);
			txtMarkdownFilename.TabIndex = 1;
			// 
			// lblMarkdownFilename
			// 
			lblMarkdownFilename.AutoSize = true;
			lblMarkdownFilename.Location = new System.Drawing.Point(22, 34);
			lblMarkdownFilename.Name = "lblMarkdownFilename";
			lblMarkdownFilename.Size = new System.Drawing.Size(109, 20);
			lblMarkdownFilename.TabIndex = 0;
			lblMarkdownFilename.Text = "Markdown File:";
			// 
			// txtDocxFilename
			// 
			txtDocxFilename.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			txtDocxFilename.Location = new System.Drawing.Point(149, 63);
			txtDocxFilename.Name = "txtDocxFilename";
			txtDocxFilename.Size = new System.Drawing.Size(352, 27);
			txtDocxFilename.TabIndex = 4;
			// 
			// lblDocxFilename
			// 
			lblDocxFilename.AutoSize = true;
			lblDocxFilename.Location = new System.Drawing.Point(22, 66);
			lblDocxFilename.Name = "lblDocxFilename";
			lblDocxFilename.Size = new System.Drawing.Size(121, 20);
			lblDocxFilename.TabIndex = 3;
			lblDocxFilename.Text = "Word Document:";
			// 
			// grpConversion
			// 
			grpConversion.Controls.Add(chkConvertToTable);
			grpConversion.Location = new System.Drawing.Point(24, 200);
			grpConversion.Name = "grpConversion";
			grpConversion.Size = new System.Drawing.Size(575, 138);
			grpConversion.TabIndex = 1;
			grpConversion.TabStop = false;
			grpConversion.Text = "Conversion";
			// 
			// chkConvertToTable
			// 
			chkConvertToTable.AutoSize = true;
			chkConvertToTable.Location = new System.Drawing.Point(22, 35);
			chkConvertToTable.Name = "chkConvertToTable";
			chkConvertToTable.Size = new System.Drawing.Size(204, 24);
			chkConvertToTable.TabIndex = 0;
			chkConvertToTable.Text = "Convert Sections to &Tables";
			chkConvertToTable.UseVisualStyleBackColor = true;
			// 
			// tctlProjectOptions
			// 
			tctlProjectOptions.Controls.Add(tpgSettings);
			tctlProjectOptions.Controls.Add(tpgColumnMap);
			tctlProjectOptions.Controls.Add(tpgUserVariables);
			tctlProjectOptions.Controls.Add(tpgDocumentStyles);
			tctlProjectOptions.Location = new System.Drawing.Point(12, 12);
			tctlProjectOptions.Name = "tctlProjectOptions";
			tctlProjectOptions.SelectedIndex = 0;
			tctlProjectOptions.Size = new System.Drawing.Size(625, 377);
			tctlProjectOptions.TabIndex = 0;
			// 
			// tpgSettings
			// 
			tpgSettings.Controls.Add(grpDocuments);
			tpgSettings.Controls.Add(grpConversion);
			tpgSettings.Location = new System.Drawing.Point(4, 29);
			tpgSettings.Name = "tpgSettings";
			tpgSettings.Padding = new System.Windows.Forms.Padding(3);
			tpgSettings.Size = new System.Drawing.Size(617, 344);
			tpgSettings.TabIndex = 0;
			tpgSettings.Text = "Settings";
			tpgSettings.UseVisualStyleBackColor = true;
			// 
			// tpgColumnMap
			// 
			tpgColumnMap.Controls.Add(dgMappedColumns);
			tpgColumnMap.Location = new System.Drawing.Point(4, 29);
			tpgColumnMap.Name = "tpgColumnMap";
			tpgColumnMap.Size = new System.Drawing.Size(617, 344);
			tpgColumnMap.TabIndex = 3;
			tpgColumnMap.Text = "Column Map";
			tpgColumnMap.UseVisualStyleBackColor = true;
			// 
			// dgMappedColumns
			// 
			dgMappedColumns.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			dgMappedColumns.Dock = System.Windows.Forms.DockStyle.Fill;
			dgMappedColumns.Location = new System.Drawing.Point(0, 0);
			dgMappedColumns.Name = "dgMappedColumns";
			dgMappedColumns.RowHeadersWidth = 51;
			dgMappedColumns.Size = new System.Drawing.Size(617, 344);
			dgMappedColumns.TabIndex = 0;
			// 
			// tpgUserVariables
			// 
			tpgUserVariables.Controls.Add(dgVariables);
			tpgUserVariables.Location = new System.Drawing.Point(4, 29);
			tpgUserVariables.Name = "tpgUserVariables";
			tpgUserVariables.Size = new System.Drawing.Size(617, 344);
			tpgUserVariables.TabIndex = 1;
			tpgUserVariables.Text = "User Variables";
			tpgUserVariables.UseVisualStyleBackColor = true;
			// 
			// dgVariables
			// 
			dgVariables.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			dgVariables.Dock = System.Windows.Forms.DockStyle.Fill;
			dgVariables.Location = new System.Drawing.Point(0, 0);
			dgVariables.Name = "dgVariables";
			dgVariables.RowHeadersWidth = 51;
			dgVariables.Size = new System.Drawing.Size(617, 344);
			dgVariables.TabIndex = 0;
			// 
			// tpgDocumentStyles
			// 
			tpgDocumentStyles.Controls.Add(dgStyles);
			tpgDocumentStyles.Location = new System.Drawing.Point(4, 29);
			tpgDocumentStyles.Name = "tpgDocumentStyles";
			tpgDocumentStyles.Size = new System.Drawing.Size(617, 344);
			tpgDocumentStyles.TabIndex = 2;
			tpgDocumentStyles.Text = "Document Styles";
			tpgDocumentStyles.UseVisualStyleBackColor = true;
			// 
			// dgStyles
			// 
			dgStyles.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			dgStyles.Dock = System.Windows.Forms.DockStyle.Fill;
			dgStyles.Location = new System.Drawing.Point(0, 0);
			dgStyles.Name = "dgStyles";
			dgStyles.RowHeadersWidth = 51;
			dgStyles.Size = new System.Drawing.Size(617, 344);
			dgStyles.TabIndex = 0;
			// 
			// btnOK
			// 
			btnOK.Location = new System.Drawing.Point(425, 395);
			btnOK.Name = "btnOK";
			btnOK.Size = new System.Drawing.Size(103, 43);
			btnOK.TabIndex = 1;
			btnOK.Text = "&OK";
			btnOK.UseVisualStyleBackColor = true;
			// 
			// btnCancel
			// 
			btnCancel.Location = new System.Drawing.Point(534, 395);
			btnCancel.Name = "btnCancel";
			btnCancel.Size = new System.Drawing.Size(103, 43);
			btnCancel.TabIndex = 2;
			btnCancel.Text = "&Cancel";
			btnCancel.UseVisualStyleBackColor = true;
			// 
			// frmProjectOptions
			// 
			AcceptButton = btnOK;
			AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			CancelButton = btnCancel;
			ClientSize = new System.Drawing.Size(649, 450);
			Controls.Add(btnCancel);
			Controls.Add(btnOK);
			Controls.Add(tctlProjectOptions);
			Name = "frmProjectOptions";
			StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			Text = "Project Options";
			grpDocuments.ResumeLayout(false);
			grpDocuments.PerformLayout();
			grpConversion.ResumeLayout(false);
			grpConversion.PerformLayout();
			tctlProjectOptions.ResumeLayout(false);
			tpgSettings.ResumeLayout(false);
			tpgColumnMap.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)dgMappedColumns).EndInit();
			tpgUserVariables.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)dgVariables).EndInit();
			tpgDocumentStyles.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)dgStyles).EndInit();
			ResumeLayout(false);
		}

		#endregion

		private System.Windows.Forms.GroupBox grpDocuments;
		private System.Windows.Forms.Button btnHtmlFilename;
		private System.Windows.Forms.TextBox txtHtmlFilename;
		private System.Windows.Forms.Button btnDocxTemplateFilename;
		private System.Windows.Forms.TextBox txtDocxTemplateFilename;
		private System.Windows.Forms.Label lblHtmlFilename;
		private System.Windows.Forms.Button btnDocxFilename;
		private System.Windows.Forms.Label lblDocxTemplateFilename;
		private System.Windows.Forms.TextBox txtDocxFilename;
		private System.Windows.Forms.Label lblDocxFilename;
		private System.Windows.Forms.GroupBox grpConversion;
		private System.Windows.Forms.TabControl tctlProjectOptions;
		private System.Windows.Forms.TabPage tpgSettings;
		private System.Windows.Forms.TabPage tpgUserVariables;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.CheckBox chkConvertToTable;
		private System.Windows.Forms.TabPage tpgDocumentStyles;
		private System.Windows.Forms.TabPage tpgColumnMap;
		private System.Windows.Forms.DataGridView dgMappedColumns;
		private System.Windows.Forms.DataGridView dgVariables;
		private System.Windows.Forms.DataGridView dgStyles;
		private System.Windows.Forms.Button btnMarkdownFilename;
		private System.Windows.Forms.TextBox txtMarkdownFilename;
		private System.Windows.Forms.Label lblMarkdownFilename;
	}
}