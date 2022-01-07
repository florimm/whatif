using System;
using System.Security.Cryptography;
using System.Text;

namespace WhatIf.Api.Utils
{
    // Temporary solution
    public static class StringUtil
    {
        private static byte[] key = new byte[8] {1, 2, 3, 4, 5, 6, 7, 8};
        private static byte[] iv = new byte[8] {1, 2, 3, 4, 5, 6, 7, 8};

        public static string Encrypt(this string text)
        {
            SymmetricAlgorithm algorithm = DES.Create();
            ICryptoTransform transform = algorithm.CreateEncryptor(key, iv);
            byte[] inputbuffer = Encoding.Unicode.GetBytes(text);
            byte[] outputBuffer = transform.TransformFinalBlock(inputbuffer, 0, inputbuffer.Length);
            return Convert.ToBase64String(outputBuffer);
        }
    }
    
}