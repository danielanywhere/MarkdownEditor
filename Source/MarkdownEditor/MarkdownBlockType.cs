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
	//*	MarkdownTokenType																												*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Enumeration of recognized block types for markdown text blocks.
	/// </summary>
	public enum MarkdownBlockType
	{
		/// <summary>
		/// No token type specified or unknown.
		/// </summary>
		None = 0,
		/// <summary>
		/// Heading 1. Block.
		/// </summary>
		H1,
		/// <summary>
		/// Heading 2. Block.
		/// </summary>
		H2,
		/// <summary>
		/// Heading 3. Block.
		/// </summary>
		H3,
		/// <summary>
		/// Heading 4. Block.
		/// </summary>
		H4,
		/// <summary>
		/// Heading 5. Block.
		/// </summary>
		H5,
		/// <summary>
		/// Heading 6. Block.
		/// </summary>
		H6,
		/// <summary>
		/// Heading 7. Block.
		/// </summary>
		H7,
		/// <summary>
		/// Heading 8. Block.
		/// </summary>
		H8,
		/// <summary>
		/// Heading 9. Block.
		/// </summary>
		H9,
		/// <summary>
		/// Horizontal line. Block.
		/// </summary>
		HorizontalLine,
		/// <summary>
		/// Bold. Inline.
		/// </summary>
		Bold,
		/// <summary>
		/// Italic. Inline.
		/// </summary>
		Italic,
		/// <summary>
		/// Blockquote. Inline.
		/// </summary>
		Blockquote,
		/// <summary>
		/// Ordered list item. Block.
		/// </summary>
		OrderedList,
		/// <summary>
		/// Unordered list item. Block.
		/// </summary>
		UnorderedList,
		/// <summary>
		/// Code. Block.
		/// </summary>
		Code,
		/// <summary>
		/// Link. Inline.
		/// </summary>
		Link,
		/// <summary>
		/// Image. Inline.
		/// </summary>
		Image,
		/// <summary>
		/// Table. Block.
		/// </summary>
		Table,
		/// <summary>
		/// Table header row. Block.
		/// </summary>
		TableHead,
		/// <summary>
		/// Table data row. Block.
		/// </summary>
		TableRow,
		/// <summary>
		/// Reference to a footnote. Inline.
		/// </summary>
		FootnoteReference,
		/// <summary>
		/// Footnote definition. Block.
		/// </summary>
		Footnote,
		/// <summary>
		/// Definition list. Block.
		/// </summary>
		DefinitionList,
		/// <summary>
		/// Strikethrough. Inline.
		/// </summary>
		Strikethrough,
		/// <summary>
		/// Task list item. Block.
		/// </summary>
		TaskList,
		/// <summary>
		/// Emoji. Inline.
		/// </summary>
		Emoji,
		/// <summary>
		/// Highlight. Inline.
		/// </summary>
		Highlight,
		/// <summary>
		/// Subscript. Inline.
		/// </summary>
		Subscript,
		/// <summary>
		/// Superscript. Inline.
		/// </summary>
		Superscript,
		/// <summary>
		/// HTML section. Block.
		/// </summary>
		HtmlSection,
		/// <summary>
		/// End of HTML section. Block.
		/// </summary>
		HtmlSectionEnd,
		/// <summary>
		/// HTML Bold. Inline.
		/// </summary>
		HtmlBold,
		/// <summary>
		/// HTML Italic. Inline.
		/// </summary>
		HtmlItalic,
		/// <summary>
		/// HTML Underline. Inline.
		/// </summary>
		HtmlUnderline,
		/// <summary>
		/// HTML Font. Inline.
		/// </summary>
		HtmlFont,
		/// <summary>
		/// General text. Block.
		/// </summary>
		Text
	}
	//*-------------------------------------------------------------------------*

}
