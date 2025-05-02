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

using System.Windows.Forms;

namespace MarkdownEditor
{
	partial class frmMain
	{
		/// <summary>
		///  Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		///  Clean up any resources being used.
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
		///  Required method for Designer support - do not modify
		///  the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
			menuStripMain = new MenuStrip();
			mnuFile = new ToolStripMenuItem();
			mnuFileOpenProject = new ToolStripMenuItem();
			mnuFileSaveProject = new ToolStripMenuItem();
			mnuFileSaveProjectAs = new ToolStripMenuItem();
			mnuFileCloseProject = new ToolStripMenuItem();
			mnuFileSep1 = new ToolStripSeparator();
			mnuFileOpen = new ToolStripMenuItem();
			mnuFileSave = new ToolStripMenuItem();
			mnuFileSaveAs = new ToolStripMenuItem();
			mnuFileClose = new ToolStripMenuItem();
			mnuFileSep2 = new ToolStripSeparator();
			mnuFileImport = new ToolStripMenuItem();
			mnuFileImportVariablePackage = new ToolStripMenuItem();
			mnuFileImportVariables = new ToolStripMenuItem();
			mnuFileImportMappedColumnNames = new ToolStripMenuItem();
			mnuFileImportDocumentStyles = new ToolStripMenuItem();
			mnuFileExport = new ToolStripMenuItem();
			mnuFileExportText = new ToolStripMenuItem();
			mnuFileExportDocXTemplate = new ToolStripMenuItem();
			mnuFileExportDocX = new ToolStripMenuItem();
			mnuFileExportHtml = new ToolStripMenuItem();
			mnuFileExportSep1 = new ToolStripSeparator();
			mnuFileExportVariablePackage = new ToolStripMenuItem();
			mnuFileExportVariables = new ToolStripMenuItem();
			mnuFileExportMappedColumnNames = new ToolStripMenuItem();
			mnuFileExportDocumentStyles = new ToolStripMenuItem();
			mnuFileConvert = new ToolStripMenuItem();
			mnuFileConvertOldProjectToNew = new ToolStripMenuItem();
			mnuFileSep3 = new ToolStripSeparator();
			mnuFileExit = new ToolStripMenuItem();
			mnuEdit = new ToolStripMenuItem();
			mnuEditUndo = new ToolStripMenuItem();
			mnuEditRedo = new ToolStripMenuItem();
			mnuEditSep1 = new ToolStripSeparator();
			mnuEditSectionStyleClipboard = new ToolStripMenuItem();
			mnuEditSectionStyleAppend = new ToolStripMenuItem();
			mnuEditSectionInsert = new ToolStripMenuItem();
			mnuEditSep2 = new ToolStripSeparator();
			mnuEditCopy = new ToolStripMenuItem();
			mnuEditCut = new ToolStripMenuItem();
			mnuEditPaste = new ToolStripMenuItem();
			mnuEditSep3 = new ToolStripSeparator();
			mnuEditProjectOptions = new ToolStripMenuItem();
			mnuTools = new ToolStripMenuItem();
			mnuToolsConvertToTableStyle = new ToolStripMenuItem();
			mnuToolsSep1 = new ToolStripSeparator();
			mnuToolsApplyColumnMappingValues = new ToolStripMenuItem();
			mnuToolsApplyDocumentStyles = new ToolStripMenuItem();
			mnuToolsResolveUserVariables = new ToolStripMenuItem();
			statusStripMain = new StatusStrip();
			statMessage = new ToolStripStatusLabel();
			statVersion = new ToolStripStatusLabel();
			webView21 = new Microsoft.Web.WebView2.WinForms.WebView2();
			menuStripMain.SuspendLayout();
			statusStripMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)webView21).BeginInit();
			SuspendLayout();
			// 
			// menuStripMain
			// 
			menuStripMain.ImageScalingSize = new System.Drawing.Size(20, 20);
			menuStripMain.Items.AddRange(new ToolStripItem[] { mnuFile, mnuEdit, mnuTools });
			menuStripMain.Location = new System.Drawing.Point(0, 0);
			menuStripMain.Name = "menuStripMain";
			menuStripMain.Size = new System.Drawing.Size(800, 28);
			menuStripMain.TabIndex = 0;
			menuStripMain.Text = "menuStrip1";
			// 
			// mnuFile
			// 
			mnuFile.DropDownItems.AddRange(new ToolStripItem[] { mnuFileOpenProject, mnuFileSaveProject, mnuFileSaveProjectAs, mnuFileCloseProject, mnuFileSep1, mnuFileOpen, mnuFileSave, mnuFileSaveAs, mnuFileClose, mnuFileSep2, mnuFileImport, mnuFileExport, mnuFileConvert, mnuFileSep3, mnuFileExit });
			mnuFile.Name = "mnuFile";
			mnuFile.Size = new System.Drawing.Size(46, 24);
			mnuFile.Text = "&File";
			// 
			// mnuFileOpenProject
			// 
			mnuFileOpenProject.Name = "mnuFileOpenProject";
			mnuFileOpenProject.ShortcutKeys = Keys.Control | Keys.Shift | Keys.O;
			mnuFileOpenProject.Size = new System.Drawing.Size(350, 26);
			mnuFileOpenProject.Text = "Open Markdown &Project";
			// 
			// mnuFileSaveProject
			// 
			mnuFileSaveProject.Name = "mnuFileSaveProject";
			mnuFileSaveProject.ShortcutKeys = Keys.Control | Keys.Shift | Keys.S;
			mnuFileSaveProject.Size = new System.Drawing.Size(350, 26);
			mnuFileSaveProject.Text = "Save Project";
			// 
			// mnuFileSaveProjectAs
			// 
			mnuFileSaveProjectAs.Name = "mnuFileSaveProjectAs";
			mnuFileSaveProjectAs.Size = new System.Drawing.Size(350, 26);
			mnuFileSaveProjectAs.Text = "Sa&ve Project As";
			// 
			// mnuFileCloseProject
			// 
			mnuFileCloseProject.Name = "mnuFileCloseProject";
			mnuFileCloseProject.ShortcutKeys = Keys.Control | Keys.Alt | Keys.Shift | Keys.C;
			mnuFileCloseProject.Size = new System.Drawing.Size(350, 26);
			mnuFileCloseProject.Text = "Close Current Project";
			// 
			// mnuFileSep1
			// 
			mnuFileSep1.Name = "mnuFileSep1";
			mnuFileSep1.Size = new System.Drawing.Size(347, 6);
			// 
			// mnuFileOpen
			// 
			mnuFileOpen.Name = "mnuFileOpen";
			mnuFileOpen.ShortcutKeys = Keys.Control | Keys.O;
			mnuFileOpen.Size = new System.Drawing.Size(350, 26);
			mnuFileOpen.Text = "&Open Markdown File";
			// 
			// mnuFileSave
			// 
			mnuFileSave.Name = "mnuFileSave";
			mnuFileSave.ShortcutKeys = Keys.Control | Keys.S;
			mnuFileSave.Size = new System.Drawing.Size(350, 26);
			mnuFileSave.Text = "&Save File";
			// 
			// mnuFileSaveAs
			// 
			mnuFileSaveAs.Name = "mnuFileSaveAs";
			mnuFileSaveAs.Size = new System.Drawing.Size(350, 26);
			mnuFileSaveAs.Text = "Save &As";
			// 
			// mnuFileClose
			// 
			mnuFileClose.Name = "mnuFileClose";
			mnuFileClose.ShortcutKeys = Keys.Control | Keys.Alt | Keys.C;
			mnuFileClose.Size = new System.Drawing.Size(350, 26);
			mnuFileClose.Text = "&Close Current File";
			// 
			// mnuFileSep2
			// 
			mnuFileSep2.Name = "mnuFileSep2";
			mnuFileSep2.Size = new System.Drawing.Size(347, 6);
			// 
			// mnuFileImport
			// 
			mnuFileImport.DropDownItems.AddRange(new ToolStripItem[] { mnuFileImportVariablePackage, mnuFileImportVariables, mnuFileImportMappedColumnNames, mnuFileImportDocumentStyles });
			mnuFileImport.Name = "mnuFileImport";
			mnuFileImport.Size = new System.Drawing.Size(350, 26);
			mnuFileImport.Text = "&Import";
			// 
			// mnuFileImportVariablePackage
			// 
			mnuFileImportVariablePackage.Name = "mnuFileImportVariablePackage";
			mnuFileImportVariablePackage.Size = new System.Drawing.Size(328, 26);
			mnuFileImportVariablePackage.Text = "Variable Package File";
			// 
			// mnuFileImportVariables
			// 
			mnuFileImportVariables.Name = "mnuFileImportVariables";
			mnuFileImportVariables.Size = new System.Drawing.Size(328, 26);
			mnuFileImportVariables.Text = "&Variables from JSON";
			// 
			// mnuFileImportMappedColumnNames
			// 
			mnuFileImportMappedColumnNames.Name = "mnuFileImportMappedColumnNames";
			mnuFileImportMappedColumnNames.Size = new System.Drawing.Size(328, 26);
			mnuFileImportMappedColumnNames.Text = "&Mapped Column Names from JSON";
			// 
			// mnuFileImportDocumentStyles
			// 
			mnuFileImportDocumentStyles.Name = "mnuFileImportDocumentStyles";
			mnuFileImportDocumentStyles.Size = new System.Drawing.Size(328, 26);
			mnuFileImportDocumentStyles.Text = "Document &Styles from JSON";
			// 
			// mnuFileExport
			// 
			mnuFileExport.DropDownItems.AddRange(new ToolStripItem[] { mnuFileExportText, mnuFileExportDocXTemplate, mnuFileExportDocX, mnuFileExportHtml, mnuFileExportSep1, mnuFileExportVariablePackage, mnuFileExportVariables, mnuFileExportMappedColumnNames, mnuFileExportDocumentStyles });
			mnuFileExport.Name = "mnuFileExport";
			mnuFileExport.Size = new System.Drawing.Size(350, 26);
			mnuFileExport.Text = "&Export";
			// 
			// mnuFileExportText
			// 
			mnuFileExportText.Name = "mnuFileExportText";
			mnuFileExportText.Size = new System.Drawing.Size(318, 26);
			mnuFileExportText.Text = "Markdown as &Text";
			mnuFileExportText.ToolTipText = "Export a copy of this code, in its natural form, to a standard text file.";
			// 
			// mnuFileExportDocXTemplate
			// 
			mnuFileExportDocXTemplate.Name = "mnuFileExportDocXTemplate";
			mnuFileExportDocXTemplate.Size = new System.Drawing.Size(318, 26);
			mnuFileExportDocXTemplate.Text = "Markdown as &DocX with Template";
			mnuFileExportDocXTemplate.ToolTipText = "Export a rendered version of this code to a Word DOCX file, using another master DOCX file as a template.";
			// 
			// mnuFileExportDocX
			// 
			mnuFileExportDocX.Name = "mnuFileExportDocX";
			mnuFileExportDocX.Size = new System.Drawing.Size(318, 26);
			mnuFileExportDocX.Text = "Markdown as Doc&X";
			mnuFileExportDocX.ToolTipText = "Export a rendered version of this code to a Word DOCX file, using default formatting values on the target document.";
			// 
			// mnuFileExportHtml
			// 
			mnuFileExportHtml.Name = "mnuFileExportHtml";
			mnuFileExportHtml.Size = new System.Drawing.Size(318, 26);
			mnuFileExportHtml.Text = "Markdown as &HTML";
			mnuFileExportHtml.ToolTipText = "Export a copy of the code to HTML.";
			// 
			// mnuFileExportSep1
			// 
			mnuFileExportSep1.Name = "mnuFileExportSep1";
			mnuFileExportSep1.Size = new System.Drawing.Size(315, 6);
			// 
			// mnuFileExportVariablePackage
			// 
			mnuFileExportVariablePackage.Name = "mnuFileExportVariablePackage";
			mnuFileExportVariablePackage.Size = new System.Drawing.Size(318, 26);
			mnuFileExportVariablePackage.Text = "Variable Package File";
			// 
			// mnuFileExportVariables
			// 
			mnuFileExportVariables.Name = "mnuFileExportVariables";
			mnuFileExportVariables.Size = new System.Drawing.Size(318, 26);
			mnuFileExportVariables.Text = "&Variables as JSON";
			// 
			// mnuFileExportMappedColumnNames
			// 
			mnuFileExportMappedColumnNames.Name = "mnuFileExportMappedColumnNames";
			mnuFileExportMappedColumnNames.Size = new System.Drawing.Size(318, 26);
			mnuFileExportMappedColumnNames.Text = "&Mapped Column Names as JSON";
			// 
			// mnuFileExportDocumentStyles
			// 
			mnuFileExportDocumentStyles.Name = "mnuFileExportDocumentStyles";
			mnuFileExportDocumentStyles.Size = new System.Drawing.Size(318, 26);
			mnuFileExportDocumentStyles.Text = "Document &Styles as JSON";
			// 
			// mnuFileConvert
			// 
			mnuFileConvert.DropDownItems.AddRange(new ToolStripItem[] { mnuFileConvertOldProjectToNew });
			mnuFileConvert.Name = "mnuFileConvert";
			mnuFileConvert.Size = new System.Drawing.Size(350, 26);
			mnuFileConvert.Text = "Conve&rt";
			// 
			// mnuFileConvertOldProjectToNew
			// 
			mnuFileConvertOldProjectToNew.Name = "mnuFileConvertOldProjectToNew";
			mnuFileConvertOldProjectToNew.Size = new System.Drawing.Size(364, 26);
			mnuFileConvertOldProjectToNew.Text = "Old Markdown Projects to &New (.mdproj)";
			// 
			// mnuFileSep3
			// 
			mnuFileSep3.Name = "mnuFileSep3";
			mnuFileSep3.Size = new System.Drawing.Size(347, 6);
			// 
			// mnuFileExit
			// 
			mnuFileExit.Name = "mnuFileExit";
			mnuFileExit.Size = new System.Drawing.Size(350, 26);
			mnuFileExit.Text = "E&xit";
			// 
			// mnuEdit
			// 
			mnuEdit.DropDownItems.AddRange(new ToolStripItem[] { mnuEditUndo, mnuEditRedo, mnuEditSep1, mnuEditSectionStyleClipboard, mnuEditSectionStyleAppend, mnuEditSectionInsert, mnuEditSep2, mnuEditCopy, mnuEditCut, mnuEditPaste, mnuEditSep3, mnuEditProjectOptions });
			mnuEdit.Name = "mnuEdit";
			mnuEdit.Size = new System.Drawing.Size(49, 24);
			mnuEdit.Text = "&Edit";
			// 
			// mnuEditUndo
			// 
			mnuEditUndo.Name = "mnuEditUndo";
			mnuEditUndo.Size = new System.Drawing.Size(429, 26);
			mnuEditUndo.Text = "&Undo (Ctrl+Z)";
			// 
			// mnuEditRedo
			// 
			mnuEditRedo.Name = "mnuEditRedo";
			mnuEditRedo.Size = new System.Drawing.Size(429, 26);
			mnuEditRedo.Text = "&Redo (Ctrl+Y)";
			// 
			// mnuEditSep1
			// 
			mnuEditSep1.Name = "mnuEditSep1";
			mnuEditSep1.Size = new System.Drawing.Size(426, 6);
			// 
			// mnuEditSectionStyleClipboard
			// 
			mnuEditSectionStyleClipboard.Name = "mnuEditSectionStyleClipboard";
			mnuEditSectionStyleClipboard.ShortcutKeys = Keys.Control | Keys.Shift | Keys.C;
			mnuEditSectionStyleClipboard.Size = new System.Drawing.Size(429, 26);
			mnuEditSectionStyleClipboard.Text = "Load Clipboard with &Section Style";
			// 
			// mnuEditSectionStyleAppend
			// 
			mnuEditSectionStyleAppend.Name = "mnuEditSectionStyleAppend";
			mnuEditSectionStyleAppend.ShortcutKeys = Keys.Control | Keys.Shift | Keys.V;
			mnuEditSectionStyleAppend.Size = new System.Drawing.Size(429, 26);
			mnuEditSectionStyleAppend.Text = "Append S&ection Style to End of Code";
			// 
			// mnuEditSectionInsert
			// 
			mnuEditSectionInsert.Name = "mnuEditSectionInsert";
			mnuEditSectionInsert.ShortcutKeys = Keys.Control | Keys.Shift | Keys.I;
			mnuEditSectionInsert.Size = new System.Drawing.Size(429, 26);
			mnuEditSectionInsert.Text = "&Insert Section";
			// 
			// mnuEditSep2
			// 
			mnuEditSep2.Name = "mnuEditSep2";
			mnuEditSep2.Size = new System.Drawing.Size(426, 6);
			// 
			// mnuEditCopy
			// 
			mnuEditCopy.Name = "mnuEditCopy";
			mnuEditCopy.Size = new System.Drawing.Size(429, 26);
			mnuEditCopy.Text = "Copy (Ctrl+C)";
			// 
			// mnuEditCut
			// 
			mnuEditCut.Name = "mnuEditCut";
			mnuEditCut.Size = new System.Drawing.Size(429, 26);
			mnuEditCut.Text = "Cut (Ctrl+X)";
			// 
			// mnuEditPaste
			// 
			mnuEditPaste.Name = "mnuEditPaste";
			mnuEditPaste.Size = new System.Drawing.Size(429, 26);
			mnuEditPaste.Text = "Paste (Ctrl+V)";
			// 
			// mnuEditSep3
			// 
			mnuEditSep3.Name = "mnuEditSep3";
			mnuEditSep3.Size = new System.Drawing.Size(426, 6);
			// 
			// mnuEditProjectOptions
			// 
			mnuEditProjectOptions.Name = "mnuEditProjectOptions";
			mnuEditProjectOptions.ShortcutKeys = Keys.Alt | Keys.F10;
			mnuEditProjectOptions.Size = new System.Drawing.Size(429, 26);
			mnuEditProjectOptions.Text = "Project &Options";
			// 
			// mnuTools
			// 
			mnuTools.DropDownItems.AddRange(new ToolStripItem[] { mnuToolsConvertToTableStyle, mnuToolsSep1, mnuToolsApplyColumnMappingValues, mnuToolsApplyDocumentStyles, mnuToolsResolveUserVariables });
			mnuTools.Name = "mnuTools";
			mnuTools.Size = new System.Drawing.Size(58, 24);
			mnuTools.Text = "&Tools";
			// 
			// mnuToolsConvertToTableStyle
			// 
			mnuToolsConvertToTableStyle.Name = "mnuToolsConvertToTableStyle";
			mnuToolsConvertToTableStyle.Size = new System.Drawing.Size(296, 26);
			mnuToolsConvertToTableStyle.Text = "Convert Linear to &Table-Style";
			// 
			// mnuToolsSep1
			// 
			mnuToolsSep1.Name = "mnuToolsSep1";
			mnuToolsSep1.Size = new System.Drawing.Size(293, 6);
			// 
			// mnuToolsApplyColumnMappingValues
			// 
			mnuToolsApplyColumnMappingValues.Name = "mnuToolsApplyColumnMappingValues";
			mnuToolsApplyColumnMappingValues.Size = new System.Drawing.Size(296, 26);
			mnuToolsApplyColumnMappingValues.Text = "Apply &Column Mapping Values";
			// 
			// mnuToolsApplyDocumentStyles
			// 
			mnuToolsApplyDocumentStyles.Name = "mnuToolsApplyDocumentStyles";
			mnuToolsApplyDocumentStyles.Size = new System.Drawing.Size(296, 26);
			mnuToolsApplyDocumentStyles.Text = "Apply &Document Styles";
			// 
			// mnuToolsResolveUserVariables
			// 
			mnuToolsResolveUserVariables.Name = "mnuToolsResolveUserVariables";
			mnuToolsResolveUserVariables.Size = new System.Drawing.Size(296, 26);
			mnuToolsResolveUserVariables.Text = "Resolve User &Variables";
			// 
			// statusStripMain
			// 
			statusStripMain.ImageScalingSize = new System.Drawing.Size(20, 20);
			statusStripMain.Items.AddRange(new ToolStripItem[] { statMessage, statVersion });
			statusStripMain.Location = new System.Drawing.Point(0, 424);
			statusStripMain.Name = "statusStripMain";
			statusStripMain.Size = new System.Drawing.Size(800, 26);
			statusStripMain.TabIndex = 1;
			statusStripMain.Text = "statusStrip1";
			// 
			// statMessage
			// 
			statMessage.Name = "statMessage";
			statMessage.Size = new System.Drawing.Size(59, 20);
			statMessage.Text = "Ready...";
			// 
			// statVersion
			// 
			statVersion.Name = "statVersion";
			statVersion.Size = new System.Drawing.Size(726, 20);
			statVersion.Spring = true;
			statVersion.Text = "Ver: 24.0626.0953";
			statVersion.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// webView21
			// 
			webView21.AllowExternalDrop = true;
			webView21.BackColor = System.Drawing.Color.White;
			webView21.CreationProperties = null;
			webView21.DefaultBackgroundColor = System.Drawing.Color.White;
			webView21.Dock = DockStyle.Fill;
			webView21.Location = new System.Drawing.Point(0, 28);
			webView21.Name = "webView21";
			webView21.Size = new System.Drawing.Size(800, 396);
			webView21.TabIndex = 2;
			webView21.ZoomFactor = 1D;
			// 
			// frmMain
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new System.Drawing.Size(800, 450);
			Controls.Add(webView21);
			Controls.Add(statusStripMain);
			Controls.Add(menuStripMain);
			Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
			KeyPreview = true;
			MainMenuStrip = menuStripMain;
			Name = "frmMain";
			Text = "Markdown Editor";
			menuStripMain.ResumeLayout(false);
			menuStripMain.PerformLayout();
			statusStripMain.ResumeLayout(false);
			statusStripMain.PerformLayout();
			((System.ComponentModel.ISupportInitialize)webView21).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private MenuStrip menuStripMain;
		private ToolStripMenuItem mnuFile;
		private StatusStrip statusStripMain;
		private ToolStripStatusLabel statMessage;
		private Microsoft.Web.WebView2.WinForms.WebView2 webView21;
		private ToolStripMenuItem mnuFileOpen;
		private ToolStripMenuItem mnuFileSave;
		private ToolStripMenuItem mnuFileSaveAs;
		private ToolStripMenuItem mnuFileClose;
		private ToolStripSeparator mnuFileSep1;
		private ToolStripMenuItem mnuFileExit;
		private ToolStripMenuItem mnuEdit;
		private ToolStripMenuItem mnuEditSectionStyleClipboard;
		private ToolStripMenuItem mnuEditSectionStyleAppend;
		private ToolStripMenuItem mnuTools;
		private ToolStripMenuItem mnuToolsConvertToTableStyle;
		private ToolStripMenuItem mnuEditSectionInsert;
		private ToolStripSeparator mnuEditSep1;
		private ToolStripMenuItem mnuEditCopy;
		private ToolStripMenuItem mnuEditCut;
		private ToolStripMenuItem mnuEditPaste;
		private ToolStripMenuItem mnuEditUndo;
		private ToolStripMenuItem mnuFileExport;
		private ToolStripMenuItem mnuFileExportText;
		private ToolStripMenuItem mnuFileExportDocXTemplate;
		private ToolStripMenuItem mnuFileExportDocX;
		private ToolStripSeparator mnuFileSep2;
		private ToolStripMenuItem mnuEditRedo;
		private ToolStripSeparator mnuEditSep2;
		private ToolStripMenuItem mnuFileExportHtml;
		private ToolStripMenuItem mnuFileOpenProject;
		private ToolStripMenuItem mnuFileSaveProject;
		private ToolStripMenuItem mnuFileSaveProjectAs;
		private ToolStripMenuItem mnuFileCloseProject;
		private ToolStripSeparator mnuFileSep3;
		private ToolStripMenuItem mnuFileImport;
		private ToolStripMenuItem mnuFileImportVariablePackage;
		private ToolStripMenuItem mnuFileImportVariables;
		private ToolStripMenuItem mnuFileImportMappedColumnNames;
		private ToolStripMenuItem mnuFileImportDocumentStyles;
		private ToolStripSeparator mnuFileExportSep1;
		private ToolStripMenuItem mnuFileExportVariablePackage;
		private ToolStripMenuItem mnuFileExportVariables;
		private ToolStripMenuItem mnuFileExportMappedColumnNames;
		private ToolStripMenuItem mnuFileExportDocumentStyles;
		private ToolStripSeparator mnuEditSep3;
		private ToolStripMenuItem mnuEditProjectOptions;
		private ToolStripSeparator mnuToolsSep1;
		private ToolStripMenuItem mnuToolsApplyColumnMappingValues;
		private ToolStripMenuItem mnuToolsApplyDocumentStyles;
		private ToolStripMenuItem mnuToolsResolveUserVariables;
		private ToolStripStatusLabel statVersion;
		private ToolStripMenuItem mnuFileConvert;
		private ToolStripMenuItem mnuFileConvertOldProjectToNew;
	}
}
