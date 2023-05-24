using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Windows.Forms;
using LB = RabbiTeamLauncher.LauncherBody;
using System.Threading.Tasks;
using System.Net.Http;
using System.Threading;

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

                _ = BeginDownload(pendingDownloads[0]);
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
                {
                    Directory.CreateDirectory((LB.AppPath + file.OutputPath).DirectoryOf());
                }

                var handler = new HttpClientHandler();
                handler.ClientCertificateOptions = ClientCertificateOption.Manual; // ignore invalid certificate
                handler.ServerCertificateCustomValidationCallback = (x, y, z, w) => { return true; };

                using (var client = new HttpClient(handler))
                {
                    var Uri = new Uri(file.Uri);
                    var filename = Path.GetFileName(Uri.LocalPath);
                    labels[0].Text = "Downloading " + filename;
                    client.Timeout = TimeSpan.FromMinutes(5);

                    using (var fileToDownload = new FileStream(LB.AppPath + file.OutputPath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        await client.DownloadAsync(file.Uri, fileToDownload, new ProgressBarWrapper(progressBar[0], this));
                    }

                    if (file.CompletedAction != null)
                        await file.CompletedAction?.Invoke(file.CompletedArgs);
                }

                /*using (var wc = new WebClient())
                {
                    var Uri = new Uri(file.Uri);
                    var filename = Path.GetFileName(Uri.LocalPath);
                    labels[0].Text = "Downloading " + filename;
                    wc.DownloadProgressChanged += progressBarUpdate;
                    await wc.DownloadFileTaskAsync(Uri, LB.AppPath + file.OutputPath);
                    
                    if (file.CompletedAction != null)
                        await file.CompletedAction?.Invoke(file.CompletedArgs);
                }*/
            }

            if (settings.CompletedAction != null)
                await settings.CompletedAction?.Invoke(settings.CompletedArgs);
        }
        public void progressBarUpdate(object sender, DownloadProgressChangedEventArgs e)
        {
            BeginInvoke(new Action(() =>
            {
                progressBar[0].Value = e.ProgressPercentage;
            }));
        }
    }

    public static class HttpClientExtensions
    {
        public static async Task DownloadAsync(this HttpClient client, string requestUri, Stream destination, IProgress<float> progress = null, CancellationToken cancellationToken = default)
        {
            // Get the http headers first to examine the content length
            using (var response = await client.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead))
            {
                var contentLength = response.Content.Headers.ContentLength;

                using (var download = await response.Content.ReadAsStreamAsync())
                {

                    // Ignore progress reporting when no progress reporter was 
                    // passed or when the content length is unknown
                    if (progress == null || !contentLength.HasValue)
                    {
                        await download.CopyToAsync(destination);
                        return;
                    }

                    // Convert absolute progress (bytes downloaded) into relative progress (0% - 100%)
                    var relativeProgress = new Progress<long>(totalBytes => progress.Report(((float)totalBytes / contentLength.Value)));
                    // Use extension method to report progress while downloading
                    await download.CopyToAsync(destination, 81920, relativeProgress, cancellationToken);
                    progress.Report(1);
                }
            }
        }
    }

    public static class StreamExtensions
    {
        public static async Task CopyToAsync(this Stream source, Stream destination, int bufferSize, IProgress<long> progress = null, CancellationToken cancellationToken = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (!source.CanRead)
                throw new ArgumentException("Has to be readable", nameof(source));
            if (destination == null)
                throw new ArgumentNullException(nameof(destination));
            if (!destination.CanWrite)
                throw new ArgumentException("Has to be writable", nameof(destination));
            if (bufferSize < 0)
                throw new ArgumentOutOfRangeException(nameof(bufferSize));

            var buffer = new byte[bufferSize];
            long totalBytesRead = 0;
            int bytesRead;
            while ((bytesRead = await source.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false)) != 0)
            {
                await destination.WriteAsync(buffer, 0, bytesRead, cancellationToken).ConfigureAwait(false);
                totalBytesRead += bytesRead;
                progress?.Report(totalBytesRead);
            }
        }
    }

    public class ProgressBarWrapper : IProgress<float>
    {
        private readonly ProgressBar _innerBar;
        private readonly Control _parent;

        public ProgressBarWrapper(ProgressBar innerBar, Control parentControl)
        {
            _innerBar = innerBar;
            _parent = parentControl;
        }

        public void Report(float value)
        {
            _parent.BeginInvoke(new Action(() =>
            {
                _innerBar.Value = (int)(value * 100);
            }));
        }
    }
}
