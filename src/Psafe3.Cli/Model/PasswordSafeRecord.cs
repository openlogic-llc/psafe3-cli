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
    ///     A database password record
    /// </summary>
    public class PasswordSafeRecord : PasswordSafeRecordBase
    {
        /// <summary>
        ///     Gets or sets the UUID (GUID) used for synchronization etc
        /// </summary>
        /// <value>The UUID.</value>
        public Guid Uuid { get; set; }

        /// <summary>
        ///     Gets or sets the group.
        /// </summary>
        /// <value>The group.</value>
        public string Group { get; set; }

        /// <summary>
        ///     Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title { get; set; }

        /// <summary>
        ///     Gets or sets the user.
        /// </summary>
        /// <value>The user.</value>
        public string User { get; set; }

        /// <summary>
        ///     Gets or sets the notes.
        /// </summary>
        /// <value>The notes.</value>
        public string Notes { get; set; }

        /// <summary>
        ///     Gets or sets the password value. Don't use this to get the passord, use the Password property.
        /// </summary>
        /// <value>The password value.</value>
        internal string PasswordValue { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the time the record was created.
        /// </summary>
        /// <value>The time record created.</value>
        public DateTime TimeRecordCreatedUtc { get; set; } = DateTime.MinValue;

        /// <summary>
        ///     Gets or sets the time the record was accessed (UTC).
        /// </summary>
        /// <value>The date and time</value>
        public DateTime TimeRecordAccessedUtc { get; set; } = DateTime.MinValue;

        /// <summary>
        ///     Gets or sets the time the record was modified (UTC).
        /// </summary>
        /// <value>The date and time</value>
        public DateTime TimeRecordModifiedUtc { get; set; } = DateTime.MinValue;

        /// <summary>
        ///     Gets or sets the time the password expires (UTC). (Use the 'Password' property)
        /// </summary>
        /// <value>The date and time</value>
        internal DateTime TimePasswordExpiresUtc { get; set; } = DateTime.MaxValue;

        /// <summary>
        ///     Gets or sets the time the password was modified (UTC). (Use the 'Password' property)
        /// </summary>
        /// <value>The date and time</value>
        internal DateTime TimePasswordModifiedUtc { get; set; } = DateTime.MinValue;

        /// <summary>
        ///     Gets the password.
        /// </summary>
        /// <value>The password.</value>
        public PasswordSafePassword Password =>
            new PasswordSafePassword(PasswordValue, TimePasswordModifiedUtc, TimePasswordExpiresUtc);

        /// <summary>
        ///     Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
        public string ResourceLocator { get; set; }

        /// <summary>
        ///     Gets or sets the auto-type string. [Not sure if this is really a good idea, it sounds to me like
        ///     a vulnerability vector...] You may want to be careful about using the contents.
        /// </summary>
        /// <value>The string to automatically type</value>
        public string AutoType { get; set; }

        /// <summary>
        ///     Gets or sets the password history.
        /// </summary>
        /// <value>The password history.</value>
        public PasswordSafePasswordHistory PasswordHistory { get; set; }
    }
}