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
using System.Globalization;

namespace Psafe3.Cli.Model
{
    public class PasswordSafePasswordHistory
    {
        private readonly List<PasswordSafePassword> _passwordHistory = new List<PasswordSafePassword>();

        /// <summary>
        ///     Initializes a new instance of the <see cref="PasswordSafePasswordHistory" /> class.
        /// </summary>
        public PasswordSafePasswordHistory()
        {
            Status = 0;
            MaxHistory = 0;
        }

        /// <summary>
        ///     Gets or sets the status.
        /// </summary>
        /// <value>The status.</value>
        public byte Status { get; set; }

        /// <summary>
        ///     Gets or sets the max number of entries in the history.
        /// </summary>
        /// <value>The max number of history entries</value>
        public byte MaxHistory { get; set; }

        /// <summary>
        ///     Gets the password history.
        /// </summary>
        /// <value>The password history.</value>
        public ICollection<PasswordSafePassword> PasswordHistory => _passwordHistory;

        /// <summary>
        ///     Adds a password to the password history list.
        /// </summary>
        /// <param name="password">The password.</param>
        public void PasswordHistoryAdd(
            PasswordSafePassword password)
        {
            _passwordHistory.Add(password);
        }

        /// <summary>
        ///     Parses a password history from the database serialized format which is:
        ///     smmnnddddddddllll[passsword] ...
        ///     's' is a decimal status.
        ///     'mm' is a hex max number of password history entries
        ///     'nn' is a hex number of passwords in this list
        ///     The following is repeated 'nn' times:
        ///     'dddddddd' is a hex representation of 32-bit unix time when this password was changed.
        ///     'llll' is a hex representation of 16-bit length of the password that follows.
        ///     [password] is the string of the password.
        /// </summary>
        /// <param name="serialized">The serialized string</param>
        /// <returns>The decoded password history</returns>
        public static PasswordSafePasswordHistory Parse(
            string serialized)
        {
            if (serialized == null)
            {
                throw new ArgumentNullException("serialized");
            }

            var history = new PasswordSafePasswordHistory();
            if (serialized.Length < 5)
            {
                return history;
            }

            history.Status = byte.Parse(serialized.Substring(0, 1), NumberStyles.Integer, CultureInfo.InvariantCulture);
            history.MaxHistory = byte.Parse(serialized.Substring(1, 2), NumberStyles.HexNumber,
                CultureInfo.InvariantCulture);
            var numberOfPasswords = int.Parse(serialized.Substring(3, 2), NumberStyles.HexNumber,
                CultureInfo.InvariantCulture);
            var currentIndex = 5;

            for (var i = 0; i < numberOfPasswords; ++i)
            {
                var unixTimeChanged = int.Parse(serialized.Substring(currentIndex, 8), NumberStyles.HexNumber,
                    CultureInfo.InvariantCulture);
                var passwordChangedUtc = PasswordSafeUtility.GetUtcFromUnixTime(unixTimeChanged);
                currentIndex += 8;

                var passwordLength = int.Parse(serialized.Substring(currentIndex, 4), NumberStyles.HexNumber,
                    CultureInfo.InvariantCulture);
                currentIndex += 4;

                var password = serialized.Substring(currentIndex, passwordLength);
                currentIndex += passwordLength;

                history.PasswordHistoryAdd(new PasswordSafePassword(password, passwordChangedUtc));
            }

            return history;
        }
    }
}