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
using MahApps.Metro.Controls;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.IO.Compression;
using IWshRuntimeLibrary;

namespace Sabre_Updater
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public string selectedPath;
        public FolderBrowserDialog fbd;
        public string fullPath;
        public WshShell shell;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void mainWindow_Initialized(object sender, EventArgs e)
        {
            locationBox.IsReadOnly = true;

            string latestVersion;
            string latestVersionOnlineLink = "https://drive.google.com/uc?export=download&id=0Bz9aB-8O_UqfY1dqdTZjMUxMVFk";
            WebClient wc = new WebClient();
            latestVersion = wc.DownloadString(latestVersionOnlineLink);
            titleText.Text = "Upgrade to Sabre " + latestVersion;
        }

        private void browseButton_Click(object sender, RoutedEventArgs e)
        {
            fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                selectedPath = fbd.SelectedPath;
                locationBox.Text = selectedPath;
                return;
            }
        }

        private void upgradeButton_Click(object sender, RoutedEventArgs e)
        {
            
            if (selectedPath == null)
            {
                //System.Windows.Forms.MessageBox.Show("No path set.");
                locationBox.BorderBrush = Brushes.Red;
                locationBox.Text = "You must set a location before continuing!"; 
            }
            else
            {
                fullPath = selectedPath + @"\LoL Sabre\";

                if (Directory.Exists(fullPath))
                {
                    Directory.Delete(fullPath, true);
                }

                if (!Directory.Exists(fullPath))
                {
                    Directory.CreateDirectory(fullPath);
                }

                downloadSabre();
                step_one_grid.IsEnabled = false;
                step_one_grid.Visibility = Visibility.Hidden;
                step_two_grid.IsEnabled = true;
                step_two_grid.Visibility = Visibility.Visible;
                return;
            }
        }

        public void downloadSabre()
        {
            //System.Windows.Forms.MessageBox.Show(selectedPath);
            WebClient wc = new WebClient();
            string downloadZipFileLink = "https://drive.google.com/uc?export=download&id=0Bz9aB-8O_UqfcVdHenJiT1JoeEk";
            wc.DownloadFileCompleted += Wc_DownloadFileCompleted;
            wc.DownloadProgressChanged += Wc_DownloadProgressChanged;
            wc.DownloadFileAsync(new Uri(downloadZipFileLink), fullPath + @"\Sabre.zip");
        }

        private void Wc_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            extractSabre();
        }

        private void Wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            //System.Windows.Forms.MessageBox.Show(e.ProgressPercentage.ToString());
            downloadProgress.Value = e.ProgressPercentage;
            downloadText.Text = "Downloading Sabre " + e.ProgressPercentage + "%, " + e.BytesReceived / 1024 / 1024f + "MB";
            this.Title = "Upgrading Sabre: " + e.ProgressPercentage + "%";
        }

        public void extractSabre()
        {
            downloadProgress.Value = 0;
            downloadText.Text = "Extracting Sabre...";
            System.IO.Compression.ZipFile.ExtractToDirectory(fullPath + "Sabre.zip", fullPath);
            downloadText.Text = "Done.";
            createShortcut();
            Environment.Exit(0);
        }

        public void createShortcut()
        {
            WshShell shell = new WshShell();
            IWshShortcut shortcut;
            shortcut = (IWshShortcut)shell.CreateShortcut(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"\LoL Sabre.lnk");
            shortcut.TargetPath = fullPath + @"Sabre\Sabre\bin\Debug\Sabre.exe";
            shortcut.Description = "Sabre is a skin installer software for League of Legends.";
            shortcut.Save();
            System.IO.File.Delete(fullPath + "Sabre.zip");
        }
    }
}
