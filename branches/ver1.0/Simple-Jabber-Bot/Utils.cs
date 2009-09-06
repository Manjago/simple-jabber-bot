using System;
using System.Collections.Generic;
using System.Text;

namespace Temnenkov.SimpleJabberBot
{
    public static class Utils
    {
        [System.Runtime.InteropServices.DllImport("kernel32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        private static extern int GetModuleFileName(IntPtr hModule, System.Text.StringBuilder buffer, int length);

        private static string GetLocalPath(string fileName)
        {
            Uri uri1 = new Uri(fileName);
            return (uri1.LocalPath + uri1.Fragment);
        }

        public static string GetExecutablePath()
        {
            string executablePath = "";
            System.Reflection.Assembly assembly1 = System.Reflection.Assembly.GetEntryAssembly();
            if (assembly1 == null)
            {
                System.Text.StringBuilder builder1 = new System.Text.StringBuilder(260);
                GetModuleFileName(IntPtr.Zero, builder1, builder1.Capacity);
                executablePath = System.IO.Path.GetFullPath(builder1.ToString());
            }
            else
            {
                string text1 = assembly1.EscapedCodeBase;
                Uri uri1 = new Uri(text1);
                if (uri1.Scheme == "file")
                {
                    executablePath = GetLocalPath(text1);
                }
                else
                {
                    executablePath = uri1.ToString();
                }
            }

            Uri uri2 = new Uri(executablePath);
            if (uri2.Scheme == "file")
            {
                new System.Security.Permissions.FileIOPermission(System.Security.Permissions.FileIOPermissionAccess.PathDiscovery, executablePath).Demand();
            }
            return executablePath;
        }
    }
}
