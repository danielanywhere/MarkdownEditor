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
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Html;
using Xceed.Document.NET;
using static MarkdownEditor.MarkdownEditorUtil;

namespace MarkdownEditor
{
	//*-------------------------------------------------------------------------*
	//*	HtmlDocxBuilderCollection																								*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of HtmlDocxBuilderItem Items.
	/// </summary>
	public class HtmlDocxBuilderCollection : List<HtmlDocxBuilderItem>
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
	//*	HtmlDocxBuilderItem																											*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Information used for building an individual document.
	/// </summary>
	public class HtmlDocxBuilderItem
	{
		//*************************************************************************
		//*	Private																																*
		//*************************************************************************
		private bool bAppendInitialized = false;

		//*-----------------------------------------------------------------------*
		//* AppendList																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Append the provided list to the active container.
		/// </summary>
		/// <param name="list">
		/// Reference to the list to append.
		/// </param>
		private void AppendList(Xceed.Document.NET.List list)
		{
			Xceed.Document.NET.Container container = null;

			if(list != null && mContainers.Count > 0)
			{
				container = mContainers.Peek().Container;
				if(container is Xceed.Document.NET.Document document)
				{
					document.InsertList(list);
				}
				else if(container is Xceed.Document.NET.Cell cell)
				{
					cell.InsertList(list);
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* AppendTable																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Append the caller's table to the active container.
		/// </summary>
		/// <param name="table">
		/// Reference to the table to be appended.
		/// </param>
		private void AppendTable(Xceed.Document.NET.Table table)
		{
			Xceed.Document.NET.Container container = null;

			if(table != null && mContainers.Count > 0)
			{
				container = mContainers.Peek().Container;
				if(container is Xceed.Document.NET.Document document)
				{
					document.InsertTable(table);
				}
				else if(container is Xceed.Document.NET.Cell cell)
				{
					cell.InsertTable(table);
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ApplyHeadingStyle																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Apply the specified heading style to the active paragraph.
		/// </summary>
		/// <param name="styleName">
		/// Name of the heading style to apply (H1 - H9).
		/// </param>
		private void ApplyHeadingStyle(string styleName)
		{
			StyleInfoItem style = null;

			if(styleName?.Length > 0)
			{
				if(mOverrideHeaderStyles)
				{
					style = mStyles.FirstOrDefault(x => x.Name == styleName);
					if(style != null)
					{
						if(style.FontColor != Color.Empty)
						{
							mParagraph.Color(style.FontColor);
						}
						if(style.FontName?.Length > 0)
						{
							mParagraph.Font(style.FontName);
						}
						if(style.FontSize > 0d)
						{
							mParagraph.FontSize(style.FontSize);
						}
					}
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* CountColumns																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the count of columns in the specified table.
		/// </summary>
		/// <param name="node">
		/// Reference to the table node for which the columns will be counted.
		/// </param>
		/// <returns>
		/// Count of columns found in the provided table.
		/// </returns>
		private int CountColumns(HtmlNodeItem node)
		{
			HtmlNodeItem childNode = null;
			List<HtmlNodeItem> nodes = null;
			int number = 0;
			int result = 0;
			string text = "";

			if(node != null && node.Nodes.Count > 0)
			{
				childNode = node.Nodes[0];
				if((childNode.NodeType == "thead" ||
					childNode.NodeType == "tbody") &&
					childNode.Nodes.Count > 0)
				{
					//	This table uses header and body sections.
					childNode = childNode.Nodes[0];
				}
				if(childNode.NodeType == "tr")
				{
					//	We have the first row.
					nodes = childNode.Nodes.FindAll(x =>
						x.NodeType == "th" || x.NodeType == "td");
					result = nodes.Count;
					foreach(HtmlNodeItem nodeItem in nodes)
					{
						text = HtmlAttributeCollection.
							GetAttributeValue(nodeItem, "colspan");
						if(text.Length > 0)
						{
							number = ToInt(text) - 1;
							if(number > 0)
							{
								result += number;
							}
						}
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* CountRows																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the count of rows in the specified table.
		/// </summary>
		/// <param name="node">
		/// Reference to the table node for which the rows will be counted.
		/// </param>
		/// <returns>
		/// Count of rows found in the provided table.
		/// </returns>
		private int CountRows(HtmlNodeItem node)
		{
			List<HtmlNodeItem> controlNodes = null;
			List<HtmlNodeItem> nodes = null;
			int number = 0;
			int result = 0;
			string text = "";
			//HtmlNodeItem workingNode = null;

			if(node != null && node.Nodes.Count > 0)
			{
				controlNodes = node.Nodes.FindAll(x =>
					x.NodeType == "thead" || x.NodeType == "tbody");
				if(controlNodes.Count > 0)
				{
					foreach(HtmlNodeItem controlNodeItem in controlNodes)
					{
						nodes = controlNodeItem.Nodes.FindAll(x => x.NodeType == "tr");
						result += nodes.Count;
					}
				}
				else
				{
					nodes = node.Nodes.FindAll(x => x.NodeType == "tr");
					result = nodes.Count;
				}
				foreach(HtmlNodeItem nodeItem in nodes)
				{
					text = HtmlAttributeCollection.
						GetAttributeValue(nodeItem, "rowspan");
					if(text.Length > 0)
					{
						number = ToInt(text) - 1;
						if(number > 0)
						{
							result += number;
						}
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetCellNodes																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a list of table cells from the caller's row.
		/// </summary>
		/// <param name="row">
		/// Reference to the table row to enumerate.
		/// </param>
		/// <returns>
		/// Reference to a list of table headers and table data cells in the
		/// current row, if found. Otherwise, an empty list.
		/// </returns>
		private List<HtmlNodeItem> GetCellNodes(HtmlNodeItem row)
		{
			List<HtmlNodeItem> result = new List<HtmlNodeItem>();

			if(row?.Nodes.Count > 0)
			{
				result = row.Nodes.FindAll(x =>
					x.NodeType == "th" ||
					x.NodeType == "td");
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* GetRowNodes																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a list of table rows from the caller's table.
		/// </summary>
		/// <param name="table">
		/// Reference to the table to enumerate.
		/// </param>
		/// <returns>
		/// Reference to a list of table row HtmlNodeItems, if found. Otherwise,
		/// an empty list.
		/// </returns>
		private List<HtmlNodeItem> GetRowNodes(HtmlNodeItem table)
		{
			List<HtmlNodeItem> result = new List<HtmlNodeItem>();

			if(table?.Nodes.Count > 0)
			{
				if(table.Nodes[0].NodeType == "thead" ||
					table.Nodes[0].NodeType == "tbody")
				{
					//	Table head and table body are in play.
					foreach(HtmlNodeItem headBodyItem in table.Nodes)
					{
						foreach(HtmlNodeItem rowItem in headBodyItem.Nodes)
						{
							if(rowItem.NodeType == "tr")
							{
								result.Add(rowItem);
							}
						}
					}
				}
				else
				{
					//	Table rows are directly implemented.
					foreach(HtmlNodeItem rowItem in table.Nodes)
					{
						if(rowItem.NodeType == "tr")
						{
							result.Add(rowItem);
						}
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* NewParagraph																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Create and return a new paragraph in the currently active container.
		/// </summary>
		/// <returns>
		/// Reference to the newly created paragraph.
		/// </returns>
		private Xceed.Document.NET.Paragraph NewParagraph()
		{
			Xceed.Document.NET.Container container = null;
			Xceed.Document.NET.Paragraph result = null;

			if(mContainers.Count > 0)
			{
				container = mContainers.Peek().Container;
				if(container is Xceed.Document.NET.Document document)
				{
					result = document.InsertParagraph();
				}
				else if(container is Xceed.Document.NET.Cell cell)
				{
					result = cell.InsertParagraph();
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ProcessStyle																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Process any styles that could be encountered at this point in the
		/// HTML structure.
		/// </summary>
		/// <param name="node">
		/// Reference to the HTML node being inspected.
		/// </param>
		/// <param name="style">
		/// Reference to the style tracker in its current state.
		/// </param>
		/// <returns>
		/// Value indicating whether the style was pushed for this instance.
		/// </returns>
		private bool ProcessStyle(HtmlNodeItem node, StyleTrackerItem style)
		{
			HtmlAttributeItem attrib = null;
			bool bNew = false;
			char[] colon = new char[] { ':' };
			string[] nameValue = null;
			string[] properties = null;
			char[] semicolon = new char[] { ';' };
			string text = "";

			if(node != null && style != null)
			{
				attrib = node.Attributes.FirstOrDefault(x => x.Name == "style");
				if(attrib != null)
				{
					//	Styles have been specified.
					//	Syntax: 'name: value; ... name: value;'
					properties = attrib.Value.Split(semicolon,
						StringSplitOptions.RemoveEmptyEntries |
						StringSplitOptions.TrimEntries);
					foreach(string propertyItem in properties)
					{
						nameValue = propertyItem.Split(colon,
							StringSplitOptions.RemoveEmptyEntries |
							StringSplitOptions.TrimEntries);
						if(nameValue.Length > 1)
						{
							switch(nameValue[0])
							{
								case "color":
									if(nameValue[1].StartsWith('#'))
									{
										//	Only hex values are accepted in this version.
										if(!bNew)
										{
											style.Push();
											bNew = true;
										}
										style.FontColor = ColorTranslator.FromHtml(nameValue[1]);
									}
									break;
								case "font-family":
									//	Font family.
									if(nameValue[1].Length > 0)
									{
										if(!bNew)
										{
											style.Push();
											bNew = true;
										}
										style.FontName = nameValue[1];
									}
									break;
								case "font-size":
									//	Font size.
									text = GetValue(nameValue[1],
										@"(?<size>[0-9-\.]+)pt",
										"size");
									if(text.Length > 0)
									{
										if(!bNew)
										{
											style.Push();
											bNew = true;
										}
										style.FontSize = ToFloat(text);
									}
									break;
								case "font-weight":
									if(!bNew)
									{
										style.Push();
										bNew = true;
									}
									if(nameValue[1] == "bold")
									{
										style.Bold = true;
									}
									else
									{
										style.Bold = false;
									}
									break;
								case "text-decoration":
									if(!bNew)
									{
										style.Push();
										bNew = true;
									}
									if(nameValue[1] == "underline")
									{
										style.Underline = true;
									}
									else
									{
										style.Underline = false;
									}
									break;
							}
						}
					}
				}
			}
			return bNew;
		}
		//*-----------------------------------------------------------------------*

		//*************************************************************************
		//*	Protected																															*
		//*************************************************************************
		//*************************************************************************
		//*	Public																																*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//* AppendHtml																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Append the contents of the provided HTML object model to the specified
		/// document.
		/// </summary>
		/// <param name="nodes">
		/// Reference to the collection of nodes to append.
		/// </param>
		public void AppendHtml(HtmlNodeCollection nodes)
		{
			HtmlAttributeItem attrib = null;
			int processStyleCount = 0;
			Xceed.Document.NET.Cell cell = null;
			HtmlNodeItem cellNode = null;
			List<HtmlNodeItem> cellNodes = null;
			int colCount = 0;
			int colIndex = 0;
			Color colorBorder = ColorTranslator.FromHtml("#7f7f7f");
			Color colorEven = ColorTranslator.FromHtml("#ccf2ff");
			Color colorLink = ColorTranslator.FromHtml("#3535ff");
			Color colorOdd = ColorTranslator.FromHtml("#e5f9ff");
			ContainerInfoItem containerInfo = null;
			//Xceed.Document.NET.Image image = null;
			//Hyperlink link = null;
			//List list = null;
			//int listLevel = -1;
			//HtmlNodeItem node = null;
			int number = 0;
			//Xceed.Document.NET.Picture picture = null;
			Xceed.Document.NET.Row row = null;
			int rowCount = 0;
			int rowIndex = 0;
			HtmlNodeItem rowNode = null;
			List<HtmlNodeItem> rowNodes = null;
			int sectionIndex = 0;
			DocumentStyleTypeEnum styleType = DocumentStyleTypeEnum.None;
			Table table = null;
			int tableCellCount = 0;
			int tableCellIndex = 0;
			int tableRowCount = 0;
			int tableRowIndex = 0;
			NamedTagItem tag = null;
			Dictionary<TableOfContentsSwitches, string> tocSettings = null;
			
			if(!bAppendInitialized)
			{
				//	Make sure that unless overridden, the font size is not changed
				//	when non-overridden style document is active.
				if(mOverrideHeaderStyles)
				{
					//	Default Word font size.
					mStyle.FontSize = 11d;
				}
				else
				{
					mStyle.FontSize = -1;
				}
				bAppendInitialized = true;
			}

			//	Set the local values from user document styles.
			if(mUserStyles?.Count > 0)
			{
				foreach(VariableStyleItem styleItem in mUserStyles)
				{
					styleType =
						Enum.Parse<DocumentStyleTypeEnum>(styleItem.StyleName, true);
					switch(styleType)
					{
						case DocumentStyleTypeEnum.SectionBackgroundColor:
							if(Regex.IsMatch(styleItem.StyleValue,
								ResourceMain.rxHtmlHexColor))
							{
								if(!mUserStyles.Exists(x =>
									Regex.IsMatch(x.StyleName,
										ResourceMain.rxDocStyleSectionBackEven)))
								{
									//	Set the even section if not specified.
									colorEven = ColorTranslator.FromHtml(styleItem.StyleValue);
								}
								if(!mUserStyles.Exists(x =>
									Regex.IsMatch(x.StyleName,
										ResourceMain.rxDocStyleSectionBackOdd)))
								{
									//	Set the odd section if not specified.
									colorOdd = ColorTranslator.FromHtml(styleItem.StyleValue);
								}
							}
							break;
						case DocumentStyleTypeEnum.SectionBackgroundColorEven:
							if(Regex.IsMatch(styleItem.StyleValue,
								ResourceMain.rxHtmlHexColor))
							{
								colorEven = ColorTranslator.FromHtml(styleItem.StyleValue);
							}
							break;
						case DocumentStyleTypeEnum.SectionBackgroundColorOdd:
							if(Regex.IsMatch(styleItem.StyleValue,
								ResourceMain.rxHtmlHexColor))
							{
								colorOdd = ColorTranslator.FromHtml(styleItem.StyleValue);
							}
							break;
					}
				}
			}

			//	Inline nodes append to the current active paragraph, if one
			//	has already been created. Otherwise, they are appended to
			//	a new paragraph in the current container.

			//	Block nodes append new paragraphs to the current active container,
			//	which could be a document or table cell, each of which inherit from
			//	Xceed.Document.Net.Container.


			if(nodes?.Count > 0)
			{
				//	Node list and document have both been specified.
				foreach(HtmlNodeItem nodeItem in nodes)
				{
					processStyleCount = (ProcessStyle(nodeItem, mStyle) ? 1 : 0);
					switch(nodeItem.NodeType)
					{
						case "a":
							if(mParagraph != null)
							{
								mHyperlink =
									mDocument.AddHyperlink(nodeItem.Text,
									new Uri(
										HtmlAttributeCollection.
											GetAttributeValue(nodeItem, "href")));
								mParagraph.AppendHyperlink(mHyperlink);
								if(nodeItem.Nodes.Count > 0)
								{
									AppendHtml(nodeItem.Nodes);
								}
								mParagraph.Color(colorLink);
							}
							break;
						case "b":
						case "strong":
							mStyle.Push();
							processStyleCount++;
							mStyle.Bold = true;
							if(mParagraph != null)
							{
								if(nodeItem.Text.Length > 0)
								{
									mParagraph.Append(nodeItem.Text);
									ApplyStyle();
								}
							}
							if(nodeItem.Nodes.Count > 0)
							{
								AppendHtml(nodeItem.Nodes);
							}
							mStyle.Pop();
							processStyleCount--;
							break;
						case "br":
							if(mParagraph != null)
							{
								mParagraph.Append("\r\n");
								if(nodeItem.Text.Length > 0)
								{
									mParagraph.Append(nodeItem.Text);
									ApplyStyle();
								}
							}
							if(nodeItem.Nodes.Count > 0)
							{
								AppendHtml(nodeItem.Nodes);
							}
							break;
						case "em":
						case "i":
							mStyle.Push();
							processStyleCount++;
							mStyle.Italic = true;
							if(mParagraph != null)
							{
								if(nodeItem.Text.Length > 0)
								{
									mParagraph.Append(nodeItem.Text);
									ApplyStyle();
								}
							}
							if(nodeItem.Nodes.Count > 0)
							{
								AppendHtml(nodeItem.Nodes);
							}
							processStyleCount--;
							mStyle.Pop();
							break;
						case "font":
							mStyle.Push();
							processStyleCount++;
							attrib = nodeItem.Attributes.FirstOrDefault(x =>
								x.Name == "color");
							if(attrib?.Value.StartsWith("#") == true)
							{
								mStyle.FontColor = ColorTranslator.FromHtml(attrib.Value);
							}
							attrib = nodeItem.Attributes.FirstOrDefault(x =>
								x.Name == "size");
							number = ToInt(attrib?.Value);
							switch(number)
							{
								case 1:
									mStyle.FontSize = 8d;
									break;
								case 2:
									mStyle.FontSize = 10d;
									break;
								case 3:
									mStyle.FontSize = 12d;
									break;
								case 4:
									mStyle.FontSize = 14d;
									break;
								case 5:
									mStyle.FontSize = 18d;
									break;
								case 6:
									mStyle.FontSize = 24d;
									break;
								case 7:
									mStyle.FontSize = 36d;
									break;
							}
							attrib = nodeItem.Attributes.FirstOrDefault(x =>
								x.Name == "face");
							if(attrib?.Value.Length > 0)
							{
								mStyle.FontName = attrib.Value;
							}
							if(mParagraph != null)
							{
								if(nodeItem.Text.Length > 0)
								{
									mParagraph.Append(nodeItem.Text);
									ApplyStyle();
								}
							}
							if(nodeItem.Nodes.Count > 0)
							{
								AppendHtml(nodeItem.Nodes);
							}
							processStyleCount--;
							mStyle.Pop();
							break;
						case "h1":
							mParagraph = NewParagraph().
								Heading(HeadingType.Heading1);
							if(!mOverrideHeaderStyles)
							{
								mStyle.FontSize = -1;
							}
							//mParagraph =
							//	mDocument.InsertParagraph().
							//		Heading(HeadingType.Heading1);
							if(nodeItem.Text.Length > 0)
							{
								mParagraph.Append(nodeItem.Text);
							}
							if(nodeItem.Nodes.Count > 0)
							{
								AppendHtml(nodeItem.Nodes);
							}
							ApplyHeadingStyle("H1");
							break;
						case "h2":
							mParagraph = NewParagraph().
								Heading(HeadingType.Heading2);
							if(!mOverrideHeaderStyles)
							{
								mStyle.FontSize = -1;
							}
							//mParagraph =
							//	mDocument.InsertParagraph().
							//		Heading(HeadingType.Heading2);
							if(nodeItem.Text.Length > 0)
							{
								mParagraph.Append(nodeItem.Text);
							}
							if(nodeItem.Nodes.Count > 0)
							{
								AppendHtml(nodeItem.Nodes);
							}
							ApplyHeadingStyle("H2");
							break;
						case "h3":
							mParagraph = NewParagraph().
								Heading(HeadingType.Heading3);
							if(!mOverrideHeaderStyles)
							{
								mStyle.FontSize = -1;
							}
							//mParagraph =
							//	mDocument.InsertParagraph().
							//		Heading(HeadingType.Heading3);
							if(nodeItem.Text.Length > 0)
							{
								mParagraph.Append(nodeItem.Text);
							}
							if(nodeItem.Nodes.Count > 0)
							{
								AppendHtml(nodeItem.Nodes);
							}
							ApplyHeadingStyle("H3");
							break;
						case "h4":
							mParagraph = NewParagraph().
								Heading(HeadingType.Heading4);
							if(!mOverrideHeaderStyles)
							{
								mStyle.FontSize = -1;
							}
							//mParagraph =
							//	mDocument.InsertParagraph().
							//		Heading(HeadingType.Heading4);
							if(nodeItem.Text.Length > 0)
							{
								mParagraph.Append(nodeItem.Text);
							}
							if(nodeItem.Nodes.Count > 0)
							{
								AppendHtml(nodeItem.Nodes);
							}
							ApplyHeadingStyle("H4");
							break;
						case "h5":
							mParagraph = NewParagraph().
								Heading(HeadingType.Heading5);
							if(!mOverrideHeaderStyles)
							{
								mStyle.FontSize = -1;
							}
							//mParagraph =
							//	mDocument.InsertParagraph().
							//		Heading(HeadingType.Heading5);
							if(nodeItem.Text.Length > 0)
							{
								mParagraph.Append(nodeItem.Text);
							}
							if(nodeItem.Nodes.Count > 0)
							{
								AppendHtml(nodeItem.Nodes);
							}
							ApplyHeadingStyle("H5");
							break;
						case "h6":
							mParagraph = NewParagraph().
								Heading(HeadingType.Heading6);
							if(!mOverrideHeaderStyles)
							{
								mStyle.FontSize = -1;
							}
							//mParagraph =
							//	mDocument.InsertParagraph().
							//		Heading(HeadingType.Heading6);
							if(nodeItem.Text.Length > 0)
							{
								mParagraph.Append(nodeItem.Text);
							}
							if(nodeItem.Nodes.Count > 0)
							{
								AppendHtml(nodeItem.Nodes);
							}
							ApplyHeadingStyle("H6");
							break;
						case "h7":
							mParagraph = NewParagraph().
								Heading(HeadingType.Heading7);
							if(!mOverrideHeaderStyles)
							{
								mStyle.FontSize = -1;
							}
							//mParagraph =
							// mDocument.InsertParagraph().
							//	Heading(HeadingType.Heading7);
							if(nodeItem.Text.Length > 0)
							{
								mParagraph.Append(nodeItem.Text);
							}
							if(nodeItem.Nodes.Count > 0)
							{
								AppendHtml(nodeItem.Nodes);
							}
							ApplyHeadingStyle("H7");
							break;
						case "h8":
							mParagraph = NewParagraph().
								Heading(HeadingType.Heading8);
							if(!mOverrideHeaderStyles)
							{
								mStyle.FontSize = -1;
							}
							//mParagraph =
							//	mDocument.InsertParagraph().
							//		Heading(HeadingType.Heading8);
							if(nodeItem.Text.Length > 0)
							{
								mParagraph.Append(nodeItem.Text);
							}
							if(nodeItem.Nodes.Count > 0)
							{
								AppendHtml(nodeItem.Nodes);
							}
							ApplyHeadingStyle("H8");
							break;
						case "h9":
							mParagraph = NewParagraph().
								Heading(HeadingType.Heading9);
							if(!mOverrideHeaderStyles)
							{
								mStyle.FontSize = -1;
							}
							//mParagraph =
							//	mDocument.InsertParagraph().
							//		Heading(HeadingType.Heading9);
							if(nodeItem.Text.Length > 0)
							{
								mParagraph.Append(nodeItem.Text);
							}
							if(nodeItem.Nodes.Count > 0)
							{
								AppendHtml(nodeItem.Nodes);
							}
							ApplyHeadingStyle("H9");
							break;
						case "img":
							mImage = mDocument.AddImage(
								Path.Join(mBasePath,
								HtmlAttributeCollection.
									GetAttributeValue(nodeItem, "src").Replace("/", @"\")));
							mPicture = mImage.CreatePicture();
							if(mParagraph != null)
							{
								mParagraph.AppendPicture(mPicture);
							}
							break;
						case "li":
							switch(mCreateListType)
							{
								case CreateListTypeEnum.Bulleted:
									mList = mDocument.AddList(listType: ListItemType.Bulleted);
									mCreateListType = CreateListTypeEnum.Active;
									break;
								case CreateListTypeEnum.Numbered:
									mList = mDocument.AddList(listType: ListItemType.Numbered);
									mCreateListType = CreateListTypeEnum.Active;
									break;
								default:
									break;
							}
							if(mList != null)
							{
								mDocument.AddListItem(mList, "", mListLevel);
								mParagraph = mList.Items[^1];
								if(nodeItem.Text.Length > 0)
								{
									mParagraph.Append(nodeItem.Text);
									ApplyStyle();
								}
								if(nodeItem.Nodes.Count > 0)
								{
									AppendHtml(nodeItem.Nodes);
								}
								mParagraph.SetLineSpacing(LineSpacingType.After, 0f);
							}
							break;
						case "ol":
							if(mCreateListType == CreateListTypeEnum.None)
							{
								mCreateListType = CreateListTypeEnum.Numbered;
							}
							mListLevel++;
							if(nodeItem.Nodes.Count > 0)
							{
								AppendHtml(nodeItem.Nodes);
							}
							AppendList(mList);
							//mDocument.InsertList(mList);
							mListLevel--;
							if(mListLevel == -1)
							{
								mCreateListType = CreateListTypeEnum.None;
							}
							break;
						case "p":
							mParagraph = NewParagraph();
							//mParagraph = mDocument.InsertParagraph();
							if(nodeItem.Text.Length > 0)
							{
								mParagraph.Append(nodeItem.Text);
								ApplyStyle();
							}
							if(nodeItem.Nodes.Count > 0)
							{
								AppendHtml(nodeItem.Nodes);
							}
							break;
						case "section":
							//	Add everything inside the section.

							//	Add a space before the first section.
							mParagraph = NewParagraph();

							table = mDocument.AddTable(1, 1);

							//	Add a border to the table cell.
							table.SetBorder(TableBorderType.InsideH,
								new Border(BorderStyle.Tcbs_single,
									BorderSize.one, 0, colorBorder));
							table.SetBorder(TableBorderType.InsideV,
								new Border(BorderStyle.Tcbs_single,
									BorderSize.one, 0, colorBorder));
							table.SetBorder(TableBorderType.Top,
								new Border(BorderStyle.Tcbs_single,
									BorderSize.one, 0, colorBorder));
							table.SetBorder(TableBorderType.Bottom,
								new Border(BorderStyle.Tcbs_single,
									BorderSize.one, 0, colorBorder));
							table.SetBorder(TableBorderType.Left,
								new Border(BorderStyle.Tcbs_single,
									BorderSize.one, 0, colorBorder));
							table.SetBorder(TableBorderType.Right,
								new Border(BorderStyle.Tcbs_single,
									BorderSize.one, 0, colorBorder));

							//	Get the cell.
							cell = table.Rows[0].Cells[0];
							//	A cell is always built with a first paragraph.
							while(cell.Paragraphs.Count > 0)
							{
								cell.RemoveParagraphAt(0);
							}

							//	Set the cell background shading.
							cell.ShadingPattern = new ShadingPattern()
							{
								Fill = (sectionIndex % 2 == 0 ? colorEven : colorOdd),
								Style = PatternStyle.Solid,
								StyleColor = (sectionIndex % 2 == 0 ? colorEven : colorOdd)
							};

							containerInfo = new ContainerInfoItem()
							{
								Container = cell
							};
							containerInfo.Tags.Add(new NamedTagItem()
							{
								Name = "Row",
								Value = table.Rows[0]
							});
							containerInfo.Tags.Add(new NamedTagItem()
							{
								Name = "Table",
								Value = table
							});
							mContainers.Push(containerInfo);

							if(nodeItem.Nodes.Count > 0)
							{
								AppendHtml(nodeItem.Nodes);
							}

							//	Insert a trailing paragraph inside the section.
							mParagraph = NewParagraph();
							mContainers.Pop();
							AppendTable(table);

							//	Insert a separating paragraph between sections.
							mParagraph = NewParagraph();
							sectionIndex++;
							break;
						case "span":
							if(nodeItem.Text.Length > 0)
							{
								mParagraph.Append(nodeItem.Text);
								ApplyStyle();
							}
							if(nodeItem.Nodes.Count > 0)
							{
								AppendHtml(nodeItem.Nodes);
							}
							break;
						case "table":
							mParagraph = null;
							colCount = CountColumns(nodeItem);
							rowCount = CountRows(nodeItem);
							table = mDocument.AddTable(rowCount, colCount);
							table.Design = TableDesign.TableGrid;
							table.Rows[0].TableHeader = true;
							rowNodes = GetRowNodes(nodeItem);
							//	TODO: colspan and rowspan on tables.
							rowCount = Math.Min(rowNodes.Count, rowCount);
							//rowCount = rowNodes.Count;
							//if(rowCount > 0)
							//{
							//	colCount = rowNodes[0].Nodes.Count;
							//}
							for(rowIndex = 0; rowIndex < rowCount; rowIndex++)
							{
								rowNode = rowNodes[rowIndex];
								cellNodes = GetCellNodes(rowNode);
								colCount = Math.Min(cellNodes.Count, colCount);
								for(colIndex = 0; colIndex < colCount; colIndex++)
								{
									tableRowCount = table.Rows.Count;
									tableRowIndex = Math.Min(rowIndex, tableRowCount - 1);
									tableCellCount = table.Rows[tableRowIndex].Cells.Count;
									tableCellIndex = Math.Min(colIndex, tableCellCount - 1);

									containerInfo = new ContainerInfoItem()
									{
										Container = table.Rows[tableRowIndex].Cells[tableCellIndex]
									};
									containerInfo.Tags.Add(new NamedTagItem()
									{
										Name = "Row",
										Value = table.Rows[tableRowIndex]
									});
									containerInfo.Tags.Add(new NamedTagItem()
									{
										Name = "Table",
										Value = table
									});
									mContainers.Push(containerInfo);

									mParagraph =
										table.Rows[tableRowIndex].Cells[tableCellIndex].
											Paragraphs[0];
									cellNode = cellNodes[colIndex];
									if(cellNode.Text.Length > 0)
									{
										mParagraph.Append(cellNode.Text);
										ApplyStyle();
									}
									if(cellNode.Nodes.Count > 0)
									{
										AppendHtml(cellNode.Nodes);
									}
									mContainers.Pop();
								}
							}
							AppendTable(table);
							//mDocument.InsertTable(mTable);
							break;
						case "u":
							mStyle.Push();
							processStyleCount++;
							mStyle.Underline = true;
							if(mParagraph != null)
							{
								if(nodeItem.Text.Length > 0)
								{
									mParagraph.Append(nodeItem.Text);
									ApplyStyle();
								}
								if(nodeItem.Nodes.Count > 0)
								{
									AppendHtml(nodeItem.Nodes);
								}
							}
							mStyle.Pop();
							processStyleCount--;
							break;
						case "ul":
							if(mCreateListType == CreateListTypeEnum.None)
							{
								mCreateListType = CreateListTypeEnum.Bulleted;
							}
							mListLevel++;
							if(nodeItem.Nodes.Count > 0)
							{
								AppendHtml(nodeItem.Nodes);
							}
							AppendList(mList);
							//mDocument.InsertList(mList);
							mListLevel--;
							if(mListLevel == -1)
							{
								mCreateListType = CreateListTypeEnum.None;
							}
							break;
						case "":
							if(nodeItem.Text.Length > 0)
							{
								mParagraph.Append(nodeItem.Text);
								ApplyStyle();
							}
							if(nodeItem.Nodes.Count > 0)
							{
								AppendHtml(nodeItem.Nodes);
							}
							break;
						case "!--":
							//	Possible document variable.
							switch(HtmlUtil.GetHtmlCommentText(nodeItem.Original))
							{
								case "FullRow":
									//	Merge all of the columns of the current table row.
									if(mContainers.Count > 0 &&
										mContainers.Peek().Container is
											Xceed.Document.NET.Cell cellContainer)
									{
										//	The current container is a cell.
										containerInfo = mContainers.Peek();
										tag = containerInfo.Tags.FirstOrDefault(x =>
											x.Name == "Row");
										if(tag?.Value != null)
										{
											row = (Xceed.Document.NET.Row)tag.Value;
											if(row.ColumnCount > 1)
											{
												row.MergeCells(0, row.ColumnCount - 1);
												//	Reset the container to the merged area.
												containerInfo.Container = row.Cells[0];
											}
										}
									}
									break;
								case "TableOfContents":
									//	Create a table of contents at the current cursor
									//	location.
									tocSettings =
										new Dictionary<TableOfContentsSwitches, string>()
									{
										{ TableOfContentsSwitches.O, "1-3" },
										{ TableOfContentsSwitches.U, "" },
										{ TableOfContentsSwitches.Z, "" },
										{ TableOfContentsSwitches.H, "" }
									};
									mDocument.InsertTableOfContents(
										"Table of Contents", tocSettings);
									mDocument.InsertParagraph().
										InsertPageBreakAfterSelf();
									mParagraph = NewParagraph();
									mFieldsWereInserted = true;
									break;
							}
							break;
						default:
							if(nodeItem.Nodes.Count > 0)
							{
								AppendHtml(nodeItem.Nodes);
							}
							break;
					}
					while(processStyleCount > 0)
					{
						mStyle.Pop();
						processStyleCount--;
					}
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ApplyStyle																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Apply the current style to the current point in the paragraph.
		/// </summary>
		public void ApplyStyle()
		{
			if(mParagraph != null && mStyle != null)
			{
				mParagraph.Bold(mStyle.Bold);
				if(mStyle.FontColor != Color.Transparent)
				{
					mParagraph.Color(mStyle.FontColor);
				}
				if(mStyle.FontName.Length > 0)
				{
					mParagraph.Font(mStyle.FontName);
				}
				if(mStyle.FontSize > 0d)
				{
					mParagraph.FontSize(mStyle.FontSize);
				}
				mParagraph.Italic(mStyle.Italic);
				if(mStyle.Underline)
				{
					mParagraph.UnderlineStyle(UnderlineStyle.singleLine);
				}
				else
				{
					mParagraph.UnderlineStyle(UnderlineStyle.none);
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	BasePath																															*
		//*-----------------------------------------------------------------------*
		private string mBasePath = "";
		/// <summary>
		/// Get/Set the base path for all relative media operations in the HTML.
		/// </summary>
		public string BasePath
		{
			get { return mBasePath; }
			set { mBasePath = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Containers																														*
		//*-----------------------------------------------------------------------*
		private Stack<ContainerInfoItem> mContainers =
			new Stack<ContainerInfoItem>();
		/// <summary>
		/// Get a reference to the stack of working active containers into
		/// which paragraph content is being placed.
		/// </summary>
		public Stack<ContainerInfoItem> Containers
		{
			get { return mContainers; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	CreateListType																												*
		//*-----------------------------------------------------------------------*
		private CreateListTypeEnum mCreateListType = CreateListTypeEnum.None;
		/// <summary>
		/// Get/Set the type of list to create when the next list entry is
		/// encountered.
		/// </summary>
		public CreateListTypeEnum CreateListType
		{
			get { return mCreateListType; }
			set { mCreateListType = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Document																															*
		//*-----------------------------------------------------------------------*
		private Xceed.Document.NET.Document mDocument = null;
		/// <summary>
		/// Get/Set a reference to the active document.
		/// </summary>
		public Xceed.Document.NET.Document Document
		{
			get { return mDocument; }
			set
			{
				mDocument = value;
				if(value != null)
				{
					if(mContainers.Count == 0)
					{
						//	Creating a new document.
						mContainers.Push(new ContainerInfoItem()
						{
							Container = mDocument
						});
					}
					else if(mContainers.Count == 1 &&
						mContainers.Peek().Container is Xceed.Document.NET.Document)
					{
						//	Using a different document.
						mContainers.Clear();
						mContainers.Push(new ContainerInfoItem()
						{
							Container = mDocument
						});
					}
				}
				else
				{
					//	Clearing the document.
					mContainers.Clear();
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	FieldsWereInserted																										*
		//*-----------------------------------------------------------------------*
		private bool mFieldsWereInserted = false;
		/// <summary>
		/// Get/Set a value indicating whether fields have been inserted.
		/// </summary>
		/// <remarks>
		/// Use this value to decide whether to update fields on save. This
		/// version of Xceed DocX doesn't yet support the UpdateFields method,
		/// but that method will be called before Save when this value is set.
		/// </remarks>
		public bool FieldsWereInserted
		{
			get { return mFieldsWereInserted; }
			set { mFieldsWereInserted = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Hyperlink																															*
		//*-----------------------------------------------------------------------*
		private Xceed.Document.NET.Hyperlink mHyperlink = null;
		/// <summary>
		/// Get/Set a reference to the active hyperlink.
		/// </summary>
		public Xceed.Document.NET.Hyperlink Hyperlink
		{
			get { return mHyperlink; }
			set { mHyperlink = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Image																																	*
		//*-----------------------------------------------------------------------*
		private Xceed.Document.NET.Image mImage = null;
		/// <summary>
		/// Get/Set a reference to the active image.
		/// </summary>
		public Xceed.Document.NET.Image Image
		{
			get { return mImage; }
			set { mImage = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	List																																	*
		//*-----------------------------------------------------------------------*
		private Xceed.Document.NET.List mList = null;
		/// <summary>
		/// Get/Set a reference to the active list for the document.
		/// </summary>
		public Xceed.Document.NET.List List
		{
			get { return mList; }
			set { mList = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ListLevel																															*
		//*-----------------------------------------------------------------------*
		private int mListLevel = -1;
		/// <summary>
		/// Get/Set the current list building level.
		/// </summary>
		public int ListLevel
		{
			get { return mListLevel; }
			set { mListLevel = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	OverrideHeaderStyles																									*
		//*-----------------------------------------------------------------------*
		private bool mOverrideHeaderStyles = false;
		/// <summary>
		/// Get/Set a value indicating whether the header styles will be
		/// overridden in this document.
		/// </summary>
		public bool OverrideHeaderStyles
		{
			get { return mOverrideHeaderStyles; }
			set { mOverrideHeaderStyles = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Paragraph																															*
		//*-----------------------------------------------------------------------*
		private Xceed.Document.NET.Paragraph mParagraph = null;
		/// <summary>
		/// Get/Set a reference to the active paragraph.
		/// </summary>
		public Xceed.Document.NET.Paragraph Paragraph
		{
			get { return mParagraph; }
			set { mParagraph = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Picture																																*
		//*-----------------------------------------------------------------------*
		private Xceed.Document.NET.Picture mPicture = null;
		/// <summary>
		/// Get/Set a reference to the active picture.
		/// </summary>
		public Xceed.Document.NET.Picture Picture
		{
			get { return mPicture; }
			set { mPicture = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Style																																	*
		//*-----------------------------------------------------------------------*
		private StyleTrackerItem mStyle = new StyleTrackerItem();
		/// <summary>
		/// Get/Set a reference to the active style.
		/// </summary>
		public StyleTrackerItem Style
		{
			get { return mStyle; }
			set { mStyle = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Styles																																*
		//*-----------------------------------------------------------------------*
		private StyleInfoCollection mStyles = new StyleInfoCollection()
		{
			new StyleInfoItem()
			{
				Name = "H1",
				FontSize = 20d
			},
			new StyleInfoItem()
			{
				Name = "H2",
				FontSize = 16d
			},
			new StyleInfoItem()
			{
				Name = "H3",
				FontSize = 14d
			},
			new StyleInfoItem()
			{
				Name = "H4",
				FontSize = 13d
			},
			new StyleInfoItem()
			{
				Name = "H5",
				FontSize = 12d
			},
			new StyleInfoItem()
			{
				Name = "H6",
				FontSize = 11d
			},
			new StyleInfoItem()
			{
				Name = "H7",
				FontSize = 10d
			},
			new StyleInfoItem()
			{
				Name = "H8",
				FontSize = 10d
			},
			new StyleInfoItem()
			{
				Name = "H9",
				FontSize = 10d
			}
		};
		/// <summary>
		/// Get a reference to the collection of defined styles for this document.
		/// </summary>
		public StyleInfoCollection Styles
		{
			get { return mStyles; }
		}
		//*-----------------------------------------------------------------------*

		////*-----------------------------------------------------------------------*
		////*	Table																																	*
		////*-----------------------------------------------------------------------*
		//private Xceed.Document.NET.Table mTable = null;
		///// <summary>
		///// Get/Set a reference to the active Word table.
		///// </summary>
		//public Xceed.Document.NET.Table Table
		//{
		//	get { return mTable; }
		//	set { mTable = value; }
		//}
		////*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	UserStyles																														*
		//*-----------------------------------------------------------------------*
		private List<VariableStyleItem> mUserStyles =
			new List<VariableStyleItem>();
		/// <summary>
		/// Get/Set a reference to the collection of user styles applied to the
		/// document.
		/// </summary>
		public List<VariableStyleItem> UserStyles
		{
			get { return mUserStyles; }
			set { mUserStyles = value; }
		}
		//*-----------------------------------------------------------------------*


	}
	//*-------------------------------------------------------------------------*

}
