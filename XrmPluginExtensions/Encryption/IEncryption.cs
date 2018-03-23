
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCLCC.XrmPluginExtensions.Encryption
{
    public interface IEncryption
    {
        string Encrypt(string clearText, string key);
        byte[] Encrypt(byte[] clearData, string key);
        string Decrypt(string encryptedText, string key);
        byte[] Decrypt(byte[] encryptedData, string key);
    }
}
