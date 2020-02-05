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

namespace Psafe3.Cli.Model
{
    /// <summary>
    ///     The field types in a database record
    /// </summary>
    internal enum PasswordSafeRecordTypeCode
    {
        /// <summary>
        ///     The first code
        /// </summary>
        START = 0x00,

        /// <summary>
        ///     The name of entry
        /// </summary>
        NAME = 0x00,

        /// <summary>
        ///     The Guid of the entry - used for sync
        /// </summary>
        UUID = 0x01,

        /// <summary>
        ///     The Group
        /// </summary>
        GROUP = 0x02,

        /// <summary>
        ///     The title
        /// </summary>
        TITLE = 0x03,

        /// <summary>
        ///     The username associated with the entry
        /// </summary>
        USER = 0x04,

        /// <summary>
        ///     The notes associated with the entry
        /// </summary>
        NOTES = 0x05,

        /// <summary>
        ///     The current password associated with the entry
        /// </summary>
        PASSWORD = 0x06,

        /// <summary>
        ///     The time this entry was created
        /// </summary>
        CTIME = 0x07,

        /// <summary>
        ///     The time the password was last modified
        /// </summary>
        PMTIME = 0x08,

        /// <summary>
        ///     The time the entry was last accessed
        /// </summary>
        ATIME = 0x09,

        /// <summary>
        ///     The time the password expires
        /// </summary>
        LTIME = 0x0a,

        /// <summary>
        ///     Not implemented
        /// </summary>
        POLICY = 0x0b,

        /// <summary>
        ///     The time the record itself was last modified
        /// </summary>
        RMTIME = 0x0c,

        /// <summary>
        ///     The URL associated with the entry
        /// </summary>
        URL = 0x0d,

        /// <summary>
        ///     An autotype string - be careful here...
        /// </summary>
        AUTOTYPE = 0x0e,

        /// <summary>
        ///     A string encoded history of passwords
        /// </summary>
        PWHIST = 0x0f,

        /// <summary>
        ///     The last valid code marker
        /// </summary>
        LAST,

        /// <summary>
        ///     The end of fields marker
        /// </summary>
        END = 0xff
    }
}