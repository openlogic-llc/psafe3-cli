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
    ///     The defined field codes for a database header
    /// </summary>
    internal enum PasswordSafeHeaderTypeCode
    {
        /// <summary>
        ///     Version minor/major
        /// </summary>
        HDR_VERSION = 0x00,

        /// <summary>
        ///     The unique identifier for this database
        /// </summary>
        HDR_UUID = 0x01,

        /// <summary>
        ///     Non-default user preferences
        /// </summary>
        HDR_NDPREFS = 0x02,

        /// <summary>
        ///     Display status
        /// </summary>
        HDR_DISPSTAT = 0x03,

        /// <summary>
        ///     Last time updated
        /// </summary>
        HDR_LASTUPDATETIME = 0x04,

        /// <summary>
        ///     Last update by user and host (composite field - deprecated in version 3.02)
        /// </summary>
        HDR_LASTUPDATEUSERHOST = 0x05,

        /// <summary>
        ///     Last application to update the database
        /// </summary>
        HDR_LASTUPDATEAPPLICATION = 0x06,

        /// <summary>
        ///     The last user to update the database (new in 3.02)
        /// </summary>
        HDR_LASTUPDATEUSER = 0x07,

        /// <summary>
        ///     The last host name that was used to update the database (new in 3.02)
        /// </summary>
        HDR_LASTUPDATEHOST = 0x08,

        /// <summary>
        ///     The name of the database (new in 3.02)
        /// </summary>
        HDR_DBNAME = 0x09,

        /// <summary>
        ///     A description of the database (new in 3.02)
        /// </summary>
        HDR_DBDESC = 0x0a,

        /// <summary>
        ///     Beginning of unknown codes
        /// </summary>
        HDR_LAST,

        /// <summary>
        ///     The end of field marker
        /// </summary>
        HDR_END = 0xff
    }
}