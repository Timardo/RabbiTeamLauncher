using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Linq;
using System.Net.Security;

namespace RabbiTeamLauncher
{
    public partial class Downloader : Form
    {
        private readonly List<DownloadJob> _pendingJobs;
        private readonly List<ProgressBar> _progressBar;
        private readonly List<Panel> _splitter;
        private readonly List<Label> _labels;
        private int _currentDownloads = 0;

        public Downloader()
        {
            InitializeComponent();
            _labels = new List<Label>();
            _splitter = new List<Panel>();
            _progressBar = new List<ProgressBar>();
            _pendingJobs = new List<DownloadJob>();
        }
        public void AddDownload(DownloadJob job, string downloadName)
        {
            _progressBar.Add(new ProgressBar
            {
                Size = new Size(776, 23),
                Location = new Point(12, 12 + _currentDownloads * 52),
            });
            
            _labels.Add(new Label
            {
                Size = new Size(776, 13),
                Location = new Point(12, 38 + _currentDownloads * 52),
                Text = "Waiting for another download to start downloading " + downloadName
            });

            _splitter.Add(new Panel
            {
                Size = new Size(800, 4),
                Location = new Point(1, 54 + _currentDownloads * 52),
                BorderStyle = BorderStyle.Fixed3D
            });

            if (!Visible)
            {
                Show();
            }

            if (_currentDownloads == 0)
            {
                _ = BeginDownload(job);
            }

            else
            {
                _pendingJobs.Add(job);
            }

            Controls.AddRange(new Control[] { _splitter.Last(), _labels.Last(), _progressBar.Last() });

            _currentDownloads++;
        }

        public void SetLabel(string label)
        {
            BeginInvoke((Action)(() => _labels[0].Text = label));
        }

        private async Task BeginDownload(DownloadJob job)
        {
            await DownloadFiles(job);

            Controls.Remove(_splitter[0]);
            Controls.Remove(_progressBar[0]);
            Controls.Remove(_labels[0]);
            _splitter.RemoveAt(0);
            _progressBar.RemoveAt(0);
            _labels.RemoveAt(0);

            if (_pendingJobs.Count != 0)
            {
                for (int i = 0; i < _pendingJobs.Count; i++)
                {
                    _splitter[i].Location = new Point(1, _splitter[i].Location.Y - 52);
                    _progressBar[i].Location = new Point(12, _progressBar[i].Location.Y - 52);
                    _labels[i].Location = new Point(12, _labels[i].Location.Y - 52);
                }

                _ = BeginDownload(_pendingJobs[0]);
                _pendingJobs.RemoveAt(0);

            }

            _currentDownloads--;
        }

        private async Task DownloadFiles(DownloadJob job)
        {
            if (job.BeforeStartAction != null)
            {
                job.BeforeStartAction?.Action.Invoke(job.BeforeStartAction.ActionArgs);
            }

            foreach (FileToDownload file in job.FilesToDownload)
            {
                if (file.BeforeStartAction != null)
                {
                    file.BeforeStartAction?.Action.Invoke(file.BeforeStartAction.ActionArgs);
                }

                if (!Directory.Exists(Utils.DirectoryOf(file.OutputPath)))
                {
                    Directory.CreateDirectory(Utils.DirectoryOf(file.OutputPath));
                }

                // ignore certificate checks on downloading - makes problems with NUGET packages
                // TODO: THIS IS EXTREMELY UNSAFE AND SHOULDN'T BE DONE IN PRODUCTION
                var sslCheckOverride = new RemoteCertificateValidationCallback(delegate { return true; });

                try
                {
                    ServicePointManager.ServerCertificateValidationCallback += sslCheckOverride;

                    using (var wc = new WebClient())
                    {
                        var Uri = new Uri(file.Uri);
                        var filename = Path.GetFileName(Uri.LocalPath);
                        SetLabel("Downloading " + filename); // TODO: does not show the name or progress when downloading a modpack file from yandex
                        wc.DownloadProgressChanged += ProgressBarUpdate;
                        await wc.DownloadFileTaskAsync(Uri, file.OutputPath);

                        if (file.CompletedAction != null)
                        {
                            file.CompletedAction?.Action.Invoke(file.CompletedAction.ActionArgs);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    ServicePointManager.ServerCertificateValidationCallback -= sslCheckOverride;
                }
            }

            if (job.CompletedAction != null)
            {
                job.CompletedAction?.Action.Invoke(job.CompletedAction.ActionArgs);
            }
        }

        private void ProgressBarUpdate(object sender, DownloadProgressChangedEventArgs e) => BeginInvoke((Action)(() => { _progressBar[0].Value = e.ProgressPercentage; }));
    }
}
