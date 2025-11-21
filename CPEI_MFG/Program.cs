using CPEI_MFG.View;
using System;
using System.Threading;
using System.Windows.Forms;

namespace CPEI_MFG
{
    static class Program
    {
        static Mutex newMutex;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool canCreate;
            newMutex = new Mutex(true, "Already", out canCreate);
            if (!canCreate)
            {
                MessageBox.Show("Test Program already running", "Warning");
            }
            else
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }
        }
    }
}
