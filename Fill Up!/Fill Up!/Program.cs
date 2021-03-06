﻿using System;
using System.Windows.Forms;
using Fill_Up_.Fill_Up_;

namespace Fill_Up_
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            ListsOfBars allbars = new ListsOfBars();
            
            ReadFile load = new ReadFile();
            load.ReadData(allbars);
           
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Home(allbars));

            WriteFile write = new WriteFile();
            write.WriteToFile(allbars);
        }
    }
}
