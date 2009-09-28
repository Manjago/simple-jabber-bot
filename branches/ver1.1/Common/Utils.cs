using System;
using System.Text;
using System.Security.Cryptography;

namespace Temnenkov.SJB.Common
{
    public static class Utils
    {
        [System.Runtime.InteropServices.DllImport("kernel32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        private static extern int GetModuleFileName(IntPtr hModule, StringBuilder buffer, int length);

        private static string GetLocalPath(string fileName)
        {
            var uri1 = new Uri(fileName);
            return (uri1.LocalPath + uri1.Fragment);
        }

        public static string GetExecutablePath()
        {
            string executablePath;
            var assembly1 = System.Reflection.Assembly.GetEntryAssembly();
            if (assembly1 == null)
            {
                var builder1 = new StringBuilder(260);
                GetModuleFileName(IntPtr.Zero, builder1, builder1.Capacity);
                executablePath = System.IO.Path.GetFullPath(builder1.ToString());
            }
            else
            {
                var text1 = assembly1.EscapedCodeBase;
                var uri1 = new Uri(text1);
                executablePath = uri1.Scheme == "file" ? GetLocalPath(text1) : uri1.ToString();
            }

            var uri2 = new Uri(executablePath);
            if (uri2.Scheme == "file")
            {
                new System.Security.Permissions.FileIOPermission(System.Security.Permissions.FileIOPermissionAccess.PathDiscovery, executablePath).Demand();
            }
            return executablePath;
        }

        // Hash an input string and return the hash as
        // a 32 character hexadecimal string.
        public static string GetMd5Hash(string input)
        {
            if (String.IsNullOrEmpty(input))
                return String.Empty;

            // Create a new instance of the MD5CryptoServiceProvider object.
            var md5Hasher = MD5.Create();

            // Convert the input string to a byte array and compute the hash.
            var data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            var sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (var i = 0; i < data.Length; i++)
                sBuilder.Append(data[i].ToString("x2"));

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
    }
}
