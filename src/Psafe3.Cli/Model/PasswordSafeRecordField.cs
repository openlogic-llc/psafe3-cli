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
    public class PasswordSafeRecordField
    {
        private readonly byte[] _dataBuffer;

        /// <summary>
        ///     Initializes a new instance of the <see cref="PasswordSafeRecordField" /> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="dataBuffer">The data buffer.</param>
        public PasswordSafeRecordField(
            byte type,
            byte[] dataBuffer)
        {
            Type = type;
            _dataBuffer = dataBuffer;
        }

        /// <summary>
        ///     Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public byte Type { get; set; }

        /// <summary>
        ///     Get's a reference to the data buffer
        /// </summary>
        /// <returns>The data buffer</returns>
        public byte[] DataBuffer()
        {
            return _dataBuffer;
        }
    }
}