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

using System.Runtime.InteropServices;
using System.Text;

namespace Psafe3.Cli.Model
{
    /// <summary>
    ///     Implements native methods that are safe to call
    /// </summary>
    internal static class SafeNativeMethods
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern int GetACP();

        /// <summary>
        ///     Converts the string from Unicode to the current systen Ansi Code Page. This is not really
        ///     a good idea, but it's the way PasswordSafe works. Problem is that it can vary from system
        ///     to system, and then you can't open your database... :-(.
        /// </summary>
        /// <param name="passphrase">The passphrase.</param>
        /// <returns>The passphrase encoded in the current system Ansi Code Page</returns>
        public static byte[] ConvertString(
            string passphrase)
        {
            return Encoding.GetEncoding(GetACP()).GetBytes(passphrase);
        }
    }
}