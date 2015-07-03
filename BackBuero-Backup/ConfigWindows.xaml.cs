using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Forms;
using Microsoft.Win32.TaskScheduler;
using System.Diagnostics;

namespace BackBuero_Backup
{
    /// <summary>
    /// Interaktionslogik für ConfigWindows.xaml
    /// </summary>
    public partial class ConfigWindows : Window
    {
        public ConfigWindows()
        {
            InitializeComponent();
            if (MainWindow.conf.getBackupPath() != null)
            {
                cycleTB.Text = MainWindow.conf.getBackupCycle().ToString();
                pathTB.Text = MainWindow.conf.getBackupPath();
            }
        }

        private void FileDialog_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("Bitte denken Sie bei der Auswahl des Verzeichnisses daran,\n"
                + "dass eine Datensicherung auf der lokalen Festplatte im Falle eines Defektes nicht weiterhilft.\n"
                + "Sichern Sie deshalb Ihre Daten auf einem USB-Stick.", "Hinweis - BackBüroBackup", MessageBoxButton.OK, MessageBoxImage.Information);

            FolderBrowserDialog fbd = new FolderBrowserDialog();
            DialogResult result = fbd.ShowDialog();
            pathTB.Text = fbd.SelectedPath;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.conf.setBackupCycle(Convert.ToInt32(cycleTB.Text));
            MainWindow.conf.setBackupPath(pathTB.Text);
            if(MainWindow.conf.getLastBackup() == null) MainWindow.conf.setLastBackup("01.01.1990");

            if (!MainWindow.conf.WriteConfig())
            {
                System.Windows.MessageBox.Show("Es ist ein Fehler beim Speichern der Konfiguration aufgetreten.", "Fehler - BackBüroBackup", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                MainWindow.getInstance().UpdateLabel();
                SetBackupNotifier();
                this.Close();
            }
        }

        private void SetBackupNotifier()
        {
            string notiferPath = System.Windows.Forms.Application.StartupPath + @"\BackupNotifier.exe";
            using (TaskService ts = new TaskService())
            {
                TaskCollection tc = ts.RootFolder.GetTasks();
                if (tc.Exists("BBBackup"))
                {
                    ts.RootFolder.DeleteTask("BBBackup");
                }
                
                TaskDefinition td = ts.NewTask();
                td.RegistrationInfo.Description = "BackBueroBackup notifier";
                td.Triggers.Add(new DailyTrigger { DaysInterval = (short)MainWindow.conf.getBackupCycle() });
                td.Actions.Add(new ExecAction(notiferPath));
                ts.RootFolder.RegisterTaskDefinition("BBBackup", td);
            }
        }
    }
}
