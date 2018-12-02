using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Analysis.Work.Execute
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            try
            {
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new WorkHandler());
            }
            catch (Exception EX)
            {
                System.Windows.Forms.MessageBox.Show(EX.Message + EX.StackTrace);
               throw EX;
            }
           
        }
    }
}
