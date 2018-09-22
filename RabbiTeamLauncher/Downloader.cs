using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Windows.Forms;
using LB = RabbiTeamLauncher.LauncherBody;
using System.Threading.Tasks;

namespace RabbiTeamLauncher
{
    public partial class Downloader : Form
    {
        public static List<Panel> splitter = new List<Panel>();
        public static List<ProgressBar> progressBar = new List<ProgressBar>();
        public static List<Label> labels = new List<Label>();
        public static int currentDownloads = 0;
        public static List<DownloaderSettings> pendingDownloads = new List<DownloaderSettings> { };
        public static List<string> PendingForge = new List<string> { };
        public static List<string> pendingMc = new List<string> { };

        public Downloader()
        {
            InitializeComponent();
        }

        public async Task BeginDownload(DownloaderSettings settings)
        {
            await DownloadFiles(settings);

            Controls.Remove(splitter[0]);
            Controls.Remove(progressBar[0]);
            Controls.Remove(labels[0]);
            splitter.RemoveAt(0);
            progressBar.RemoveAt(0);
            labels.RemoveAt(0);

            if (pendingDownloads.Count != 0)
            {
                for (int i = 0; i < pendingDownloads.Count; i++)
                {
                    splitter[i].Location = new Point(1, splitter[i].Location.Y - 52);
                    progressBar[i].Location = new Point(12, progressBar[i].Location.Y - 52);
                    labels[i].Location = new Point(12, labels[i].Location.Y - 52);
                }

                BeginDownload(pendingDownloads[0]);
                pendingDownloads.RemoveAt(0);

            }

            currentDownloads--;
        }
        public async Task DownloadFiles(DownloaderSettings settings)
        {
            if (settings.BeforeStartAction != null)
                await settings.BeforeStartAction?.Invoke(settings.BeforeStartArgs);

            foreach (var file in settings.FilesToDownload)
            {
                if (file.BeforeStartAction != null)
                    await file.BeforeStartAction?.Invoke(file.BeforeStartArgs);

                if (!Directory.Exists((LB.AppPath + file.OutputPath).DirectoryOf()))
                    Directory.CreateDirectory((LB.AppPath + file.OutputPath).DirectoryOf());

                using (var wc = new WebClient())
                {
                    var Uri = new Uri(file.Uri);
                    var filename = Path.GetFileName(Uri.LocalPath);
                    labels[0].Text = "Downloading " + filename;
                    wc.DownloadProgressChanged += progressBarUpdate;
                    await wc.DownloadFileTaskAsync(Uri, LB.AppPath + file.OutputPath);
                    
                    if (file.CompletedAction != null)
                        await file.CompletedAction?.Invoke(file.CompletedArgs);
                }
            }

            if (settings.CompletedAction != null)
                await settings.CompletedAction?.Invoke(settings.CompletedArgs);
        }
        public void progressBarUpdate(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar[0].Value = e.ProgressPercentage;
        }
    }
}
