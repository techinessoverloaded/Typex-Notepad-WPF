using System.Windows;
using TypeX_Notepad.Models;
using System.IO;
using System;

namespace TypeX_Notepad
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
       private void Application_Startup(object sender,StartupEventArgs e)
        {
            if (e.Args.Length == 0)
            {
                new MainWindow().Show();
            }
            else
            {
                for (int i = 0; i < e.Args.Length; ++i)
                {
                    try
                    {
                        DocumentModel document = new DocumentModel();
                        document.FilePath = e.Args[i];
                        StreamReader reader = new StreamReader(document.FilePath, true);
                        document.Content = reader.ReadToEnd();
                        document.Encoding = reader.CurrentEncoding;
                        reader.Close();
                        new MainWindow(document).Show();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "TypeX Notepad", MessageBoxButton.OK);
                    }
                }
            }
       }
    }
}
