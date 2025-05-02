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

using Html;
using Markdig;
using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.NetworkInformation;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Xceed.Words.NET;

using static MarkdownEditor.MarkdownEditorUtil;

//	TODO: Adding project, import, and export features.
//	TODO: HTML Library must be open-sourced.

namespace MarkdownEditor
{
	//*-------------------------------------------------------------------------*
	//*	frmMain																																	*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// The main form for the MarkdownEditor application.
	/// </summary>
	public partial class frmMain : Form
	{
		//*************************************************************************
		//*	Private																																*
		//*************************************************************************
		//	Windows.
		/// <summary>
		/// Sends the specified message to a window or windows.
		/// </summary>
		/// <param name="hWnd">
		/// A window handle.
		/// </param>
		/// <param name="wMsg">
		/// Message to send.
		/// </param>
		/// <param name="wParam">
		/// Additional message parameter information.
		/// </param>
		/// <param name="lParam">
		/// Additional message parameter information.
		/// </param>
		/// <returns>
		/// Message processing result.
		/// </returns>
		[DllImport("user32.dll")]
		public static extern int SendMessage(IntPtr hWnd, int wMsg,
			IntPtr wParam, IntPtr lParam);
		private const int WM_UPDATEUISTATE = 0x0128;
		private const int UISF_HIDEACCEL = 0x2;
		private const int UIS_CLEAR = 0x2;

		//	Local.
		private bool mActivated = false;
		//private bool mContentChanged = false;
		//private string mDocxFilename = "";
		private bool mDomReady = false;
		private MarkdownEditorStyleItem mEditorStyles =
			new MarkdownEditorStyleItem();
		//private string mFilename = "";
		private bool mInitialized = false;
		private ProjectPackageItem mProject = null;
		private string mProjectFilename = "";
		private bool mQueueTickBusy = false;
		private System.Windows.Forms.Timer mQueueTimer = null;
		private RunningStateEnum mRunningState = RunningStateEnum.Running;

		//*-----------------------------------------------------------------------*
		//* ApplyColumnMappingValues																							*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Apply the column mapping values to the caller's source string from the
		/// provided mapped columns collection.
		/// </summary>
		/// <param name="source">
		/// Source string containing potential mapped columns.
		/// </param>
		/// <param name="mappedColumns">
		/// Collection of mapped column assignments.
		/// </param>
		/// <returns>
		/// Converted string where mapped columns have all been substituted.
		/// </returns>
		/// <remarks>
		/// In this version, a mapped column can either be a markdown paragraph
		/// heading or a table header cell.
		/// </remarks>
		private static string ApplyColumnMappingValues(string source,
			VariableEntryCollection mappedColumns)
		{
			string pattern = "";
			string result = "";

			if(source?.Length > 0)
			{
				result = source;
				if(mappedColumns?.Count > 0)
				{
					foreach(VariableEntryItem mappedColItem in mappedColumns)
					{
						pattern = ResourceMain.rxColumnMapReplacement.Replace("{content}",
							mappedColItem.Name);
						result = Regex.Replace(result,
							pattern, "${head}" + mappedColItem.Value);
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ApplyDocumentStyles																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Apply entries in the document styles collection to the caller's source
		/// string using the provided style definition collection.
		/// </summary>
		/// <param name="source">
		/// The caller's source string to be updated.
		/// </param>
		/// <param name="userStyles">
		/// Reference to the collection of style definitions to apply.
		/// </param>
		/// <returns>
		/// The caller's source markdown where all of applicable document styles
		/// have been updated.
		/// </returns>
		private static string ApplyDocumentStyles(string source,
			VariableStyleCollection userStyles)
		{
			string result = "";
			List<VariableStyleItem> styles = new List<VariableStyleItem>();

			//	In this version, the HTML 4 <font> element is used with the
			//	attributes:
			//	- color
			//	- size
			//	- face

			if(source?.Length > 0)
			{
				//	H{1-9}FontColor. Color of the numbered paragraph heading font.
				//	H{1-9}FontName. Name of the numbered paragraph heading font.
				//	H{1-9}FontSize. Size of the numbered paragraph heading font.
				//	HeadingFontColor. Default color of every paragraph heading font.
				//	HeadingFontName. Default name of every paragraph heading font.
				//	HeadingFontSize. Default size of every paragraph heading font.
				//	SectionBackgroundColor. Background color of every section.
				//	SectionBackgroundColorEven. Back color of even-numbered sections.
				//	SectionBackgroundColorOdd. Back color of odd-numbered sections.
				//	TableHeaderFontColor. Color of the table header cell font.
				//	TableHeaderFontName. Name of the table header cell font.
				//	TableHeaderFontSize. Size of the table header cell font.
				result = source;
				if(userStyles?.Count > 0)
				{
					VariableStyleCollection.Clone(userStyles, styles);
					//	Lines are specific to Markdown. HTML elements are not bound to
					//	lines. Because of this difference, two separate passes will be
					//	made: one that addresses Markdown content and one that
					//	addresses HTML content.
					source = ApplyDocumentStylesHtml(source, styles);
					source = ApplyDocumentStylesMarkdown(source, styles);
					result = source;
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ApplyDocumentStylesHtml																								*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Apply the defined document styles to raw HTML elements.
		/// </summary>
		/// <param name="source">
		/// The string potentially containing raw HTML elements to style.
		/// </param>
		/// <param name="styles">
		/// Reference to a list of style items to be referenced when formatting the
		/// applicable elements.
		/// </param>
		/// <returns>
		/// The caller's string where all of the appropriate HTML elements have
		/// received applicable styling, if a source string was provided.
		/// Otherwise, an empty string.
		/// </returns>
		private static string ApplyDocumentStylesHtml(string source,
			List<VariableStyleItem> styles)
		{
			StringBuilder builder = new StringBuilder();
			string digit = "";
			string attributes = "";
			LinePropertyItem line = null;
			LinePropertyCollection lines = new LinePropertyCollection();
			Match match = null;
			MatchCollection matches = null;
			MatchCollection matches2 = null;
			PropertyItem prop = null;
			ReplacementInfoCollection replacements = new ReplacementInfoCollection();
			string result = "";
			List<VariableStyleItem> stylesCurrent = null;
			DocumentStyleTypeEnum styleType = DocumentStyleTypeEnum.None;


			if(source?.Length > 0 && styles != null)
			{
				result = source;

				if(styles.Exists(x => Regex.IsMatch(x.StyleName,
					ResourceMain.rxStyleHxType)))
				{
					//	Heading types are specified.
					lines.Clear();
					matches = Regex.Matches(result, ResourceMain.rxHtmlHeadingNoFont);
					foreach(Match matchItem in matches)
					{
						line = new LinePropertyItem()
						{
							Index = matchItem.Index,
							Line = matchItem.Value,
							LineType = TextLineTypeEnum.HtmlHeading
						};
						lines.Add(line);
						digit = GetValue(matchItem, "digit");
						foreach(VariableStyleItem styleItem in styles)
						{
							styleType =
								Enum.Parse<DocumentStyleTypeEnum>(styleItem.StyleName, true);
							switch(styleType)
							{
								case DocumentStyleTypeEnum.HeadingFontColor:
									//	Only set this color on styles that don't have an explicit
									//	override.
									if(!styles.Exists(x => Regex.IsMatch(x.StyleName,
										ResourceMain.rxStyleHxType.Replace(@"\d|eading", digit))))
									{
										//	The specific heading color has not been specified.
										line.SetProperty("FontColor", styleItem.StyleValue);
									}
									break;
								case DocumentStyleTypeEnum.HeadingFontName:
									//	Only set this font on styles that don't have an explicit
									//	override.
									if(!styles.Exists(x => Regex.IsMatch(x.StyleName,
										ResourceMain.rxStyleHxType.Replace(@"\d|eading", digit))))
									{
										//	The specific heading font has not been specified.
										line.SetProperty("FontName", styleItem.StyleValue);
									}
									break;
								case DocumentStyleTypeEnum.HeadingFontSize:
									//	Only set this size on styles that don't have an explicit
									//	override.
									if(!styles.Exists(x => Regex.IsMatch(x.StyleName,
										ResourceMain.rxStyleHxType.Replace(@"\d|eading", digit))))
									{
										//	The specific heading font has not been specified.
										line.SetProperty("FontSize", styleItem.StyleValue);
									}
									break;
								case DocumentStyleTypeEnum.H1FontColor:
								case DocumentStyleTypeEnum.H2FontColor:
								case DocumentStyleTypeEnum.H3FontColor:
								case DocumentStyleTypeEnum.H4FontColor:
								case DocumentStyleTypeEnum.H5FontColor:
								case DocumentStyleTypeEnum.H6FontColor:
								case DocumentStyleTypeEnum.H7FontColor:
								case DocumentStyleTypeEnum.H8FontColor:
								case DocumentStyleTypeEnum.H9FontColor:
									if(styleItem.StyleName.Substring(1, 1) == digit)
									{
										//	This specific heading is matched.
										line.SetProperty("FontColor", styleItem.StyleValue);
									}
									break;
								case DocumentStyleTypeEnum.H1FontName:
								case DocumentStyleTypeEnum.H2FontName:
								case DocumentStyleTypeEnum.H3FontName:
								case DocumentStyleTypeEnum.H4FontName:
								case DocumentStyleTypeEnum.H5FontName:
								case DocumentStyleTypeEnum.H6FontName:
								case DocumentStyleTypeEnum.H7FontName:
								case DocumentStyleTypeEnum.H8FontName:
								case DocumentStyleTypeEnum.H9FontName:
									if(styleItem.StyleName.Substring(1, 1) == digit)
									{
										//	This specific heading is matched.
										line.SetProperty("FontName", styleItem.StyleValue);
									}
									break;
								case DocumentStyleTypeEnum.H1FontSize:
								case DocumentStyleTypeEnum.H2FontSize:
								case DocumentStyleTypeEnum.H3FontSize:
								case DocumentStyleTypeEnum.H4FontSize:
								case DocumentStyleTypeEnum.H5FontSize:
								case DocumentStyleTypeEnum.H6FontSize:
								case DocumentStyleTypeEnum.H7FontSize:
								case DocumentStyleTypeEnum.H8FontSize:
								case DocumentStyleTypeEnum.H9FontSize:
									if(styleItem.StyleName.Substring(1, 1) == digit)
									{
										//	This specific heading is matched.
										line.SetProperty("FontSize", styleItem.StyleValue);
									}
									break;
							}
						}
					}
					//	Replace all of the matched instances.
					foreach(LinePropertyItem lineItem in lines)
					{
						if(lineItem.Properties.Exists(x =>
							x.Name == "FontColor" ||
							x.Name == "FontName" ||
							x.Name == "FontSize"))
						{
							Clear(builder);
							match = Regex.Match(lineItem.Line,
								ResourceMain.rxHtmlHeadingNoFont);
							digit = GetValue(match, "digit");
							attributes = GetValue(match, "attributes");
							builder.Append($"<h{digit}{attributes}>");
							builder.Append("<font");
							prop = lineItem.Properties.FirstOrDefault(x =>
								x.Name == "FontColor");
							if(prop != null)
							{
								builder.Append($" color=\"{prop.Value}\"");
							}
							prop = lineItem.Properties.FirstOrDefault(x =>
								x.Name == "FontName");
							if(prop != null)
							{
								builder.Append($" face=\"{prop.Value}\"");
							}
							prop = lineItem.Properties.FirstOrDefault(x =>
								x.Name == "FontSize");
							if(prop != null)
							{
								builder.Append($" style=\"font-size:{prop.Value}pt\"");
							}
							builder.Append('>');
							builder.Append(GetValue(match, "content"));
							builder.Append("</font>");
							builder.Append($"</h{digit}>");
							replacements.Add(new ReplacementInfoItem()
							{
								Index = lineItem.Index,
								Length = lineItem.Line.Length,
								Text = builder.ToString()
							});
						}
					}
					if(replacements.Count > 0)
					{
						result = ReplacementInfoCollection.Replace(result, replacements);
					}
				}
				stylesCurrent = styles.FindAll(x =>
					Regex.IsMatch(x.StyleName, ResourceMain.rxStyleTableHeaderType));
				if(stylesCurrent.Count > 0)
				{
					//	Table header types are specified.
					lines.Clear();
					replacements.Clear();
					matches = Regex.Matches(result, ResourceMain.rxHtmlTableFirstRow);
					foreach(Match matchItem in matches)
					{
						//	Each of the first rows of the tables in the source.
						matches2 = Regex.Matches(matchItem.Value,
							ResourceMain.rxHtmlTableCellNoFont);
						foreach(Match matchItem2 in matches2)
						{
							//	Each of the cells with no font specification.
							line = new LinePropertyItem()
							{
								Index = matchItem.Index + GetIndex(matchItem2, "content"),
								Line = GetValue(matchItem2, "content"),
								LineType = TextLineTypeEnum.HtmlTableHeaderCell
							};
							lines.Add(line);
							foreach(VariableStyleItem styleItem in stylesCurrent)
							{
								styleType =
									Enum.Parse<DocumentStyleTypeEnum>(styleItem.StyleName, true);
								switch(styleType)
								{
									case DocumentStyleTypeEnum.TableHeaderFontColor:
										line.Properties.Add(new PropertyItem()
										{
											Name = "FontColor",
											Value = styleItem.StyleValue
										});
										break;
									case DocumentStyleTypeEnum.TableHeaderFontName:
										line.Properties.Add(new PropertyItem()
										{
											Name = "FontName",
											Value = styleItem.StyleValue
										});
										break;
									case DocumentStyleTypeEnum.TableHeaderFontSize:
										line.Properties.Add(new PropertyItem()
										{
											Name = "FontSize",
											Value = styleItem.StyleValue
										});
										break;
								}
							}
						}
					}
					//	Replace all of the matched instances.
					foreach(LinePropertyItem lineItem in lines)
					{
						if(lineItem.Properties.Exists(x =>
							x.Name == "FontColor" ||
							x.Name == "FontName" ||
							x.Name == "FontSize"))
						{
							Clear(builder);
							//match = Regex.Match(lineItem.Line,
							//	ResourceMain.rxHtmlTableCellNoFont);
							//cellType = GetValue(match, "celltype");
							//attributes = GetValue(match, "attributes");
							//builder.Append($"<t{cellType}{attributes}>");
							builder.Append("<font");
							prop = lineItem.Properties.FirstOrDefault(x =>
								x.Name == "FontColor");
							if(prop != null)
							{
								builder.Append($" color=\"{prop.Value}\"");
							}
							prop = lineItem.Properties.FirstOrDefault(x =>
								x.Name == "FontName");
							if(prop != null)
							{
								builder.Append($" face=\"{prop.Value}\"");
							}
							prop = lineItem.Properties.FirstOrDefault(x =>
								x.Name == "FontSize");
							if(prop != null)
							{
								builder.Append($" style=\"font-size:{prop.Value}pt\"");
							}
							builder.Append('>');
							builder.Append(lineItem.Line);
							builder.Append("</font>");
							//builder.Append($"</t{cellType}>");

							replacements.Add(new ReplacementInfoItem()
							{
								Index = lineItem.Index,
								Length = lineItem.Line.Length,
								Text = builder.ToString()
							});
						}
					}
					if(replacements.Count > 0)
					{
						result = ReplacementInfoCollection.Replace(result, replacements);
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ApplyDocumentStylesMarkdown																						*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Apply the defined document styles to raw Markdown lines.
		/// </summary>
		/// <param name="source">
		/// The string potentially containing raw Markdown lines to style.
		/// </param>
		/// <param name="styles">
		/// Reference to a list of style items to be referenced when formatting the
		/// applicable lines.
		/// </param>
		/// <returns>
		/// The caller's string where all of the appropriate Markdown lines have
		/// received applicable styling, if a source string was provided.
		/// Otherwise, an empty string.
		/// </returns>
		private static string ApplyDocumentStylesMarkdown(string source,
			List<VariableStyleItem> styles)
		{
			bool bRow = false;
			bool bRowPrev = false;
			StringBuilder builder = new StringBuilder();
			string digit = "";
			string level = "";
			LinePropertyItem line = null;
			LinePropertyCollection lines = null;
			Match match = null;
			MatchCollection matches2 = null;
			List<LinePropertyItem> partialLines = new List<LinePropertyItem>();
			string prefix = "";
			PropertyItem prop = null;
			ReplacementInfoCollection replacements = new ReplacementInfoCollection();
			string result = "";
			List<VariableStyleItem> stylesCurrent = null;
			DocumentStyleTypeEnum styleType = DocumentStyleTypeEnum.None;

			if(source?.Length > 0 && styles != null)
			{
				result = source;
				lines = new LinePropertyCollection(source);
				if(styles.Exists(x => Regex.IsMatch(x.StyleName,
					ResourceMain.rxStyleHxType)))
				{
					//	Heading types are specified.
					foreach(LinePropertyItem lineItem in lines)
					{
						//	Check every line.
						match = Regex.Match(lineItem.Line,
							ResourceMain.rxMarkdownHeadingNoFont);
						if(match.Success)
						{
							//	Heading found.
							digit = GetValue(match, "level").Length.ToString();
							foreach(VariableStyleItem styleItem in styles)
							{
								styleType =
									Enum.Parse<DocumentStyleTypeEnum>(styleItem.StyleName, true);
								switch(styleType)
								{
									case DocumentStyleTypeEnum.HeadingFontColor:
										//	Only set this color on styles that don't have an explicit
										//	override.
										if(!styles.Exists(x => Regex.IsMatch(x.StyleName,
											ResourceMain.rxStyleHxType.Replace(@"\d|eading", digit))))
										{
											//	The specific heading color has not been specified.
											lineItem.SetProperty("FontColor", styleItem.StyleValue);
										}
										break;
									case DocumentStyleTypeEnum.HeadingFontName:
										//	Only set this font on styles that don't have an explicit
										//	override.
										if(!styles.Exists(x => Regex.IsMatch(x.StyleName,
											ResourceMain.rxStyleHxType.Replace(@"\d|eading", digit))))
										{
											//	The specific heading font has not been specified.
											lineItem.SetProperty("FontName", styleItem.StyleValue);
										}
										break;
									case DocumentStyleTypeEnum.HeadingFontSize:
										//	Only set this size on styles that don't have an explicit
										//	override.
										if(!styles.Exists(x => Regex.IsMatch(x.StyleName,
											ResourceMain.rxStyleHxType.Replace(@"\d|eading", digit))))
										{
											//	The specific heading font has not been specified.
											lineItem.SetProperty("FontSize", styleItem.StyleValue);
										}
										break;
									case DocumentStyleTypeEnum.H1FontColor:
									case DocumentStyleTypeEnum.H2FontColor:
									case DocumentStyleTypeEnum.H3FontColor:
									case DocumentStyleTypeEnum.H4FontColor:
									case DocumentStyleTypeEnum.H5FontColor:
									case DocumentStyleTypeEnum.H6FontColor:
									case DocumentStyleTypeEnum.H7FontColor:
									case DocumentStyleTypeEnum.H8FontColor:
									case DocumentStyleTypeEnum.H9FontColor:
										if(styleItem.StyleName.Substring(1, 1) == digit)
										{
											//	This specific heading is matched.
											lineItem.SetProperty("FontColor", styleItem.StyleValue);
										}
										break;
									case DocumentStyleTypeEnum.H1FontName:
									case DocumentStyleTypeEnum.H2FontName:
									case DocumentStyleTypeEnum.H3FontName:
									case DocumentStyleTypeEnum.H4FontName:
									case DocumentStyleTypeEnum.H5FontName:
									case DocumentStyleTypeEnum.H6FontName:
									case DocumentStyleTypeEnum.H7FontName:
									case DocumentStyleTypeEnum.H8FontName:
									case DocumentStyleTypeEnum.H9FontName:
										if(styleItem.StyleName.Substring(1, 1) == digit)
										{
											//	This specific heading is matched.
											lineItem.SetProperty("FontName", styleItem.StyleValue);
										}
										break;
									case DocumentStyleTypeEnum.H1FontSize:
									case DocumentStyleTypeEnum.H2FontSize:
									case DocumentStyleTypeEnum.H3FontSize:
									case DocumentStyleTypeEnum.H4FontSize:
									case DocumentStyleTypeEnum.H5FontSize:
									case DocumentStyleTypeEnum.H6FontSize:
									case DocumentStyleTypeEnum.H7FontSize:
									case DocumentStyleTypeEnum.H8FontSize:
									case DocumentStyleTypeEnum.H9FontSize:
										if(styleItem.StyleName.Substring(1, 1) == digit)
										{
											//	This specific heading is matched.
											lineItem.SetProperty("FontSize", styleItem.StyleValue);
										}
										break;
								}
							}
						}
					}
					//	Replace all of the matched instances.
					foreach(LinePropertyItem lineItem in lines)
					{
						if(lineItem.Properties.Exists(x =>
							x.Name == "FontColor" ||
							x.Name == "FontName" ||
							x.Name == "FontSize"))
						{
							Clear(builder);
							match = Regex.Match(lineItem.Line,
								ResourceMain.rxMarkdownHeadingNoFont);
							level = GetValue(match, "level");
							digit = level.Length.ToString();
							builder.Append($"{prefix}{level} ");
							builder.Append("<font");
							prop = lineItem.Properties.FirstOrDefault(x =>
								x.Name == "FontColor");
							if(prop != null)
							{
								builder.Append($" color=\"{prop.Value}\"");
							}
							prop = lineItem.Properties.FirstOrDefault(x =>
								x.Name == "FontName");
							if(prop != null)
							{
								builder.Append($" face=\"{prop.Value}\"");
							}
							prop = lineItem.Properties.FirstOrDefault(x =>
								x.Name == "FontSize");
							if(prop != null)
							{
								builder.Append($" style=\"font-size:{prop.Value}pt\"");
							}
							builder.Append('>');
							builder.Append(GetValue(match, "line"));
							builder.Append("</font>");
							replacements.Add(new ReplacementInfoItem()
							{
								Index = lineItem.Index,
								Length = lineItem.Line.Length,
								Text = builder.ToString()
							});
							lineItem.Properties.Clear();
						}
					}
				}
				stylesCurrent = styles.FindAll(x =>
					Regex.IsMatch(x.StyleName, ResourceMain.rxStyleTableHeaderType));
				if(stylesCurrent.Count > 0)
				{
					//	Table header types are specified.
					partialLines.Clear();
					bRow = false;
					bRowPrev = false;
					foreach(LinePropertyItem lineItem in lines)
					{
						match = Regex.Match(lineItem.Line,
							ResourceMain.rxMarkdownTableRow);
						if(match.Success)
						{
							bRow = true;
							if(bRow && !bRowPrev)
							{
								//	This is the first row of a markdown table.
								matches2 = Regex.Matches(match.Value,
									ResourceMain.rxMarkdownTableCellNoFont);
								foreach(Match matchItem2 in matches2)
								{
									//	Each of the cells with no font specification.
									line = new LinePropertyItem()
									{
										Index = lineItem.Index + GetIndex(matchItem2, "content"),
										Line = GetValue(matchItem2, "content"),
										LineType = TextLineTypeEnum.HtmlTableHeaderCell
									};
									partialLines.Add(line);
									foreach(VariableStyleItem styleItem in stylesCurrent)
									{
										styleType =
											Enum.Parse<DocumentStyleTypeEnum>(
												styleItem.StyleName, true);
										switch(styleType)
										{
											case DocumentStyleTypeEnum.TableHeaderFontColor:
												line.Properties.Add(new PropertyItem()
												{
													Name = "FontColor",
													Value = styleItem.StyleValue
												});
												break;
											case DocumentStyleTypeEnum.TableHeaderFontName:
												line.Properties.Add(new PropertyItem()
												{
													Name = "FontName",
													Value = styleItem.StyleValue
												});
												break;
											case DocumentStyleTypeEnum.TableHeaderFontSize:
												line.Properties.Add(new PropertyItem()
												{
													Name = "FontSize",
													Value = styleItem.StyleValue
												});
												break;
										}
									}
								}
							}
							bRowPrev = bRow;
						}
					}
					//	Replace all of the matched instances.
					foreach(LinePropertyItem lineItem in partialLines)
					{
						if(lineItem.Properties.Exists(x =>
							x.Name == "FontColor" ||
							x.Name == "FontName" ||
							x.Name == "FontSize"))
						{
							Clear(builder);
							match = Regex.Match(lineItem.Line,
								ResourceMain.rxMarkdownTableCellNoFont);
							builder.Append("<font");
							prop = lineItem.Properties.FirstOrDefault(x =>
								x.Name == "FontColor");
							if(prop != null)
							{
								builder.Append($" color=\"{prop.Value}\"");
							}
							prop = lineItem.Properties.FirstOrDefault(x =>
								x.Name == "FontName");
							if(prop != null)
							{
								builder.Append($" face=\"{prop.Value}\"");
							}
							prop = lineItem.Properties.FirstOrDefault(x =>
								x.Name == "FontSize");
							if(prop != null)
							{
								builder.Append($" style=\"font-size:{prop.Value}pt\"");
							}
							builder.Append('>');
							builder.Append(lineItem.Line);
							builder.Append("</font>");
							replacements.Add(new ReplacementInfoItem()
							{
								Index = lineItem.Index,
								Length = lineItem.Line.Length,
								Text = builder.ToString()
							});
						}
					}
				}
				if(replacements.Count > 0)
				{
					result = ReplacementInfoCollection.Replace(result, replacements);
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ApplyUserVariables																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the caller's string with all of its user-defined interpolated
		/// string variables resolved.
		/// </summary>
		/// <param name="source">
		/// The source string to parse for variables.
		/// </param>
		/// <param name="variables">
		/// Reference to the collection of variable definitions to process.
		/// </param>
		/// <returns>
		/// The caller's string with interpolated strings resolved and replaced.
		/// </returns>
		private static string ApplyUserVariables(string source,
			VariableEntryCollection variables)
		{
			MatchCollection matches = null;
			FindReplaceCollection replacements = new FindReplaceCollection();
			string result = "";

			if(source?.Length > 0)
			{
				result = source;
				if(variables?.Count > 0)
				{
					//	Resolve the internal values.
					VariableEntryCollection.ResolveValues(variables);
					matches = Regex.Matches(source,
						ResourceMain.rxInterpolatedExpression);
					foreach(Match matchItem in matches)
					{
						replacements.Add(new FindReplaceItem()
						{
							Find = matchItem.Value,
							Replace = VariableEntryCollection.ResolveValue(
								variables, matchItem.Value)
						});
					}
					foreach(FindReplaceItem replacementItem in replacements)
					{
						result = result.Replace(
							replacementItem.Find, replacementItem.Replace);
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* CloseFile																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Close the currently open file.
		/// </summary>
		/// <returns>
		/// Reference to an awaitable task.
		/// </returns>
		private async Task CloseFile()
		{
			if(await SaveFileChangesDialog() != DialogResult.Cancel)
			{
				SetWebViewContentChangeEnabled(false);
				SetWebViewMarkdown("");
				SetWebViewUserVariablesEmpty();
				if(mProject.ProjectMode != ProjectModeEnum.MarkdownProject)
				{
					//	If no project is active, then clear the markdown
					//	filename.
					mProject.MarkdownFilename = "";
					mProject.ProjectMode = ProjectModeEnum.None;
				}
				//mFilename = "";
				SetFileContentChanged(false);
				SetWebViewContentChangeEnabled(true);
				UpdateMenuControls();
			}

		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* CloseProject																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Close the currently open project.
		/// </summary>
		/// <returns>
		/// Reference to an awaitable task.
		/// </returns>
		private async Task CloseProject()
		{
			if(await SaveProjectChangesDialog() != DialogResult.Cancel)
			{
				SetWebViewContentChangeEnabled(false);
				SetWebViewMarkdown("");
				SetWebViewUserVariablesEmpty();
				mProject = new ProjectPackageItem();
				mProject.ProjectPackageChanged += mProject_ProjectPackageChanged;
				mProject.ProjectMode = ProjectModeEnum.None;
				SetProjectContentChanged(false);
				SetWebViewContentChangeEnabled(true);
				UpdateMenuControls();
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ConvertLinearToTableStyle																							*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Convert linear editing style text to table style.
		/// </summary>
		/// <param name="markdown">
		/// The caller's linear editing style markdown.
		/// </param>
		/// <param name="showUI">
		/// Value indicating whether the user interface will be displayed for
		/// actions and messages in this method.
		/// </param>
		/// <returns>
		/// A version of the markdown that uses table rows to represent the
		/// sections found in the linear style.
		/// </returns>
		private string ConvertLinearToTableStyle(string markdown,
			bool showUI = false)
		{
			MarkdownBlockCollection blocks = null;
			StringBuilder builder = new StringBuilder();
			string result = "";

			if(markdown?.Length > 0)
			{
				StringTrackerItem message = new StringTrackerItem();

				blocks = MarkdownBlockCollection.Parse(markdown);

				if(MarkdownBlockCollection.HasHtmlSections(blocks))
				{
					//	This collection is using HTML sections to separate detail records.
					if(MarkdownBlockCollection.FormatSectionsAsRecords(blocks, message))
					{
						Clear(builder);
						MarkdownBlockCollection.RenderText(blocks, builder);
						result = builder.ToString();
						//SetWebViewMarkdown(builder.ToString());
						if(showUI)
						{
							SetStatusMessage("Sections have been formatted as tables...");
						}
					}
					else if(showUI)
					{
						MessageDialog(message.Text, "Convert to Table Style");
					}
				}
				else if(MarkdownBlockCollection.HasHorizontalLines(blocks))
				{
					//	This collection is using horizontal lines to separate detail
					//	records.
					if(MarkdownBlockCollection.FormatLinesAsRecords(blocks, message))
					{
						Clear(builder);
						MarkdownBlockCollection.RenderText(blocks, builder);
						result = builder.ToString();
						//SetWebViewMarkdown(builder.ToString());
						if(showUI)
						{
							SetStatusMessage(
								"Horizontal lines have been formatted as tables...");
						}
					}
					else if(showUI)
					{
						MessageDialog(message.Text, "Convert to Table Style");
					}
				}
				else
				{
					result = markdown;
					if(showUI)
					{
						//	No contents were found that could be converted to table.
						MessageDialog(
							"None of this content has been formatted as tables.\r\n" +
							"Try enclosing records in HTML <section> ... </section> nodes " +
							"or separating them using Markdown's horizontal line '---'.",
							"Convert to Table Style");
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		////*-----------------------------------------------------------------------*
		////* GetDetailContent																											*
		////*-----------------------------------------------------------------------*
		///// <summary>
		///// Return the detail content for the heading with the specified level and
		///// value.
		///// </summary>
		///// <param name="token">
		///// Reference to a detail record token.
		///// </param>
		///// <param name="level">
		///// The heading level at which the headings will be searched for a
		///// match.
		///// </param>
		///// <param name="heading">
		///// The name of the heading to match.
		///// </param>
		///// <returns>
		///// Child content belonging to the heading of the specified level and name.
		///// </returns>
		//private string GetDetailContent(MarkdownBlockItem token, int level,
		//	string heading)
		//{
		//	StringBuilder builder = new StringBuilder();

		//	//	TODO: Get the detail content for this heading.
		//	return builder.ToString();
		//}
		////*-----------------------------------------------------------------------*

		////*-----------------------------------------------------------------------*
		////* GetDetailHeadingLevel																									*
		////*-----------------------------------------------------------------------*
		///// <summary>
		///// Return the main heading level for the provided detail records.
		///// </summary>
		///// <param name="details">
		///// Reference to a list of detail records to inspect.
		///// </param>
		///// <returns>
		///// The main heading level serving as field names in this set of details.
		///// </returns>
		//public int GetDetailHeadingLevel(List<MarkdownBlockItem> details)
		//{
		//	bool bFound = false;
		//	int level = 0;
		//	MatchCollection matches = null;

		//	if(details?.Count > 0)
		//	{
		//		foreach(MarkdownBlockItem detailItem in details)
		//		{
		//			matches = Regex.Matches(detailItem.Value, ResourceMain.rxLine);
		//			foreach(Match matchItem in matches)
		//			{
		//				if(matchItem.Length > 0)
		//				{
		//					if(IsHeading(matchItem.Value))
		//					{
		//						if(!bFound)
		//						{
		//							//	First heading encountered.
		//							level = GetHeadingLevel(matchItem.Value);
		//							if(level > 0)
		//							{
		//								bFound = true;
		//								break;
		//							}
		//						}
		//					}
		//				}
		//			}
		//			if(bFound)
		//			{
		//				break;
		//			}
		//		}
		//	}
		//	return level;
		//}
		////*-----------------------------------------------------------------------*

		////*-----------------------------------------------------------------------*
		////* GetDetailTokens																												*
		////*-----------------------------------------------------------------------*
		///// <summary>
		///// Retrieve detail tokens for the specified text.
		///// </summary>
		///// <param name="source">
		///// The source string to be searched.
		///// </param>
		///// <returns>
		///// Reference to a collection of tokens representing detail records in the
		///// source text.
		///// </returns>
		//private MarkdownBlockCollection GetDetailTokens(string source)
		//{
		//	bool bDetailRunning = false;
		//	bool bDetailStart = false;
		//	StringBuilder builder = new StringBuilder();
		//	MarkdownBlockCollection detailTokens = new MarkdownBlockCollection();
		//	//string entry = "";
		//	//MatchCollection innerMatches = null;
		//	int level = 0;
		//	//List<string> levels = new List<string>();
		//	Match match = null;
		//	MatchCollection matches = null;
		//	int start = 0;
		//	string text = "";

		//	//	Record-separating methods.
		//	//	1 - Section-encapsulated:
		//	//			<section>record</section>[---]<section>record</section>
		//	//	2 - Horizontal-line delineated
		//	//			---
		//	if(source?.Length > 0)
		//	{
		//		//	Find <section> nodes
		//		matches = Regex.Matches(source, ResourceMain.rxSection);
		//		if(matches.Count > 0)
		//		{
		//			//	The first separation option is selected: Use sections.
		//			foreach(Match matchItem in matches)
		//			{
		//				//	Get the highest header level in the details.
		//				detailTokens.Add(new MarkdownBlockItem()
		//				{
		//					StartIndex = matchItem.Index,
		//					Value = matchItem.Value
		//				});
		//				//entry = GetValue(matchItem, "content");
		//				//innerMatches = Regex.Matches(entry, ResourceMain.rxMarkdownHeading);
		//				//foreach(Match innerMatchItem in innerMatches)
		//				//{
		//				//	text = GetValue(innerMatchItem, "level");
		//				//	if(!levels.Exists(x => x == text))
		//				//	{
		//				//		levels.Add(text);
		//				//	}
		//				//}
		//			}
		//			////	At this point, we have each of the known detail levels in the
		//			////	levels list.
		//			//level = 0;
		//			//levels.Sort();
		//			//if(levels.Count > 0)
		//			//{
		//			//	level = levels[0].Length;
		//			//}
		//			//if(level > 0)
		//			//{
		//			//	//	This is the detail level. Every smaller level number is going
		//			//	//	to be a parent to ancestor of the items in each table.
		//			//}
		//		}
		//		else
		//		{
		//			match = Regex.Match(source, ResourceMain.rxMarkdownLine);
		//			if(match.Success)
		//			{
		//				//	The second separation option is selected: Use horizontal lines.
		//				matches = Regex.Matches(source, ResourceMain.rxLine);
		//				foreach(Match matchItem in matches)
		//				{
		//					//	Check each file line.
		//					text = GetValue(matchItem, "line");
		//					if(text.Trim() == "---")
		//					{
		//						//	Horizontal line.
		//						if(builder.Length > 0)
		//						{
		//							//	Previous detail was open.
		//							detailTokens.Add(new MarkdownBlockItem()
		//							{
		//								StartIndex = start,
		//								Value = builder.ToString()
		//							});
		//							Clear(builder);
		//						}
		//						bDetailStart = true;
		//						bDetailRunning = false;
		//					}
		//					else if(bDetailStart)
		//					{
		//						if(matchItem.Value.Length > 0)
		//						{
		//							if(GetHeadingLevel(matchItem.Value) > level)
		//							{
		//								//	Another record is starting.
		//								start = matchItem.Index;
		//								bDetailStart = false;
		//								bDetailRunning = true;
		//							}
		//							else
		//							{
		//								//	End of this table.
		//								bDetailStart = false;
		//							}
		//						}
		//					}
		//					else if(!bDetailRunning && text.TrimStart().StartsWith('#'))
		//					{
		//						//	This is a header.
		//						level = GetHeadingLevel(text);
		//					}
		//					if(bDetailRunning)
		//					{
		//						builder.AppendLine(text);
		//					}
		//				}
		//				//	We're not going to add the content that was found beyond the
		//				//	last horizontal line.
		//				Clear(builder);

		//			}
		//			else
		//			{
		//				SetStatusMessage("No detail records found. Conversion cancelled.");
		//			}
		//		}
		//	}
		//	else
		//	{
		//		SetStatusMessage("No Markdown text found. Conversion cancelled.");
		//	}
		//	return detailTokens;

		//}
		////*-----------------------------------------------------------------------*

		////*-----------------------------------------------------------------------*
		////* GetHeadingValues																											*
		////*-----------------------------------------------------------------------*
		///// <summary>
		///// Return the Markdown heading values from the caller-provided source
		///// text.
		///// </summary>
		///// <param name="details">
		///// The detail records source text to search for headings.
		///// </param>
		///// <param name="level">
		///// The heading level to accept.
		///// </param>
		///// <returns>
		///// List of headings in the same level as the first heading encountered
		///// in the text.
		///// </returns>
		//public List<string> GetHeadingValues(List<MarkdownBlockItem> details,
		//	int level)
		//{
		//	MatchCollection matches = null;
		//	List<string> result = new List<string>();
		//	string text = "";

		//	if(details?.Count > 0)
		//	{
		//		foreach(MarkdownBlockItem detailItem in details)
		//		{
		//			matches = Regex.Matches(detailItem.Value, ResourceMain.rxLine);
		//			foreach(Match matchItem in matches)
		//			{
		//				if(matchItem.Length > 0)
		//				{
		//					if(IsHeading(matchItem.Value))
		//					{
		//						if(GetHeadingLevel(matchItem.Value) == level)
		//						{
		//							text = GetValue(matchItem.Value,
		//								ResourceMain.rxMarkdownHeading, "line");
		//							if(!result.Contains(text))
		//							{
		//								result.Add(text);
		//							}
		//						}
		//					}
		//				}
		//			}
		//		}
		//	}
		//	return result;
		//}
		////*-----------------------------------------------------------------------*

		////*-----------------------------------------------------------------------*
		////* GetParentHeadingTokens																								*
		////*-----------------------------------------------------------------------*
		///// <summary>
		///// Retrieve parent header tokens for the detail tokens present in the
		///// provided list.
		///// </summary>
		///// <param name="parentLevel">
		///// The maximum header level for any parent or ancestor to be found.
		///// </param>
		///// <param name="details">
		///// Reference to a collection of detail tokens already located.
		///// </param>
		///// <param name="source">
		///// The source string to be searched.
		///// </param>
		///// <returns>
		///// Reference to a collection of tokens representing headers of the
		///// provided details.
		///// </returns>
		//private MarkdownBlockCollection GetParentHeadingTokens(
		//	MarkdownBlockCollection details, string source)
		//{
		//	int count = 0;
		//	//int detailLevel = int.MaxValue;
		//	int index = 0;
		//	MatchCollection matches = null;
		//	MarkdownBlockItem parentToken = null;
		//	MarkdownBlockCollection parentTokens = null;
		//	string pattern = "";
		//	MarkdownBlockItem tokenPrev = null;

		//	if(details?.Count > 0 && source?.Length > 0)
		//	{
		//		////	Find the header levels present within the details.
		//		//foreach(MarkdownBlockItem detailTokenItem in details)
		//		//{
		//		//	if(IsHeading(detailTokenItem.Value))
		//		//	{
		//		//		//	This detail line is a header.
		//		//		detailLevel =
		//		//			Math.Min(GetHeadingLevel(detailTokenItem.Value), detailLevel);
		//		//	}
		//		//}
		//		//if(detailLevel == 0)
		//		//{
		//		//	detailLevel = 1;
		//		//}
		//		pattern = ResourceMain.rxMarkdownHeadingN.
		//			Replace("{minLevel}", $"1").
		//				Replace("{maxLevel}", $"9");
		//		matches = Regex.Matches(source, pattern);
		//		parentTokens = MarkdownBlockCollection.Parse(matches, "level");
		//		count = parentTokens.Count;
		//		for(index = 0; index < count; index ++)
		//		{
		//			parentToken = parentTokens[index];
		//			if(details.IsSourceIndexInContent(parentToken.StartIndex))
		//			{
		//				//	This item is located in the detail content.
		//				parentTokens.RemoveAt(index);
		//				index--;  //	Delist.
		//				count--;	//	Discount.
		//			}
		//		}
		//		foreach(MarkdownBlockItem tokenItem in details)
		//		{
		//			//	Find a parent token for each of the detail tokens.
		//			tokenPrev = null;
		//			foreach(MarkdownBlockItem parentTokenItem in parentTokens)
		//			{
		//				if(parentTokenItem.StartIndex >= tokenItem.StartIndex)
		//				{
		//					//	If this parent follows the detail then the previous
		//					//	non-following object leads this detail.
		//					if(tokenPrev != null)
		//					{
		//						tokenItem.NamedTags["ParentToken"] = tokenPrev;
		//					}
		//					break;
		//				}
		//				tokenPrev = parentTokenItem;
		//			}
		//			if(tokenPrev != null &&
		//				tokenPrev.StartIndex < tokenItem.StartIndex &&
		//				!tokenItem.NamedTags.ContainsKey("ParentToken"))
		//			{
		//				//	This level only had a single parent token.
		//				tokenItem.NamedTags["ParentToken"] = tokenPrev;
		//			}
		//		}
		//	}
		//	return parentTokens;
		//}
		////*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetWebViewCursorInfo																									*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a Start/End item containing the current user cursor location
		/// within the markdown content.
		/// </summary>
		/// <returns>
		/// Reference to a Start/End item indicating the user cursor position,
		/// if found. Otherwise, an empty Start/End item.
		/// </returns>
		public async Task<CursorInfoCollection> GetWebViewCursorInfo()
		{
			string content = "";
			CursorInfoCollection cursorInfo = null;
			CursorInfoCollection result = null;

			if(webView21 != null)
			{
				content = await webView21.ExecuteScriptAsync(
					"getCursorInfo();");
				if(content.Length > 0)
				{
					cursorInfo =
						JsonConvert.DeserializeObject<CursorInfoCollection>(content);
					if(cursorInfo != null)
					{
						result = cursorInfo;
					}
				}
			}
			if(result == null)
			{
				result = new CursorInfoCollection();
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetWebViewMarkdown																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a string containing the markdown content from the page.
		/// </summary>
		/// <returns>
		/// String containing the markdown content of the page.
		/// </returns>
		public async Task<string> GetWebViewMarkdown()
		{
			string content = "";
			ContentInfoItem contentInfo = null;
			string result = "";

			//	TODO: BUG - GetWebViewMarkdown can crash the app during OnClosing.
			if(webView21 != null)
			{
				content = await webView21.ExecuteScriptAsync(
					"getMarkdown();");
				if(content?.Length > 0)
				{
					contentInfo =
						JsonConvert.DeserializeObject<ContentInfoItem>(content);
					if(contentInfo.Content?.Length > 0)
					{
						result = contentInfo.Content;
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* LoadEditorStyles																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Load the styles for the editor.
		/// </summary>
		private void LoadEditorStyles()
		{
			string content = "";
			string filename = Path.Combine(
				AppDomain.CurrentDomain.BaseDirectory,
				@"Settings\MarkdownEditorStyles.json");

			if(Path.Exists(filename))
			{
				//	The markdown settings file exists.
				content = File.ReadAllText(filename);
				mEditorStyles =
					JsonConvert.DeserializeObject<MarkdownEditorStyleItem>(content);
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* MessageDialog																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Notify the user of a condition.
		/// </summary>
		/// <param name="prompt">
		/// The prompt message to display to the user.
		/// </param>
		/// <param name="caption">
		/// The title of the dialog.
		/// </param>
		private void MessageDialog(string prompt, string caption = "")
		{
			string titleText = "Markdown Editor";

			if(prompt?.Length > 0)
			{
				if(caption?.Length > 0)
				{
					titleText = caption;
				}
				//using(new CenterWinDialog(this))
				//{
				MessageBox.Show(prompt, caption);
				//}

			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* mnuEditCopy_Click																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// The Edit / Copy menu option has been clicked.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Standard event arguments.
		/// </param>
		private void mnuEditCopy_Click(object sender, EventArgs e)
		{
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* mnuEditCut_Click																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// The Edit / Cut menu option has been clicked.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Standard event arguments.
		/// </param>
		private void mnuEditCut_Click(object sender, EventArgs e)
		{
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* mnuEditPaste_Click																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// The Edit / Paste menu option has been clicked.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Standard event arguments.
		/// </param>
		private void mnuEditPaste_Click(object sender, EventArgs e)
		{
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* mnuEditProjectOptions_Click																						*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// The Edit / Project Options menu option has been clicked.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Standard event arguments.
		/// </param>
		private void mnuEditProjectOptions_Click(object sender, EventArgs e)
		{
			frmProjectOptions dialog = new frmProjectOptions();

			dialog.WorkingPath = mProject.WorkingPath;
			dialog.MarkdownFilename =
				GetRelativeFilename(
					mProject.WorkingPath, mProject.MarkdownFilename);
			dialog.DocxFilename = mProject.DocxFilename;
			dialog.DocxTemplateFilename = mProject.DocxTemplateFilename;
			dialog.HtmlFilename = mProject.HtmlFilename;
			dialog.OptionConvertToTable = mProject.OptionConvertToTable;
			VariableEntryCollection.Clone(
				mProject.MappedColumns, dialog.MappedColumns);
			VariableStyleCollection.Clone(
				mProject.Styles, dialog.Styles);
			VariableEntryCollection.Clone(
				mProject.Variables, dialog.Variables);
			dialog.Owner = this;
			dialog.FormClosing += projectOptionDialog_FormClosing;
			dialog.Show();

		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* mnuEditRedo_Click																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// The Edit / Redo option has been clicked.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Standard event arguments.
		/// </param>
		private void mnuEditRedo_Click(object sender, EventArgs e)
		{
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* mnuEditSectionInsert_Click																						*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// The Edit / Insert Section menu option has been clicked.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Standard event arguments.
		/// </param>
		private async void mnuEditSectionInsert_Click(object sender, EventArgs e)
		{
			StringBuilder builder = new StringBuilder();
			bool bUpdated = false;
			string content = await GetWebViewMarkdown();
			CursorInfoCollection cursorInfo = await GetWebViewCursorInfo();
			CursorInfoItem cursorItem = null;
			int indexEnd = 0;
			int indexStart = 0;

			if(cursorInfo.Exists(x => x.Name == "Start") &&
				cursorInfo.Exists(x => x.Name == "End"))
			{
				//	Start.
				cursorItem = cursorInfo.First(x => x.Name == "Start");
				indexStart =
					GetStringIndex(content, cursorItem.Line, cursorItem.Column);
				//	End.
				cursorItem = cursorInfo.First(x => x.Name == "End");
				indexEnd =
					GetStringIndex(content, cursorItem.Line, cursorItem.Column);

				Trace.WriteLine($"SectionInsert: Start: {indexStart}, End: {indexEnd}");
				if(indexStart > 0 && indexEnd >= indexStart)
				{
					builder.Append(content.Substring(0, indexStart));
					builder.AppendLine("<section>\r\n</section>");
					if(indexStart == indexEnd)
					{
						//	No active highlight.
						if(builder.Length > indexEnd)
						{
							builder.Append(content.Substring(indexEnd));
						}
					}
					else if(builder.Length > indexEnd + 1)
					{
						//	Append the portion after the highlight.
						builder.Append(content.Substring(indexEnd + 1));
					}
					bUpdated = true;
				}
				else
				{
					builder.AppendLine("<section>\r\n</section>");
					builder.Append(content);
					indexEnd = GetLineCount(content);
					cursorInfo = new CursorInfoCollection();
					cursorInfo.Add(new CursorInfoItem()
					{
						Name = "Start",
						Line = indexEnd
					});
					cursorInfo.Add(new CursorInfoItem()
					{
						Name = "End",
						Line = indexEnd
					});
					bUpdated = true;
				}
				SetWebViewMarkdown(builder.ToString());
				if(bUpdated)
				{
					SetWebViewCursorInfo(cursorInfo);
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* mnuEditSectionStyleAppend_Click																				*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// The Edit / Append Section Style to End of Code menu option has been
		/// clicked.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Standard event arguments.
		/// </param>
		private async void mnuEditSectionStyleAppend_Click(object sender,
			EventArgs e)
		{
			StringBuilder builder = new StringBuilder();

			builder.AppendLine(await GetWebViewMarkdown());
			builder.AppendLine("");
			builder.AppendLine(
				ToMultiLineString(mEditorStyles.HtmlSectionFormatting));

			SetWebViewMarkdown(builder.ToString());
			SetStatusMessage("Section styling has been appended to markdown.");
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* mnuEditSectionStyleClipboard_Click																		*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// The Edit / Load Clipboard with Section Style menu option has been
		/// clicked.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Standard event arguments.
		/// </param>
		private void mnuEditSectionStyleClipboard_Click(object sender, EventArgs e)
		{
			string content = ToMultiLineString(mEditorStyles.HtmlSectionFormatting);

			Clipboard.SetText(content);

			MessageDialog("Section styling has been copied to the clipboard.\r\n" +
				"Paste to the bottom of your favorite markdown editor.");
			SetStatusMessage("Section styling has been copied to the clipboard.");
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* mnuEditUndo_Click																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// The Edit / Undo menu option has been clicked.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Standard event arguments.
		/// </param>
		private void mnuEditUndo_Click(object sender, EventArgs e)
		{
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* mnuFileClose_Click																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// The File / Close Current File menu option has been clicked.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Standard event arguments.
		/// </param>
		private async void mnuFileClose_Click(object sender, EventArgs e)
		{
			await CloseFile();
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* mnuFileCloseProject_Click																							*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// The File / Close Project menu option has been clicked.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Standard event arguments.
		/// </param>
		private async void mnuFileCloseProject_Click(object sender, EventArgs e)
		{
			await CloseProject();
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* mnuFileConvertOldProjectToNew_Click																		*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// The File / Convert / Old Markdown Projects to New (.mdproj) menu
		/// option has been clicked.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Standard event arguments.
		/// </param>
		private void mnuFileConvertOldProjectToNew_Click(object sender,
			EventArgs e)
		{
			bool bContinue = true;
			int count = 0;
			string[] filenames =
				OpenMarkdownProjectsDialog(
					"Select Projects to Convert", ".mdproj.json");
			string targetFilename = "";
			string targetPath = "";

			if(filenames?.Length > 0)
			{
				foreach(string filenameItem in filenames)
				{
					if(File.Exists(filenameItem))
					{
						targetPath = Path.GetDirectoryName(filenameItem);
						targetFilename = Regex.Replace(Path.GetFileName(filenameItem),
							ResourceMain.rxFilenameAndExtension, "${filename}.mdproj");
						targetFilename = Path.Combine(targetPath, targetFilename);
						if(targetFilename.Length > 0 && targetFilename != filenameItem)
						{
							bContinue = true;
							if(File.Exists(targetFilename))
							{
								if(QuestionDialog("The file " +
									$"{Path.GetFileName(targetFilename)} already exists.\r\n" +
									"\r\nOverwrite?",
									"Convert Project Files", MessageBoxButtons.YesNo) !=
									DialogResult.Yes)
								{
									bContinue = false;
								}
							}
							if(bContinue)
							{
								//	Rename the project.
								File.Move(filenameItem, targetFilename, true);
								count++;
							}
						}
					}
				}
			}
			SetStatusMessage($"Project files converted: {count}");
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* mnuFileExit_Click																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// The File / Exit menu option has been clicked.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Standard event arguments.
		/// </param>
		private void mnuFileExit_Click(object sender, EventArgs e)
		{
			this.Close();
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* mnuFileExportDocumentStyles_Click																			*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// The File / Export / Document Styles as JSON menu option has been
		/// clicked.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Standard event arguments.
		/// </param>
		private void mnuFileExportDocumentStyles_Click(object sender, EventArgs e)
		{
			string content = "";
			string filename = SaveStylesDialog();

			if(filename?.Length > 0)
			{
				content = JsonConvert.SerializeObject(mProject.Styles);
				File.WriteAllText(filename, content);
				SetStatusMessage(
					"Current styles have been exported to " +
					$"{Path.GetFileName(filename)}");
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* mnuFileExportDocX_Click																								*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// The File / Export / as DocX menu option has been clicked.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Standard event arguments.
		/// </param>
		/// <remarks>
		/// Export the file to a fresh DOCX page.
		/// </remarks>
		private async void mnuFileExportDocX_Click(object sender, EventArgs e)
		{
			HtmlDocxBuilderItem docBuilder = null;
			string filename = "";
			Html.HtmlDocument html = null;
			string markdown = "";
			MarkdownPipeline pipeline = null;
			Process proc = null;
			string text = "";

			filename = SaveWordDialog(
				defaultFilename: mProject.GetDefaultDocxFilename());
			if(filename.Length > 0)
			{
				mProject.DocxFilename =
					GetRelativeFilename(mProject.WorkingPath, filename);
				SetStatusMessage(
					$"Exporting markdown to {Path.GetFileName(filename)}");
				markdown = await GetWebViewMarkdown();
				markdown = ProcessMarkdown(markdown, mProject);

				pipeline = new MarkdownPipelineBuilder().
					UseAdvancedExtensions().Build();
				text = Markdown.ToHtml(markdown, pipeline);
				html = new Html.HtmlDocument(text, true);
				HtmlUtil.DecodeHtmlText(html.Nodes);
				ProcessWhitespace(html.Nodes);

				//	TODO: Table header font size should not set the size of all subsequent text.
				using(DocX document = DocX.Create(filename))
				{
					docBuilder = new HtmlDocxBuilderItem();
					docBuilder.BasePath = Path.GetDirectoryName(filename);
					docBuilder.Document = document;
					VariableStyleCollection.Clone(
						mProject.Styles, docBuilder.UserStyles);
					//	New documents don't have complete headers defined.
					docBuilder.OverrideHeaderStyles = true;
					docBuilder.AppendHtml(html.Nodes);

					document.Save();

					SetStatusMessage(
						$"Code exported to {Path.GetFileName(filename)}...");
				}

				proc = new Process();
				proc.StartInfo = new ProcessStartInfo(filename)
				{
					UseShellExecute = true
				};
				proc.Start();

			}

		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* mnuFileExportDocXTemplate_Click																				*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// The File / Export / as DocX with Template menu option has been clicked.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Standard event arguments.
		/// </param>
		private async void mnuFileExportDocXTemplate_Click(object sender,
			EventArgs e)
		{
			HtmlDocxBuilderItem docBuilder = null;
			string filename = "";
			string filenameReference = "";
			Html.HtmlDocument html = null;
			string markdown = "";
			MarkdownPipeline pipeline = null;
			Process proc = null;
			string text = "";

			filename = SaveWordDialog(
				defaultFilename: mProject.GetDefaultDocxFilename());
			if(filename.Length > 0)
			{
				filenameReference = OpenWordDialog(
					dialogTitle: "Open Word Template as a Format Reference",
					defaultFilename: mProject.GetDefaultDocxTemplateFilename());
				if(filenameReference.Length > 0 && File.Exists(filenameReference))
				{
					mProject.DocxFilename =
						GetRelativeFilename(mProject.WorkingPath, filename);
					mProject.DocxTemplateFilename =
						GetRelativeFilename(mProject.WorkingPath, filenameReference);
					SetStatusMessage(
						$"Exporting markdown to {Path.GetFileName(filename)}");
					markdown = await GetWebViewMarkdown();
					markdown = ProcessMarkdown(markdown, mProject);

					pipeline = new MarkdownPipelineBuilder().
						UseAdvancedExtensions().Build();
					text = Markdown.ToHtml(markdown, pipeline);
					html = new Html.HtmlDocument(text, true);
					HtmlUtil.DecodeHtmlText(html.Nodes);
					ProcessWhitespace(html.Nodes);

					using(DocX document = DocX.Load(filenameReference))
					{
						docBuilder = new HtmlDocxBuilderItem();
						docBuilder.BasePath = Path.GetDirectoryName(filename);
						docBuilder.Document = document;
						VariableStyleCollection.Clone(
							mProject.Styles, docBuilder.UserStyles);
						////	New documents don't have complete headers defined.
						//docBuilder.OverrideHeaderStyles = true;
						docBuilder.AppendHtml(html.Nodes);

						document.SaveAs(filename);

						SetStatusMessage(
							$"Code exported to {Path.GetFileName(filename)}...");
					}
					proc = new Process();
					proc.StartInfo = new ProcessStartInfo(filename)
					{
						UseShellExecute = true
					};
					proc.Start();

				}
				else
				{
					SetStatusMessage("Export cancelled...");
				}
			}
			else
			{
				SetStatusMessage("Export cancelled...");
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* mnuFileExportHtml_Click																								*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// The File / Export / as HTML menu option has been clicked.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Standard event arguments.
		/// </param>
		private void mnuFileExportHtml_Click(object sender, EventArgs e)
		{
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* mnuFileExportMappedColumnNames_Click																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// The File / Export / Mapped Column Names as JSON menu option has been
		/// clicked.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Standard event arguments.
		/// </param>
		private void mnuFileExportMappedColumnNames_Click(object sender,
			EventArgs e)
		{
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* mnuFileExportText_Click																								*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// The File / Export / as Text menu option has been clicked.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Standard event arguments.
		/// </param>
		private void mnuFileExportText_Click(object sender, EventArgs e)
		{
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* mnuFileExportVariablePackage_Click																		*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// The File / Export / Variable Package File menu option has been clicked.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Standard event arguments.
		/// </param>
		private void mnuFileExportVariablePackage_Click(object sender, EventArgs e)
		{
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* mnuFileExportVariables_Click																					*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// The File / Export / Variables as JSON menu option has been clicked.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Standard event arguments.
		/// </param>
		private void mnuFileExportVariables_Click(object sender, EventArgs e)
		{
			string content = "";
			string filename = SaveVariablesDialog();

			if(filename?.Length > 0)
			{
				content = JsonConvert.SerializeObject(mProject.Variables);
				File.WriteAllText(filename, content);
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* mnuFileImportDocumentStyles_Click																			*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// The File / Import / Document Styles from JSON menu option has been
		/// clicked.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Standard event arguments.
		/// </param>
		private void mnuFileImportDocumentStyles_Click(object sender, EventArgs e)
		{
			string content = "";
			string filename = OpenStylesDialog();
			VariableStyleItem style = null;
			VariableStyleCollection styles = null;

			if(filename?.Length > 0)
			{
				content = File.ReadAllText(filename);
				styles =
					JsonConvert.DeserializeObject<VariableStyleCollection>(content);
				foreach(VariableStyleItem styleItem in styles)
				{
					style = mProject.Styles.FirstOrDefault(x =>
						x.StyleName == styleItem.StyleName);
					if(style == null)
					{
						style = styleItem;
						mProject.Styles.Add(style);
					}
					style.StyleValue = styleItem.StyleValue;
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* mnuFileImportMappedColumnNames_Click																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// The File / Import / Mapped Column Names from JSON menu option has been
		/// clicked.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Standard event arguments.
		/// </param>
		private void mnuFileImportMappedColumnNames_Click(object sender,
			EventArgs e)
		{
			string content = "";
			string filename = OpenMappedNamesDialog();
			VariableEntryItem map = null;
			VariableEntryCollection maps = null;

			if(filename?.Length > 0)
			{
				content = File.ReadAllText(filename);
				maps =
					JsonConvert.DeserializeObject<VariableEntryCollection>(content);
				foreach(VariableEntryItem mapItem in maps)
				{
					map = mProject.MappedColumns.FirstOrDefault(x =>
						x.Name == mapItem.Name);
					if(map == null)
					{
						map = mapItem;
						mProject.MappedColumns.Add(map);
					}
					map.Value = mapItem.Value;
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* mnuFileImportVariablePackage_Click																		*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// The File / Import / Variable Package File menu option has been clicked.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Standard event arguments.
		/// </param>
		private void mnuFileImportVariablePackage_Click(object sender, EventArgs e)
		{
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* mnuFileImportVariables_Click																					*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// The File / Import / Variables from JSON menu option has been clicked.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Standard event arguments.
		/// </param>
		private void mnuFileImportVariables_Click(object sender, EventArgs e)
		{
			string content = "";
			string filename = OpenVariablesDialog();
			VariableEntryItem variable = null;
			VariableEntryCollection variables = null;

			if(filename?.Length > 0)
			{
				content = File.ReadAllText(filename);
				variables =
					JsonConvert.DeserializeObject<VariableEntryCollection>(content);
				foreach(VariableEntryItem varItem in variables)
				{
					variable = mProject.Variables.FirstOrDefault(x =>
						x.Name == varItem.Name);
					if(variable == null)
					{
						variable = varItem;
						mProject.Variables.Add(variable);
					}
					variable.Value = varItem.Value;
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* mnuFileOpen_Click																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// The File / Open menu option has been clicked.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Standard event arguments.
		/// </param>
		private async void mnuFileOpen_Click(object sender, EventArgs e)
		{
			await OpenFile();
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* mnuFileOpenProject_Click																							*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// The File / Open Project menu option has been clicked.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Standard event arguments.
		/// </param>
		private async void mnuFileOpenProject_Click(object sender, EventArgs e)
		{
			await OpenProject();
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* mnuFileSave_Click																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// The File / Save menu option has been clicked.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Standard event arguments.
		/// </param>
		private async void mnuFileSave_Click(object sender, EventArgs e)
		{
			await SaveFile();
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* mnuFileSaveAs_Click																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// The File / Save As menu option has been clicked.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Standard event arguments.
		/// </param>
		private async void mnuFileSaveAs_Click(object sender, EventArgs e)
		{
			await SaveFileAs();
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* mnuFileSaveProject_Click																							*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// The File / Save Project menu option has been clicked.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Standard event arguments.
		/// </param>
		private async void mnuFileSaveProject_Click(object sender, EventArgs e)
		{
			await SaveProject();
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* mnuFileSaveProjectAs_Click																						*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// The File / Save Project As menu option has been clicked.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Standard event arguments.
		/// </param>
		private async void mnuFileSaveProjectAs_Click(object sender, EventArgs e)
		{
			await SaveProjectAs();
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* mnuToolsApplyColumnMappingValues_Click																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// The Tools / Apply Column Mapping Values menu option has been clicked.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Standard event arguments.
		/// </param>
		private async void mnuToolsApplyColumnMappingValues_Click(object sender,
			EventArgs e)
		{
			string content = "";

			if(QuestionDialog("This will permanently convert your " +
				"heading abbreviations to their mapped equivalents. Do you " +
				"wish to continue?", "Apply Column Mapping Values",
				MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				content = await GetWebViewMarkdown();
				content = ApplyColumnMappingValues(content, mProject.MappedColumns);
				SetWebViewMarkdown(content);
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* mnuToolsApplyDocumentStyles_Click																			*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// The Tools / Apply Document Styles menu option has been clicked.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Standard event arguments.
		/// </param>
		private async void mnuToolsApplyDocumentStyles_Click(object sender,
			EventArgs e)
		{
			string content = "";
			string newContent = "";

			if(QuestionDialog("This will permanently apply styles to elements of " +
				"your source document. Are you sure you wish to continue?",
				"Apply Document Styles", MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				content = await GetWebViewMarkdown();
				if(content.Length > 0)
				{
					newContent = ApplyDocumentStyles(content, mProject.Styles);
					if(newContent != content)
					{
						SetWebViewMarkdown(newContent);
					}
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* mnuToolsConvertToTableStyle_Click																			*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// The Tools / Convert code to Table Style menu option has been clicked.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Standard event arguments.
		/// </param>
		private async void mnuToolsConvertToTableStyle_Click(object sender,
			EventArgs e)
		{
			string markdown = await GetWebViewMarkdown();
			string result = "";

			if(QuestionDialog("This will permanently convert your linear layout " +
				"to table style. Do you wish to continue?",
				"Convert Linear To Table Style", MessageBoxButtons.YesNo) ==
				DialogResult.Yes)
			{
				result = ConvertLinearToTableStyle(markdown, true);
				if(result.Length > 0)
				{
					SetWebViewMarkdown(result);
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* mnuToolsResolveUserVariables_Click																		*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// The Tools / Resolve User Variables menu option has been clicked.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Standard event arguments.
		/// </param>
		private async void mnuToolsResolveUserVariables_Click(object sender,
			EventArgs e)
		{
			string markdown = "";

			if(QuestionDialog("This will permanently replace variable references " +
				"with their current resolved definitions. Are you sure you want to " +
				"do this?", "Resolve User Variables",
				MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				markdown = await GetWebViewMarkdown();
				markdown = ApplyUserVariables(markdown, mProject.Variables);
				SetWebViewMarkdown(markdown);
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* mProject_ProjectPackageChanged																				*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Something has changed in the project package.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Standard event arguments.
		/// </param>
		private void mProject_ProjectPackageChanged(object sender, EventArgs e)
		{
			SetProjectContentChanged(true);
			UpdateMenuControls();
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* mQueueTimer_Tick																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// The queue timer has elapsed.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Standard event arguments.
		/// </param>
		private async void mQueueTimer_Tick(object sender, EventArgs e)
		{
			bool bCancel = false;
			DialogResult result = DialogResult.None;

			if(!mQueueTickBusy)
			{
				mQueueTickBusy = true;
				if(mRunningState == RunningStateEnum.Preclose)
				{
					//	TODO: BUG - Make sure we get async web content before closing.
					if(mProject.ProjectChanged)
					{
						result = await SaveProjectChangesDialog("Exit Markdown Editor");
					}
					else
					{
						result = await SaveFileChangesDialog("Exit Markdown Editor");
					}

					if(result == DialogResult.Cancel)
					{
						bCancel = true;
					}
					if(bCancel)
					{
						mRunningState = RunningStateEnum.Running;
					}
					else
					{
						mRunningState = RunningStateEnum.CloseWhenReady;
						this.Close();
					}
				}
				//	If the form is not closing, then run any outstanding queued
				//	actions here.
				mQueueTickBusy = false;
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* OpenFileBase																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Base method to open the specified file.
		/// </summary>
		/// <param name="filename">
		/// Fully qualified path and filename to open.
		/// </param>
		private void OpenFileBase(string filename)
		{
			string content = "";

			if(filename?.Length > 0 && File.Exists(filename))
			{
				if(mProject.ProjectMode != ProjectModeEnum.MarkdownProject)
				{
					//	When the project is not active, open file always determines
					//	working path.
					mProject.WorkingPath = Path.GetDirectoryName(filename);
					mProject.ProjectMode = ProjectModeEnum.MarkdownFile;
				}
				mProject.MarkdownFilename =
					GetRelativeFilename(mProject.WorkingPath, filename);
				content = File.ReadAllText(filename);
				SetWebViewBasePath(mProject.WorkingPath);
				SetWebViewContentChangeEnabled(false);
				SetWebViewUserVariables(mProject);
				SetWebViewMarkdown(content);
				SetFileContentChanged(false);
				SetWebViewContentChangeEnabled(true);
				UpdateMenuControls();
				UpdateTitle();
				SetStatusMessage("File opened...");
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* OpenFile																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Open a file.
		/// </summary>
		/// <returns>
		/// Reference to an awaitable task.
		/// </returns>
		private async Task OpenFile()
		{
			string filename = "";
			DialogResult result = DialogResult.None;

			if(mProject.ProjectMode == ProjectModeEnum.MarkdownProject &&
				mProject.MarkdownFilename?.Length > 0)
			{
				//	A filename has already been specified for this project.
				result = QuestionDialog("The file " +
					$"{Path.GetFileName(mProject.MarkdownFilename)} has already been " +
					"assigned to this project. Do you want to change the source file?",
					"Change Project Source File", MessageBoxButtons.YesNo);
			}
			if(result != DialogResult.No)
			{
				result = await SaveFileChangesDialog();
				if(result != DialogResult.Cancel)
				{
					filename = OpenMarkdownDialog();
					if(filename.Length > 0)
					{
						OpenFileBase(filename);
					}
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* OpenProject																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Open a project.
		/// </summary>
		/// <returns>
		/// Reference to an awaitable task.
		/// </returns>
		private async Task OpenProject()
		{
			//string content = "";
			//FileInfo fileInfo = null;
			string filename = "";
			DialogResult result = DialogResult.None;

			if(mProject.ProjectMode == ProjectModeEnum.MarkdownProject)
			{
				//	If a project was previously open, then prompt to save it.
				result = await SaveProjectChangesDialog("Open Project");
			}
			else if(mProject.MarkdownFilename?.Length > 0)
			{
				//	No project was open but an individual file was.
				result = await SaveFileChangesDialog("Open Project");
			}
			if(result != DialogResult.Cancel)
			{
				filename = OpenMarkdownProjectDialog();
				if(filename.Length > 0)
				{
					OpenProjectBase(filename);
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* OpenProjectBase																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Open the base elements of the project.
		/// </summary>
		/// <param name="filename">
		/// The filename of the Markdown project to open.
		/// </param>
		private void OpenProjectBase(string filename)
		{
			string content = "";
			FileInfo fileInfo = null;

			if(filename?.Length > 0 && Path.Exists(filename))
			{
				SetWebViewContentChangeEnabled(false);
				SetWebViewMarkdown("");
				mProjectFilename = filename;
				content = File.ReadAllText(mProjectFilename);
				mProject =
					JsonConvert.DeserializeObject<ProjectPackageItem>(content);

				//mProject.MarkdownFilename =
				//	FixFilename(mProject.WorkingPath, mProject.MarkdownFilename);
				//mProject.DocxFilename =
				//	FixFilename(mProject.WorkingPath, mProject.DocxFilename);
				//mProject.DocxTemplateFilename =
				//	FixFilename(mProject.WorkingPath, mProject.DocxTemplateFilename);
				//mProject.HtmlFilename =
				//	FixFilename(mProject.WorkingPath, mProject.HtmlFilename);

				mProject.WorkingPath = Path.GetDirectoryName(filename);
				SetWebViewBasePath(mProject.WorkingPath);
				mProject.ProjectPackageChanged += mProject_ProjectPackageChanged;
				mProject.ProjectMode = ProjectModeEnum.MarkdownProject;
				SetWebViewUserVariables(mProject);
				if(mProject.MarkdownFilename.Length > 0)
				{
					fileInfo = new FileInfo(
						GetFullFilename(mProject.WorkingPath,
							mProject.MarkdownFilename));
					if(fileInfo.Exists)
					{
						//	The specified file exists.
						content = File.ReadAllText(fileInfo.FullName);
						SetWebViewMarkdown(content);
					}
				}
				SetProjectContentChanged(false);
				SetWebViewContentChangeEnabled(true);
				UpdateMenuControls();
				UpdateTitle();
				SetStatusMessage("Project opened...");
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ProcessMarkdown																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Process the user's markdown in preparation for formatted rendering.
		/// </summary>
		/// <param name="markdown">
		/// The markdown source to process.
		/// </param>
		/// <param name="project">
		/// Reference to the current project.
		/// </param>
		/// <returns>
		/// Version of the caller's markdown that is ready for rendering to HTML
		/// or Word.
		/// </returns>
		private string ProcessMarkdown(string markdown,
			ProjectPackageItem project)
		{
			string response = "";
			string result = "";

			if(markdown?.Length > 0)
			{
				result = markdown;
				if(project.OptionConvertToTable)
				{
					response = ConvertLinearToTableStyle(markdown, false);
					if(response.Length > 0)
					{
						result = response;
					}
				}
				result = ApplyColumnMappingValues(result, project.MappedColumns);
				result = ApplyDocumentStyles(result, project.Styles);
				result = ApplyUserVariables(result, project.Variables);
				result = VerticalSpaceInlineHtml(result);
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* projectOptionDialog_FormClosing																				*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// The project option dialog is closing.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Form closing event arguments.
		/// </param>
		private void projectOptionDialog_FormClosing(object sender,
			FormClosingEventArgs e)
		{
			string content = "";
			string fullFilename = "";

			if(sender is frmProjectOptions dialog &&
				dialog.DialogResult == DialogResult.OK)
			{
				//	Save the changes.
				if(dialog.MarkdownFilename != mProject.MarkdownFilename)
				{
					//	The markdown filename has been changed.
					fullFilename =
						GetFullFilename(mProject.WorkingPath, dialog.MarkdownFilename);
					if(Path.Exists(fullFilename))
					{
						//	The specified file exists.
						content = File.ReadAllText(fullFilename);
					}
					else
					{
						//	The specified file doesn't exist. Attempt to create it.
						content = "\r\n";
						try
						{
							File.WriteAllText(fullFilename, content);
						}
						catch
						{
							SetStatusMessage("Could not switch markdown filename.");
							fullFilename = "";
						}
					}
					if(fullFilename?.Length > 0 && Path.Exists(fullFilename))
					{
						//	The file can be opened.
						SetWebViewContentChangeEnabled(false);
						SetWebViewMarkdown(content);
						mProject.MarkdownFilename = dialog.MarkdownFilename;
						SetWebViewContentChangeEnabled(true);
						SetStatusMessage("Markdown file has been switched...");
					}
				}
				if(dialog.DocxFilename != mProject.DocxFilename)
				{
					mProject.DocxFilename = dialog.DocxFilename;
				}
				if(dialog.DocxTemplateFilename != mProject.DocxTemplateFilename)
				{
					mProject.DocxTemplateFilename = dialog.DocxTemplateFilename;
				}
				if(dialog.HtmlFilename != mProject.HtmlFilename)
				{
					mProject.HtmlFilename = dialog.HtmlFilename;
				}
				if(dialog.OptionConvertToTable != mProject.OptionConvertToTable)
				{
					mProject.OptionConvertToTable = dialog.OptionConvertToTable;
				}
				VariableEntryCollection.ApplyChanges(
					dialog.MappedColumns, mProject.MappedColumns);
				VariableStyleCollection.ApplyChanges(
					dialog.Styles, mProject.Styles);
				VariableEntryCollection.ApplyChanges(
					dialog.Variables, mProject.Variables);
				SetWebViewUserVariables(mProject);
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* SaveFile																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Save the current file.
		/// </summary>
		/// <returns>
		/// Reference to an awaitable running task.
		/// </returns>
		private async Task SaveFile()
		{
			string content = "";

			if(mProject.MarkdownFilename?.Length > 0)
			{
				content = await GetWebViewMarkdown();
				File.WriteAllText(
					GetFullFilename(mProject.WorkingPath,
						mProject.MarkdownFilename), content);
				SetFileContentChanged(false);
				UpdateTitle();
				SetStatusMessage("File has been saved...");
			}
			else
			{
				await SaveFileAs();
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* SaveFileAs																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Save the loaded content as a new file.
		/// </summary>
		/// <returns>
		/// Reference to an awaitable running task.
		/// </returns>
		private async Task SaveFileAs()
		{
			string content = "";
			string filename = SaveMarkdownDialog();

			if(filename.Length > 0)
			{
				//	A file was selected.
				content = await GetWebViewMarkdown();
				File.WriteAllText(filename, content);
				mProject.MarkdownFilename =
					GetRelativeFilename(mProject.WorkingPath, filename);
				SetFileContentChanged(false);
				if(mProject.ProjectMode == ProjectModeEnum.None)
				{
					mProject.ProjectMode = ProjectModeEnum.MarkdownFile;
				}
				UpdateMenuControls();
				UpdateTitle();
				SetStatusMessage("File has been saved...");
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* SaveFileChangesDialog																									*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// If changes have been made to the Markdown file, present the user with a
		/// dialog about whether to save changes, then save those changes if
		/// prompted, returning the final status of the choice to the caller.
		/// </summary>
		/// <param name="titleText">
		/// Optional title of the prompt dialog.
		/// </param>
		/// <returns>
		/// One of the following values.
		/// <list type="bullet">
		/// <item>None. No changes were present.</item>
		/// <item>Yes. Changes were saved.</item>
		/// <item>No. Changes were no saved.</item>
		/// <item>Cancel. The user wishes to cancel the current operation.</item>
		/// </list>
		/// </returns>
		private async Task<DialogResult> SaveFileChangesDialog(
			string titleText = "")
		{
			DialogResult result = DialogResult.None;
			string title = "";

			if(mProject.MarkdownChanged)
			{
				title = (titleText?.Length > 0 ?
					titleText : "Markdown Has Changed");
				result = QuestionDialog("Do you wish to save your changes?",
					title, MessageBoxButtons.YesNoCancel);
				switch(result)
				{
					case DialogResult.Yes:
						//if(mFilename.Length > 0)
						if(mProject.MarkdownFilename.Length > 0)
						{
							//	A file is open.
							await SaveFile();
						}
						else
						{
							//	This is a new file.
							await SaveFileAs();
						}
						break;
					case DialogResult.No:
						//	Leave without saving the file.
						break;
					case DialogResult.Cancel:
						//	Cancel form closure.
						break;
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* SaveProject																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Save the current project.
		/// </summary>
		/// <returns>
		/// Reference to an awaitable running task.
		/// </returns>
		private async Task SaveProject()
		{
			string content = "";

			if(mProjectFilename?.Length > 0)
			{
				//	Save the project portion of the data.
				content = JsonConvert.SerializeObject(mProject);
				File.WriteAllText(mProjectFilename, content);
				content = await GetWebViewMarkdown();
				if(content.Length > 0)
				{
					//	Markdown is present.
					await SaveFile();
				}
				SetStatusMessage("Project has been saved...");
				SetProjectContentChanged(false);
				UpdateTitle();
			}
			else
			{
				await SaveProjectAs();
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* SaveProjectAs																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Save the project content as a new file.
		/// </summary>
		/// <returns>
		/// Reference to an awaitable running task.
		/// </returns>
		private async Task SaveProjectAs()
		{
			string content = "";
			string filename = SaveMarkdownProjectDialog();

			if(filename.Length > 0)
			{
				//	A file was selected.
				await SaveFile();
				mProjectFilename = filename;
				mProject.WorkingPath = Path.GetDirectoryName(filename);
				if(mProject.DocxFilename.Length > 0)
				{
					mProject.DocxFilename =
						GetRelativeFilename(mProject.WorkingPath, mProject.DocxFilename);
				}
				if(mProject.DocxTemplateFilename.Length > 0)
				{
					mProject.DocxTemplateFilename =
						GetRelativeFilename(mProject.WorkingPath,
							mProject.DocxTemplateFilename);
				}
				if(mProject.HtmlFilename.Length > 0)
				{
					mProject.HtmlFilename =
						GetRelativeFilename(mProject.WorkingPath, mProject.HtmlFilename);
				}
				if(mProject.MarkdownFilename.Length > 0)
				{
					mProject.MarkdownFilename =
						GetRelativeFilename(mProject.WorkingPath,
							mProject.MarkdownFilename);
				}
				content = JsonConvert.SerializeObject(mProject);
				File.WriteAllText(filename, content);
				mProject.ProjectMode = ProjectModeEnum.MarkdownProject;
				SetProjectContentChanged(false);
				UpdateTitle();
				SetStatusMessage("Project has been saved...");
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* SaveProjectChangesDialog																							*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// If changes have been made to the Markdown project, present the user
		/// with a dialog about whether to save changes, then save those changes if
		/// prompted, returning the final status of the choice to the caller.
		/// </summary>
		/// <param name="titleText">
		/// Optional title of the prompt dialog.
		/// </param>
		/// <returns>
		/// One of the following values.
		/// <list type="bullet">
		/// <item>None. No changes were present.</item>
		/// <item>Yes. Changes were saved.</item>
		/// <item>No. Changes were no saved.</item>
		/// <item>Cancel. The user wishes to cancel the current operation.</item>
		/// </list>
		/// </returns>
		private async Task<DialogResult> SaveProjectChangesDialog(
			string titleText = "")
		{
			DialogResult result = DialogResult.None;
			string title = "";

			if(mProject.ProjectChanged)
			{
				title = (titleText?.Length > 0 ?
					titleText : "Project Has Changed");
				result = QuestionDialog("Do you wish to save your changes?",
					title, MessageBoxButtons.YesNoCancel);
				switch(result)
				{
					case DialogResult.Yes:
						//if(mFilename.Length > 0)
						if(mProjectFilename.Length > 0)
						{
							//	A project file is open.
							await SaveProject();
						}
						else
						{
							//	This is a new project file.
							await SaveProjectAs();
						}
						break;
					case DialogResult.No:
						//	Leave without saving the file.
						break;
					case DialogResult.Cancel:
						//	Cancel form closure.
						break;
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* SetFileContentChanged																									*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Set the changed status of the content to changed or unchanged.
		/// </summary>
		/// <param name="changed">
		/// Value indicating whether or not the content has changed.
		/// </param>
		private void SetFileContentChanged(bool changed)
		{
			mProject.MarkdownChanged = changed;
			//mContentChanged = changed;
			mnuFileSave.Enabled = changed;
			UpdateTitle();
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* SetProjectContentChanged																							*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Set the changed status of the project content to changed or unchanged.
		/// </summary>
		/// <param name="changed">
		/// Value indicating whether or not the content has changed.
		/// </param>
		private void SetProjectContentChanged(bool changed)
		{
			mProject.ProjectChanged = changed;
			//mContentChanged = changed;
			mnuFileSaveProject.Enabled = changed;
			if(changed)
			{
				UpdateTitle();
			}
			else
			{
				SetFileContentChanged(false);
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* SetStatusMessage																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Set the status message on this form.
		/// </summary>
		/// <param name="statusMessage">
		/// Status message text.
		/// </param>
		private void SetStatusMessage(string statusMessage)
		{
			if(statusStripMain.InvokeRequired)
			{
				Action safeWrite = delegate { SetStatusMessage(statusMessage); };
				statusStripMain.Invoke(safeWrite);
			}
			else
			{
				if(statusMessage != null)
				{
					statMessage.Text = statusMessage;
				}
				else
				{
					statMessage.Text = "";
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* SetWebViewBasePath																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Set the base path for media and external files on the webview control.
		/// </summary>
		/// <param name="path">
		/// Pathname to set.
		/// </param>
		private async void SetWebViewBasePath(string path)
		{
			if(mInitialized && mDomReady && mActivated)
			{
				_ = await webView21.ExecuteScriptAsync(
					$"setBasePath(`file:///{path.Replace(@"\", "/")}`);");
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* SetWebViewCursorInfo																									*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Set the current cursor on the markdown editor.
		/// </summary>
		/// <param name="cursorInfo">
		/// Collection of start and end cursor information items.
		/// </param>
		private async void SetWebViewCursorInfo(CursorInfoCollection cursorInfo)
		{
			string content = "";
			if(mInitialized && mDomReady && mActivated &&
				cursorInfo?.Count > 0)
			{
				content = JsonConvert.SerializeObject(cursorInfo);
				_ = await webView21.ExecuteScriptAsync(
					$"setCursorInfo(`{content}`);");
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* SetWebViewContentChangeEnabled																				*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Set or reset the content change event on the browser.
		/// </summary>
		/// <param name="enabled">
		/// Value indicating whether or not the browser event will be enabled.
		/// </param>
		private async void SetWebViewContentChangeEnabled(bool enabled)
		{
			if(mInitialized && mDomReady && mActivated)
			{
				_ = await webView21.ExecuteScriptAsync(
					$"setContentChangeEnabled(`{(enabled ? "true" : "false")}`);");
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* SetWebViewMarkdown																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Set the markdown content of the editor on the webview control.
		/// </summary>
		/// <param name="content">
		/// Content to set.
		/// </param>
		private async void SetWebViewMarkdown(string content)
		{
			if(mInitialized && mDomReady && mActivated)
			{
				_ = await webView21.ExecuteScriptAsync(
					$"setMarkdown(`{content.Replace(@"\", @"\\")}`);");
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* SetWebViewUserVariables																								*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Set the content of the user variables collection on the webview
		/// control.
		/// </summary>
		/// <param name="projectPackage">
		/// Collection of user variables to activate.
		/// </param>
		private async void SetWebViewUserVariables(
			ProjectPackageItem projectPackage)
		{
			string content = "";
			List<VariableEntryItem> variables = new List<VariableEntryItem>();


			if(projectPackage != null && mInitialized && mDomReady && mActivated)
			{
				VariableEntryCollection.Clone(projectPackage.Variables, variables);
				content = JsonConvert.SerializeObject(variables);
				_ = await webView21.ExecuteScriptAsync(
					$"setUserVariables(`{content}`);");
				variables.Clear();
				VariableEntryCollection.Clone(projectPackage.MappedColumns, variables);
				content = JsonConvert.SerializeObject(variables);
				_ = await webView21.ExecuteScriptAsync(
					$"setColumnMaps(`{content}`);");
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* SetWebViewUserVariablesEmpty																					*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Clear the user variables in the markdown editor on the webview control.
		/// </summary>
		private async void SetWebViewUserVariablesEmpty()
		{
			if(mInitialized && mDomReady && mActivated)
			{
				_ = await webView21.ExecuteScriptAsync(
					$"clearUserVariables();");
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* UpdateMenuControls																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Update the menu option enabled and visible states with respect to
		/// various application states.
		/// </summary>
		private void UpdateMenuControls()
		{
			//	In this version, when a project is active, the Markdown file
			//	operations only apply to setting or replacing the file within the
			//	project. When a project is not active, all file operations are
			//	normal.
			//	TODO: Update menus for application states.
			mnuToolsApplyColumnMappingValues.Enabled =
				mProject.MappedColumns.Count > 0;
			mnuToolsApplyDocumentStyles.Enabled =
				mProject.Styles.Count > 0;
			mnuToolsResolveUserVariables.Enabled =
				mProject.Variables.Count > 0;

			switch(mProject.ProjectMode)
			{
				case ProjectModeEnum.MarkdownFile:
					mnuFileCloseProject.Enabled = false;
					mnuFileClose.Enabled =
						mnuFileSave.Enabled = mProject.MarkdownFilename?.Length > 0;
					mnuFileSaveProject.Enabled = false;
					break;
				case ProjectModeEnum.MarkdownProject:
					mnuFileClose.Enabled = false;
					mnuFileSave.Enabled =
						mProject.MarkdownFilename?.Length > 0 &&
						mProject.MarkdownChanged;
					mnuFileSaveProject.Enabled = mProject.ProjectChanged;
					mnuFileCloseProject.Enabled = true;
					break;
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* UpdateTitle																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Update the form title.
		/// </summary>
		private void UpdateTitle()
		{
			StringBuilder builder = new StringBuilder();

			builder.Append("Markdown Editor");
			if(mProject.ProjectMode == ProjectModeEnum.MarkdownProject &&
				mProjectFilename?.Length > 0)
			{
				builder.Append($" - {Path.GetFileName(mProjectFilename)}");
			}
			else if(mProject.ProjectMode == ProjectModeEnum.MarkdownFile &&
				mProject.MarkdownFilename?.Length > 0)
			{
				builder.Append($" - {Path.GetFileName(mProject.MarkdownFilename)}");
			}
			if(mProject.ProjectChanged)
			{
				builder.Append("*");
			}
			this.Text = builder.ToString();
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* VerticalSpaceInlineHtml																								*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a version of the caller's markdown where general markdown text
		/// lines have been spaced away from HTML lines by at least one empty
		/// space.
		/// </summary>
		/// <param name="markdown">
		/// Source markdown to scan for markdown lines butted against HTML lines.
		/// </param>
		/// <returns>
		/// A version of caller's source where the markdown and HTML lines are
		/// all separated by at least one blank line.
		/// </returns>
		private static string VerticalSpaceInlineHtml(string markdown)
		{
			StringBuilder builder = new StringBuilder();
			List<string> lines = null;
			Match match = null;
			//	State:
			//	0 - Blank line.
			//	1 - Markdown.
			//	2 - Html.
			int state = 0;
			int statePrev = 0;

			if(markdown?.Length > 0)
			{
				lines = GetLines(markdown);
				foreach(string lineItem in lines)
				{
					if(lineItem.Length > 0)
					{
						match = Regex.Match(lineItem, ResourceMain.rxMarkdownHtmlLine);
						if(match.Success)
						{
							//	This is an HTML line.
							state = 2;
						}
						else
						{
							//	This is a normal markdown line.
							state = 1;
						}
						if((state == 1 && statePrev == 2) ||
							(state == 2 && statePrev == 1))
						{
							//	Markdown and HTML are intersecting.
							builder.AppendLine("");
						}
						builder.AppendLine(lineItem);
					}
					else
					{
						//	Blank line.
						state = 0;
						builder.AppendLine("");
					}
					statePrev = state;
				}
			}
			return builder.ToString();
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* webView21_CoreWebView2InitializationCompleted													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// The WebView control has been initialized.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Core WebView2 initialization completed event arguments.
		/// </param>
		private void webView21_CoreWebView2InitializationCompleted(
			object sender, Microsoft.Web.WebView2.Core.
			CoreWebView2InitializationCompletedEventArgs e)
		{
			string filename = "";

			mInitialized = true;
			webView21.CoreWebView2.DOMContentLoaded += webView21_DOMContentLoaded;
			filename =
				Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
				@"Components\Index.html");
			filename = filename.Replace('\\', '/');

			webView21.CoreWebView2.Navigate($"file:///{filename}");
			Trace.WriteLine("WebView Initialized...");
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* webView21_DOMContentLoaded																						*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// The DOM has loaded for the current page and is ready to display.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Core WebView2 DOM content loaded event arguments.
		/// </param>
		private void webView21_DOMContentLoaded(object sender,
			Microsoft.Web.WebView2.Core.CoreWebView2DOMContentLoadedEventArgs e)
		{
			mDomReady = true;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* webView21_SourceChanged																								*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// The Source property has changed on the WebView control.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Core WebView2 source changed event arguments.
		/// </param>
		private void webView21_SourceChanged(object sender,
			Microsoft.Web.WebView2.Core.CoreWebView2SourceChangedEventArgs e)
		{
			mDomReady = false;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* webView21_WebMessageReceived																					*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// A web message has been received from the WebView control.
		/// </summary>
		/// <param name="sender">
		/// The object raising this event.
		/// </param>
		/// <param name="e">
		/// Core WebView web message received event arguments.
		/// </param>
		private async void webView21_WebMessageReceived(object sender,
			Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs e)
		{
			MessageTypeKeyDown keyDownMessage = null;
			string messageType = "";
			string text = e.WebMessageAsJson;

			if(text.Length > 0)
			{
				text = Regex.Unescape(text);
				if((text.StartsWith('\"') && text.EndsWith('\"')) ||
					(text.StartsWith("'") && text.EndsWith("'")))
				{
					if(text.Length > 2)
					{
						text = text.Substring(1, text.Length - 2);
					}
					else
					{
						text = "";
					}
				}
				if(text.Length > 0)
				{
					//	Object is present.
					messageType = GetValue(text,
						"(?i:\\\"messagetype\\\"\\:\\s*\\\"(?<messageType>[^\\\"]+)\\\")",
						"messageType");
					switch(messageType)
					{
						case "CodeChange":
							SetFileContentChanged(true);
							break;
						case "KeyDown":
							try
							{
								keyDownMessage =
									JsonConvert.DeserializeObject<MessageTypeKeyDown>(text);
								if(keyDownMessage.CtrlKey &&
									!keyDownMessage.AltKey &&
									!keyDownMessage.ShiftKey)
								{
									//	CTRL.
									switch(keyDownMessage.Key)
									{
										case "o":
											if(mnuFileOpen.Enabled)
											{
												await OpenFile();
												//mnuFileOpen.PerformClick();
											}
											break;
										case "s":
											if(mnuFileSave.Enabled)
											{
												await SaveFile();
												//mnuFileSave.PerformClick();
											}
											break;
									}
								}
								if(keyDownMessage.AltKey &&
									!keyDownMessage.CtrlKey &&
									!keyDownMessage.ShiftKey)
								{
									//	ALT
									Trace.WriteLine(keyDownMessage.Key);
									//	TODO: Add setKeysActive(bool) to ignore keys in editor.
									//	TODO: Create post-alt keystroke series to navigate menu.
									switch(keyDownMessage.Key)
									{
										//case "Alt":
										//	this.Focus();
										//	this.BeginInvoke(() => SendKeys.SendWait("%"));
										//	break;
										case "f":
											this.Focus();
											mnuFile.ShowDropDown();
											mnuFile.Select();
											break;
										case "F10":
											//	When the menu tracing functionality is implemented,
											//	make sure this entry is self-closing upon use.
											if(mnuEditProjectOptions.Enabled)
											{
												mnuEditProjectOptions.PerformClick();
											}
											break;
									}
								}
								if(keyDownMessage.CtrlKey &&
									keyDownMessage.AltKey &&
									!keyDownMessage.ShiftKey)
								{
									//	CTRL + ALT.
									switch(keyDownMessage.Key)
									{
										case "c":
											if(mnuFileClose.Enabled)
											{
												await CloseFile();
												//mnuFileClose.PerformClick();
											}
											break;
									}
								}
								if(keyDownMessage.CtrlKey &&
									keyDownMessage.ShiftKey &&
									!keyDownMessage.AltKey)
								{
									//	CTRL + SHIFT.
									switch(keyDownMessage.Key)
									{
										case "c":
											if(mnuEditSectionStyleClipboard.Enabled)
											{
												mnuEditSectionStyleClipboard.PerformClick();
											}
											break;
										case "i":
											if(mnuEditSectionInsert.Enabled)
											{
												mnuEditSectionInsert.PerformClick();
											}
											break;
										case "o":
											if(mnuFileOpenProject.Enabled)
											{
												await OpenProject();
												//mnuFileOpenProject.PerformClick();
											}
											break;
										case "s":
											if(mnuFileSaveProject.Enabled)
											{
												await SaveProject();
												//mnuFileSaveProject.PerformClick();
											}
											break;
										case "v":
											if(mnuEditSectionStyleAppend.Enabled)
											{
												mnuEditSectionStyleAppend.PerformClick();
											}
											break;
									}
								}
							}
							catch(Exception ex)
							{
								Trace.WriteLine($"Error processing key down: {ex.Message} " +
									$"Source: {text}");
							}
							break;
						case "Message":
							break;
					}
				}
			}
			//if((text.StartsWith('\"') && text.EndsWith('\"')) ||
			//	(text.StartsWith("'") && text.EndsWith("'")))
			//{
			//	if(text.Length > 2)
			//	{
			//		text = text.Substring(1, text.Length - 2);
			//	}
			//	else
			//	{
			//		text = "";
			//	}
			//}
			//switch(text)
			//{
			//	case "TargetTextChange":
			//		break;
			//	case "TargetTextRegistered":
			//		Trace.WriteLine("Target text registered on client.");
			//		break;
			//	case "TargetTextSet":
			//		Trace.WriteLine("Setting target text on client...");
			//		break;
			//	default:
			//		Trace.WriteLine($"Message from client: {text}");
			//		break;
			//}
		}
		//*-----------------------------------------------------------------------*

		//*************************************************************************
		//*	Protected																															*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//* OnActivated																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Raises the Activated event when the form has been displayed and
		/// activated.
		/// </summary>
		/// <param name="e">
		/// Standard event arguments.
		/// </param>
		protected override void OnActivated(EventArgs e)
		{
			base.OnActivated(e);
			mActivated = true;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* OnClosing																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Raises the Closing event when the form is going to be closed.
		/// </summary>
		/// <param name="e">
		/// Cancel event arguments.
		/// </param>
		protected override void OnClosing(CancelEventArgs e)
		{
			if(mRunningState == RunningStateEnum.Running)
			{
				//	If the form is in running mode, then queue up the bookkeeping
				//	phase, cancelling close until later.
				mRunningState = RunningStateEnum.Preclose;
				e.Cancel = true;
			}
			base.OnClosing(e);
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* OnLoad																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Raises the Load event when the form has been loaded and is ready to
		/// display for the first time.
		/// </summary>
		/// <param name="e">
		/// Standard event arguments.
		/// </param>
		protected async override void OnLoad(EventArgs e)
		{
			//	The following code was going to be used to help with intercepting
			//	command keys for the form, but the act of PreTranslateMessage
			//	seems to be underdocumented and avoided at all costs.
			//	I will use the WebMessageReceived method, receiving key presses from
			//	the browser.
			//CoreWebView2EnvironmentOptions envOptions =
			//	new CoreWebView2EnvironmentOptions()
			//{
			//	AdditionalBrowserArguments =
			//		"--enable-features=msWebView2BrowserHitTransparent"
			//};
			//CoreWebView2Environment environment =
			//	await CoreWebView2Environment.CreateAsync(options: envOptions);
			//await webView21.EnsureCoreWebView2Async(environment);

			await webView21.EnsureCoreWebView2Async();
			base.OnLoad(e);

		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* OnShown																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Raises the Shown event when the form has been shown.
		/// </summary>
		/// <param name="e">
		/// Standard event arguments.
		/// </param>
		protected override void OnShown(EventArgs e)
		{
			base.OnShown(e);
			ToolStripManager.Renderer = new MnemonicToolStripRenderer();
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* WndProc																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Process a low-level Windows message.
		/// </summary>
		/// <param name="m">
		/// Windows message to process.
		/// </param>
		protected override void WndProc(ref Message m)
		{
			if(m.Msg == WM_UPDATEUISTATE)
			{
				m.WParam = (IntPtr)((UISF_HIDEACCEL & 0x0000FFFF) | (UIS_CLEAR << 16));
			}
			base.WndProc(ref m);
		}
		//*-----------------------------------------------------------------------*

		//*************************************************************************
		//*	Public																																*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//*	_Constructor																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Create a new instance of the frmMain Item.
		/// </summary>
		public frmMain()
		{
			InitializeComponent();

			//	TODO: Get version from application.
			statVersion.Text = "Ver: 24.0717.1547";

			LoadEditorStyles();

			mQueueTimer = new System.Windows.Forms.Timer();
			mQueueTimer.Tick += mQueueTimer_Tick;
			mQueueTimer.Interval = 500;
			mQueueTimer.Start();

			//	Edit menu.
			mnuEditCopy.Click += mnuEditCopy_Click;
			mnuEditCopy.Visible = false;
			mnuEditCut.Click += mnuEditCut_Click;
			mnuEditCut.Visible = false;
			mnuEditPaste.Click += mnuEditPaste_Click;
			mnuEditPaste.Visible = false;
			mnuEditProjectOptions.Click += mnuEditProjectOptions_Click;
			mnuEditRedo.Click += mnuEditRedo_Click;
			mnuEditRedo.Visible = false;
			mnuEditSectionInsert.Click += mnuEditSectionInsert_Click;
			mnuEditSectionStyleAppend.Click += mnuEditSectionStyleAppend_Click;
			mnuEditSectionStyleClipboard.Click += mnuEditSectionStyleClipboard_Click;
			mnuEditSep1.Visible = false;
			mnuEditSep2.Visible = false;
			mnuEditUndo.Click += mnuEditUndo_Click;
			mnuEditUndo.Visible = false;

			//	File menu.
			mnuFileClose.Click += mnuFileClose_Click;
			mnuFileCloseProject.Click += mnuFileCloseProject_Click;
			mnuFileConvertOldProjectToNew.Click +=
				mnuFileConvertOldProjectToNew_Click;
			mnuFileExit.Click += mnuFileExit_Click;
			mnuFileExportDocumentStyles.Click += mnuFileExportDocumentStyles_Click;
			mnuFileExportDocX.Click += mnuFileExportDocX_Click;
			mnuFileExportDocXTemplate.Click += mnuFileExportDocXTemplate_Click;
			mnuFileExportHtml.Click += mnuFileExportHtml_Click;
			mnuFileExportMappedColumnNames.Click +=
				mnuFileExportMappedColumnNames_Click;
			mnuFileExportText.Click += mnuFileExportText_Click;
			mnuFileExportVariablePackage.Click += mnuFileExportVariablePackage_Click;
			mnuFileExportVariables.Click += mnuFileExportVariables_Click;
			mnuFileImportDocumentStyles.Click += mnuFileImportDocumentStyles_Click;
			mnuFileImportMappedColumnNames.Click +=
				mnuFileImportMappedColumnNames_Click;
			mnuFileImportVariablePackage.Click += mnuFileImportVariablePackage_Click;
			mnuFileImportVariables.Click += mnuFileImportVariables_Click;
			mnuFileOpen.Click += mnuFileOpen_Click;
			mnuFileOpenProject.Click += mnuFileOpenProject_Click;
			mnuFileSave.Click += mnuFileSave_Click;
			mnuFileSaveProject.Click += mnuFileSaveProject_Click;
			mnuFileSaveAs.Click += mnuFileSaveAs_Click;
			mnuFileSaveProjectAs.Click += mnuFileSaveProjectAs_Click;


			//	Tools menu.
			mnuToolsApplyColumnMappingValues.Click +=
				mnuToolsApplyColumnMappingValues_Click;
			mnuToolsApplyDocumentStyles.Click += mnuToolsApplyDocumentStyles_Click;
			mnuToolsConvertToTableStyle.Click += mnuToolsConvertToTableStyle_Click;
			mnuToolsResolveUserVariables.Click += mnuToolsResolveUserVariables_Click;

			//	Project.
			mProject = new ProjectPackageItem();
			mProject.ProjectPackageChanged += mProject_ProjectPackageChanged;

			UpdateMenuControls();

			//	WebView.
			webView21.CoreWebView2InitializationCompleted +=
				webView21_CoreWebView2InitializationCompleted;
			webView21.SourceChanged += webView21_SourceChanged;
			webView21.WebMessageReceived += webView21_WebMessageReceived;

			if(!FileTypeRegister.IsRegistered(".mdproj"))
			{
				FileTypeRegister.RegisterProject(".mdproj");
			}

		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* OpenProject																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Open the specified Markdown project.
		/// </summary>
		/// <param name="filename">
		/// Path and filename of the project file to open.
		/// </param>
		/// <returns>
		/// Reference to an awaitable task.
		/// </returns>
		public async Task OpenProject(string filename)
		{
			Action safeWrite = null;

			await Task.Run(() =>
			{
				Thread.Sleep(1000);
				if(this.InvokeRequired)
				{
					safeWrite = delegate
					{
						OpenProjectBase(filename);
					};
					this.Invoke(safeWrite);
				}
				else
				{
					OpenProjectBase(filename);
				}
			});
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

}
