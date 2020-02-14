using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using System.Text;

namespace JA
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //GenLicense();
            //SagTensTests.TestCatSol();
            //SagTensTests.TestSagTension();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new JA.CatenarySpans.CatenaryForm());
        }

        static void GenLicense()
        {
            Guid guid;
            string strGuid = null;
            do
            {
                guid = Guid.NewGuid();
                strGuid = guid.ToString();
                //byte[] guidBytes = guid.ToByteArray();
                //byte[] userBytes = Encoding.UTF7.GetBytes(Environment.UserName);
                //var sha1 = guidBytes.Concat(userBytes).HashSha1();
                var sha1 = (strGuid + Environment.UserName).HashSha1();
                strGuid += "-" + sha1.ToString("x8");

            } while (strGuid.Length!=53);

            Debug.WriteLine(strGuid);
        }

    }


}
