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

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MarkdownEditor
{
	//*-------------------------------------------------------------------------*
	//*	FileTypeRegister																												*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Registration activities for file types on this application.
	/// </summary>
	public class FileTypeRegister
	{
		/// <summary>
		/// Notifies the system that the application has performed an event.
		/// </summary>
		/// <param name="wEventId">
		/// System event ID.
		/// </param>
		/// <param name="uFlags">
		/// Flags that indicate the meaning of the dwItem1 and dwItem2 parameters.
		/// </param>
		/// <param name="dwItem1">
		/// First descriptor of the event.
		/// </param>
		/// <param name="dwItem2">
		/// Second descriptor of the event.
		/// </param>
		[DllImport("shell32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern void SHChangeNotify(uint wEventId, uint uFlags,
			IntPtr dwItem1, IntPtr dwItem2);

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
		//*	IsRegistered																													*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Return a value indicating whether the file types for this project are
		/// registered.
		/// </summary>
		/// <param name="projectExtension">
		/// The filename extension of the project, without a leading period.
		/// </param>
		public static bool IsRegistered(string projectExtension)
		{
			RegistryKey extensionReg = null;
			string[] extensions = null;
			bool result = false;

			extensionReg = Registry.CurrentUser.OpenSubKey("Software\\Classes\\");
			extensions = extensionReg.GetSubKeyNames();

			//	ie "mdproj.json"
			result = extensions.Contains(projectExtension);

			extensionReg.Close();
			extensionReg.Dispose();

			return result;
		}
		//*-----------------------------------------------------------------------*


		//*-----------------------------------------------------------------------*
		//* RegisterProject																												*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Register this application to the specified project extension.
		/// </summary>
		/// <param name="projectExtension">
		/// Extension of the project filename, without a leading period.
		/// </param>
		public static void RegisterProject(string projectExtension)
		{
			string applicationPath = "";
			RegistryKey extReg = null;

			if(projectExtension?.Length > 0)
			{
				applicationPath =
					System.Reflection.Assembly.GetExecutingAssembly().Location.
					Replace(".dll", ".exe");
				extReg = Registry.CurrentUser.CreateSubKey(
					"Software\\Classes\\" + projectExtension);
				extReg.CreateSubKey("shell\\open\\command").
					SetValue("", $"\"{applicationPath}\" \"%1\"");
				extReg.Close();
				extReg.Dispose();
				SHChangeNotify(0x08000000, 0x0000, IntPtr.Zero, IntPtr.Zero);
			}
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*

}
