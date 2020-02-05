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

#region Credits

/*
 *  The TwoFish code is written by Shaun Wilde, http://www.many-monkeys.com, and is available at http://www.codeproject.com/cs/algorithms/twofish_csharp.asp .
 */

#endregion Credits

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using InvalidDataException = Psafe3.Cli.Helpers.InvalidDataException;

//using InOut = System.IO; // InvalidDataException
// InvalidDataException

//using Crypto = System.Security.Cryptography;

// Also works for CE now

namespace Psafe3.Cli.Model
{
    /// <summary>
    ///     Represents a reader that provides a forward only access to the data in PasswordSafe database.
    ///     See http://passwordsafe.sourceforge.net for details.
    ///     Throws InvalidDataException if the file is corrupt or the wrong passphrase is provided.
    ///     Throws InvalidOperationException if the caller performs improper operations.
    /// </summary>
    public class PasswordSafeReader : IDisposable
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="PasswordSafeReader" /> class.
        /// </summary>
        /// <param name="stream">The stream. Note that it is disposed when this instance is disposed.</param>
        public PasswordSafeReader(
            Stream stream)
        {
            _stream = stream;
            CurrentPartType = PasswordSafePartType.None;
        }

        #endregion Constructors

        #region Private Fields

        private Stream _stream;
        private bool _error;
        private ICryptoTransform _decryptor;
        private HMACSHA256 _hmac;

        private static readonly byte[] _eofMarkerBytes =
        {
            (byte) 'P', (byte) 'W', (byte) 'S', (byte) '3', (byte) '-', (byte) 'E', (byte) 'O', (byte) 'F',
            (byte) 'P', (byte) 'W', (byte) 'S', (byte) '3', (byte) '-', (byte) 'E', (byte) 'O', (byte) 'F'
        };

        private static readonly byte[] _tagBytes = { (byte)'P', (byte)'W', (byte)'S', (byte)'3' };

        #endregion Private Fields

        #region Properties

        /// <summary>
        ///     Gets or sets the type of the current part.
        /// </summary>
        /// <value>The type of the current part.</value>
        public PasswordSafePartType CurrentPartType { get; protected set; }

        /// <summary>
        ///     Gets the header.
        /// </summary>
        /// <value>The header.</value>
        public PasswordSafeHeader Header { get; protected set; }

        /// <summary>
        ///     Gets the current record.
        /// </summary>
        /// <value>The record.</value>
        public PasswordSafeRecord Record { get; protected set; }

        #endregion Properties

        #region Public Method API

        /// <summary>
        ///     Sets the passphrase.
        /// </summary>
        /// <param name="passphrase">The passphrase.</param>
        public void SetPassphrase(
            string passphrase)
        {
            ReadTag();
            ReadKeys(passphrase);

            CurrentPartType = PasswordSafePartType.Keys;
        }

        /// <summary>
        ///     Reads the next part of the database.
        /// </summary>
        /// <returns>true if there was a next part</returns>
        public bool Read()
        {
            if (_error)
            {
                throw new InvalidOperationException("An error has already been reported.");
            }

            if (CurrentPartType == PasswordSafePartType.None)
            {
                _error = true;
                throw new InvalidOperationException("No passphrase set");
            }

            if (CurrentPartType == PasswordSafePartType.End)
            {
                return false;
            }

            if (CurrentPartType == PasswordSafePartType.Keys)
            {
                Header = ReadHeader();
                if (Header != null)
                {
                    CurrentPartType = PasswordSafePartType.Header;
                    return true;
                }

                return false;
            }

            if (CurrentPartType == PasswordSafePartType.Header || CurrentPartType == PasswordSafePartType.Record)
            {
                Record = ReadRecord();
                if (Record != null)
                {
                    CurrentPartType = PasswordSafePartType.Record;
                    return true;
                }
            }

            ReadHmac();
            CurrentPartType = PasswordSafePartType.End;

            return true;
        }

        /// <summary>
        ///     Reads one data record.
        /// </summary>
        /// <returns>The record</returns>
        private PasswordSafeRecord ReadRecord()
        {
            var fields = ReadGenericRecord();
            if (fields == null)
            {
                return null;
            }

            var record = new PasswordSafeRecord();
            var endSeen = false;
            foreach (var field in fields)
            {
                if (endSeen)
                {
                    throw new InvalidDataException("The END filed must be last in the header");
                }

                var data = field.DataBuffer();

                switch ((PasswordSafeRecordTypeCode)field.Type)
                {
                    case PasswordSafeRecordTypeCode.NAME:
                        throw new InvalidDataException("The NAME field type is not allowed here.");

                    case PasswordSafeRecordTypeCode.UUID:
                        if (data.Length != 16)
                        {
                            throw new InvalidDataException("UUID field length wrong");
                        }

                        record.Uuid = new Guid(data);
                        break;

                    case PasswordSafeRecordTypeCode.GROUP:
                        record.Group =
                            Encoding.UTF8.GetString(data, 0, data.Length); //.UTF8.GetString(data,0,data.Length);
                        break;

                    case PasswordSafeRecordTypeCode.TITLE:
                        record.Title = Encoding.UTF8.GetString(data, 0, data.Length);
                        break;

                    case PasswordSafeRecordTypeCode.USER:
                        record.User = Encoding.UTF8.GetString(data, 0, data.Length);
                        break;

                    case PasswordSafeRecordTypeCode.NOTES:
                        record.Notes = Encoding.UTF8.GetString(data, 0, data.Length);
                        break;

                    case PasswordSafeRecordTypeCode.PASSWORD:
                        record.PasswordValue = Encoding.UTF8.GetString(data, 0, data.Length);
                        break;

                    case PasswordSafeRecordTypeCode.CTIME:
                        record.TimeRecordCreatedUtc = GetUtcFromUnixTime(data);
                        break;

                    case PasswordSafeRecordTypeCode.PMTIME:
                        record.TimePasswordModifiedUtc = GetUtcFromUnixTime(data);
                        break;

                    case PasswordSafeRecordTypeCode.ATIME:
                        record.TimeRecordAccessedUtc = GetUtcFromUnixTime(data);
                        break;

                    case PasswordSafeRecordTypeCode.LTIME:
                        record.TimePasswordExpiresUtc = GetUtcFromUnixTime(data);
                        break;

                    case PasswordSafeRecordTypeCode.POLICY:
                        throw new InvalidDataException("The POLICY field type is not allowed here.");

                    case PasswordSafeRecordTypeCode.RMTIME:
                        record.TimeRecordModifiedUtc = GetUtcFromUnixTime(data);
                        break;

                    case PasswordSafeRecordTypeCode.URL:
                        record.ResourceLocator = Encoding.UTF8.GetString(data, 0, data.Length);
                        break;

                    case PasswordSafeRecordTypeCode.AUTOTYPE:
                        record.AutoType = Encoding.UTF8.GetString(data, 0, data.Length);
                        break;

                    case PasswordSafeRecordTypeCode.PWHIST:
                        var serializedHistory = Encoding.UTF8.GetString(data, 0, data.Length);
                        record.PasswordHistory = PasswordSafePasswordHistory.Parse(serializedHistory);
                        break;

                    case PasswordSafeRecordTypeCode.END:
                        endSeen = true;
                        break;

                    default:
                        record.UnknownFieldEntriesAdd(field);
                        break;
                }
            }

            return record;
        }

        /// <summary>
        ///     Reads the header.
        /// </summary>
        /// <returns>The Header</returns>
        private PasswordSafeHeader ReadHeader()
        {
            var fields = ReadGenericRecord();
            if (fields == null)
            {
                return null;
            }

            var header = new PasswordSafeHeader();
            var endSeen = false;
            foreach (var field in fields)
            {
                if (endSeen)
                {
                    throw new InvalidDataException("The END filed must be last in the header");
                }

                var data = field.DataBuffer();
                switch ((PasswordSafeHeaderTypeCode)field.Type)
                {
                    case PasswordSafeHeaderTypeCode.HDR_VERSION:
                        var majorVersion = data[1];
                        if (majorVersion != 3)
                        {
                            throw new InvalidDataException("Only support version 3 databases");
                        }

                        var minorVersion = data[0];
                        header.MinorVersion = minorVersion;
                        header.MajorVersion = majorVersion;
                        break;

                    case PasswordSafeHeaderTypeCode.HDR_UUID:
                        if (data.Length != 16)
                        {
                            throw new InvalidDataException("UUID field length wrong");
                        }

                        header.Uuid = new Guid(data);
                        break;

                    case PasswordSafeHeaderTypeCode.HDR_NDPREFS:
                        header.NonDefaultUserPrefs = Encoding.UTF8.GetString(data, 0, data.Length);
                        break;

                    case PasswordSafeHeaderTypeCode.HDR_DISPSTAT:
                        var displayStatus = Encoding.UTF8.GetString(data, 0, data.Length);
                        foreach (var c in displayStatus) header.TreeDisplayStatusAdd(c == '1');
                        break;

                    case PasswordSafeHeaderTypeCode.HDR_LASTUPDATETIME:
                        // Due to bug in pre-3.09, this was then stored as a hex value...
                        uint lastUnixUpdateTime = 0;
                        if (data.Length == 8)
                        {
                            var lastUpdateTimeHex = Encoding.UTF8.GetString(data, 0, data.Length);
                            lastUnixUpdateTime = uint.Parse(lastUpdateTimeHex, NumberStyles.HexNumber,
                                CultureInfo.InvariantCulture);
                        }
                        else if (data.Length == 4)
                        {
                            lastUnixUpdateTime = BitConverter.ToUInt32(data, 0);
                        }

                        var lastUpdateTimeUtc = DateTime.MinValue;
                        if (lastUnixUpdateTime != 0)
                        {
                            lastUpdateTimeUtc =
                                new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(lastUnixUpdateTime);
                        }

                        header.LastUpdateTimeUtc = lastUpdateTimeUtc;
                        break;

                    case PasswordSafeHeaderTypeCode.HDR_LASTUPDATEUSERHOST:
                        // This is actually deprecated, and we won't overwrite the values if they are set by the new format
                        if (string.IsNullOrEmpty(header.LastSavedBy) && string.IsNullOrEmpty(header.LastSavedOn))
                        {
                            var lastSavedByAndOn = Encoding.UTF8.GetString(data, 0, data.Length);
                            var lastSavedByLength = int.Parse(lastSavedByAndOn.Substring(0, 4), NumberStyles.HexNumber,
                                CultureInfo.InvariantCulture);
                            header.LastSavedBy = lastSavedByAndOn.Substring(4, lastSavedByLength);
                            header.LastSavedOn = lastSavedByAndOn.Substring(4 + lastSavedByLength);
                        }

                        break;

                    case PasswordSafeHeaderTypeCode.HDR_LASTUPDATEAPPLICATION:
                        header.WhatLastSaved = Encoding.UTF8.GetString(data, 0, data.Length);
                        break;

                    case PasswordSafeHeaderTypeCode.HDR_LASTUPDATEUSER:
                        header.LastUpdateUser = Encoding.UTF8.GetString(data, 0, data.Length);
                        break;

                    case PasswordSafeHeaderTypeCode.HDR_LASTUPDATEHOST:
                        header.LastUpdateHost = Encoding.UTF8.GetString(data, 0, data.Length);
                        break;

                    case PasswordSafeHeaderTypeCode.HDR_DBNAME:
                        header.DBName = Encoding.UTF8.GetString(data, 0, data.Length);
                        break;

                    case PasswordSafeHeaderTypeCode.HDR_DBDESC:
                        header.DBDescription = Encoding.UTF8.GetString(data, 0, data.Length);
                        break;

                    case PasswordSafeHeaderTypeCode.HDR_END:
                        endSeen = true;
                        break;

                    default:
                        header.UnknownFieldEntriesAdd(field);
                        break;
                }
            }

            return header;
        }

        #endregion Public Method API

        #region High-level Private Helpers

        /// <summary>
        ///     Reads the leading identifying tag.
        /// </summary>
        private void ReadTag()
        {
            var tagBytesActual = new byte[_tagBytes.Length];

            ReadBytes(tagBytesActual, "TAG");
            if (!BufferCompare(_tagBytes, tagBytesActual))
            {
                _error = true;
                throw new InvalidDataException("Invalid value of TAG");
            }
        }

        /// <summary>
        ///     Reads the databse preamble before the headers, containing the file identifier
        ///     and the session and HMAC key, and passphrase verifier.
        /// </summary>
        /// <param name="passphrase">The passphrase.</param>
        private void ReadKeys(
            string passphrase)
        {
            var saltBytes = new byte[32];
            ReadBytes(saltBytes, "SALT");

            var iterBytes = new byte[4];
            ReadBytes(iterBytes, "ITER");

            if (!BitConverter.IsLittleEndian)
            {
                throw new InvalidProgramException("We must be little endian!");
            }

            var iterations = BitConverter.ToInt32(iterBytes, 0);
            if (iterations < 0)
            {
                throw new InvalidDataException("Invalid iteration counter");
            }

            var expectedStretchedPassphraseHashBytes = new byte[32];
            ReadBytes(expectedStretchedPassphraseHashBytes, "H(P')");

            var stretchedKey = StretchKey(saltBytes, passphrase, iterations);
            var stretchedPassphraseHashBytes = SHA256.Create().ComputeHash(stretchedKey);

            // Check if the stored hashed streched key compares ok with the user provided.
            if (!BufferCompare(stretchedPassphraseHashBytes, expectedStretchedPassphraseHashBytes))
            {
                throw new InvalidDataException("Invalid passphrase");
            }

            // Create the TwoFish ECB transform from the stretched key, to decrypt session and hmac keys with
            using (var keyDecryption = new Twofish())
            {
                keyDecryption.BlockSize = 128;
                keyDecryption.Mode = CipherMode.ECB;
                keyDecryption.KeySize = 256;
                keyDecryption.Key = stretchedKey;
                using (var keyDecryptor = keyDecryption.CreateDecryptor())
                {
                    var encryptedSessionKeyK = new byte[32];
                    ReadBytes(encryptedSessionKeyK, "Encrypted Session Key K (B1|B2)");

                    var sessionKeyBytes = new byte[32];
                    // Due to limitation in TwoFish implementation, we encrypt a block at a time
                    keyDecryptor.TransformBlock(encryptedSessionKeyK, 0, 16, sessionKeyBytes, 0);
                    keyDecryptor.TransformBlock(encryptedSessionKeyK, 16, 16, sessionKeyBytes, 16);

                    var encryptedHmacKeyL = new byte[32];
                    ReadBytes(encryptedHmacKeyL, "Encrypted HMAC Key L (B3|B4)");

                    var hmacKeyBytes = new byte[32];
                    // Due to limitation in TwoFish implementation, we encrypt a block at a time
                    keyDecryptor.TransformBlock(encryptedHmacKeyL, 0, 16, hmacKeyBytes, 0);
                    keyDecryptor.TransformBlock(encryptedHmacKeyL, 16, 16, hmacKeyBytes, 16);

                    var initializationVectorBytes = new byte[16];
                    ReadBytes(initializationVectorBytes, "IV");

                    // Initialize and save the Decryptor
                    var dataDecryption = new Twofish();
                    dataDecryption.BlockSize = 128;
                    dataDecryption.Mode = CipherMode.CBC;
                    dataDecryption.KeySize = 256;
                    _decryptor = dataDecryption.CreateDecryptor(sessionKeyBytes, initializationVectorBytes);

                    // Initialize and save the HMAC calculator
                    _hmac = new HMACSHA256(hmacKeyBytes);
                }
            }
        }

        /// <summary>
        ///     Reads the hmac, and compares with the calculated value.
        /// </summary>
        private void ReadHmac()
        {
            var hmacBytes = new byte[32];
            ReadBytes(hmacBytes, "HMAC");

            _hmac.TransformFinalBlock(new byte[0], 0, 0);
            var actualHmac = _hmac.Hash;
            if (!BufferCompare(actualHmac, hmacBytes))
            {
                //throw new InOut.InvalidDataException("Error in HMAC - possible file corruption");
                // ALPHONS TODO
            }
        }

        /// <summary>
        ///     Reads one generic record, which may be a header or a data record.
        /// </summary>
        /// <returns>A collection of the fields</returns>
        private ICollection<PasswordSafeRecordField> ReadGenericRecord()
        {
            var fields = new List<PasswordSafeRecordField>();

            var emergencyExitCounter = 255;
            PasswordSafeRecordField field;
            do
            {
                field = ReadField();
                if (field == null)
                {
                    if (fields.Count > 0)
                    {
                        throw new InvalidDataException("End of file marker in the middle of a record");
                    }

                    return null;
                }

                fields.Add(field);
            } while (field.Type != 255 && --emergencyExitCounter > 0);

            if (emergencyExitCounter <= 0)
            {
                throw new InvalidDataException("Too many fields in a record");
            }

            return fields;
        }

        /// <summary>
        ///     Reads one field.
        /// </summary>
        /// <returns></returns>
        private PasswordSafeRecordField ReadField()
        {
            var currentBlock = new byte[16];

            ReadBytes(currentBlock, "Generic Field");
            if (BufferCompare(currentBlock, _eofMarkerBytes))
            {
                return null;
            }

            _decryptor.TransformBlock(currentBlock, 0, currentBlock.Length, currentBlock, 0);

            if (!BitConverter.IsLittleEndian)
            {
                throw new InvalidProgramException("We must be little endian!");
            }

            var fieldLength = BitConverter.ToInt32(currentBlock, 0);
            if (fieldLength < 0)
            {
                throw new InvalidDataException("Invalid field length");
            }

            var type = currentBlock[4];
            var dataBuffer = new byte[fieldLength];

            // There's a maximum of 11 bytes left of data in the first block.
            var lengthToCopy = fieldLength > 11 ? 11 : fieldLength;
            Buffer.BlockCopy(currentBlock, 5, dataBuffer, 0, lengthToCopy);
            fieldLength -= lengthToCopy;

            var currentOffset = lengthToCopy;
            // Now we've handled the first block. Let's see if there's more....
            if (fieldLength > 0)
            {
                var numberOfBlocks = (fieldLength - 1) / 16 + 1;
                while (numberOfBlocks-- > 0)
                {
                    ReadBytes(currentBlock, "Generic Field");
                    if (BufferCompare(currentBlock, _eofMarkerBytes))
                    {
                        throw new InvalidDataException("Unexpected end of file marker");
                        ;
                    }

                    _decryptor.TransformBlock(currentBlock, 0, currentBlock.Length, currentBlock, 0);

                    lengthToCopy = currentBlock.Length > fieldLength ? fieldLength : currentBlock.Length;
                    Buffer.BlockCopy(currentBlock, 0, dataBuffer, currentOffset, lengthToCopy);
                    fieldLength -= lengthToCopy;
                    currentOffset += lengthToCopy;
                }
            }

            var tmpNull = new byte[dataBuffer.Length];

            // We only HMAC the actual data - not the length and type. That *SUCKS*!!!
            _hmac.TransformBlock(dataBuffer, 0, dataBuffer.Length, tmpNull, 0);

            return new PasswordSafeRecordField(type, dataBuffer);
        }

        #endregion High-level Private Helpers

        #region Low-level Private Helpers

        /// <summary>
        ///     Gets the UTC from unix time.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>The date and time as UTC</returns>
        private static DateTime GetUtcFromUnixTime(
            byte[] data)
        {
            if (data.Length != 4)
            {
                throw new InvalidDataException("Unix time must be a 32-bit integer");
            }

            var unixTime = BitConverter.ToInt32(data, 0);
            var netTime = PasswordSafeUtility.GetUtcFromUnixTime(unixTime);

            return netTime;
        }

        /// <summary>
        ///     Reads bytes from the stream
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="what">A string identifying what is read - used for identifying purposes if an error occurs.</param>
        private void ReadBytes(
            byte[] bytes,
            string what)
        {
            if (_stream.Read(bytes, 0, bytes.Length) != bytes.Length)
            {
                _error = true;
                throw new InvalidDataException(string.Format(CultureInfo.InvariantCulture, "Could not read {0} bytes",
                    what));
            }
        }

        /// <summary>
        ///     Just a simple byte-buffer comparer
        /// </summary>
        /// <param name="bytes1">The bytes1.</param>
        /// <param name="bytes2">The bytes2.</param>
        /// <returns>true if the buffers contain identical data</returns>
        private static bool BufferCompare(
            byte[] bytes1,
            byte[] bytes2)
        {
            if (bytes1 == null)
            {
                throw new ArgumentNullException("bytes1");
            }

            if (bytes2 == null)
            {
                throw new ArgumentNullException("bytes2");
            }

            if (bytes1.Length != bytes2.Length)
            {
                return false;
            }

            for (var i = 0; i < bytes1.Length; ++i)
                if (bytes1[i] != bytes2[i])
                {
                    return false;
                }

            return true;
        }

        /// <summary>
        ///     Stretches the key by first hashing passphrase|salt, then iteratively hashing
        ///     the hash, for the specified number of iterations. 'Stretching' in this context
        ///     means artificially applying an extra work-factor so as to effectively lengthen
        ///     the effective bit-length of the provided passphrase.
        /// </summary>
        /// <param name="salt">The salt.</param>
        /// <param name="passphrase">The passphrase.</param>
        /// <param name="iterations">The iterations.</param>
        /// <returns></returns>
        private static byte[] StretchKey(
            byte[] salt,
            string passphrase,
            int iterations)
        {
            var ansiEncodedPassphrase =
                passphrase.Select(x => (byte)x).ToArray(); //SHA256.ASCIIEncoder(passphrase);

            byte[] iteratedHashValue;
            using (SHA256 hash = new SHA256Managed())
            {
                hash.TransformBlock(ansiEncodedPassphrase, 0, ansiEncodedPassphrase.Length, null, 0);
                hash.TransformFinalBlock(salt, 0, salt.Length);
                iteratedHashValue = hash.Hash;
            }

            for (var i = 0; i < iterations; ++i)
            {
                using SHA256 iterationHash = new SHA256Managed();

                iteratedHashValue = iterationHash.ComputeHash(iteratedHashValue);
            }

            return iteratedHashValue;
        }

        #endregion Low-level Private Helpers

        #region IDisposable Members

        public void Close()
        {
            Dispose(true);
        }

        protected virtual void Dispose(
            bool disposing)
        {
            if (disposing)
            {
                if (_stream != null)
                {
                    _stream.Close();
                    _stream = null;
                }

                if (_decryptor != null)
                {
                    _decryptor.Dispose();
                    _decryptor = null;
                }

                if (_hmac != null)
                {
                    _hmac.Clear();
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion IDisposable Members
    }
}