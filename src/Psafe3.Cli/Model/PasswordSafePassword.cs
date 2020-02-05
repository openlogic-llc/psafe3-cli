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
    ///     Represents a password along with critical time information.
    /// </summary>
    public class PasswordSafePassword
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PasswordSafePassword" /> class.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <param name="modifiedUtc">The modified UTC.</param>
        /// <param name="expiresUtc">The expires UTC.</param>
        public PasswordSafePassword(
            string password,
            DateTime modifiedUtc,
            DateTime expiresUtc)
        {
            Password = password;
            ModifiedUtc = modifiedUtc;
            ExpiresUtc = expiresUtc;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="PasswordSafePassword" /> class.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <param name="modifiedUtc">The modified UTC.</param>
        public PasswordSafePassword(
            string password,
            DateTime modifiedUtc)
            : this(password, modifiedUtc, DateTime.MaxValue)
        {
        }

        /// <summary>
        ///     Gets or sets the actual password.
        /// </summary>
        /// <value>The password.</value>
        public string Password { get; set; }

        /// <summary>
        ///     Gets or sets the time the password was modified UTC.
        /// </summary>
        /// <value>The modified UTC.</value>
        public DateTime ModifiedUtc { get; set; }

        /// <summary>
        ///     Gets or sets the time the password expires UTC.
        /// </summary>
        /// <value>The expires UTC.</value>
        public DateTime ExpiresUtc { get; set; }
    }
}