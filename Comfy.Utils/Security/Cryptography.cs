using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace Comfy.Utils.Security
{
    /// <summary>
    /// String encrypt/decrypt.
    /// </summary>
    public class Cryptography
    {
        /// <summary>
        /// MD5 encrypt.
        /// </summary>
        /// <param name="passWord">String to encrypt</param>
        /// <returns>String encrypted</returns>
        public static string Md5(string passWord)
        {
            Byte[] clearBytes = new UnicodeEncoding().GetBytes(passWord);
            Byte[] hashedBytes = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(clearBytes);
            return BitConverter.ToString(hashedBytes).Replace("-", "");
        }

        /// <summary>
        /// SHA1 encrypt.
        /// </summary>
        /// <param name="passWord">String to encrypt</param>
        /// <returns>String encrypted</returns>
        public static string Sha1(string passWord)
        {
            Byte[] clearBytes = new UnicodeEncoding().GetBytes(passWord);
            Byte[] hashedBytes = ((HashAlgorithm)CryptoConfig.CreateFromName("SHA1")).ComputeHash(clearBytes);
            return BitConverter.ToString(hashedBytes);
        }

        /// <summary>
        /// Rijndael(AES) Encrypt.
        /// </summary>
        /// <param name="source">Message to encrypt</param>
        /// <param name="aesKey">Key</param>
        /// <returns>Message encrypted</returns>
        public static string AesEncrypt(string source, string aesKey)
        {
            byte[] bytIn = Encoding.UTF8.GetBytes(source);

            //Bulid the Rijndael Key.
            byte[] bKey = new byte[32];
            Array.Copy(Encoding.UTF8.GetBytes(aesKey.PadRight(bKey.Length)), bKey, bKey.Length);

            //Build the vector.
            string aesIV = "KnXmC0KXxB3aQ4Hb";
            byte[] bVector = new byte[16];
            Array.Copy(Encoding.UTF8.GetBytes(aesIV.PadRight(bVector.Length)), bVector, bVector.Length);

            //Create the Rijndael Object.
            Rijndael aes = Rijndael.Create();

            using (MemoryStream mStream = new MemoryStream())
            {
                using (CryptoStream cStream = new CryptoStream(mStream,
                    aes.CreateEncryptor(bKey, bVector),
                    CryptoStreamMode.Write))
                {
                    cStream.Write(bytIn, 0, bytIn.Length);
                    cStream.FlushFinalBlock();
                    return Convert.ToBase64String(mStream.ToArray());
                }
            }
        }

        /// <summary>
        /// Rijndael(AES) Decrypt.
        /// </summary>
        /// <param name="source">Message to decrypt</param>
        /// <param name="aesKey">Key</param>
        /// <returns>Message decrypted</returns>
        public static string AesDecrypt(string source, string aesKey)
        {
            byte[] bytIn = Convert.FromBase64String(source);

            //Bulid the Rijndael Key.
            byte[] bKey = new byte[32];
            Array.Copy(Encoding.UTF8.GetBytes(aesKey.PadRight(bKey.Length)), bKey, bKey.Length);

            //Build the vector.
            string aesIV = "KnXmC0KXxB3aQ4Hb";
            byte[] bVector = new byte[16];
            Array.Copy(Encoding.UTF8.GetBytes(aesIV.PadRight(bVector.Length)), bVector, bVector.Length);

            //Create the Rijndael Object.
            Rijndael aes = Rijndael.Create();

            using (MemoryStream mStraem = new MemoryStream(bytIn))
            {
                using (CryptoStream cStream = new CryptoStream(mStraem,
                    aes.CreateDecryptor(bKey, bVector),
                    CryptoStreamMode.Read))
                {
                    using (StreamReader sReader = new StreamReader(cStream))
                    {
                        return sReader.ReadToEnd();
                    }
                }
            }
        }
    }
}

