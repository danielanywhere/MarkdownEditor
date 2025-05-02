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
	//*	ContainerInfoCollection																									*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Collection of ContainerInfoItem Items.
	/// </summary>
	public class ContainerInfoCollection : List<ContainerInfoItem>
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
	//*	ContainerInfoItem																												*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Information about an individual container.
	/// </summary>
	public class ContainerInfoItem
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
		//*	Container																															*
		//*-----------------------------------------------------------------------*
		private Xceed.Document.NET.Container mContainer = null;
		/// <summary>
		/// Get/Set a reference to the container being represented by this item.
		/// </summary>
		public Xceed.Document.NET.Container Container
		{
			get { return mContainer; }
			set { mContainer = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Tags																																	*
		//*-----------------------------------------------------------------------*
		private NamedTagCollection mTags = new NamedTagCollection();
		/// <summary>
		/// Get a reference to the collection of tags for this container.
		/// </summary>
		public NamedTagCollection Tags
		{
			get { return mTags; }
		}
		//*-----------------------------------------------------------------------*


	}
	//*-------------------------------------------------------------------------*

}
