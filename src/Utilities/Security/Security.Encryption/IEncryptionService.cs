using System;
using System.Collections.Generic;
using System.Text;

namespace Riverside.Cms.Utilities.Security.Encryption
{
    public interface IEncryptionService
    {
        /// <summary>
        /// Compares two byte arrays. Returns true if their contents are the same.
        /// </summary>
        /// <param name="byteArray1">Byte array 1.</param>
        /// <param name="byteArray2">Byte array 2.</param>
        /// <returns>True if arrays equal (i.e. same length and containing the same bytes).</returns>
        bool ByteArraysEqual(byte[] byteArray1, byte[] byteArray2);

        /// <summary>
        /// Gets salt and salted hash of the plain text password supplied.
        /// </summary>
        /// <param name="password">Plain text password.</param>
        /// <param name="salt">Used to salt hash of password.</param>
        /// <returns>Salted hash of plain text password.</returns>
        byte[] EncryptPassword(string password, byte[] salt);

        /// <summary>
        /// Converts text to byte array.
        /// </summary>
        /// <param name="text">The text to convert.</param>
        /// <returns>The resuting byte array.</returns>
        byte[] GetBytes(string text);
    }
}
