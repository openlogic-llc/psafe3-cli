#region License

/*
 *  PasswordSafe Database Reader/Writer
 *
 *  Copyright (C) 2007 Svante Seleborg
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 *  If you'd like to license this program under any other terms than the
 *  above, please contact the author and copyright holder.
 *
 *  Contact: mailto:svante@axantum.com
 */

#endregion License

using System;
using System.Collections.Generic;

namespace Psafe3.Cli.Model
{
    /// <summary>
    ///     Represents the header record
    /// </summary>
    public class PasswordSafeHeader : PasswordSafeRecordBase
    {
        private readonly List<bool> _treeDisplayStatus = new List<bool>();

        /// <summary>
        ///     Gets or sets the minor version.
        /// </summary>
        /// <value>The minor version.</value>
        public byte MinorVersion { get; set; }

        /// <summary>
        ///     Gets or sets the major version.
        /// </summary>
        /// <value>The major version.</value>
        public byte MajorVersion { get; set; }

        /// <summary>
        ///     Gets or sets the UUID for the database.
        /// </summary>
        /// <value>The UUID.</value>
        public Guid Uuid { get; set; }

        /// <summary>
        ///     Gets or sets the non default user prefs.
        /// </summary>
        /// <value>The non default user prefs.</value>
        public string NonDefaultUserPrefs { get; set; }

        /// <summary>
        ///     Gets the tree display status. Used to remember the GUI state.
        /// </summary>
        /// <value>The tree display status.</value>
        public IList<bool> TreeDisplayStatus => _treeDisplayStatus;

        /// <summary>
        ///     Gets or sets the last saved by user.
        /// </summary>
        /// <value>Last saved by.</value>
        public string LastSavedBy { get; set; }

        /// <summary>
        ///     Gets or sets the date and time the database was last saved.
        /// </summary>
        /// <value>When last saved</value>
        public string LastSavedOn { get; set; }

        /// <summary>
        ///     Gets or sets the the application that saved last.
        /// </summary>
        /// <value>A string identifying the application.</value>
        public string WhatLastSaved { get; set; }

        /// <summary>
        ///     Gets or sets the last update user.
        /// </summary>
        /// <value>The last update user.</value>
        public string LastUpdateUser { get; set; }

        /// <summary>
        ///     Gets or sets the last update host.
        /// </summary>
        /// <value>The last update host.</value>
        public string LastUpdateHost { get; set; }

        public DateTime LastUpdateTimeUtc { get; set; }

        /// <summary>
        ///     Gets or sets the name of the database
        /// </summary>
        /// <value>The name of the database.</value>
        public string DBName { get; set; }

        /// <summary>
        ///     Gets or sets the database description.
        /// </summary>
        /// <value>The database description.</value>
        public string DBDescription { get; set; }

        /// <summary>
        ///     Adds another status to the tree display status.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public void TreeDisplayStatusAdd(
            bool value)
        {
            _treeDisplayStatus.Add(value);
        }
    }
}