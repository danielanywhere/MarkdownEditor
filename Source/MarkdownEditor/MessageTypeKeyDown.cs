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

using Newtonsoft.Json;

namespace MarkdownEditor
{
	//*-------------------------------------------------------------------------*
	//*	MessageTypeKeyDown																											*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// KeyDown message type for transferring key down messages.
	/// </summary>
	public class MessageTypeKeyDown
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
		//*	AltKey																																*
		//*-----------------------------------------------------------------------*
		private bool mAltKey = false;
		/// <summary>
		/// Get/Set the Alt key status.
		/// </summary>
		[JsonProperty(Order = 1)]
		public bool AltKey
		{
			get { return mAltKey; }
			set { mAltKey = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Code																																	*
		//*-----------------------------------------------------------------------*
		private string mCode = "";
		/// <summary>
		/// Get/Set the official code for the key.
		/// </summary>
		[JsonProperty(Order = 4)]
		public string Code
		{
			get { return mCode; }
			set { mCode = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	CtrlKey																																*
		//*-----------------------------------------------------------------------*
		private bool mCtrlKey = false;
		/// <summary>
		/// Get/Set the control key status.
		/// </summary>
		[JsonProperty(Order = 2)]
		public bool CtrlKey
		{
			get { return mCtrlKey; }
			set { mCtrlKey = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Key																																		*
		//*-----------------------------------------------------------------------*
		private string mKey = "";
		/// <summary>
		/// Get/Set the printable key.
		/// </summary>
		[JsonProperty(Order = 5)]
		public string Key
		{
			get { return mKey; }
			set { mKey = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	MessageType																														*
		//*-----------------------------------------------------------------------*
		private string mMessageType = "KeyDown";
		/// <summary>
		/// Get/Set the message type name.
		/// </summary>
		[JsonProperty(Order = 0)]		
		public string MessageType
		{
			get { return mMessageType; }
			set { mMessageType = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ShiftKey																															*
		//*-----------------------------------------------------------------------*
		private bool mShiftKey = false;
		/// <summary>
		/// Get/Set the shift key status.
		/// </summary>
		[JsonProperty(Order = 3)]
		public bool ShiftKey
		{
			get { return mShiftKey; }
			set { mShiftKey = value; }
		}
		//*-----------------------------------------------------------------------*


	}
	//*-------------------------------------------------------------------------*

}
