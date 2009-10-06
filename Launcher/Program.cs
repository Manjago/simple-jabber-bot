using System;
using System.Windows.Forms;
using System.Threading;
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
                    using (var p = new Process())
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

// ReSharper disable FunctionNeverReturns
		}
// ReSharper restore FunctionNeverReturns

        private static bool OurProcessIsAbsent()
        {
            foreach (var p in Process.GetProcesses())
            {
                if (Module.Equals(p.ProcessName))
                    return false;
            }
            return true;
        }


    }
}