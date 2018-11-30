using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Riverside.Cms.Utilities.Security.Encryption
{
    public class EncryptionService : IEncryptionService
    {
        /// <summary>
        /// Compares two byte arrays. Returns true if their contents are the same.
        /// </summary>
        /// <param name="byteArray1">Byte array 1.</param>
        /// <param name="byteArray2">Byte array 2.</param>
        /// <returns>True if arrays equal (i.e. same length and containing the same bytes).</returns>
        public bool ByteArraysEqual(byte[] byteArray1, byte[] byteArray2)
        {
            // Byte arrays are equal if references are the same
            if (byteArray1 == byteArray2)
                return true;

            // Byte arrays are not equal if one or other array is null
            if (byteArray1 == null || byteArray2 == null)
                return false;

            // Byte arrays are not equal if lengths are not the same
            if (byteArray1.Length != byteArray2.Length)
                return false;

            // Inspect individual bytes to determine if byte arrays are the same
            for (int index = 0; index < byteArray1.Length; index++)
                if (byteArray1[index] != byteArray2[index])
                    return false;

            //  Byte arrays must be equal
            return true;
        }

        /// <summary>
        /// Gets salt and salted hash of the plain text password supplied.
        /// See http://stackoverflow.com/questions/2138429/hash-and-salt-passwords-in-c-sharp for more details.
        /// </summary>
        /// <param name="password">Plain text password.</param>
        /// <param name="salt">Used to salt hash of password.</param>
        /// <returns>Salted hash of plain text password.</returns>
        public byte[] EncryptPassword(string password, byte[] salt)
        {
            byte[] saltedHash = null;
            byte[] plainText = Encoding.UTF8.GetBytes(password);
            using (HashAlgorithm hashAlgorithm = new SHA256Managed())
            {
                byte[] plainTextWithSaltBytes = new byte[plainText.Length + salt.Length];
                for (int index = 0; index < plainText.Length; index++)
                    plainTextWithSaltBytes[index] = plainText[index];
                for (int index = 0; index < salt.Length; index++)
                    plainTextWithSaltBytes[plainText.Length + index] = salt[index];
                saltedHash = hashAlgorithm.ComputeHash(plainTextWithSaltBytes);
            }
            return saltedHash;
        }

        /// <summary>
        /// Converts text to byte array.
        /// </summary>
        /// <param name="text">The text to convert.</param>
        /// <returns>The resuting byte array.</returns>
        public byte[] GetBytes(string text)
        {
            return Convert.FromBase64String(text);
        }
    }
}
