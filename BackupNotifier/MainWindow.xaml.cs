using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;

namespace BackupNotifier
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartProgram_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(System.Windows.Forms.Application.StartupPath + @"\BackBuero-Backup.exe");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Es ist ein Fehler aufgetreten:\n" + ex.ToString(), "Fehler - BackBüro Backup", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Environment.Exit(0);
        }
    }
}
