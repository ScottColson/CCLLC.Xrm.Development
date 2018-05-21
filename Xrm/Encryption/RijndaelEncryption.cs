using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CCLLC.Xrm.Sdk.Encryption
{
    public class RijndaelEncryption : IRijndaelEncryption
    {
        private readonly string defaultKey = "7a5a64brEgaceqenuyegac7era3Ape6aWatrewegeka94waqegayathudrebuc7t";
        private readonly byte[] salt = { 73, 118, 97, 110, 32, 77, 101, 100, 118, 101, 100, 101, 118 };

        public string DefaultKey
        {
            get { return defaultKey; }
        }

        public string Decrypt(string cipherText, string Password)
        {
            byte[] cipherData = Convert.FromBase64String(cipherText);
            byte[] clearData = this.Decrypt(cipherData, Password);
            return Encoding.Unicode.GetString(clearData);
        }

        public byte[] Decrypt(byte[] cipherData, string Password)
        {
            PasswordDeriveBytes passwordDeriveByte = new PasswordDeriveBytes(Password, salt);
            return this.Decrypt(cipherData, passwordDeriveByte.GetBytes(32), passwordDeriveByte.GetBytes(16));
        }

        private byte[] Decrypt(byte[] cipherData, byte[] Key, byte[] IV)
        {
            MemoryStream memoryStream = new MemoryStream();
            Rijndael key = Rijndael.Create();
            key.Key = Key;
            key.IV = IV;
            CryptoStream cryptoStream = new CryptoStream(memoryStream, key.CreateDecryptor(), CryptoStreamMode.Write);
            cryptoStream.Write(cipherData, 0, (int)cipherData.Length);
            cryptoStream.Close();
            return memoryStream.ToArray();
        }

        public string Encrypt(string clearText, string Password)
        {
            byte[] clearData = Encoding.Unicode.GetBytes(clearText);
            byte[] encryptedData = this.Encrypt(clearData, Password);
            return Convert.ToBase64String(encryptedData);
        }

        public byte[] Encrypt(byte[] clearData, string Password)
        {
            PasswordDeriveBytes passwordDeriveByte = new PasswordDeriveBytes(Password, salt);
            return this.Encrypt(clearData, passwordDeriveByte.GetBytes(32), passwordDeriveByte.GetBytes(16));
        }

        private byte[] Encrypt(byte[] clearData, byte[] Key, byte[] IV)
        {
            MemoryStream memoryStream = new MemoryStream();
            Rijndael key = Rijndael.Create();
            key.Key = Key;
            key.IV = IV;
            CryptoStream cryptoStream = new CryptoStream(memoryStream, key.CreateEncryptor(), CryptoStreamMode.Write);
            cryptoStream.Write(clearData, 0, (int)clearData.Length);
            cryptoStream.Close();
            byte[] array = memoryStream.ToArray();
            byte[] numArray = array;
            return numArray;
        }
    }
}