using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Util;

namespace BackBuero_Backup
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Utility util = new Utility();
        public static Config conf = new Config();

        private static MainWindow instance;

        public MainWindow()
        {
            InitializeComponent();
            instance = this;

            if (!util.IsUserAdministrator())
            {
                MessageBox.Show("Sie verfügen über keine Administratorrechte!\nSie können diese Anwendung leider nicht ausführen", "Keine Rechte! - BackBüroBackup", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(0);
            }
            if (!conf.ReadConfig())
            {
                MessageBox.Show("Es scheint, als würden Sie die Anwendung das erste Mal starten.\nBitte stellen Sie deshalb die Software nach ihren Bedürfnissen ein.", "Erster Start - BackBüreBackup", MessageBoxButton.OK, MessageBoxImage.Information);
                ConfigWindows confWindow = new ConfigWindows();
                confWindow.ShowInTaskbar = false;
                confWindow.ShowDialog();
            }
            else
            {
                UpdateLabel();
            }
        }

        public static MainWindow getInstance()
        {
            return instance;
        }

        public void UpdateLabel()
        {
            lastBackupLbl.Content = conf.getLastBackup();
            pathLbl.Content = conf.getBackupPath();
        }

        private void StartBackup_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult msgBoxResult = MessageBox.Show("Bitte schließen Sie BackBüro, bzw. lassen es während der Datensicherung geschlossen."
                + "\nOK, wenn Sie fortfahren möchten.", "HINWEIS - BackBüroBackup", MessageBoxButton.OKCancel, MessageBoxImage.Warning);

            switch (msgBoxResult)
            {
                case MessageBoxResult.Cancel:
                    return;
                case MessageBoxResult.OK:
                    break;
                default:
                    return;
            }

            const string MssqlPath = @"C:\Program Files\Microsoft SQL Server\MSSQL10_50.BB\MSSQL\DATA";
            if (Directory.Exists(conf.getBackupPath()))
            {
                startBackup.IsEnabled = false;
                progressBar.Minimum = 0;
                progressBar.Maximum = 7;
                progressBar.Value = 0;
                progressBar.Value++; progressBar.Refresh();
                
                //================== 1
                conf.setLastBackup();
                string[] datePath = conf.getLastBackup().Split('.');
                string path = conf.getBackupPath() + @"\" + datePath[2] + @"\" + datePath[1] + @"\" + datePath[0];
                Directory.CreateDirectory(path);
                progressBar.Value++; progressBar.Refresh();
                //================== 2
                var process = Process.Start("CMD.exe", "/C NET STOP MSSQL$BB && exit");
                process.WaitForExit();
                progressBar.Value++;
                //================== 3
                if (Directory.Exists(MssqlPath)) {
                    try
                    {
                        File.Copy(MssqlPath + @"\Data.mdf", path + @"\Data.mdf", true); progressBar.Value++; progressBar.Refresh(); //4
                        File.Copy(MssqlPath + @"\Data_log.LDF", path + @"\Data_log.LDF", true); progressBar.Value++; progressBar.Refresh(); //5
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
                //================== 6
                var processStart = Process.Start("CMD.exe", "/C NET START MSSQL$BB && exit");
                processStart.WaitForExit();
                progressBar.Value++; progressBar.Refresh();
                //================== 7
                UpdateLabel();
                progressBar.Value++; progressBar.Refresh();
                MessageBox.Show("Datensicherung erfolgreich.", "Datensicherung erfolgreich - BackBüroBackup", MessageBoxButton.OK, MessageBoxImage.Information);
                startBackup.IsEnabled = true;
                conf.WriteConfig();
            }
            else
            {
                MessageBox.Show("Das angegebene Verzeichnis wurde nicht gefunden. Bitte überprüfen Sie, ob Sie den USB-Stick angeschlossen haben.", "Verzeichnis nicht gefunden - BackBüroBackup", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ConfigWindows configWin = new ConfigWindows();
            configWin.ShowInTaskbar = false;
            configWin.ShowDialog();
        }

        private void Restore_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult msgBoxResult = MessageBox.Show("Bevor Sie die Datenbanken wiederherstellen, sollten Sie noch eine Datensicherung von den aktuellen Datensätzen machen, damit es nicht zu Datenverlusten kommen kann." 
                + "\nOK, wenn Sie fortfahren möchten.", "HINWEIS - BackBüroBackup", MessageBoxButton.OKCancel, MessageBoxImage.Warning);

            switch (msgBoxResult)
            {
                case MessageBoxResult.Cancel:
                    return;
                case MessageBoxResult.OK:
                    break;
                default:
                    return;
            }

            const string MssqlPath = @"C:\Program Files\Microsoft SQL Server\MSSQL10_50.BB\MSSQL\DATA";
            if (Directory.Exists(conf.getBackupPath()))
            {
                Restore.IsEnabled = false;
                progressBar.Minimum = 0;
                progressBar.Maximum = 8;
                progressBar.Value = 0;
                progressBar.Value++; progressBar.Refresh();
                //================== 1
                
                progressBar.Value++; progressBar.Refresh();
                //================== 2
                System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
                fbd.SelectedPath = conf.getBackupPath();
                fbd.ShowNewFolderButton = false;
                fbd.Description = "Bitte wählen Sie den Datensicherungspfad mit dem zu wiederherstellenden Datum aus.";
                System.Windows.Forms.DialogResult result = fbd.ShowDialog();
                string path = fbd.SelectedPath;
                if (!File.Exists(path + @"\Data.mdf"))
                {
                    MessageBox.Show("Keine gesicherten Datenbanken gefunden. Bitte wählen Sie ein anderes Verzeichnis!", "Fehlerhaftes Verzeichnis - BackBüroBackup", MessageBoxButton.OK, MessageBoxImage.Error);
                    progressBar.Value = 0;
                    Restore.IsEnabled = true;
                    return;
                }
                progressBar.Value++; progressBar.Refresh();
                //================== 3
                var process = Process.Start("CMD.exe", "/C NET STOP MSSQL$BB && exit");
                process.WaitForExit();
                progressBar.Value++;
                //================== 4
                if (Directory.Exists(MssqlPath))
                {
                    try
                    {
                        File.Copy(path + @"\Data.mdf", MssqlPath + @"\Data.mdf", true); progressBar.Value++; progressBar.Refresh(); //5
                        File.Copy(path + @"\Data_log.LDF", MssqlPath + @"\Data_log.LDF", true); progressBar.Value++; progressBar.Refresh(); //6
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
                //================== 7
                var processStart = Process.Start("CMD.exe", "/C NET START MSSQL$BB && exit");
                processStart.WaitForExit();
                progressBar.Value++; progressBar.Refresh();
                //================== 8
                UpdateLabel();
                progressBar.Value++; progressBar.Refresh();
                MessageBox.Show("Wiederherstellung erfolgreich.", "Wiederherstellung erfolgreich - BackBüroBackup", MessageBoxButton.OK, MessageBoxImage.Information);
                Restore.IsEnabled = true;
                conf.WriteConfig();
            }
            else
            {
                MessageBox.Show("Das angegebene Verzeichnis wurde nicht gefunden. Bitte überprüfen Sie, ob Sie den USB-Stick angeschlossen haben.", "Verzeichnis nicht gefunden - BackBüroBackup", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    public static class ExtionsionMethods
    {
        private static Action EmptyDelegate = delegate() { };

        public static void Refresh(this UIElement uiElement)
        {
            uiElement.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
        }
    }
}
