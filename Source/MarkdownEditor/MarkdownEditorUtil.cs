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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

using Html;
using Newtonsoft.Json;

namespace MarkdownEditor
{
	//*-------------------------------------------------------------------------*
	//*	MarkdownEditorUtil																											*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Utility features and functionality for the MarkdownEditor application.
	/// </summary>
	public class MarkdownEditorUtil
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
		//* Clear																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Clear the contents of the specified string builder.
		/// </summary>
		/// <param name="builder">
		/// Reference to the string builder to clear.
		/// </param>
		public static void Clear(StringBuilder builder)
		{
			if(builder?.Length > 0)
			{
				builder.Remove(0, builder.Length);
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetFontOpenTag																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the font open tag for the font properties.
		/// </summary>
		/// <param name="properties">
		/// Reference to a list of properties to be used to build the tag.
		/// </param>
		/// <returns>
		/// An HTML4 font tag with associated properties.
		/// </returns>
		public static string GetFontOpenTag(List<PropertyItem> properties)
		{
			StringBuilder builder = new StringBuilder();
			PropertyItem property = null;

			builder.Append("<font");
			if(properties?.Count > 0)
			{
				property = properties.FirstOrDefault(x => x.Name == "FontColor");
				if(property?.Value != null)
				{
					builder.Append($" color=\"{property.Value}\"");
				}
				property = properties.FirstOrDefault(x => x.Name == "FontName");
				if(property?.Value != null)
				{
					builder.Append($" face=\"{property.Value}\"");
				}
				property = properties.FirstOrDefault(x => x.Name == "FontSize");
				if(property.Value != null)
				{
					builder.Append($" style=\"font-size:{property.Value}pt\"");
				}
			}
			builder.Append(">");
			return builder.ToString();
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetFullFilename																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the full filename of the caller's base working path and relative
		/// filename.
		/// </summary>
		/// <param name="workingPath">
		/// The base working path to use with a relative filename.
		/// </param>
		/// <param name="relativeFilename">
		/// A full or relative filename.
		/// </param>
		/// <returns>
		/// The fully qualified path and filename of the caller's base path and
		/// filename, if found. Otherwise, an empty string.
		/// </returns>
		public static string GetFullFilename(string workingPath,
			string relativeFilename)
		{
			string result = "";

			if(relativeFilename?.Length > 0)
			{
				//	A relative filename was provided.
				if(relativeFilename.IndexOf(':') > -1 ||
					relativeFilename.IndexOf("\\\\") > -1 ||
					!(workingPath?.Length > 0))
				{
					//	Anything containing a colon or double backslash is a full
					//	path.
					result = relativeFilename;
				}
				else
				{
					//	Otherwise, a working path is present.
					result = Path.Combine(workingPath, relativeFilename);
				}
			}
			else if(workingPath?.Length > 0)
			{
				//	A relative filename was not provided, but a working path was.
				result = workingPath;
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetGroup																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a reference to the specified group member in the provided match.
		/// </summary>
		/// <param name="match">
		/// Reference to the match to be inspected.
		/// </param>
		/// <param name="groupName">
		/// Name of the group to be found.
		/// </param>
		/// <returns>
		/// The reference to the specified group, if found. Otherwise, null.
		/// </returns>
		public static Group GetGroup(Match match, string groupName)
		{
			Group result = null;

			if(match != null && match.Groups[groupName] != null &&
				match.Groups[groupName].Value != null)
			{
				result = match.Groups[groupName];
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetHeadingLevel																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the heading level of the provided Markdown line.
		/// </summary>
		/// <param name="line">
		/// A Markdown text line that might express a heading level.
		/// </param>
		/// <returns>
		/// The heading level of the text line, if found. Otherwise, 0.
		/// </returns>
		public static int GetHeadingLevel(string line)
		{
			int result = 0;
			string text = "";

			if(line?.Length > 0)
			{
				text = GetValue(line, ResourceMain.rxMarkdownHeading, "level");
				result = text.Length;
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetHeadingText																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the text of the caller's Markdown heading line.
		/// </summary>
		/// <param name="headingLine">
		/// Markdown heading line to inspect.
		/// </param>
		/// <returns>
		/// Text portion of the heading.
		/// </returns>
		public static string GetHeadingText(string headingLine)
		{
			string result = "";

			if(headingLine?.Length > 0)
			{
				result = GetValue(headingLine,
					ResourceMain.rxMarkdownHeading, "line");
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetIndex																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the source index of the specified group member in the provided
		/// match.
		/// </summary>
		/// <param name="match">
		/// Reference to the match to be inspected.
		/// </param>
		/// <param name="groupName">
		/// Name of the group for which the value will be found.
		/// </param>
		/// <returns>
		/// The source index found in the specified group, if found. Otherwise, -1.
		/// </returns>
		public static int GetIndex(Match match, string groupName)
		{
			int result = -1;

			if(match != null && match.Groups[groupName] != null &&
				match.Groups[groupName].Value != null)
			{
				result = match.Groups[groupName].Index;
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetLineCount																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the count of lines in the provided source string.
		/// </summary>
		/// <param name="source">
		/// String to search.
		/// </param>
		/// <returns>
		/// Count of lines in the caller's source string.
		/// </returns>
		public static int GetLineCount(string source)
		{
			MatchCollection matches = null;
			int result = 0;

			if(source?.Length > 0)
			{
				matches = Regex.Matches(source, ResourceMain.rxLine);
				result = matches.Count;
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetLines																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the caller's string, separated into distinct lines.
		/// </summary>
		/// <param name="source">
		/// String to parse.
		/// </param>
		/// <returns>
		/// Reference to a collection of lines.
		/// </returns>
		public static List<string> GetLines(string source)
		{
			MatchCollection matches = null;
			List<string> result = new List<string>();

			if(source?.Length > 0)
			{
				matches = Regex.Matches(source, ResourceMain.rxLine);
				foreach(Match matchItem in matches)
				{
					result.Add(GetValue(matchItem, "line"));
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	GetRelativeFilename																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the relative form of the filename, given a base directory and
		/// a filename to prepare.
		/// </summary>
		/// <param name="baseName">
		/// Base directory name.
		/// </param>
		/// <param name="filename">
		/// Full or partial name of the file to prepare.
		/// </param>
		/// <returns>
		/// Relative filename.
		/// </returns>
		public static string GetRelativeFilename(string baseName, string filename)
		{
			List<string> baseElements = new List<string>();
			MatchCollection baseMatches = null;
			StringBuilder builder = new StringBuilder();
			int count = 0;
			List<string> filenameElements = new List<string>();
			MatchCollection filenameMatches = null;
			int index = 0;
			Match match = null;
			int matchIndex = 0;
			StringBuilder prefix = new StringBuilder();
			string result = (filename?.Length > 0 ? filename : "");

			//	Some portion of the base and file names was shared, allowing for
			//	a relative name.
			baseMatches = Regex.Matches(baseName.ToLower(),
				ResourceMain.rxFolderElement);
			foreach(Match matchItem in baseMatches)
			{
				baseElements.Add(GetValue(matchItem, "folder"));
			}
			filenameMatches = Regex.Matches(filename.ToLower(),
				ResourceMain.rxFolderElement);
			foreach(Match matchItem in filenameMatches)
			{
				filenameElements.Add(GetValue(matchItem, "folder"));
			}
			count = Math.Min(baseElements.Count, filenameElements.Count);
			for(index = 0; index < count; index++)
			{
				if(baseElements[index] == filenameElements[index])
				{
					matchIndex = index;
				}
				else
				{
					if(matchIndex > 1)
					{
						//	Some portion of the filename is in common.
						if(matchIndex + 1 < count)
						{
							//	Additional levels exist past the current matching level.
							prefix.Append(
								Repeat("..\\", baseMatches.Count - matchIndex - 1));
						}
					}
					break;
				}
			}
			if(matchIndex > 1)
			{
				//	Complete the filename from the base.
				for(matchIndex++; matchIndex < filenameElements.Count; matchIndex++)
				{
					if(builder.Length > 0)
					{
						builder.Append('\\');
					}
					match = filenameMatches[matchIndex];
					builder.Append(filename.Substring(match.Index, match.Length));
				}
				if(prefix.Length > 0)
				{
					builder.Insert(0, prefix);
				}
				result = builder.ToString();
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetStringIndex																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the absolute index of the character found at the specified line
		/// and column of the caller's source string.
		/// </summary>
		/// <param name="source">
		/// String to search.
		/// </param>
		/// <param name="lineIndex">
		/// Line index to locate.
		/// </param>
		/// <param name="colIndex">
		/// Column index to locate on the specified line.
		/// </param>
		/// <returns>
		/// Absolute index of the specified line and column within the caller's
		/// source string.
		/// </returns>
		public static int GetStringIndex(string source,
			int lineIndex, int colIndex)
		{
			MatchCollection matches = null;
			int result = 0;

			if(source?.Length > 0 && lineIndex > -1 && colIndex > -1)
			{
				matches = Regex.Matches(source, ResourceMain.rxLine);
				if(lineIndex < matches.Count)
				{
					//	A line is known.
					result = matches[lineIndex].Index + colIndex;
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetValue																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the value of the specified group member in the provided match.
		/// </summary>
		/// <param name="match">
		/// Reference to the match to be inspected.
		/// </param>
		/// <param name="groupName">
		/// Name of the group for which the value will be found.
		/// </param>
		/// <returns>
		/// The value found in the specified group, if found. Otherwise, empty
		/// string.
		/// </returns>
		public static string GetValue(Match match, string groupName)
		{
			string result = "";

			if(match != null && match.Groups[groupName] != null &&
				match.Groups[groupName].Value != null)
			{
				result = match.Groups[groupName].Value;
			}
			return result;
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Return the value of the specified group member in a match found with
		/// the provided source and pattern.
		/// </summary>
		/// <param name="source">
		/// Source string to search.
		/// </param>
		/// <param name="pattern">
		/// Regular expression pattern to apply.
		/// </param>
		/// <param name="groupName">
		/// Name of the group for which the value will be found.
		/// </param>
		/// <returns>
		/// The value found in the specified group, if found. Otherwise, empty
		/// string.
		/// </returns>
		public static string GetValue(string source, string pattern,
			string groupName)
		{
			Match match = null;
			string result = "";

			if(source?.Length > 0 && pattern?.Length > 0 && groupName?.Length > 0)
			{
				match = Regex.Match(source, pattern);
				if(match.Success)
				{
					result = GetValue(match, groupName);
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* IsHeading																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the provided line is a Markdown
		/// heading.
		/// </summary>
		/// <param name="line">
		/// The line string to inspect.
		/// </param>
		/// <returns>
		/// True if the string is a line. Otherwise, false.
		/// </returns>
		public static bool IsHeading(string line)
		{
			bool result = false;

			if(line?.Length > 0)
			{
				result = (Regex.IsMatch(line, ResourceMain.rxMarkdownHeading));
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* IsHorizontalLine																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether this line is a horizontal line.
		/// </summary>
		/// <param name="line">
		/// The Markdown text to inspect.
		/// </param>
		/// <returns>
		/// True if the caller's line is a horizontal line. Otherwise, false.
		/// </returns>
		public static bool IsHorizontalLine(string line)
		{
			bool result = false;

			if(line?.Length > 0)
			{
				result = Regex.IsMatch(line, ResourceMain.rxMarkdownLinePart);
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* IsSectionEnd																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the current line is an ending
		/// section element.
		/// </summary>
		/// <param name="line">
		/// Markdown line to inspect.
		/// </param>
		/// <returns>
		/// True if the caller's line is a section ending element. Otherwise,
		/// false.
		/// </returns>
		public static bool IsSectionEnd(string line)
		{
			bool result = false;

			if(line?.Length > 0)
			{
				result = Regex.IsMatch(line, ResourceMain.rxSectionEnd);
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* IsSectionStart																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the current line is a starting
		/// section element.
		/// </summary>
		/// <param name="line">
		/// Markdown line to inspect.
		/// </param>
		/// <returns>
		/// True if the caller's line is a section starting element. Otherwise,
		/// false.
		/// </returns>
		public static bool IsSectionStart(string line)
		{
			bool result = false;

			if(line?.Length > 0)
			{
				result = Regex.IsMatch(line, ResourceMain.rxSectionStart);
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* OpenHtmlDialog																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Present the file open dialog for HTML files and return the result to
		/// the caller.
		/// </summary>
		/// <param name="dialogTitle">
		/// The title to display on the open file dialog.
		/// </param>
		/// <param name="defaultFilename">
		/// The optional default filename to select.
		/// </param>
		/// <param name="mustExist">
		/// Value indicating whether the selected file must already exist.
		/// </param>
		/// <returns>
		/// Full path and filename of the parameters file to open, if a file was
		/// selected. Otherwise, an empty string.
		/// </returns>
		public static string OpenHtmlDialog(string dialogTitle = "",
			string defaultFilename = "", bool mustExist = true)
		{
			FileAttributes attr;
			OpenFileDialog dialog = new OpenFileDialog();
			string directoryName = "";
			string filename = "";
			string result = "";

			dialog.AddExtension = true;
			dialog.AutoUpgradeEnabled = true;
			dialog.CheckFileExists = mustExist;
			dialog.DefaultExt = ".docx";
			dialog.DereferenceLinks = true;
			dialog.Filter =
				"HTML Files " +
				"(*.htm,*.html)|" +
				"*.htm,*.html;|" +
				"All Files (*.*)|*.*";
			dialog.FilterIndex = 0;
			dialog.Multiselect = false;
			dialog.SupportMultiDottedExtensions = true;
			dialog.Title = (dialogTitle?.Length > 0 ?
				dialogTitle : "Open HTML File");
			dialog.ValidateNames = true;
			if(defaultFilename?.Length > 0 &&
				Path.Exists(defaultFilename))
			{
				//	The show help version of this dialog allows the proper display
				//	of a pre-existing filename.
				dialog.ShowHelp = true;
				attr = File.GetAttributes(defaultFilename);
				if(attr.HasFlag(FileAttributes.Directory))
				{
					//	The value was a directory.
					dialog.InitialDirectory = defaultFilename;
				}
				else
				{
					//	The value was a filename.
					directoryName = Path.GetDirectoryName(defaultFilename);
					filename = Path.GetFileName(defaultFilename);
					dialog.InitialDirectory = directoryName;
					dialog.FileName = filename;
				}
			}
			if(dialog.ShowDialog() == DialogResult.OK)
			{
				result = dialog.FileName;
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* OpenMappedNamesDialog																									*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Present the file open dialog for mapped names files and return the
		/// result to the caller.
		/// </summary>
		/// <param name="dialogTitle">
		/// The title to display on the open file dialog.
		/// </param>
		/// <returns>
		/// Full path and filename of the parameters file to open, if a file was
		/// selected. Otherwise, an empty string.
		/// </returns>
		public static string OpenMappedNamesDialog(string dialogTitle = "")
		{
			OpenFileDialog dialog = new OpenFileDialog();
			string result = "";

			dialog.AddExtension = true;
			dialog.AutoUpgradeEnabled = true;
			dialog.CheckFileExists = true;
			dialog.DefaultExt = ".mappednames.json";
			dialog.DereferenceLinks = true;
			dialog.Filter =
				"Mapped-Name Definition Files " +
				"(*.mappednames.json)|" +
				"*.mappednames.json;|" +
				"JSON Files " +
				"(*.json)|" +
				"*.json;|" +
				"All Files (*.*)|*.*";
			dialog.FilterIndex = 0;
			dialog.Multiselect = false;
			dialog.SupportMultiDottedExtensions = true;
			dialog.Title = (dialogTitle?.Length > 0 ?
				dialogTitle : "Open Mapped-Names File");
			dialog.ValidateNames = true;
			if(dialog.ShowDialog() == DialogResult.OK)
			{
				result = dialog.FileName;
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* OpenMarkdownDialog																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Present the file open dialog for Markdown files and return the result
		/// to the caller.
		/// </summary>
		/// <param name="dialogTitle">
		/// The title to display on the open file dialog.
		/// </param>
		/// <param name="defaultFilename">
		/// The optional default filename to select.
		/// </param>
		/// <param name="mustExist">
		/// Value indicating whether the selected file must already exist.
		/// </param>
		/// <returns>
		/// Full path and filename of the Markdown file to open, if a file was
		/// selected. Otherwise, an empty string.
		/// </returns>
		public static string OpenMarkdownDialog(string dialogTitle = "",
			string defaultFilename = "", bool mustExist = true)
		{
			FileAttributes attr;
			OpenFileDialog dialog = new OpenFileDialog();
			string directoryName = "";
			string filename = "";
			string result = "";

			dialog.AddExtension = true;
			dialog.AutoUpgradeEnabled = true;
			dialog.CheckFileExists = mustExist;
			dialog.DefaultExt = ".md";
			dialog.DereferenceLinks = true;
			dialog.Filter =
				"Markdown Files " +
				"(*.md)|" +
				"*.md;|" +
				"Text Files " +
				"(*.txt)|" +
				"*.txt;|" +
				"All Files (*.*)|*.*";
			dialog.FilterIndex = 0;
			dialog.Multiselect = false;
			dialog.SupportMultiDottedExtensions = true;
			dialog.Title = (dialogTitle?.Length > 0 ?
				dialogTitle : "Open Markdown File");
			dialog.ValidateNames = true;
			if(defaultFilename?.Length > 0 &&
				Path.Exists(defaultFilename))
			{
				//	The show help version of this dialog allows the proper display
				//	of a pre-existing filename.
				dialog.ShowHelp = true;
				attr = File.GetAttributes(defaultFilename);
				if(attr.HasFlag(FileAttributes.Directory))
				{
					//	The value was a directory.
					dialog.InitialDirectory = defaultFilename;
				}
				else
				{
					//	The value was a filename.
					directoryName = Path.GetDirectoryName(defaultFilename);
					filename = Path.GetFileName(defaultFilename);
					dialog.InitialDirectory = directoryName;
					dialog.FileName = filename;
				}
			}
			if(dialog.ShowDialog() == DialogResult.OK)
			{
				result = dialog.FileName;
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* OpenMarkdownProjectDialog																							*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Present the file open dialog for Markdown project files and return the
		/// result to the caller.
		/// </summary>
		/// <param name="dialogTitle">
		/// The title to display on the open file dialog.
		/// </param>
		/// <returns>
		/// Full path and filename of the Markdown project file to open, if a file
		/// was selected. Otherwise, an empty string.
		/// </returns>
		public static string OpenMarkdownProjectDialog(string dialogTitle = "")
		{
			OpenFileDialog dialog = new OpenFileDialog();
			string result = "";

			dialog.AddExtension = true;
			dialog.AutoUpgradeEnabled = true;
			dialog.CheckFileExists = true;
			dialog.DefaultExt = ".mdproj";
			dialog.DereferenceLinks = true;
			dialog.Filter =
				"Markdown Project Files " +
				"(*.mdproj)|" +
				"*.mdproj;|" +
				"Old Markdown Project Files " +
				"(*.mdproj.json)|" +
				"*.mdproj.json;|" +
				"JSON Files " +
				"(*.json)|" +
				"*.json;|" +
				"All Files (*.*)|*.*";
			dialog.FilterIndex = 0;
			dialog.Multiselect = false;
			dialog.SupportMultiDottedExtensions = true;
			dialog.Title = (dialogTitle?.Length > 0 ?
				dialogTitle : "Open Markdown Project File");
			dialog.ValidateNames = true;
			if(dialog.ShowDialog() == DialogResult.OK)
			{
				result = dialog.FileName;
				if(dialog.FileName.EndsWith(".mdproj.json"))
				{
					MessageBox.Show(
						"Markdown Editor can now open files with the .mdproj " +
						"extension when double-clicking on them from Windows File " +
						"Explorer.\r\n\r\n" +
						"Your project has the older extension of .mdproj.json.\r\n\r\n" +
						"Please save your project as the new markdown project type " +
						"(*.mdproj) to be able to open it from Windows Explorer.");
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* OpenMarkdownProjectsDialog																						*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Present the file open dialog for multiple Markdown project files and
		/// return the result to the caller.
		/// </summary>
		/// <param name="dialogTitle">
		/// The title to display on the open file dialog.
		/// </param>
		/// <param name="defaultExtension">
		/// The default extension to open.
		/// </param>
		/// <returns>
		/// Full path and filename of the Markdown project file to open, if a file
		/// was selected. Otherwise, an empty string.
		/// </returns>
		public static string[] OpenMarkdownProjectsDialog(string dialogTitle = "",
			string defaultExtension = ".mdproj")
		{
			string ext =
				(defaultExtension?.Length > 0 ? defaultExtension : ".mdproj");
			OpenFileDialog dialog = new OpenFileDialog();
			string[] result = new string[0];

			dialog.AddExtension = true;
			dialog.AutoUpgradeEnabled = true;
			dialog.CheckFileExists = true;
			dialog.DefaultExt = ext;
			dialog.DereferenceLinks = true;
			dialog.Filter =
				"Markdown Project Files " +
				"(*.mdproj)|" +
				"*.mdproj;|" +
				"Old Markdown Project Files " +
				"(*.mdproj.json)|" +
				"*.mdproj.json;|" +
				"JSON Files " +
				"(*.json)|" +
				"*.json;|" +
				"All Files (*.*)|*.*";
			dialog.FilterIndex = (ext.ToLower() == ".mdproj.json" ? 2 : 0);
			dialog.Multiselect = true;
			dialog.SupportMultiDottedExtensions = true;
			dialog.Title = (dialogTitle?.Length > 0 ?
				dialogTitle : "Open Markdown Project File");
			dialog.ValidateNames = true;
			if(dialog.ShowDialog() == DialogResult.OK)
			{
				result = dialog.FileNames;
				//if(dialog.FileName.EndsWith(".mdproj.json"))
				//{
				//	MessageBox.Show(
				//		"Markdown Editor can now open files with the .mdproj " +
				//		"extension when double-clicking on them from Windows File " +
				//		"Explorer.\r\n\r\n" +
				//		"Your project has the older extension of .mdproj.json.\r\n\r\n" +
				//		"Please save your project as the new markdown project type " +
				//		"(*.mdproj) to be able to open it from Windows Explorer.");
				//}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* OpenStylesDialog																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Present the file open dialog for style files and return the result
		/// to the caller.
		/// </summary>
		/// <param name="dialogTitle">
		/// The title to display on the open file dialog.
		/// </param>
		/// <returns>
		/// Full path and filename of the parameters file to open, if a file was
		/// selected. Otherwise, an empty string.
		/// </returns>
		public static string OpenStylesDialog(string dialogTitle = "")
		{
			OpenFileDialog dialog = new OpenFileDialog();
			string result = "";

			dialog.AddExtension = true;
			dialog.AutoUpgradeEnabled = true;
			dialog.CheckFileExists = true;
			dialog.DefaultExt = ".styles.json";
			dialog.DereferenceLinks = true;
			dialog.Filter =
				"Style Definition Files " +
				"(*.styles.json)|" +
				"*.styles.json;|" +
				"JSON Files " +
				"(*.json)|" +
				"*.json;|" +
				"All Files (*.*)|*.*";
			dialog.FilterIndex = 0;
			dialog.Multiselect = false;
			dialog.SupportMultiDottedExtensions = true;
			dialog.Title = (dialogTitle?.Length > 0 ?
				dialogTitle : "Open Document Styles File");
			dialog.ValidateNames = true;
			if(dialog.ShowDialog() == DialogResult.OK)
			{
				result = dialog.FileName;
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* OpenVariablesDialog																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Present the file open dialog for parameter files and return the result
		/// to the caller.
		/// </summary>
		/// <param name="dialogTitle">
		/// The title to display on the open file dialog.
		/// </param>
		/// <returns>
		/// Full path and filename of the parameters file to open, if a file was
		/// selected. Otherwise, an empty string.
		/// </returns>
		public static string OpenVariablesDialog(string dialogTitle = "")
		{
			OpenFileDialog dialog = new OpenFileDialog();
			string result = "";

			dialog.AddExtension = true;
			dialog.AutoUpgradeEnabled = true;
			dialog.CheckFileExists = true;
			dialog.DefaultExt = ".vars.json";
			dialog.DereferenceLinks = true;
			dialog.Filter =
				"Variable Definition Files " +
				"(*.vars.json)|" +
				"*.vars.json;|" +
				"JSON Files " +
				"(*.json)|" +
				"*.json;|" +
				"All Files (*.*)|*.*";
			dialog.FilterIndex = 0;
			dialog.Multiselect = false;
			dialog.SupportMultiDottedExtensions = true;
			dialog.Title = (dialogTitle?.Length > 0 ?
				dialogTitle : "Open Parameter Variables File");
			dialog.ValidateNames = true;
			if(dialog.ShowDialog() == DialogResult.OK)
			{
				result = dialog.FileName;
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* OpenWordDialog																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Present the file open dialog for Microsoft Word files and return the
		/// result to the caller.
		/// </summary>
		/// <param name="dialogTitle">
		/// The title to display on the open file dialog.
		/// </param>
		/// <param name="defaultFilename">
		/// The optional name of the default file to select.
		/// </param>
		/// <param name="mustExist">
		/// Value indicating whether the selected file must already exist.
		/// </param>
		/// <returns>
		/// Full path and filename of the parameters file to open, if a file was
		/// selected. Otherwise, an empty string.
		/// </returns>
		public static string OpenWordDialog(string dialogTitle = "",
			string defaultFilename = "", bool mustExist = true)
		{
			FileAttributes attr;
			OpenFileDialog dialog = new OpenFileDialog();
			string directoryName = "";
			string filename = "";
			string result = "";

			dialog.AddExtension = true;
			dialog.AutoUpgradeEnabled = true;
			dialog.CheckFileExists = mustExist;
			dialog.DefaultExt = ".docx";
			dialog.DereferenceLinks = true;
			dialog.Filter =
				"Microsoft Word Files " +
				"(*.docx)|" +
				"*.docx;|" +
				"All Files (*.*)|*.*";
			dialog.FilterIndex = 0;
			dialog.Multiselect = false;
			dialog.SupportMultiDottedExtensions = true;
			dialog.Title = (dialogTitle?.Length > 0 ?
				dialogTitle : "Open Word File");
			dialog.ValidateNames = true;
			if(defaultFilename?.Length > 0 &&
				Path.Exists(defaultFilename))
			{
				//	The show help version of this dialog allows the proper display
				//	of a pre-existing filename.
				dialog.ShowHelp = true;
				attr = File.GetAttributes(defaultFilename);
				if(attr.HasFlag(FileAttributes.Directory))
				{
					//	The value was a directory.
					dialog.InitialDirectory = defaultFilename;
				}
				else
				{
					//	The value was a filename.
					directoryName = Path.GetDirectoryName(defaultFilename);
					filename = Path.GetFileName(defaultFilename);
					dialog.InitialDirectory = directoryName;
					dialog.FileName = filename;
				}
			}
			if(dialog.ShowDialog() == DialogResult.OK)
			{
				result = dialog.FileName;
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ProcessWhitespace																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Process remaining whitespace in the supplied nodes collection.
		/// </summary>
		/// <param name="nodes">
		/// Reference to the collection of nodes to inspect.
		/// </param>
		/// <remarks>
		///	In this version, line feeds and carriage returns are removed from
		///	the natural order. Wherever they appear within text, they are
		///	replaced with a single space. Whenever they appear alone, they are
		///	removed. If no child nodes are present on a node that has been
		///	blanked, that node is removed from the collection.
		/// </remarks>
		public static void ProcessWhitespace(HtmlNodeCollection nodes)
		{
			int count = 0;
			int index = 0;
			HtmlNodeItem node = null;

			if(nodes?.Count > 0)
			{
				count = nodes.Count;
				for(index = 0; index < count; index++)
				{
					node = nodes[index];
					if(node.NodeType == "tbody")
					{
						Trace.WriteLine("ProcessWhitespace: Break here...");
					}
					if(node.NodeType == "br")
					{
						node.Text = node.Text.TrimStart();
					}
					if(node.Nodes.Count > 0)
					{
						ProcessWhitespace(node.Nodes);
					}
					else if(node.NodeType == "" && node.Text.Length == 0)
					{
						//	This item has no text and no child nodes.
						nodes.RemoveAt(index);
						index--;  //	Delist.
						count--;  //	Discount.
					}
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* QuestionDialog																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Ask the user a question and return the response.
		/// </summary>
		/// <param name="prompt">
		/// The prompt message to display to the user.
		/// </param>
		/// <param name="caption">
		/// The title of the dialog.
		/// </param>
		/// <param name="buttons">
		/// Message box button set to display.
		/// </param>
		/// <returns>
		/// Dialog result corresponding to the user's response.
		/// </returns>
		public static DialogResult QuestionDialog(string prompt,
			string caption = "",
			MessageBoxButtons buttons = MessageBoxButtons.YesNo)
		{
			DialogResult result = DialogResult.None;
			string titleText = "Markdown Editor";

			if(prompt?.Length > 0)
			{
				if(caption?.Length > 0)
				{
					titleText = caption;
				}
				//using(new CenterWinDialog(this))
				//{
				result = MessageBox.Show(prompt, caption, buttons);
				//}

			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* Repeat																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Repeat the specified pattern.
		/// </summary>
		/// <param name="pattern">
		/// Pattern to repeat.
		/// </param>
		/// <param name="count">
		/// Count of times to repeat the pattern.
		/// </param>
		/// <returns>
		/// Resulting combination of repeated strings.
		/// </returns>
		public static string Repeat(string pattern, int count)
		{
			StringBuilder builder = new StringBuilder();
			int index = 0;

			for(index = 0; index < count; index++)
			{
				builder.Append(pattern);
			}
			return builder.ToString();
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ReplaceLineBreaks																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Replace all of the natural line breaks in the caller's text with
		/// corresponding HTML line breaks.
		/// </summary>
		/// <param name="source">
		/// The source string to evaluate.
		/// </param>
		/// <returns>
		/// A version of the caller's string where all CRLF and LF line breaks
		/// have been replaced with HTML p and br elements.
		/// </returns>
		public static string ReplaceLineBreaks(string source)
		{
			string result = "";

			if(source?.Length > 0)
			{
				result = Regex.Replace(source,
					@"(?s:(\r{0,1}\n){2,})", "<p>&nbsp;</p>");
				result = Regex.Replace(result,
					@"(?s:(\r{0,1}\n))", "<br>");
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		////*-----------------------------------------------------------------------*
		////* ResolveVariables																											*
		////*-----------------------------------------------------------------------*
		///// <summary>
		///// Return the caller's string with all of its user-defined interpolated
		///// string variables resolved.
		///// </summary>
		///// <param name="source">
		///// The source string to parse for variables.
		///// </param>
		///// <returns>
		///// The caller's string with interpolated strings resolved and replaced.
		///// </returns>
		//public string ResolveVariables(string source)
		//{
		//	string content = "";
		//	MatchCollection matches = null;
		//	ProjectPackageItem parameters = null;
		//	FindReplaceCollection replacements = new FindReplaceCollection();
		//	DialogResult response = DialogResult.None;
		//	string result = "";
		//	string settingsFilename = "";

		//	if(source?.Length > 0)
		//	{
		//		if(Regex.IsMatch(source, ResourceMain.rxInterpolatedExpression))
		//		{
		//			response = QuestionDialog("Your code contains variable parameter " +
		//				"names. Do you wish to load a parameter variables file to " +
		//				"resolve them while exporting?",
		//				"Export to Docx", MessageBoxButtons.YesNo);
		//			if(response == DialogResult.Yes)
		//			{
		//				settingsFilename = OpenVariablesDialog();
		//				if(settingsFilename.Length > 0)
		//				{
		//					content = File.ReadAllText(settingsFilename);
		//					parameters =
		//						JsonConvert.DeserializeObject<ProjectPackageItem>(content);
		//					//	Resolve the internal values.
		//					ProjectPackageItem.ResolveVariables(parameters);
		//					matches = Regex.Matches(source,
		//						ResourceMain.rxInterpolatedExpression);
		//					foreach(Match matchItem in matches)
		//					{
		//						replacements.Add(new FindReplaceItem()
		//						{
		//							Find = matchItem.Value,
		//							Replace = ProjectPackageItem.ResolveVariable(
		//								parameters, matchItem.Value)
		//						});
		//					}
		//				}
		//			}
		//			else
		//			{
		//				parameters = new ProjectPackageItem();
		//				matches = Regex.Matches(source,
		//					ResourceMain.rxInterpolatedExpression);
		//				foreach(Match matchItem in matches)
		//				{
		//					replacements.Add(new FindReplaceItem()
		//					{
		//						Find = matchItem.Value,
		//						Replace = ProjectPackageItem.ResolveVariable(
		//							parameters, matchItem.Value)
		//					});
		//				}
		//			}
		//			result = source;
		//			foreach(FindReplaceItem replacementItem in replacements)
		//			{
		//				result = result.Replace(
		//					replacementItem.Find, replacementItem.Replace);
		//			}
		//		}
		//		else
		//		{
		//			result = source;
		//		}
		//	}
		//	return result;
		//}
		////*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* SaveMarkdownDialog																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Display a Save Markdown file dialog and return the name of the selected
		/// file.
		/// </summary>
		/// <param name="dialogTitle">
		/// Optional title to display on the save file dialog.
		/// </param>
		/// <param name="defaultFilename">
		/// Optional default filename to select.
		/// </param>
		/// <returns>
		/// The full path and filename to save, if selected. Otherwise, and empty
		/// string.
		/// </returns>
		public static string SaveMarkdownDialog(string dialogTitle = "",
			string defaultFilename = "")
		{
			SaveFileDialog dialog = new SaveFileDialog();
			FileInfo file = null;
			string result = "";

			dialog.AddExtension = true;
			dialog.AutoUpgradeEnabled = true;
			dialog.CheckFileExists = false;
			dialog.CheckPathExists = true;
			dialog.CreatePrompt = false;
			dialog.DefaultExt = ".md";
			dialog.DereferenceLinks = true;
			if(defaultFilename?.Length > 0)
			{
				file = new FileInfo(defaultFilename);
				dialog.InitialDirectory = file.Directory.FullName;
				dialog.FileName = file.Name;
			}
			dialog.Filter =
				"Markdown Files " +
				"(*.md)|" +
				"*.md;|" +
				"Text Files " +
				"(*.txt)|" +
				"*.txt;|" +
				"All Files (*.*)|*.*";
			dialog.FilterIndex = 0;
			dialog.OverwritePrompt = true;
			dialog.SupportMultiDottedExtensions = true;
			dialog.Title = (dialogTitle?.Length > 0 ?
				dialogTitle : "Save Markdown File As");
			dialog.ValidateNames = true;
			if(dialog.ShowDialog() == DialogResult.OK)
			{
				result = dialog.FileName;
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* SaveMarkdownProjectDialog																							*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Display a Save Markdown Project file dialog and return the name of the
		/// selected file.
		/// </summary>
		/// <param name="dialogTitle">
		/// Optional title to display on the save file dialog.
		/// </param>
		/// <param name="defaultFilename">
		/// Optional default filename to select.
		/// </param>
		/// <returns>
		/// The full path and filename to save, if selected. Otherwise, and empty
		/// string.
		/// </returns>
		public static string SaveMarkdownProjectDialog(string dialogTitle = "",
			string defaultFilename = "")
		{
			SaveFileDialog dialog = new SaveFileDialog();
			FileInfo file = null;
			string result = "";

			dialog.AddExtension = true;
			dialog.AutoUpgradeEnabled = true;
			dialog.CheckFileExists = false;
			dialog.CheckPathExists = true;
			dialog.CreatePrompt = false;
			dialog.DefaultExt = ".mdproj";
			dialog.DereferenceLinks = true;
			if(defaultFilename?.Length > 0)
			{
				file = new FileInfo(defaultFilename);
				dialog.InitialDirectory = file.Directory.FullName;
				dialog.FileName = file.Name;
			}
			dialog.Filter =
				"Markdown Project Files " +
				"(*.mdproj)|" +
				"*.mdproj;|" +
				"Old Markdown Project Files " +
				"(*.mdproj.json)|" +
				"*.mdproj.json;|" +
				"JSON Files " +
				"(*.json)|" +
				"*.json;|" +
				"All Files (*.*)|*.*";
			dialog.FilterIndex = 0;
			dialog.OverwritePrompt = true;
			dialog.SupportMultiDottedExtensions = true;
			dialog.Title = (dialogTitle?.Length > 0 ?
				dialogTitle : "Save Markdown Project File As");
			dialog.ValidateNames = true;
			if(dialog.ShowDialog() == DialogResult.OK)
			{
				result = dialog.FileName;
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* SaveStylesDialog																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Display a Save Styles file dialog and return the name of the selected
		/// file.
		/// </summary>
		/// <param name="dialogTitle">
		/// Optional title to display on the save file dialog.
		/// </param>
		/// <param name="defaultFilename">
		/// Optional default filename to select.
		/// </param>
		/// <returns>
		/// The full path and filename to save, if selected. Otherwise, and empty
		/// string.
		/// </returns>
		public static string SaveStylesDialog(string dialogTitle = "",
			string defaultFilename = "")
		{
			SaveFileDialog dialog = new SaveFileDialog();
			FileInfo file = null;
			string result = "";

			dialog.AddExtension = true;
			dialog.AutoUpgradeEnabled = true;
			dialog.CheckFileExists = false;
			dialog.CheckPathExists = true;
			dialog.CreatePrompt = false;
			dialog.DefaultExt = ".styles.json";
			dialog.DereferenceLinks = true;
			if(defaultFilename?.Length > 0)
			{
				file = new FileInfo(defaultFilename);
				dialog.InitialDirectory = file.Directory.FullName;
				dialog.FileName = file.Name;
			}
			dialog.Filter =
				"Style Definition Files " +
				"(*.styles.json)|" +
				"*.styles.json;|" +
				"JSON Files " +
				"(*.json)|" +
				"*.json;|" +
				"Text Files " +
				"(*.txt)|" +
				"*.txt;|" +
				"All Files (*.*)|*.*";
			dialog.FilterIndex = 0;
			dialog.OverwritePrompt = true;
			dialog.SupportMultiDottedExtensions = true;
			dialog.Title = (dialogTitle?.Length > 0 ?
				dialogTitle : "Save Style File As");
			dialog.ValidateNames = true;
			if(dialog.ShowDialog() == DialogResult.OK)
			{
				result = dialog.FileName;
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* SaveVariablesDialog																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Display a Save Variables file dialog and return the name of the
		/// selected file.
		/// </summary>
		/// <param name="dialogTitle">
		/// Optional title to display on the save file dialog.
		/// </param>
		/// <param name="defaultFilename">
		/// Optional default filename to select.
		/// </param>
		/// <returns>
		/// The full path and filename to save, if selected. Otherwise, and empty
		/// string.
		/// </returns>
		public static string SaveVariablesDialog(string dialogTitle = "",
			string defaultFilename = "")
		{
			SaveFileDialog dialog = new SaveFileDialog();
			FileInfo file = null;
			string result = "";

			dialog.AddExtension = true;
			dialog.AutoUpgradeEnabled = true;
			dialog.CheckFileExists = false;
			dialog.CheckPathExists = true;
			dialog.CreatePrompt = false;
			dialog.DefaultExt = ".vars.json";
			dialog.DereferenceLinks = true;
			if(defaultFilename?.Length > 0)
			{
				file = new FileInfo(defaultFilename);
				dialog.InitialDirectory = file.Directory.FullName;
				dialog.FileName = file.Name;
			}
			dialog.Filter =
				"Variable Definition Files " +
				"(*.vars.json)|" +
				"*.vars.json;|" +
				"JSON Files " +
				"(*.json)|" +
				"*.json;|" +
				"Text Files " +
				"(*.txt)|" +
				"*.txt;|" +
				"All Files (*.*)|*.*";
			dialog.FilterIndex = 0;
			dialog.OverwritePrompt = true;
			dialog.SupportMultiDottedExtensions = true;
			dialog.Title = (dialogTitle?.Length > 0 ?
				dialogTitle : "Save Variable File As");
			dialog.ValidateNames = true;
			if(dialog.ShowDialog() == DialogResult.OK)
			{
				result = dialog.FileName;
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* SaveWordDialog																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Display a Save Word file dialog and return the name of the selected
		/// file.
		/// </summary>
		/// <param name="dialogTitle">
		/// Optional title to display on the save file dialog.
		/// </param>
		/// <param name="defaultFilename">
		/// Optional default filename to select.
		/// </param>
		/// <returns>
		/// The full path and filename to save, if selected. Otherwise, and empty
		/// string.
		/// </returns>
		public static string SaveWordDialog(string dialogTitle = "",
			string defaultFilename = "")
		{
			SaveFileDialog dialog = new SaveFileDialog();
			FileInfo file = null;
			string result = "";

			dialog.AddExtension = true;
			dialog.AutoUpgradeEnabled = true;
			dialog.CheckFileExists = false;
			dialog.CheckPathExists = true;
			dialog.CreatePrompt = false;
			dialog.DefaultExt = ".docx";
			dialog.DereferenceLinks = true;
			if(defaultFilename?.Length > 0)
			{
				file = new FileInfo(defaultFilename);
				dialog.InitialDirectory = file.Directory.FullName;
				dialog.FileName = file.Name;
			}
			dialog.Filter =
				"Word Files " +
				"(*.docx)|" +
				"*.docx;|" +
				"All Files (*.*)|*.*";
			dialog.FilterIndex = 0;
			dialog.OverwritePrompt = true;
			dialog.SupportMultiDottedExtensions = true;
			dialog.Title = (dialogTitle?.Length > 0 ?
				dialogTitle : "Save Word File As");
			dialog.ValidateNames = true;
			if(dialog.ShowDialog() == DialogResult.OK)
			{
				result = dialog.FileName;
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ToFloat																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Provide fail-safe conversion of string to numeric value.
		/// </summary>
		/// <param name="value">
		/// Value to convert.
		/// </param>
		/// <returns>
		/// Floating point value. 0 if not convertible.
		/// </returns>
		public static float ToFloat(object value)
		{
			float result = 0f;
			if(value != null)
			{
				result = ToFloat(value.ToString());
			}
			return result;
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Provide fail-safe conversion of string to numeric value.
		/// </summary>
		/// <param name="value">
		/// Value to convert.
		/// </param>
		/// <returns>
		/// Floating point value. 0 if not convertible.
		/// </returns>
		public static float ToFloat(string value)
		{
			float result = 0f;
			try
			{
				result = float.Parse(value);
			}
			catch { }
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ToInt																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Provide fail-safe conversion of string to numeric value.
		/// </summary>
		/// <param name="value">
		/// Value to convert.
		/// </param>
		/// <returns>
		/// Int32 value. 0 if not convertible.
		/// </returns>
		public static int ToInt(object value)
		{
			int result = 0;
			if(value != null)
			{
				result = ToInt(value.ToString());
			}
			return result;
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Provide fail-safe conversion of string to numeric value.
		/// </summary>
		/// <param name="value">
		/// Value to convert.
		/// </param>
		/// <returns>
		/// Int32 value. 0 if not convertible.
		/// </returns>
		public static int ToInt(string value)
		{
			int result = 0;
			try
			{
				result = int.Parse(value);
			}
			catch { }
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ToMultiLineString																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the multi-line single string representation of the items found
		/// in the caller's list, where lines that end with a space are continued
		/// without a line-break and all other lines are ended with a line break.
		/// </summary>
		/// <param name="list">
		/// Reference to the list of string values to be converted to a single
		/// string.
		/// </param>
		/// <returns>
		/// A single multi-line representation of the caller's string list.
		/// </returns>
		public static string ToMultiLineString(List<string> list)
		{
			StringBuilder builder = new StringBuilder();

			if(list?.Count > 0)
			{
				foreach(string listItem in list)
				{
					if(listItem.EndsWith(' '))
					{
						builder.Append(listItem);
					}
					else
					{
						builder.AppendLine(listItem);
					}
				}
			}
			return builder.ToString();
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

}
