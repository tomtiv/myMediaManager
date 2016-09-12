namespace Utility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    class DisplayMessage
    {
/*
        //[STAThread]
        static void DisplayMessage()
        {
            IsConsole = AllocConsole();
        }

        static String GetMessage(String message)
        {
            if (IsConsole)
            {
//                return Console.WriteLine(message.to);
            }
            else
            {
                return
             }
        }
*/

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

         static bool IsConsole { get; set; }
    }
}
