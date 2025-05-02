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

using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using static MarkdownEditor.MarkdownEditorUtil;

namespace MarkdownEditor
{
	//*-------------------------------------------------------------------------*
	//*	MarkdownBlockCollection																									*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of MarkdownBlockItem Items.
	/// </summary>
	public class MarkdownBlockCollection : List<MarkdownBlockItem>
	{
		//*************************************************************************
		//*	Private																																*
		//*************************************************************************
		private bool mHorizontalLineActive = false;

		//*-----------------------------------------------------------------------*
		//* AddHeading																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Add the current line as a heading to the current level, then add the
		/// following content as child content.
		/// </summary>
		/// <param name="blocks">
		/// Reference to a collection of the blocks that will receive this heading.
		/// </param>
		/// <param name="lines">
		/// Reference to a collection of regex-matched file lines.
		/// </param>
		/// <param name="headLine">
		/// Reference to the current matching line.
		/// </param>
		/// <param name="indexItem">
		/// Reference to the current line index tracker.
		/// </param>
		private static void AddHeading(MarkdownBlockCollection blocks,
			MatchCollection lines, Group headLine, IndexTrackerItem indexItem)
		{
			MarkdownBlockItem block = null;
			MarkdownBlockCollection childBlocks = null;
			int count = 0;
			Group group = null;
			int index = 0;
			int level = 0;
			string line = "";
			int newLevel = 0;

			if(blocks != null && lines?.Count > 0 && headLine != null &&
				indexItem != null)
			{
				level = GetHeadingLevel(headLine.Value);
				block = new MarkdownBlockItem()
				{
					BlockType = (MarkdownBlockType)level,
					StartIndex = headLine.Index,
					Value = headLine.Value
				};
				blocks.Add(block);
				childBlocks = block.Blocks;
				index = indexItem.Index;
				count = indexItem.Count;
				for(++ index; index < count; index ++)
				{
					group = GetGroup(lines[index], "line");
					line = GetValue(lines[index], "line");
					if(IsHeading(line))
					{
						//	This line is a heading.
						newLevel = GetHeadingLevel(line);
						if(newLevel > level)
						{
							//	If the new level is greater than the current one, then
							//	continue inward with a new header.
							indexItem.Index = index;
							indexItem.Count = count;
							AddHeading(childBlocks, lines, group, indexItem);
							index = indexItem.Index;
							count = indexItem.Count;
						}
						else if(newLevel < level)
						{
							//	Popping back to a lower level.
							//	Re-process this item at the parent level.
							index--;
							break;
						}
						else
						{
							//	When the level is equal, create a new block and reset the
							//	active child items.
							block = new MarkdownBlockItem()
							{
								BlockType = (MarkdownBlockType)level,
								StartIndex = group.Index,
								Value = line
							};
							blocks.Add(block);
							childBlocks = block.Blocks;
						}
					}
					else if(IsSectionStart(line))
					{
						//	Starting a new section.
						//	That item is built in the children collection of the current
						//	item.
						indexItem.Index = index;
						indexItem.Count = count;
						AddSection(childBlocks, lines, group, indexItem);
						index = indexItem.Index;
						count = indexItem.Count;
					}
					else if(IsSectionEnd(line))
					{
						//	Ending a section.
						//	Reprocess this item on the parent.
						index--;
						break;
					}
					else if(IsHorizontalLine(line))
					{
						//	Horizontal line.
						if(HorizontalLineIsActive(childBlocks))
						{
							if(childBlocks.mHorizontalLineActive)
							{
								//	The current block set has the horizontal line.
								block = new MarkdownBlockItem()
								{
									BlockType = MarkdownBlockType.HorizontalLine,
									StartIndex = group.Index,
									Value = line
								};
								childBlocks.Add(block);
							}
							else
							{
								//	An ancestor set has the horizontal line.
								//	Reprocess the current line at the parent.
								index--;
								break;
							}
						}
						else
						{
							//	This is the first time a horizontal line has been
							//	encountered in this context. The horizontal line
							//	is at this level.
							//	Create the line.
							block = new MarkdownBlockItem()
							{
								BlockType = MarkdownBlockType.HorizontalLine,
								StartIndex = group.Index,
								Value = line
							};
							childBlocks.Add(block);
							childBlocks.mHorizontalLineActive = true;
						}
					}
					else
					{
						//	This line is not a heading, section, or horizontal line.
						block = new MarkdownBlockItem()
						{
							BlockType = MarkdownBlockType.Text,
							StartIndex = group.Index,
							Value = line
						};
						//	General text lines are posted at the current level.
						childBlocks.Add(block);
					}
				}
				indexItem.Index = index;
				indexItem.Count = count;
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* AddSection																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Add the current line as the start of a section to the current level,
		/// then add the following content as child content.
		/// </summary>
		/// <param name="blocks">
		/// Reference to a collection of the blocks that will receive this heading.
		/// </param>
		/// <param name="lines">
		/// Reference to a collection of regex-matched file lines.
		/// </param>
		/// <param name="sectionLine">
		/// Reference to the current matching line group.
		/// </param>
		/// <param name="indexItem">
		/// Reference to the current line index tracker.
		/// </param>
		private static void AddSection(MarkdownBlockCollection blocks,
			MatchCollection lines, Group sectionLine, IndexTrackerItem indexItem)
		{
			MarkdownBlockItem block = null;
			MarkdownBlockCollection childBlocks = null;
			int count = 0;
			Group group = null;
			int index = 0;
			int level = 0;
			string line = "";
			int newLevel = 0;

			if(blocks != null && lines?.Count > 0 && sectionLine != null &&
				indexItem != null)
			{
				block = new MarkdownBlockItem()
				{
					BlockType = MarkdownBlockType.HtmlSection,
					StartIndex = sectionLine.Index,
					Value = sectionLine.Value
				};
				blocks.Add(block);
				childBlocks = block.Blocks;
				index = indexItem.Index;
				count = indexItem.Count;
				for(++index; index < count; index++)
				{
					group = GetGroup(lines[index], "line");
					line = GetValue(lines[index], "line");
					if(IsHeading(line))
					{
						//	This line is a heading.
						newLevel = GetHeadingLevel(line);
						if(newLevel > level)
						{
							//	If the new level is greater than the current one, then
							//	continue inward with a new header.
							indexItem.Index = index;
							indexItem.Count = count;
							AddHeading(childBlocks, lines, group, indexItem);
							index = indexItem.Index;
							count = indexItem.Count;
						}
						else if(newLevel < level)
						{
							//	Popping back to a lower level.
							//	Re-process this item at the parent level.
							index--;
							break;
						}
						else
						{
							//	When the level is equal, create a new block and reset the
							//	active child items.
							block = new MarkdownBlockItem()
							{
								BlockType = (MarkdownBlockType)level,
								StartIndex = group.Index,
								Value = line
							};
							blocks.Add(block);
							childBlocks = block.Blocks;
						}
					}
					else if(IsSectionStart(line))
					{
						//	Starting a new section.
						//	That item is built in the children collection of the current
						//	item.
						indexItem.Index = index;
						indexItem.Count = count;
						AddSection(childBlocks, lines, group, indexItem);
						index = indexItem.Index;
						count = indexItem.Count;
					}
					else if(IsSectionEnd(line))
					{
						//	Ending the current section.
						block = new MarkdownBlockItem()
						{
							BlockType = MarkdownBlockType.HtmlSectionEnd,
							StartIndex = group.Index,
							Value = line
						};
						blocks.Add(block);
						break;
					}
					else if(IsHorizontalLine(line))
					{
						//	Horizontal line.
						if(HorizontalLineIsActive(childBlocks))
						{
							if(childBlocks.mHorizontalLineActive)
							{
								//	The current block set has the horizontal line.
								block = new MarkdownBlockItem()
								{
									BlockType = MarkdownBlockType.HorizontalLine,
									StartIndex = group.Index,
									Value = line
								};
								childBlocks.Add(block);
							}
							else
							{
								//	An ancestor set has the horizontal line.
								//	Reprocess the current line at the parent.
								index--;
								break;
							}
						}
						else
						{
							//	This is the first time a horizontal line has been
							//	encountered in this context. The horizontal line
							//	is at this level.
							//	Create the line.
							block = new MarkdownBlockItem()
							{
								BlockType = MarkdownBlockType.HorizontalLine,
								StartIndex = group.Index,
								Value = line
							};
							childBlocks.Add(block);
							childBlocks.mHorizontalLineActive = true;
						}
					}
					else
					{
						//	Not a horizontal line.
						block = new MarkdownBlockItem()
						{
							BlockType = MarkdownBlockType.Text,
							StartIndex = group.Index,
							Value = line
						};
						//	General text lines are posted at the current level.
						childBlocks.Add(block);
					}
				}
				indexItem.Index = index;
				indexItem.Count = count;
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* AppendListItem																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Append an HTML list item to the supplied builder.
		/// </summary>
		/// <param name="appendListHead">
		/// Value indicating whether to append the head of the list prior to the
		/// item.
		/// </param>
		/// <param name="listType">
		/// Type of list to append to. 'U' or 'O' are recognized.
		/// </param>
		/// <param name="entry">
		/// List entry value. '-' or one of any allowable ordered entries.
		/// </param>
		/// <param name="text">
		/// Item text.
		/// </param>
		/// <param name="builder">
		/// Reference to the builder to which the item will be appended.
		/// </param>
		private static void AppendListItem(bool appendListHead, string listType,
			string entry, string text, StringBuilder builder)
		{
			if(listType?.Length > 0 && entry?.Length > 0 && builder != null)
			{
				switch(listType)
				{
					case "U":
						if(appendListHead)
						{
							builder.Append("<ul>");
						}
						builder.Append($"<li>{text}</li>");
						break;
					case "O":
						if(appendListHead)
						{
							builder.Append("<ol>");
						}
						builder.Append(
							$"<li value='{entry}'>{text}</li>");
						break;
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* CompleteList																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Complete the list from the currently outstanding indent levels.
		/// </summary>
		/// <param name="indentLevels">
		/// Reference to a stack of indent levels currently open.
		/// </param>
		/// <param name="builder">
		/// Reference to the string builder to receive the completion posts.
		/// </param>
		private static void CompleteList(Stack<IndentLevelItem> indentLevels,
			StringBuilder builder)
		{
			IndentLevelItem indentLevel = null;

			if(indentLevels?.Count > 0 && builder != null)
			{
				indentLevel = indentLevels.Peek();
				while(indentLevels.Count > 0)
				{
					//	Complete the current list.
					switch(indentLevel.ListType)
					{
						case "U":
							builder.Append("</ul>");
							break;
						case "O":
							builder.Append("</ol>");
							break;
					}
					//	Remove the current list.
					indentLevels.Pop();
					if(indentLevels.Count > 0)
					{
						//	Activate the previous list.
						indentLevel = indentLevels.Peek();
					}
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* CompleteListIndent																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Complete the list at the current index
		/// </summary>
		/// <param name="indentLevels">
		/// Reference to a stack of indent levels.
		/// </param>
		/// <param name="builder">
		/// Reference to the string builder to receive the post.
		/// </param>
		private static void CompleteListIndent(Stack<IndentLevelItem> indentLevels,
			StringBuilder builder)
		{
			IndentLevelItem indentLevel = null;

			if(indentLevels?.Count > 0 && builder != null)
			{
				indentLevel = indentLevels.Peek();
				switch(indentLevel.ListType)
				{
					case "U":
						builder.Append("</ul>");
						break;
					case "O":
						builder.Append("</ol>");
						break;
				}
				indentLevels.Pop();
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ExchangeCheckedWithString																							*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Exchange checked blocks with the caller's string content.
		/// </summary>
		/// <param name="blocks">
		/// Reference to a collection of blocks that might be checked.
		/// </param>
		/// <param name="replacement">
		/// Replacement string to place in the first of the checked blocks before
		/// deleting all others from the collection.
		/// </param>
		private static void ExchangeCheckedWithString(
			List<MarkdownBlockItem> blocks, string replacement)
		{
			MarkdownBlockItem block = null;

			if(blocks?.Count > 0 && replacement != null)
			{
				block = blocks.FirstOrDefault(x => x.Checked == true);
				if(block != null)
				{
					block.Value = replacement;
					block.Checked = false;
					blocks.RemoveAll(x => x.Checked == true);
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* HorizontalLineIsActive																								*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the current collection or any of its
		/// ancestors have the active horizontal line.
		/// </summary>
		/// <param name="blocks">
		/// The current level block collection from which to start.
		/// </param>
		/// <returns>
		/// True if the current or ancestor collections have an active horizontal
		/// line.
		/// </returns>
		private static bool HorizontalLineIsActive(MarkdownBlockCollection blocks)
		{
			bool result = false;

			if(blocks != null)
			{
				if(blocks.mHorizontalLineActive)
				{
					result = true;
				}
				else if(blocks.mParent != null)
				{
					result = HorizontalLineIsActive(blocks.mParent.Parent);
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* IsHeadingBlock																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the specified block is a heading.
		/// </summary>
		/// <param name="block">
		/// Reference to the block to inspect.
		/// </param>
		/// <returns>
		/// True if the block has a BlockType of H1 through H9. Otherwise, false.
		/// </returns>
		private static bool IsHeadingBlock(MarkdownBlockItem block)
		{
			int number = 0;
			bool result = false;

			if(block != null)
			{
				number = (int)block.BlockType;
				if(number > 0 && number < 10)
				{
					result = true;
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*************************************************************************
		//*	Protected																															*
		//*************************************************************************
		//*************************************************************************
		//*	Public																																*
		//*************************************************************************

		//*-----------------------------------------------------------------------*
		//* Add																																		*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Add an existing item to the collection.
		/// </summary>
		/// <param name="item">
		/// Reference to the MarkdownBlockItem to be added.
		/// </param>
		public new void Add(MarkdownBlockItem item)
		{
			if(item != null)
			{
				if(item.Parent == null)
				{
					item.Parent = this;
				}
				base.Add(item);
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ConvertListToHtml																											*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Convert the list content in the provided block collection to HTML
		/// entries.
		/// </summary>
		/// <param name="blocks">
		/// Reference to the collection of blocks that might be occupied by list
		/// entries.
		/// </param>
		public static void ConvertListToHtml(MarkdownBlockCollection blocks)
		{
			MarkdownBlockItem block = null;
			StringBuilder blockBuilder = new StringBuilder();
			int count = 0;
			string entry = "";
			IndentLevelItem indentLevel = null;
			Stack<IndentLevelItem> indentLevels =
				new Stack<IndentLevelItem>();
			int index = 0;
			string listType = "";
			Match match = null;
			int spaceLevel = -1;
			string text = "";

			if(blocks?.Count > 0)
			{
				foreach(MarkdownBlockItem blockItem in blocks)
				{
					blockItem.Checked = false;
				}
				count = blocks.Count;
				for(index = 0; index < count; index ++)
				{
					block = blocks[index];
					match = Regex.Match(block.Value, ResourceMain.rxMarkdownListItem);
					if(match.Success)
					{
						text = GetValue(match, "text");
						entry = GetValue(match, "listType");
						if(entry.EndsWith("."))
						{
							entry = entry.Substring(0, entry.Length - 1);
						}
						listType = (GetValue(match, "listType") == "-" ? "U" : "O");
						spaceLevel = GetValue(match, "leadingSpace").Length;
						if(indentLevels.Count > 0)
						{
							//	We are already working in the list.
							indentLevel = indentLevels.Peek();
							if(spaceLevel == indentLevel.IndentLevel)
							{
								//	Same list level.
								if(indentLevel.ListType == listType)
								{
									//	Same list type.
									AppendListItem(false, listType, entry, text, blockBuilder);
									block.Checked = true;
								}
								else
								{
									//	Here, we have encountered a different list type.
									//	Close the previous list and create a new one of the
									//	specified type.
									CompleteListIndent(indentLevels, blockBuilder);
									indentLevels.Push(new IndentLevelItem()
									{
										IndentLevel = spaceLevel,
										ListType = listType
									});
									AppendListItem(true, listType, entry, text, blockBuilder);
									block.Checked = true;
								}
							}
							else if(spaceLevel > indentLevel.IndentLevel)
							{
								//	Inner level.
								AppendListItem(true, listType, entry, text, blockBuilder);
								indentLevels.Push(new IndentLevelItem()
								{
									IndentLevel = spaceLevel,
									ListType = listType
								});
								block.Checked = true;
							}
							else
							{
								//	Outer level.
								while(indentLevels.Count > 0 &&
									spaceLevel < indentLevel.IndentLevel)
								{
									CompleteListIndent(indentLevels, blockBuilder);
									if(indentLevels.Count > 0)
									{
										indentLevel = indentLevels.Peek();
									}
									else
									{
										indentLevel = null;
									}
								}
								if(indentLevel != null &&
									spaceLevel == indentLevel.IndentLevel)
								{
									//	Levels are currently equal.
									if(indentLevel.ListType == listType)
									{
										//	Same list type.
										AppendListItem(false, listType, entry, text, blockBuilder);
										block.Checked = true;
									}
									else
									{
										//	Here, we have encountered a different list type.
										//	Close the previous list and create a new one of the
										//	specified type.
										CompleteListIndent(indentLevels, blockBuilder);
										indentLevels.Push(new IndentLevelItem()
										{
											IndentLevel = spaceLevel,
											ListType = listType
										});
										AppendListItem(true, listType, entry, text, blockBuilder);
										block.Checked = true;
									}
								}
							}
						}
						else
						{
							//	If not in the list already, then the first space level is the
							//	base level.
							AppendListItem(true, listType, entry, text, blockBuilder);
							indentLevels.Push(new IndentLevelItem()
							{
								IndentLevel = spaceLevel,
								ListType = listType
							});
							block.Checked = true;
						}
					}
					else
					{
						CompleteList(indentLevels, blockBuilder);
						spaceLevel = -1;
					}
					if(indentLevels.Count == 0 && blockBuilder.Length > 0)
					{
						//	If the list was cleared out in this pass, then exchange
						//	the first affected block with the new value then delete
						//	the following affected blocks.
						ExchangeCheckedWithString(blocks, blockBuilder.ToString());
						MarkdownEditorUtil.Clear(blockBuilder);
						count = blocks.Count;
						index = -1;
					}
				}
				if(indentLevels.Count > 0)
				{
					//	The entire block set was a list.
					CompleteList(indentLevels, blockBuilder);
					ExchangeCheckedWithString(blocks, blockBuilder.ToString());
					count = blocks.Count;
					index = -1;
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* FormatLinesAsRecords																									*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Format Markdown horizontal line separators as record containers.
		/// </summary>
		/// <param name="blocks">
		/// Reference to the collection of blocks upon which to begin the
		/// conversion.
		/// </param>
		/// <param name="message">
		/// Message to be returned to the caller, if applicable.
		/// </param>
		/// <returns>
		/// A value indicating whether conversions were performed.
		/// </returns>
		public static bool FormatLinesAsRecords(MarkdownBlockCollection blocks,
			StringTrackerItem message)
		{
			bool bDetailStarted = false;
			MarkdownBlockItem block = null;
			//List<MarkdownBlockItem> blockItems = null;
			StringBuilder builder = new StringBuilder();
			NameValueItem cellValue = null;
			List<NameValueItem> cellValues = new List<NameValueItem>();
			int count = 0;
			int index = 0;
			int indexFirst = -1;
			int indexLast = -1;
			List<MarkdownBlockItem> newBlocks = new List<MarkdownBlockItem>();
			StringBuilder renderedText = new StringBuilder();
			bool result = false;
			DataRow row = null;
			DataTable table = null;
			string text = "";
			List<MarkdownBlockItem> workingItems = new List<MarkdownBlockItem>();

			if(blocks?.Count > 0)
			{
				if(blocks.Exists(x => x.BlockType == MarkdownBlockType.HorizontalLine))
				{
					//	There are horizontal lines at this level.
					//	The first horizontal line will be the start of the first record.
					block = blocks.FirstOrDefault(x =>
						x.BlockType == MarkdownBlockType.HorizontalLine);
					if(block != null)
					{
						indexFirst = blocks.IndexOf(block);
					}
					//	The last horizontal line will be the end of the last record.
					block = blocks.LastOrDefault(x =>
						x.BlockType == MarkdownBlockType.HorizontalLine);
					if(block != null)
					{
						indexLast = blocks.IndexOf(block);
					}
					if(indexFirst > -1 && indexLast > -1 && indexFirst != indexLast)
					{
						//	Multiple separators were found.
						workingItems.Clear();
						for(index = indexFirst; index <= indexLast; index ++)
						{
							workingItems.Add(blocks[index]);
						}
						count = workingItems.Count;
						for(index = 0; index < count; index ++)
						{
							//	Remove any blank lines preceding or following the horizontal
							//	lines.
							block = workingItems[index];
							if(block.BlockType == MarkdownBlockType.HorizontalLine)
							{
								//	Remove preceding line breaks.
								while(index > 0 &&
									workingItems[index - 1].BlockType ==
										MarkdownBlockType.Text &&
									workingItems[index - 1].Value == "")
								{
									workingItems.RemoveAt(index - 1);
									index--;		//	Delist.
									count--;		//	Discount.
								}
								//	Remove following line breaks.
								while(index + 1 < count &&
									workingItems[index + 1].BlockType ==
										MarkdownBlockType.Text &&
									workingItems[index + 1].Value == "")
								{
									workingItems.RemoveAt(index + 1);
									count--;		//	Discount.
								}
							}
						}
						//	Check all headers at this level.
						for(index = 1; index < count - 1; index ++)
						{
							block = workingItems[index];
							if(IsHeadingBlock(block))
							{
								//	This block is a heading.
								text = GetHeadingText(block.Value);
								if(!cellValues.Exists(x => x.Name == text))
								{
									cellValues.Add(new NameValueItem()
									{
										Name = text
									});
								}
							}
						}
						//	Create the target table.
						table = new DataTable("CellValues");
						if(cellValues.Count > 0)
						{
							//	This table has multiple columns.
							foreach(NameValueItem columnNameItem in cellValues)
							{
								table.Columns.Add(columnNameItem.Name, typeof(string));
							}
							for(index = 0; index < count; index ++)
							{
								//	Scan each of the source lines, including the first bar.
								block = workingItems[index];
								if(block.BlockType == MarkdownBlockType.HorizontalLine)
								{
									//	Complete the previous row.
									if(bDetailStarted)
									{
										row = table.NewRow();
										table.Rows.Add(row);
										foreach(NameValueItem cellItem in cellValues)
										{
											row.SetField<string>(cellItem.Name, cellItem.Value);
										}
									}
									//	Clear the cells.
									foreach(NameValueItem valueItem in cellValues)
									{
										valueItem.Value = "";
									}
									MarkdownEditorUtil.Clear(builder);
									bDetailStarted = true;
								}
								else
								{
									//	Current row.
									if(IsHeadingBlock(block))
									{
										//	This block is a heading.
										text = GetHeadingText(block.Value);
										//	Get the associated cell.
										cellValue = cellValues.FirstOrDefault(x => x.Name == text);
										if(cellValue != null)
										{
											//	Assign the inner text of the block as the cell value.
											MarkdownEditorUtil.Clear(renderedText);
											RenderText(block.Blocks, renderedText);
											cellValue.Value = renderedText.ToString();
										}
									}
									else
									{
										//	There shouldn't be any non-heading text unless it is
										//	the first text before the header.
										cellValue = cellValues.FirstOrDefault(x =>
											x.Name == "UnnamedText");
										if(cellValue == null)
										{
											cellValue = new NameValueItem()
											{
												Name = "UnnamedText"
											};
											cellValues.Add(cellValue);
											table.Columns.Add("UnnamedText", typeof(string));
										}
										MarkdownEditorUtil.Clear(renderedText);
										MarkdownBlockItem.RenderText(block, renderedText);
										cellValue.Value = renderedText.ToString();
									}
									//	Add content to the builder.
									MarkdownEditorUtil.Clear(renderedText);
								}
							}
							//	At this point, we may have content in the cell values
							//	to place in the last row of the table.
							if(bDetailStarted && cellValues.Exists(x => x.Value.Length > 0))
							{
								//	Write the final row to the table.
								row = table.NewRow();
								table.Rows.Add(row);
								foreach(NameValueItem cellItem in cellValues)
								{
									row.SetField<string>(cellItem.Name, cellItem.Value);
								}
								bDetailStarted = false;
							}
						}
						else
						{
							//	This table has only one unnamed column.
							cellValue = new NameValueItem()
							{
								Name = "UnnamedText"
							};
							cellValues.Add(cellValue);
							table.Columns.Add("UnnamedText", typeof(string));
							for(index = 0; index < count - 1; index ++)
							{
								block = workingItems[index];
								if(block.BlockType == MarkdownBlockType.HorizontalLine)
								{
									//	Complete the previous row.
									if(row != null)
									{
										row.SetField<string>(0, builder.ToString());
									}
									MarkdownEditorUtil.Clear(builder);
									//	Create a new row at the beginning of each line.
									row = table.NewRow();
									table.Rows.Add(row);
								}
								else
								{
									//	Add content to the builder.
									MarkdownEditorUtil.Clear(renderedText);
									MarkdownBlockItem.RenderText(block, renderedText);
									builder.Append(renderedText.ToString());
								}
							}
							//	Complete the last row.
							if(row != null)
							{
								row.SetField<string>(0, builder.ToString());
							}
							MarkdownEditorUtil.Clear(builder);
						}
						//	At this point, we have content in the table.
						//	Rearrange all of the source blocks in the range, also
						//	removing the horizontal lines in the scope.
						MarkdownEditorUtil.Clear(builder);
						//	Build the table header.
						foreach(NameValueItem cellItem in cellValues)
						{
							builder.Append("| ");
							if(cellItem.Name != "UnnamedText")
							{
								builder.Append(cellItem.Name);
							}
							else
							{
								builder.Append(
									new string(' ', cellItem.Name.Length));
							}
							builder.Append(' ');
						}
						builder.Append("|");
						block = new MarkdownBlockItem()
						{
							BlockType = MarkdownBlockType.TableHead,
							Value = ReplaceLineBreaks(builder.ToString())
						};
						newBlocks.Add(block);
						//	Build the table header separator.
						MarkdownEditorUtil.Clear(builder);
						foreach(NameValueItem cellItem in cellValues)
						{
							builder.Append("|-");
							builder.Append(new string('-', cellItem.Name.Length));
							builder.Append('-');
						}
						builder.Append("|");
						block = new MarkdownBlockItem()
						{
							BlockType = MarkdownBlockType.TableHead,
							Value = builder.ToString()
						};
						newBlocks.Add(block);
						//	Build each of the table rows.
						foreach(DataRow rowItem in table.Rows)
						{
							MarkdownEditorUtil.Clear(builder);
							foreach(NameValueItem cellItem in cellValues)
							{
								builder.Append("| ");
								builder.Append(
									rowItem.Field<string>(cellItem.Name).Trim());
								builder.Append(' ');
							}
							builder.Append("|");
							block = new MarkdownBlockItem()
							{
								BlockType = MarkdownBlockType.TableRow,
								Value = ReplaceLineBreaks(builder.ToString())
							};
							newBlocks.Add(block);
						}
						//	Remove the original blocks.
						workingItems.Clear();
						for(index = indexFirst; index <= indexLast; index ++)
						{
							workingItems.Add(blocks[index]);
						}
						foreach(MarkdownBlockItem blockItem in workingItems)
						{
							blocks.Remove(blockItem);
						}
						//	Replace with new blocks containing the table.
						count = newBlocks.Count;
						for(index = count - 1; index > -1; index --)
						{
							//	Insert in reverse order so we can use an unchanging insert
							//	index.
							blocks.Insert(indexFirst, newBlocks[index]);
						}
						result = true;
					}
					else
					{
						if(indexFirst == -1 || indexLast == -1)
						{
							message.Text = "A valid separator was not found...";
						}
						else
						{
							message.Text = "Only one separator was found.\r\n" +
								"Please use a horizontal line before the first record " +
								"and after the last record.";
						}
					}
				}
				if(!result)
				{
					foreach(MarkdownBlockItem blockItem in blocks)
					{
						//	Attempt the conversion at all child levels.
						result |= FormatLinesAsRecords(blockItem.Blocks, message);
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* FormatSectionsAsRecords																								*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Format HTML section nodes as record containers.
		/// </summary>
		/// <param name="blocks">
		/// Reference to the collection of blocks upon which to begin the
		/// conversion.
		/// </param>
		/// <param name="message">
		/// Message to be returned to the caller, if applicable.
		/// </param>
		/// <returns>
		/// Value indicating whether conversions were made.
		/// </returns>
		public static bool FormatSectionsAsRecords(MarkdownBlockCollection blocks,
			StringTrackerItem message)
		{
			MarkdownBlockItem block = null;
			StringBuilder builder = new StringBuilder();
			NameValueItem cellValue = null;
			List<NameValueItem> cellValues = new List<NameValueItem>();
			int count = 0;
			int index = 0;
			int indexFirst = -1;
			int indexLast = -1;
			List<MarkdownBlockItem> newBlocks = new List<MarkdownBlockItem>();
			StringBuilder renderedText = new StringBuilder();
			bool result = false;
			DataRow row = null;
			DataTable table = null;
			string text = "";
			List<MarkdownBlockItem> workingItems = new List<MarkdownBlockItem>();

			if(blocks?.Count > 0)
			{
				if(blocks.Exists(x => x.BlockType == MarkdownBlockType.HtmlSection))
				{
					//	There are sections at this level.
					//	Each section start is a container of everything in that row.
					//	The first section start will be the start of the first record.
					block = blocks.FirstOrDefault(x =>
						x.BlockType == MarkdownBlockType.HtmlSection);
					if(block != null)
					{
						indexFirst = blocks.IndexOf(block);
					}
					//	The last section end will be the end of the last record.
					block = blocks.LastOrDefault(x =>
						x.BlockType == MarkdownBlockType.HtmlSectionEnd);
					if(block != null)
					{
						indexLast = blocks.IndexOf(block);
					}
					if(indexFirst > -1 && indexLast > -1 && indexFirst != indexLast)
					{
						//	Multiple delineators were found.
						//	Check all headers at this level.
						for(index = indexFirst; index < indexLast; index ++)
						{
							block = blocks[index];
							if(block.BlockType == MarkdownBlockType.HtmlSection)
							{
								foreach(MarkdownBlockItem blockItem in block.Blocks)
								{
									if(IsHeadingBlock(blockItem))
									{
										//	This block is a heading.
										text = GetHeadingText(blockItem.Value);
										if(!cellValues.Exists(x => x.Name == text))
										{
											cellValues.Add(new NameValueItem()
											{
												Name = text
											});
										}
									}
								}
							}
						}
						//	Create the target table.
						table = new DataTable("CellValues");
						if(cellValues.Count > 0)
						{
							//	This table has multiple columns.
							foreach(NameValueItem columnNameItem in cellValues)
							{
								table.Columns.Add(columnNameItem.Name, typeof(string));
							}
							for(index = indexFirst; index < indexLast; index++)
							{
								//	Scan each of the source lines.
								block = blocks[index];
								if(block.BlockType == MarkdownBlockType.HtmlSection)
								{
									//	We found a detail row.
									foreach(NameValueItem cellItem in cellValues)
									{
										cellItem.Value = "";
									}
									foreach(MarkdownBlockItem blockItem in block.Blocks)
									{
										if(IsHeadingBlock(blockItem))
										{
											//	This block is a heading.
											text = GetHeadingText(blockItem.Value);
											//	Get the associated cell.
											cellValue =
												cellValues.FirstOrDefault(x => x.Name == text);
											if(cellValue != null)
											{
												//	Assign the inner text of the block as the cell
												//	value.
												MarkdownEditorUtil.Clear(renderedText);
												ConvertListToHtml(blockItem.Blocks);
												RenderText(blockItem.Blocks, renderedText);
												cellValue.Value = renderedText.ToString();
											}
										}
										else if(blockItem.BlockType != MarkdownBlockType.Text ||
											blockItem.Value != "")
										{
											//	There shouldn't be any non-heading text unless it is
											//	the first text before the header.
											cellValue = cellValues.FirstOrDefault(x =>
												x.Name == "UnnamedText");
											if(cellValue == null)
											{
												cellValue = new NameValueItem()
												{
													Name = "UnnamedText"
												};
												cellValues.Add(cellValue);
												table.Columns.Add("UnnamedText", typeof(string));
											}
											MarkdownEditorUtil.Clear(renderedText);
											MarkdownBlockItem.RenderText(block, renderedText);
											cellValue.Value = renderedText.ToString();
										}
									}
									row = table.NewRow();
									table.Rows.Add(row);
									foreach(NameValueItem cellItem in cellValues)
									{
										row.SetField<string>(cellItem.Name, cellItem.Value);
									}
								}
							}
						}
						else
						{
							//	This table has only one unnamed column.
							cellValue = new NameValueItem()
							{
								Name = "UnnamedText"
							};
							cellValues.Add(cellValue);
							table.Columns.Add("UnnamedText", typeof(string));
							for(index = indexFirst; index < indexLast; index++)
							{
								block = blocks[index];
								if(block.BlockType == MarkdownBlockType.HtmlSection)
								{
									//	We found a detail row.
									foreach(NameValueItem cellItem in cellValues)
									{
										cellItem.Value = "";
									}
									MarkdownEditorUtil.Clear(renderedText);
									MarkdownBlockCollection.RenderText(block.Blocks,
										renderedText);
									cellValue.Value = renderedText.ToString();
									row = table.NewRow();
									table.Rows.Add(row);
									foreach(NameValueItem cellItem in cellValues)
									{
										row.SetField<string>(cellItem.Name, cellItem.Value);
									}
								}
							}
						}
						//	At this point, we have content in the table.
						//	Rearrange all of the source blocks in the range, also
						//	removing the horizontal lines in the scope.
						MarkdownEditorUtil.Clear(builder);
						//	Build the table header.
						foreach(NameValueItem cellItem in cellValues)
						{
							builder.Append("| ");
							if(cellItem.Name != "UnnamedText")
							{
								builder.Append(cellItem.Name);
							}
							else
							{
								builder.Append(
									new string(' ', cellItem.Name.Length));
							}
							builder.Append(' ');
						}
						builder.Append("|");
						block = new MarkdownBlockItem()
						{
							BlockType = MarkdownBlockType.TableHead,
							Value = ReplaceLineBreaks(builder.ToString())
						};
						newBlocks.Add(block);
						//	Build the table header separator.
						MarkdownEditorUtil.Clear(builder);
						foreach(NameValueItem cellItem in cellValues)
						{
							builder.Append("|-");
							builder.Append(new string('-', cellItem.Name.Length));
							builder.Append('-');
						}
						builder.Append("|");
						block = new MarkdownBlockItem()
						{
							BlockType = MarkdownBlockType.TableHead,
							Value = builder.ToString()
						};
						newBlocks.Add(block);
						//	Build each of the table rows.
						foreach(DataRow rowItem in table.Rows)
						{
							MarkdownEditorUtil.Clear(builder);
							foreach(NameValueItem cellItem in cellValues)
							{
								builder.Append("| ");
								builder.Append(
									rowItem.Field<string>(cellItem.Name).Trim());
								builder.Append(' ');
							}
							builder.Append("|");
							block = new MarkdownBlockItem()
							{
								BlockType = MarkdownBlockType.TableRow,
								Value = ReplaceLineBreaks(builder.ToString())
							};
							newBlocks.Add(block);
						}
						//	Remove the original blocks.
						workingItems.Clear();
						for(index = indexFirst; index <= indexLast; index++)
						{
							workingItems.Add(blocks[index]);
						}
						foreach(MarkdownBlockItem blockItem in workingItems)
						{
							blocks.Remove(blockItem);
						}
						//	Replace with new blocks containing the table.
						count = newBlocks.Count;
						for(index = count - 1; index > -1; index--)
						{
							//	Insert in reverse order so we can use an unchanging insert
							//	index.
							blocks.Insert(indexFirst, newBlocks[index]);
						}
						result = true;
					}
					else
					{
						if(indexFirst == -1 || indexLast == -1)
						{
							message.Text = "No complete sections were found...";
						}
						else
						{
							message.Text = "Only one part of the section was found.\r\n" +
								"Please use a section opener before each record and a " +
								"section closer after each record.";
						}
					}
				}
				else
				{
					foreach(MarkdownBlockItem blockItem in blocks)
					{
						//	Attempt the conversion at all child levels.
						result |= FormatSectionsAsRecords(blockItem.Blocks, message);
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	HasHorizontalLines																										*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the provided block collection
		/// contains any Markdown horizontal lines.
		/// </summary>
		/// <param name="blocks">
		/// Reference to a collection of Markdown blocks to inspect.
		/// </param>
		/// <returns>
		/// True if the collection or any of its ancestors contain Markdown
		/// horizontal lines.
		/// </returns>
		public static bool HasHorizontalLines(MarkdownBlockCollection blocks)
		{
			bool result = false;

			if(blocks?.Count > 0)
			{
				foreach(MarkdownBlockItem blockItem in blocks)
				{
					if(blockItem.BlockType == MarkdownBlockType.HorizontalLine)
					{
						result = true;
						break;
					}
					else
					{
						result = HasHorizontalLines(blockItem.Blocks);
						if(result)
						{
							break;
						}
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	HasHtmlSections																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the provided block collection
		/// contains any HTML sections.
		/// </summary>
		/// <param name="blocks">
		/// Reference to a collection of Markdown blocks to inspect.
		/// </param>
		/// <returns>
		/// True if the collection or any of its ancestors contain HTML sections.
		/// </returns>
		public static bool HasHtmlSections(MarkdownBlockCollection blocks)
		{
			bool result = false;

			if(blocks?.Count > 0)
			{
				foreach(MarkdownBlockItem blockItem in blocks)
				{
					if(blockItem.BlockType == MarkdownBlockType.HtmlSection)
					{
						result = true;
						break;
					}
					else
					{
						result = HasHtmlSections(blockItem.Blocks);
						if(result)
						{
							break;
						}
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* IsSourceIndexInContent																								*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the specified source index is located
		/// within token content.
		/// </summary>
		/// <param name="sourceIndex">
		/// The source index to test for.
		/// </param>
		/// <returns>
		/// True if the specified source index is located within the content of
		/// any of the member values of this collection. Otherwise, false.
		/// </returns>
		public bool IsSourceIndexInContent(int sourceIndex)
		{
			bool result = false;

			if(sourceIndex > -1)
			{
				foreach(MarkdownBlockItem tokenItem in this)
				{
					if(tokenItem.StartIndex <= sourceIndex &&
						tokenItem.StartIndex + tokenItem.Length - 1 >= sourceIndex)
					{
						result = true;
						break;
					}
				}
			}
			return result;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Original																															*
		//*-----------------------------------------------------------------------*
		private string mOriginal = "";
		/// <summary>
		/// Get/Set the original string from which these tokens have been parsed.
		/// </summary>
		public string Original
		{
			get { return mOriginal; }
			set { mOriginal = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Parent																																*
		//*-----------------------------------------------------------------------*
		private MarkdownBlockItem mParent = null;
		/// <summary>
		/// Get/Set a reference to the parent item of this block collection.
		/// </summary>
		public MarkdownBlockItem Parent
		{
			get { return mParent; }
			set { mParent = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* Parse																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Create and return a collection of string tokens from the caller's
		/// regular expression match collection.
		/// </summary>
		/// <param name="matches">
		/// Reference to a collection of matches, each of which will be converted
		/// to string tokens.
		/// </param>
		/// <param name="groupName">
		/// Optional name of the regular expression matching named group to be used
		/// to create each string token. If this value is blank or null, the entire
		/// match is used.
		/// </param>
		/// <returns>
		/// Reference to the newly created and populated string token collection.
		/// </returns>
		public static MarkdownBlockCollection Parse(MatchCollection matches,
			string groupName = "")
		{
			Group group = null;
			MarkdownBlockCollection result = new MarkdownBlockCollection();

			if(matches?.Count > 0)
			{
				if(groupName?.Length > 0)
				{
					//	Group name specified.
					foreach(Match matchItem in matches)
					{
						group = matchItem.Groups[groupName];
						if(matchItem.Success && group != null && group.Value != null)
						{
							result.Add(new MarkdownBlockItem()
							{
								StartIndex = group.Index,
								Value = group.Value
							});
						}
					}
				}
				else
				{
					//	No group name specified.
					foreach(Match matchItem in matches)
					{
						if(matchItem.Success)
						{
							result.Add(new MarkdownBlockItem()
							{
								StartIndex = matchItem.Index,
								Value = matchItem.Value
							});
						}
					}
				}
			}
			return result;
		}
		//*- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -*
		/// <summary>
		/// Parse a Markdown string and return a corresponding block collection.
		/// </summary>
		/// <param name="markdown">
		/// The Markdown string to parse.
		/// </param>
		/// <returns>
		/// Reference to a newly created and populated Markdown block collection
		/// representing the original Markdown source.
		/// </returns>
		public static MarkdownBlockCollection Parse(string markdown)
		{
			MarkdownBlockItem block = null;
			MarkdownBlockCollection blocks = new MarkdownBlockCollection();
			int count = 0;
			Group group = null;
			int index = 0;
			IndexTrackerItem indexItem = new IndexTrackerItem();
			int level = 0;
			string line = null;
			MatchCollection lines = null;
			int newLevel = 0;

			if(markdown?.Length > 0)
			{
				lines = Regex.Matches(markdown, ResourceMain.rxLine);
				count = indexItem.Count = lines.Count;
				for(index = 0; index < count; index++)
				{
					group = GetGroup(lines[index], "line");
					line = GetValue(lines[index], "line");
					if(IsHeading(line))
					{
						//	This line is a heading.
						newLevel = GetHeadingLevel(line);
						if(newLevel > level)
						{
							//	If the new level is greater than the current one, then
							//	continue inward with a new header.
							indexItem.Index = index;
							indexItem.Count = count;
							AddHeading(blocks, lines, group, indexItem);
							index = indexItem.Index;
							count = indexItem.Count;
						}
						else
						{
							//	When the level is equal or lower, create a new block and
							//	reset the active child items.
							level = newLevel;
							block = new MarkdownBlockItem()
							{
								BlockType = (MarkdownBlockType)level,
								StartIndex = group.Index,
								Value = line
							};
							blocks.Add(block);
						}
					}
					else if(IsSectionStart(line))
					{
						//	Start of section.
						indexItem.Index = index;
						indexItem.Count = count;
						AddSection(blocks, lines, group, indexItem);
						index = indexItem.Index;
						count = indexItem.Count;
					}
					else if(IsSectionEnd(line))
					{
						//	End of section.
					}
					else if(IsHorizontalLine(line))
					{
						//	Horizontal line.
						block = new MarkdownBlockItem()
						{
							BlockType = MarkdownBlockType.HorizontalLine,
							StartIndex = group.Index,
							Value = line
						};
						blocks.mHorizontalLineActive = true;
						blocks.Add(block);
					}
					else
					{
						//	This line is not a heading, separator, or horizontal line.
						block = new MarkdownBlockItem()
						{
							BlockType = MarkdownBlockType.Text,
							StartIndex = group.Index,
							Value = line
						};
						blocks.Add(block);
					}
				}
				indexItem.Index = index;
				indexItem.Count = count;
			}

			return blocks;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* RenderText																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Render the source text of the contents of the provided block item.
		/// </summary>
		/// <param name="blocks">
		/// Reference to the MarkdownBlockItem for which the complete text will be
		/// rendered.
		/// </param>
		/// <param name="builder">
		/// Reference to the optional string builder used to assemble the text.
		/// </param>
		public static void RenderText(MarkdownBlockCollection blocks,
			StringBuilder builder)
		{
			if(blocks != null && builder != null)
			{
				foreach(MarkdownBlockItem blockItem in blocks)
				{
					MarkdownBlockItem.RenderText(blockItem, builder);
				}
			}
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

	//*-------------------------------------------------------------------------*
	//*	MarkdownBlockItem																												*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Individual tokenized markdown string.
	/// </summary>
	public class MarkdownBlockItem
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
		//*	_Constructor																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Create a new instance of the MarkdownBlockItem Item.
		/// </summary>
		public MarkdownBlockItem()
		{
			mBlocks = new MarkdownBlockCollection();
			mBlocks.Parent = this;
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Blocks																																*
		//*-----------------------------------------------------------------------*
		private MarkdownBlockCollection mBlocks = null;
		/// <summary>
		/// Get a reference to the collection of child blocks of this item.
		/// </summary>
		public MarkdownBlockCollection Blocks
		{
			get { return mBlocks; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	BlockType																															*
		//*-----------------------------------------------------------------------*
		private MarkdownBlockType mBlockType = MarkdownBlockType.None;
		/// <summary>
		/// Get/Set the block type of this entry.
		/// </summary>
		public MarkdownBlockType BlockType
		{
			get { return mBlockType; }
			set { mBlockType = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Checked																																*
		//*-----------------------------------------------------------------------*
		private bool mChecked = false;
		/// <summary>
		/// Get/Set a value indicating whether this item is checked.
		/// </summary>
		public bool Checked
		{
			get { return mChecked; }
			set { mChecked = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	End																																		*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Get the end position of the last character in the string.
		/// </summary>
		public int End
		{
			get
			{
				int result = mStartIndex + mValue.Length;
				if(mValue.Length > 0)
				{
					result--;
				}
				return result;
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Length																																*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Get the length of the token.
		/// </summary>
		public int Length
		{
			get { return (mValue?.Length > 0 ? mValue.Length : 0); }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	NamedTags																															*
		//*-----------------------------------------------------------------------*
		private Dictionary<string, object> mNamedTags =
			new Dictionary<string, object>();
		/// <summary>
		/// Get a reference to the collection of named tags assigned to this token.
		/// </summary>
		public Dictionary<string, object> NamedTags
		{
			get { return mNamedTags; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Parent																																*
		//*-----------------------------------------------------------------------*
		private MarkdownBlockCollection mParent = null;
		/// <summary>
		/// Get/Set a reference to the parent collection of which this block
		/// item is a member.
		/// </summary>
		public MarkdownBlockCollection Parent
		{
			get { return mParent; }
			set { mParent = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* RenderText																														*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Render the source text of the contents of the provided block item.
		/// </summary>
		/// <param name="block">
		/// Reference to the MarkdownBlockItem for which the complete text will be
		/// rendered.
		/// </param>
		/// <param name="builder">
		/// Reference to the optional string builder used to assemble the text.
		/// </param>
		public static void RenderText(MarkdownBlockItem block,
			StringBuilder builder)
		{
			if(block != null && builder != null)
			{
				builder.AppendLine(block.mValue);
				if(block.mBlocks.Count > 0)
				{
					MarkdownBlockCollection.RenderText(block.mBlocks, builder);
				}
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	StartIndex																														*
		//*-----------------------------------------------------------------------*
		private int mStartIndex = 0;
		/// <summary>
		/// Get/Set the staring index of this item within the source content.
		/// </summary>
		public int StartIndex
		{
			get { return mStartIndex; }
			set { mStartIndex = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//* ToString																															*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return the string representation of this item.
		/// </summary>
		/// <returns>
		/// The string representation of this block item.
		/// </returns>
		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();

			builder.Append('[');
			builder.Append(this.BlockType.ToString());
			builder.Append(']');
			if(mValue?.Length > 0)
			{
				//builder.Append(" - ");
				builder.Append(' ');
				builder.Append(mValue);
			}
			builder.Append(' ');
			builder.Append('(');
			builder.Append(mBlocks.Count.ToString());
			builder.Append(')');
			return builder.ToString();
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Value																																	*
		//*-----------------------------------------------------------------------*
		private string mValue = "";
		/// <summary>
		/// Get/Set the token value.
		/// </summary>
		public string Value
		{
			get { return mValue; }
			set { mValue = value; }
		}
		//*-----------------------------------------------------------------------*


	}
	//*-------------------------------------------------------------------------*

}
