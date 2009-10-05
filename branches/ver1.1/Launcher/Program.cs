using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Text;
using System.Diagnostics;

namespace Temnenkov.SJB.Launcher
{
	static class Program
	{
        private const string Module = "Temnenkov.SJB.Bot";

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
            while (true)
            {
                while (OurProcessIsAbsent())
                {
                    using (Process p = new Process())
                    {
                        p.StartInfo.FileName = string.Format("{0}.exe", Module); 
                        p.StartInfo.WorkingDirectory = Application.StartupPath;
                        p.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                        p.Start();
                    }

                    Thread.Sleep(1000 * 20);
                }
                Thread.Sleep(1000 * 60 * 5);
            }

		}

        private static bool OurProcessIsAbsent()
        {
            foreach (Process p in Process.GetProcesses())
            {
                System.Diagnostics.Debug.WriteLine(p.ProcessName);
                if (Module.Equals(p.ProcessName))
                    return false;
            }
            return true;
        }


    }
}