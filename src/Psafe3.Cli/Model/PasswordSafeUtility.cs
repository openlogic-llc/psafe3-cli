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

namespace Psafe3.Cli.Model
{
    /// <summary>
    /// Common utility methods for PasswordSafe reading and writing.
    /// </summary>
    internal static class PasswordSafeUtility
    {
        /// <summary>
        /// Gets the UTC from unix time.
        /// </summary>
        /// <param name="unixTime">The unix time.</param>
        /// <returns>The time as UTC DateTime</returns>
        public static DateTime GetUtcFromUnixTime(int unixTime)
        {
            DateTime netTime = DateTime.MinValue;
            if (unixTime != 0)
            {
                netTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(unixTime);
            }
            return netTime;
        }
    }
}