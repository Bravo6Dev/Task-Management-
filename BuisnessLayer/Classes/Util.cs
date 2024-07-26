using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLayer.Classes
{
    public class Util
    {
        static public string Hashing(string input)
        {
            using (SHA256 sHA256 = SHA256.Create())
            {
                sHA256.ComputeHash(Encoding.UTF8.GetBytes(input));
                return BitConverter.ToString(sHA256.Hash).Replace("-", "").ToLower();
            }
        }
    }
}
