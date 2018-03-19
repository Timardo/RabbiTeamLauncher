using System;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Windows.Forms;

namespace RabbiTeamLauncher
{
    public partial class updater : Form
    {
        private static string url;
        public updater()
        {
            InitializeComponent();
            url = testLauncher.getURL(testLauncher.updateURL);
            using (WebClient wc = new WebClient())
            {
                wc.DownloadFileCompleted += new AsyncCompletedEventHandler(extractUpdate);
                wc.DownloadProgressChanged += wc_DownloadProgressChanged;
                wc.DownloadFileTaskAsync(new Uri(url), Application.StartupPath + "//rabbiteam.zip");
            }
        }
        void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }
        public void extractUpdate(object sender, AsyncCompletedEventArgs e)
        {
            {
                ZipFile.ExtractToDirectory(testLauncher.appPath + "//rabbiteam.zip", testLauncher.appPath + "//temporary_archive_files");
                Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory(testLauncher.appPath + "//temporary_archive_files", testLauncher.appPath, true);
                Directory.Delete(testLauncher.appPath + "//temporary_archive_files", true);
                testLauncher.lineChanger(url, testLauncher.settings, 5);
                if (File.Exists(testLauncher.appPath + "//update_launcher.bat"))
                    System.Diagnostics.Process.Start(testLauncher.appPath + "//update_launcher.bat");
                MessageBox.Show("Update succesfully downloaded and extracted!");
                Close();
            }
        }
    }
}
