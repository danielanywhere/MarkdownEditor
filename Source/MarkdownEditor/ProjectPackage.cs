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
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

using static MarkdownEditor.MarkdownEditorUtil;

namespace MarkdownEditor
{
	//*-------------------------------------------------------------------------*
	//*	ProjectPackageCollection																								*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of ProjectPackageItem Items.
	/// </summary>
	public class ProjectPackageCollection : List<ProjectPackageItem>
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
	//*	ProjectPackageItem																											*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Individual package of variables.
	/// </summary>
	/// <remarks>
	/// For the most part, these variables are resolved using curly-braced
	/// interpolated string references. For example: {MyVariable}
	/// </remarks>
	public class ProjectPackageItem
	{
		//*************************************************************************
		//*	Private																																*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//* mMappedColumns_CollectionChanged																			*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// The contents of the mapped columns collection have changed.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Notify collection changed event arguments.
		/// </param>
		private void mMappedColumns_CollectionChanged(object sender,
			NotifyCollectionChangedEventArgs e)
		{
			mMappedColumnsChanged = true;
			OnProjectPackageChanged(new EventArgs());
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* mMappedColumns_ItemPropertyChanged																		*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// The property value of an item in the mapped columns collection has
		/// changed.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Property changed event arguments.
		/// </param>
		private void mMappedColumns_ItemPropertyChanged(object sender,
			PropertyChangedEventArgs e)
		{
			mMappedColumnsChanged = true;
			OnProjectPackageChanged(new EventArgs());
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* mSettings_CollectionChanged																						*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// The contents of the project settings collection have changed.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Notify collection changed event arguments.
		/// </param>
		private void mSettings_CollectionChanged(object sender,
			NotifyCollectionChangedEventArgs e)
		{
			mSettingsChanged = true;
			OnProjectPackageChanged(new EventArgs());
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* mSettings_ItemPropertyChanged																					*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// The property value of an item in the project settings collection has
		/// changed.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Property changed event arguments.
		/// </param>
		private void mSettings_ItemPropertyChanged(object sender,
			PropertyChangedEventArgs e)
		{
			mSettingsChanged = true;
			OnProjectPackageChanged(new EventArgs());
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* mStyles_CollectionChanged																							*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// The contents of the styles collection have changed.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Notify collection changed event arguments.
		/// </param>
		private void mStyles_CollectionChanged(object sender,
			NotifyCollectionChangedEventArgs e)
		{
			mStylesChanged = true;
			OnProjectPackageChanged(new EventArgs());
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* mStyles_ItemPropertyChanged																						*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// The property value of an item in the styles collection has changed.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Property changed event arguments.
		/// </param>
		private void mStyles_ItemPropertyChanged(object sender,
			PropertyChangedEventArgs e)
		{
			mStylesChanged = true;
			OnProjectPackageChanged(new EventArgs());
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* mVariables_CollectionChanged																					*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// The contents of the variables collection have changed.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Notify collection changed event arguments.
		/// </param>
		private void mVariables_CollectionChanged(object sender,
			NotifyCollectionChangedEventArgs e)
		{
			mVariablesChanged = true;
			OnProjectPackageChanged(new EventArgs());
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* mVariables_ItemPropertyChanged																				*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// The property value of an item in the variables collection has changed.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Property changed event arguments.
		/// </param>
		private void mVariables_ItemPropertyChanged(object sender,
			PropertyChangedEventArgs e)
		{
			mVariablesChanged = true;
			OnProjectPackageChanged(new EventArgs());
		}
		//*-----------------------------------------------------------------------*

		//*************************************************************************
		//*	Protected																															*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//* OnProjectPackageChanged																								*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Raises the ProjectPackageChanged event when any part of the project
		/// package content has changed.
		/// </summary>
		/// <param name="e">
		/// Standard event arguments.
		/// </param>
		protected virtual void OnProjectPackageChanged(EventArgs e)
		{
			if(ProjectPackageChanged != null)
			{
				ProjectPackageChanged.Invoke(this, e);
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* OnPropertyChanged																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Raises the PropertyChanged event when the value of a property has been
		/// changed.
		/// </summary>
		/// <param name="name">
		/// Name of the property. If called from within a property, this parameter
		/// will be auto-filled with the member's name.
		/// </param>
		protected void OnPropertyChanged([CallerMemberName] string name = null)
		{
			//PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
			mProjectChanged = true;
			OnProjectPackageChanged(new EventArgs());
		}
		//*-----------------------------------------------------------------------*

		//*************************************************************************
		//*	Public																																*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//*	_Constructor																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Create a new instance of the ProjectPackageItem Item.
		/// </summary>
		public ProjectPackageItem()
		{
			mMappedColumns = new VariableEntryCollection();
			mMappedColumns.CollectionChanged += mMappedColumns_CollectionChanged;
			mMappedColumns.ItemPropertyChanged += mMappedColumns_ItemPropertyChanged;
			mSettings = new VariableEntryCollection();
			mSettings.CollectionChanged += mSettings_CollectionChanged;
			mSettings.ItemPropertyChanged += mSettings_ItemPropertyChanged;
			mStyles = new VariableStyleCollection();
			mStyles.CollectionChanged += mStyles_CollectionChanged;
			mStyles.ItemPropertyChanged += mStyles_ItemPropertyChanged;
			mVariables = new VariableEntryCollection();
			mVariables.CollectionChanged += mVariables_CollectionChanged;
			mVariables.ItemPropertyChanged += mVariables_ItemPropertyChanged;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	DocxFilename																													*
		//*-----------------------------------------------------------------------*
		private string mDocxFilename = "";
		/// <summary>
		/// Get/Set the target Word Document filename.
		/// </summary>
		[JsonProperty(Order = 2)]
		public string DocxFilename
		{
			get { return mDocxFilename; }
			set
			{
				bool changed = (mDocxFilename != value);

				mDocxFilename = value;
				if(changed)
				{
					OnPropertyChanged();
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	DocxTemplateFilename																									*
		//*-----------------------------------------------------------------------*
		private string mDocxTemplateFilename = "";
		/// <summary>
		/// Get/Set the source template Word Document filename.
		/// </summary>
		[JsonProperty(Order = 3)]
		public string DocxTemplateFilename
		{
			get { return mDocxTemplateFilename; }
			set
			{
				bool changed = (mDocxTemplateFilename != value);

				mDocxTemplateFilename = value;
				if(changed)
				{
					OnPropertyChanged();
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetDefaultDocxFilename																								*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the name of the default DOCX file.
		/// </summary>
		/// <returns>
		/// Default proposed DOCX filename used to save the file.
		/// </returns>
		public string GetDefaultDocxFilename()
		{
			string result = "";

			if(mDocxFilename?.Length > 0)
			{
				result = GetFullFilename(mWorkingPath, mDocxFilename);
			}
			else if(mMarkdownFilename?.Length > 0)
			{
				result = GetFullFilename(mWorkingPath,
					Path.GetFileNameWithoutExtension(mMarkdownFilename) + ".docx");
			}
			else
			{
				result = GetFullFilename(mWorkingPath,
					$"NewFile-{DateTime.Now:yyyyMMddHHmm}.docx");
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetDefaultDocxTemplateFilename																				*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the name of the default DOCX template file.
		/// </summary>
		/// <returns>
		/// Default proposed DOCX template filename used to save the file.
		/// </returns>
		public string GetDefaultDocxTemplateFilename()
		{
			string result = "";

			if(mDocxTemplateFilename?.Length > 0)
			{
				result = GetFullFilename(mWorkingPath, mDocxTemplateFilename);
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	HtmlFilename																													*
		//*-----------------------------------------------------------------------*
		private string mHtmlFilename = "";
		/// <summary>
		/// Get/Set the filename of the target HTML file.
		/// </summary>
		[JsonProperty(Order = 4)]
		public string HtmlFilename
		{
			get { return mHtmlFilename; }
			set
			{
				bool changed = (mHtmlFilename != value);

				mHtmlFilename = value;
				if(changed)
				{
					OnPropertyChanged();
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	MappedColumns																													*
		//*-----------------------------------------------------------------------*
		private VariableEntryCollection mMappedColumns = null;
		/// <summary>
		/// Get a reference to the collection of mapped section and column names.
		/// </summary>
		[JsonProperty(Order = 9)]
		public VariableEntryCollection MappedColumns
		{
			get { return mMappedColumns; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	MappedColumnsChanged																									*
		//*-----------------------------------------------------------------------*
		private bool mMappedColumnsChanged = false;
		/// <summary>
		/// Get/Set a value indicating whether any of the contents of the mapped
		/// columns collection have changed.
		/// </summary>
		[JsonIgnore]
		public bool MappedColumnsChanged
		{
			get { return mMappedColumnsChanged; }
			set { mMappedColumnsChanged = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	MarkdownChanged																												*
		//*-----------------------------------------------------------------------*
		private bool mMarkdownChanged = false;
		/// <summary>
		/// Get/Set a value indicating whether the content of the markdown text
		/// has changed.
		/// </summary>
		/// <remarks>
		/// Regardless of whether or not a project file is open, the Markdown file
		/// should be storable if its content has changed.
		/// </remarks>
		[JsonIgnore]
		public bool MarkdownChanged
		{
			get { return mMarkdownChanged; }
			set
			{
				mMarkdownChanged = value;
				if(value && mProjectMode == ProjectModeEnum.None)
				{
					mProjectMode = ProjectModeEnum.MarkdownFile;
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	MarkdownFilename																											*
		//*-----------------------------------------------------------------------*
		private string mMarkdownFilename = "";
		/// <summary>
		/// Get/Set the filename of the Markdown file.
		/// </summary>
		[JsonProperty(Order = 1)]
		public string MarkdownFilename
		{
			get { return mMarkdownFilename; }
			set
			{
				bool changed = (mMarkdownFilename != value);

				mMarkdownFilename = value;
				if(changed)
				{
					OnPropertyChanged();
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	OptionConvertToTable																									*
		//*-----------------------------------------------------------------------*
		private bool mOptionConvertToTable = false;
		/// <summary>
		/// Get/Set a value indicating whether to convert sections to tables upon
		/// export.
		/// </summary>
		public bool OptionConvertToTable
		{
			get { return mOptionConvertToTable; }
			set { mOptionConvertToTable = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ProjectChanged																												*
		//*-----------------------------------------------------------------------*
		private bool mProjectChanged = false;
		/// <summary>
		/// Get/Set a value indicating whether any aspect of the project has
		/// changed.
		/// </summary>
		[JsonIgnore]
		public bool ProjectChanged
		{
			get
			{
				return
					(mProjectMode == ProjectModeEnum.MarkdownFile &&
					mMarkdownChanged) ||
					(mProjectMode == ProjectModeEnum.MarkdownProject &&
					(mProjectChanged ||
					mMappedColumnsChanged ||
					mMarkdownChanged ||
					mSettingsChanged ||
					mStylesChanged ||
					mVariablesChanged));
			}
			set
			{
				if(value)
				{
					mProjectChanged = true;
				}
				else
				{
					mProjectChanged =
						mMappedColumnsChanged =
						mMarkdownChanged =
						mSettingsChanged =
						mStylesChanged =
						mVariablesChanged = false;
				}
			}
		}
		//*-----------------------------------------------------------------------*

		////*-----------------------------------------------------------------------*
		////*	ProjectFileActive																											*
		////*-----------------------------------------------------------------------*
		//private bool mProjectFileActive = false;
		///// <summary>
		///// Get/Set a value indicating whether a project file is active.
		///// </summary>
		//[JsonIgnore]
		//public bool ProjectFileActive
		//{
		//	get { return mProjectFileActive; }
		//	set { mProjectFileActive = value; }
		//}
		////*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ProjectMode																														*
		//*-----------------------------------------------------------------------*
		private ProjectModeEnum mProjectMode = ProjectModeEnum.None;
		/// <summary>
		/// Get/Set the current project mode.
		/// </summary>
		[JsonIgnore]
		public ProjectModeEnum ProjectMode
		{
			get { return mProjectMode; }
			set { mProjectMode = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ProjectPackageChanged																									*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Fired when any part of the project package has changed.
		/// </summary>
		public event EventHandler ProjectPackageChanged;
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ResolveVariable																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Resolve the caller's interpolated variable from values within the
		/// package and return the resolved value.
		/// </summary>
		/// <param name="package">
		/// Reference to the variable package containing the values to search.
		/// </param>
		/// <param name="variable">
		/// Interpolated string variable to resolve, using curly brace syntax.
		/// </param>
		/// <param name="unresolvedTemplate">
		/// A string template to use when the specified variable could not be
		/// resolved. Available interpolated values for this template are:
		/// <list type="bullet">
		/// <item>{variable} - The entire variable pattern, as presented by
		/// the caller.</item>
		/// <item>{variableText} - The inner text portion of the caller's
		/// interpolated value.</item>
		/// </list>
		/// </param>
		/// <returns>
		/// The value of the resolved variable, if found. Otherwise, the
		/// value of the unresolvedTemplate parameter, if supplied. Otherwise,
		/// an empty string.
		/// </returns>
		public static string ResolveVariable(ProjectPackageItem package,
			string variable,
			string unresolvedTemplate = "(Unresolved Variable: {variableText})")
		{
			string result = "";
			string text = "";

			if(package != null && variable?.Length > 0)
			{
				text = VariableEntryCollection.ResolveValue(
					package.mVariables, variable);
				if(text.Length > 0)
				{
					//	A value was returned.
					result = text;
				}
				else if(unresolvedTemplate?.Length > 0)
				{
					//	No value was returned but an unresolved template has been
					//	provided.
					text = GetValue(variable,
						ResourceMain.rxInterpolatedExpression, "variable");
					result = unresolvedTemplate.
						Replace("{variableText}", text).
							Replace("{variable}", variable);
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ResolveVariables																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Resolve all of the internal macro values for the specified package.
		/// </summary>
		/// <param name="package">
		/// Reference to the variables package to resolve.
		/// </param>
		public static void ResolveVariables(ProjectPackageItem package)
		{
			if(package != null)
			{
				VariableEntryCollection.ResolveValues(package.mVariables);
			}
		}
		//*-----------------------------------------------------------------------*

		//	Notably, ConvertLinearToTable=[true|false] with default false setting.
		//*-----------------------------------------------------------------------*
		//*	Settings																															*
		//*-----------------------------------------------------------------------*
		private VariableEntryCollection mSettings = null;
		/// <summary>
		/// Get a reference to the collection of project settings.
		/// </summary>
		[JsonProperty(Order = 6)]
		public VariableEntryCollection Settings
		{
			get { return mSettings; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	SettingsChanged																												*
		//*-----------------------------------------------------------------------*
		private bool mSettingsChanged = false;
		/// <summary>
		/// Get/Set a value indicating whether any of the contents of the project
		/// settings collection have changed.
		/// </summary>
		[JsonIgnore]
		public bool SettingsChanged
		{
			get { return mSettingsChanged; }
			set { mSettingsChanged = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ShouldSerializeDocxFilename																						*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether to serialize the DocxFilename
		/// property.
		/// </summary>
		/// <returns>
		/// Value indicating whether the property will be serialized.
		/// </returns>
		public bool ShouldSerializeDocxFilename()
		{
			return mDocxFilename?.Length > 0;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ShouldSerializeDocxTemplateFilename																		*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether to serialize the DocxTemplateFilename
		/// property.
		/// </summary>
		/// <returns>
		/// Value indicating whether the property will be serialized.
		/// </returns>
		public bool ShouldSerializeDocxTemplateFilename()
		{
			return mDocxTemplateFilename?.Length > 0;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ShouldSerializeHtmlFilename																						*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether to serialize the HtmlFilename
		/// property.
		/// </summary>
		/// <returns>
		/// Value indicating whether the property will be serialized.
		/// </returns>
		public bool ShouldSerializeHtmlFilename()
		{
			return mHtmlFilename?.Length > 0;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ShouldSerializeMappedColumns																					*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether to serialize the MappedColumns
		/// property.
		/// </summary>
		/// <returns>
		/// Value indicating whether the property will be serialized.
		/// </returns>
		public bool ShouldSerializeMappedColumns()
		{
			return mMappedColumns.Count > 0;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ShouldSerializeMarkdownFilename																				*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether to serialize the MarkdownFilename
		/// property.
		/// </summary>
		/// <returns>
		/// Value indicating whether the property will be serialized.
		/// </returns>
		public bool ShouldSerializeMarkdownFilename()
		{
			return mMarkdownFilename?.Length > 0;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ShouldSerializeSettings																								*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether to serialize the Settings property.
		/// </summary>
		/// <returns>
		/// Value indicating whether the property will be serialized.
		/// </returns>
		public bool ShouldSerializeSettings()
		{
			return mSettings.Count > 0;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ShouldSerializeStyles																									*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether to serialize the Styles property.
		/// </summary>
		/// <returns>
		/// Value indicating whether the property will be serialized.
		/// </returns>
		public bool ShouldSerializeStyles()
		{
			return mStyles.Count > 0;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ShouldSerializeTextFilename																						*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether to serialize the TextFilename
		/// property.
		/// </summary>
		/// <returns>
		/// Value indicating whether the property will be serialized.
		/// </returns>
		public bool ShouldSerializeTextFilename()
		{
			return mTextFilename?.Length > 0;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ShouldSerializeVariables																							*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether to serialize the Variables property.
		/// </summary>
		/// <returns>
		/// Value indicating whether the property will be serialized.
		/// </returns>
		public bool ShouldSerializeVariables()
		{
			return mVariables.Count > 0;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ShouldSerializeWorkingPath																						*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether to serialize the WorkingPath
		/// property.
		/// </summary>
		/// <returns>
		/// Value indicating whether the property will be serialized.
		/// </returns>
		public bool ShouldSerializeWorkingPath()
		{
			return mWorkingPath?.Length > 0;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Styles																																*
		//*-----------------------------------------------------------------------*
		private VariableStyleCollection mStyles = null;
		/// <summary>
		/// Get a reference to the collection of styles for this package.
		/// </summary>
		[JsonProperty(Order = 8)]
		public VariableStyleCollection Styles
		{
			get { return mStyles; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	StylesChanged																													*
		//*-----------------------------------------------------------------------*
		private bool mStylesChanged = false;
		/// <summary>
		/// Get/Set a value indicating whether any of the contents of the styles
		/// collection have changed.
		/// </summary>
		[JsonIgnore]
		public bool StylesChanged
		{
			get { return mStylesChanged; }
			set { mStylesChanged = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	TextFilename																													*
		//*-----------------------------------------------------------------------*
		private string mTextFilename = "";
		/// <summary>
		/// Get/Set the filename of the export text file.
		/// </summary>
		[JsonProperty(Order = 5)]
		public string TextFilename
		{
			get { return mTextFilename; }
			set
			{
				bool changed = (mTextFilename != value);

				mTextFilename = value;
				if(changed)
				{
					OnPropertyChanged();
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Variables																															*
		//*-----------------------------------------------------------------------*
		private VariableEntryCollection mVariables = null;
		/// <summary>
		/// Get a reference to the collection of variable definition entries in
		/// this package.
		/// </summary>
		[JsonProperty(Order = 7)]
		public VariableEntryCollection Variables
		{
			get { return mVariables; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	VariablesChanged																											*
		//*-----------------------------------------------------------------------*
		private bool mVariablesChanged = false;
		/// <summary>
		/// Get/Set a value indicating whether any of the contents of the variables
		/// collection have changed.
		/// </summary>
		[JsonIgnore]
		public bool VariablesChanged
		{
			get { return mVariablesChanged; }
			set { mVariablesChanged = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	WorkingPath																														*
		//*-----------------------------------------------------------------------*
		private string mWorkingPath = "";
		/// <summary>
		/// Get/Set the working path for files in this project.
		/// </summary>
		[JsonProperty(Order = 0)]
		public string WorkingPath
		{
			get { return mWorkingPath; }
			set
			{
				bool changed = (mWorkingPath != value);

				mWorkingPath = value;
				if(changed)
				{
					OnPropertyChanged();
				}
			}
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*


}
