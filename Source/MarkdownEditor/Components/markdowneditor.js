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

//	markdowneditor.js
//	The base JavaScript file for the MarkdownEditor application.

	// var simplemde = new SimpleMDE({
	// 	autofocus: true,
	// 	autoDownloadFontAwesome: true,
	// 	element: document.getElementById("editor")
	// 	});

	//	Global startup.
	var mBasePath = "";
	var mColumnMaps = [];
	var mContentChangeEnabled = true;
	var mUserVariables = [];
	var renderer = new marked.Renderer();
	var simplemde = null;

	//----------
	// Override the image function in the marked renderer
	renderer.image = function(href, title, text)
	{
		console.log('Render image...');

		var path =
			(href.indexOf(':') > -1 ?
				href :
				mBasePath + href);
		var result = `<img src="${path}" alt="${text}"`;

	if(title)
	{
		result += `title="${title}"`;
	}
	result += (this.options.xhtml ? '/>' : '>');
		return result;
	};
	//----------

	//----------
	marked.use({ renderer });
	//----------

	//----------
	simplemde = new SimpleMDE({
		autofocus: true,
		autoDownloadFontAwesome: true,
		element: document.getElementById("editor"),
		previewRender: function(plainText)
		{
			var pattern = "";
			var rxPattern = null;
			// var testMatch = null;

			//	Column Maps.
			if(mColumnMaps.length > 0)
			{
				mColumnMaps.forEach(columnMapItem =>
				{
					pattern = `^([\\t ]*\\#{1,9}[\\t ]+)(${columnMapItem.Name})(\\s*)$`;
					rxPattern = new RegExp(pattern, "gim");
					// testMatch = plainText.match(rxPattern);
					// console.log(`Column map: ${pattern}`);
					// console.log(JSON.stringify(testMatch));
					plainText =
						plainText.replaceAll(
							rxPattern, "$1" + columnMapItem.Value + "$3");
				});
			}
			//	Variables.
			if(mUserVariables.length > 0)
			{
				mUserVariables.forEach(userVariableItem =>
				{
					plainText =
						plainText.replaceAll(
							`{${userVariableItem.Name}}`, userVariableItem.Value);
				});
			}

			plainText = marked.parse(plainText);

			//	Post-rendering processing.
			//	All links to load off-page.
			pattern = `(\\<a[\\t ]+(?!target\\s*=\\s*\\"[^\\"]*\\")[^\\>]+(?<!target\\s*=\\s*\\"[^\\"]*\\"))\\>`;
			//pattern = `(<a[\t ]+[^>]*)>`;
			// console.log(`Pattern: ${pattern}`);
			rxPattern = new RegExp(pattern, "gi");
			// testMatch = plainText.match(rxPattern);
			// console.log(`Link treatment:`);
			// console.log(JSON.stringify(testMatch));
			plainText = plainText.replaceAll(rxPattern, "$1" + ` target="blank">`);

			return plainText;
		},
		renderingConfig:
		{
			singleLineBreaks: false,
			codeSyntaxHighlighting: true
		}
	});
	//----------

	//----------
	simplemde.codemirror.on('change', () =>
	{
		var result =
		{
			MessageType: "CodeChange"
		};
		if(mContentChangeEnabled &&
			window.chrome && window.chrome.webview)
		{
			window.chrome.webview.postMessage(JSON.stringify(result));
		}
	});
	//----------

	//	General functions.

	/**
		* A keydown event was not consumed and bubbled up to the body.
		* Pass it to the host.
		*/
	function bodyKeyDown(event)
	{
		var result =
		{
			MessageType: "KeyDown",
			AltKey: false,
			CtrlKey: false,
			ShiftKey: false,
			Code: "",
			Key: ""
		};

		if(event.ctrlKey)
		{
			result.CtrlKey = true;
		}
		if(event.altKey)
		{
			result.AltKey = true;
		}
		if(event.shiftKey)
		{
			result.ShiftKey = true;
		}
		result.Code = event.code;
		result.Key = event.key;
		if(window.chrome && window.chrome.webview)
		{
			window.chrome.webview.postMessage(JSON.stringify(result));
		}
		else
		{
		console.log(JSON.stringify(result));
		}
	}

	//	clearUserVariables
	//----------
	/**
		* Clear the user variables for this instance.
		*/
	function clearUserVariables()
	{
		mColumnMaps.length = 0;
		mUserVariables.length = 0;
		// simplemde.togglePreview();
		// simplemde.togglePreview();
	}

	//	getCursorInfo
	//----------
	/**
		* Return the starting and ending cursor locations.
		* @returns {object} The cursor Start and End indices.
		*/
	function getCursorInfo()
	{
		var codeMirror = simplemde.codemirror;
		var cur = null;
		var nCur = null;
		var result = [];


		cur = codeMirror.getCursor('start');
		nCur = {};
		nCur.Name = "Start";
		nCur.Line = cur.line;
		nCur.Column = cur.ch;
		nCur.Relative = cur.xRel;
		result.push(nCur);

		cur = codeMirror.getCursor('end');
		nCur = {};
		nCur.Name = "End";
		nCur.Line = cur.line;
		nCur.Column = cur.ch;
		nCur.Relative = cur.xRel;
		result.push(nCur);

		return result;
	}

	//	getMarkdown
	//----------
	/**
		* Return the markdown content from the editor pane.
		* @returns {object}
		*/
	function getMarkdown()
	{
		var content = simplemde.value();
		var result = { Content: "" };

		console.log("Retrieving Markdown content...");
		if(content)
		{
			result.Content = content;
		}
		return result;
	}
	//----------

	//	setBasePath
	//----------
	/**
		* Set the base path to use for resources and media on this file.
		* @param {string} pathName Fully qualified path to use for base.
		*/
	function setBasePath(pathName)
	{
		if(pathName)
		{
			mBasePath = pathName;
			if(!mBasePath.endsWith('/'))
			{
				mBasePath += '/';
			}
		}
		else
		{
			mBasePath = "";
		}
	}
	//----------

	// setColumnMaps
	//----------
	/**
		* Set the user column maps for this state.
		* @param {string} variableJson JSON string containing the user
		* variables to apply.
		*/
	function setColumnMaps(variableJson)
	{
		var data = null;

		if(variableJson?.length > 0)
		{
			data = JSON.parse(variableJson);
			mColumnMaps.length = 0;
			data.forEach(item =>
			{
				mColumnMaps.push(
					{
						Name: item.Name,
						Value: item.Value
					}
				)
			});
			// simplemde.togglePreview();
			// simplemde.togglePreview();
			simplemde.toggleSideBySide();
			simplemde.toggleSideBySide();
		}
	}
	//----------

	//	setContentChangeEnabled
	//----------
	/**
		* Enable or disable the content change event to the host.
		* @param {string} enabledValue Value indicating whether to enable
		* the content change event.
		*/
	function setContentChangeEnabled(enabledValue)
	{
		if(enabledValue == 'true')
		{
			mContentChangeEnabled = true;
		}
		else
		{
			mContentChangeEnabled = false;
		}
	}

	//	setCursorInfo
	//----------
	/**
		* Set the starting and ending cursor locations.
		* @param {array} cursorInfo An array of cursorinfo objects.
		*/
	function setCursorInfo(cursorInfo)
	{
		var codeMirror = simplemde.codemirror;
		// var cur = null;
		var data = JSON.parse(cursorInfo);
		// var line = 0;

		//	Start first.
		if(data)
		{
			// console.log('Set cursor info: Data exists');
			data.some((item) =>
			{
				if(item.Name == "Start")
				{
					codeMirror.focus();
					codeMirror.setCursor({ line: item.Line, ch: 0 });
					codeMirror.scrollIntoView({line: item.Line, char: 0}, 100);
					// cur = codeMirror.getCursor('start');
					// cur.xRel = item.Relative;
					// cur.line = item.Line;
					// // console.log(` Start line: ${item.Line}`);
					// cur.ch = item.Column;
					// line = item.Line;
					return true;
				}
			});
			// data.some((item) =>
			// {
			// 	if(item.Name == "End")
			// 	{
			// 		cur = codeMirror.getCursor('end');
			// 		cur.xRel = item.Relative;
			// 		cur.line = item.Line;
			// 		// console.log(` End line: ${item.Line}`);
			// 		cur.ch = item.Column;
			// 		return true;
			// 	}
			// });
			// codeMirror.scrollIntoView({line: line, char: 0}, 400);
		}
	}

	//	setMarkdown
	//----------
	/**
		* Set the markdown content on the editor.
		* @param {string} content The markdown content to load into
		* the editor.
		*/
	function setMarkdown(content)
	{
		console.log("Setting Markdown content...");
		if(content)
		{
			simplemde.value(content);
		}
		else
		{
			simplemde.value("");
		}
	}
	//----------

	// setUserVariables
	//----------
	/**
		* Set the user variables for this state.
		* @param {string} variableJson JSON string containing the user
		* variables to apply.
		*/
	function setUserVariables(variableJson)
	{
		var data = null;

		if(variableJson?.length > 0)
		{
			data = JSON.parse(variableJson);
			mUserVariables.length = 0;
			data.forEach(item =>
			{
				mUserVariables.push(
					{
						Name: item.Name,
						Value: item.Value
					}
				)
			});
			// simplemde.togglePreview();
			// simplemde.togglePreview();
			simplemde.toggleSideBySide();
			simplemde.toggleSideBySide();
		}
	}
	//----------

	//	Document ready.
	//----------
	/**
		* The document object is loaded and ready to use.
	 */
	$(document).ready(function()
	{
		simplemde.toggleSideBySide();
		setBasePath("file:///C:/Users/Daniel/Ascendant/Ascendant - Product Development/Projects/LinearToTableDoc/Data");
	});
	//----------
