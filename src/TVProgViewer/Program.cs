﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using TVProgViewer.TVProgApp.Classes;
using TVProgViewer.TVProgApp.Logger;
using TVProgViewer.TVProgApp.Properties;

namespace TVProgViewer.TVProgApp
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>


        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);


            ExportBuilder.OnExportCompleted += new ExportBuilder.ExportCompleted(ExportBuilder_OnExportCompleted);

            CultureInfo ci = new CultureInfo(Settings.Default.LocalizationSetting);
            Thread.CurrentThread.CurrentCulture = ci;

            Thread.CurrentThread.CurrentUICulture = ci;
            
            bool onlyInstance;
                        
            if (Settings.Default.OneCopyApp)
            {
                Mutex mtx = new Mutex(false, "TVProgViewer.TVProgApp", out onlyInstance);
                // Если другие процессы не владеют мьютексом, то
                // Приложение запущено в единственном экземпляре
                if (onlyInstance)
                {
                    Application.Run(new Welcome());
                }
                else
                {
                    MessageBox.Show(Resources.AppAlreadyRunText, Resources.ErrorText, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    Application.Exit();
                    return;
                }
            }
            else
            {
                Application.Run(new Welcome());
            }
        }

        static void ExportBuilder_OnExportCompleted(string file)
        {
            var rd = Application.StartupPath + "\\Export\\";
            if (!Directory.Exists(rd))
            {
                Directory.CreateDirectory(rd);
            }
            if (File.Exists(rd + file))
            {
                var showdir = new Process() { StartInfo = { FileName = "explorer", Arguments = @"/select," + (rd + file) } };
                showdir.Start();
            }
            else
            {
                MessageBox.Show(Resources.DontShowExportResText1 + file + Resources.DontShowExportResText2,
                                Resources.ExportErrorText, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
    }
}
