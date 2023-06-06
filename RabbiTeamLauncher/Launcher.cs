using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using Microsoft.VisualBasic.Devices;
using CefSharp;
using CefSharp.WinForms;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

using static RabbiTeamLauncher.StaticElements;
using static RabbiTeamLauncher.Constants;

namespace RabbiTeamLauncher
{
    public partial class Launcher : Form
    {
        private readonly Downloader _downloader = new Downloader(); // downloader "singleton" TODO: prevent disposing after closing
        private static Launcher _launcher; // launcher singleton

        public ModPackLocalJson Packs { get; }
        public LauncherSettings Settings { get; }
        public Downloader Downloader => _downloader;
        public static Launcher Instance
        {
            get
            {
                return _launcher;
            }

            set
            {
                if (_launcher == null) _launcher = value;
                else throw new NotSupportedException("Cannot assign to singleton!");
            }
        }

        public Launcher()
        {
            Instance = this;
            CheckLauncherVersion();

            if (!CheckDependencies()) return;

            InitializeComponent();

            Settings = LoadSettings();
            Packs = LoadModpacks();
            
            UpdatePackList();

            InitializeGUI();
        }

        #region Events
        public void PlayButtonClick()
        {
            try
            {
                StartMinecraft();
            }

            catch (Exception ex)
            {
                MessageBox.Show("Failed to start Minecraft - \n" + ex);
            }
        }

        private void StartMinecraft()
        {
            if (Settings.LatestModpack == null)
            {
                MessageBox.Show("You have no pack selected, please select one");
                return;
            }

            if (string.IsNullOrEmpty(NickBox.Text) || NickBox.Text.Contains(" ") || NickBox.Text.Contains("\\") || NickBox.Text.Contains("\""))
            {
                MessageBox.Show("Invalid nick format! Forbidden characters are spaces, backslashes and quotation marks. It also cannot be empty.");
                return;
            }

            string uuid = Utils.GetUUID(NickBox.Text);
            Process proc = new Process();

            ProcessStartInfo info = new ProcessStartInfo
            {
                WorkingDirectory = ModpacksRoot + Settings.LatestModpack,
                UseShellExecute = false
            };

            ulong memory = new ComputerInfo().TotalPhysicalMemory / (1024 ^ 2);
            int memorymb = (int)memory;
            int allocatedMemory;
            info.FileName = $"{JavaInstallationPath}/bin/java{(!Settings.ShowConsole ? "w" : "")}.exe";

            try
            {
                allocatedMemory = Convert.ToInt32(MemoryAllocationBox.Text);
            }

            catch
            {
                MessageBox.Show("Invalid memory allocation format! Type only numbers!");
                MemoryAllocationBox.Text = DefaultRam.ToString();
                return;
            }

            if (memorymb < allocatedMemory)
            {
                MessageBox.Show("Not enough memory to allocate! You selected: " + allocatedMemory + "MB, and your maximum memory is " + memory + "MB.");
                return;
            }

            List<string> noLibList = new List<string>();
            string classPath = " -cp \"";
            ModpackEntry modpackJson = Packs.ModpackList[Settings.LatestModpack];
            string versionIdToUse = modpackJson.ForgeVersion != null ? modpackJson.ForgeFullId : modpackJson.MinecraftVersion;
            McVersionJson mcVersionJson = Utils.FromJsonString<McVersionJson>(File.ReadAllText($"{VersionsRoot}{modpackJson.MinecraftVersion}/{modpackJson.MinecraftVersion}.json"));
            string instanceIdentifier = $"{versionIdToUse}-natives-{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - 3600000L}"; //official MC Launcher stuff
            string mainClass = Utils.FromJsonString<McVersionJson>(File.ReadAllText($"{VersionsRoot}{modpackJson.MinecraftVersion}/{modpackJson.MinecraftVersion}.json")).MainClass;
            string librariesPath = $"{VersionsRoot}{versionIdToUse}/{instanceIdentifier}";

            if (modpackJson.ForgeVersion != null)
            {
                ForgeJson forgeJson = Utils.FromJsonString<ForgeJson>(File.ReadAllText($"{VersionsRoot}{modpackJson.ForgeFullId}/{modpackJson.ForgeFullId}.json"));
                mainClass = forgeJson.MainClass;

                foreach (var lib in forgeJson.Libraries)
                {
                    string pathToLib = lib.Name.ToPath();

                    if (!File.Exists(LibrariesRoot + pathToLib))
                    {
                        throw new MissingLibException(LibrariesRoot + pathToLib);
                    }

                    else
                    {
                        classPath += LibrariesRoot + pathToLib + ";";
                    }
                }
            }

            string version = Packs.ModpackList[Settings.LatestModpack].MinecraftVersion;
            List<Library> libs = Utils.FromJsonString<McVersionJson>(File.ReadAllText(VersionsRoot + version + "/" + version + ".json")).Libraries;
            // TODO: remove this code duplicity in Utils.cs
            foreach (var lib in libs)
            {
                if (lib.Rules != null) //check if rules match our environment
                {
                    foreach (var rule in lib.Rules)
                    {
                        if (rule.Action == "allow")
                        {
                            if (rule.Os != null)
                            {
                                if (rule.Os.Name == "windows")
                                {
                                    break; //if so, continue
                                }

                                else
                                {
                                    goto skip; //if one rule doesn't match the environent needs, abort the whole loading -- needs testing
                                }
                            }
                        }
                    }
                }

                if (lib.Natives != null) //whether the lib contains natives
                {
                    if (lib.Natives.Windows != null) //whether the lib contains also windows natives
                    {
                        if (lib.Natives.Windows.Contains("${arch}")) //whether the natives are arch-specific
                        {
                            string libPath = LibrariesRoot + (Environment.Is64BitOperatingSystem ? lib.Downloads.Classifiers.NativesWindows64.Path : lib.Downloads.Classifiers.NativesWindows32.Path);

                            if (!File.Exists(libPath))
                            {
                                throw new MissingLibException(libPath);
                            }

                            else
                            {
                                Utils.ExtractNatives(libPath, librariesPath);
                            }
                        }

                        else if (!File.Exists(LibrariesRoot + lib.Downloads.Classifiers.NativesWindows.Path))
                        {
                            throw new MissingLibException(lib.Downloads.Classifiers.NativesWindows.Path);
                        }

                        else
                        {
                            Utils.ExtractNatives(LibrariesRoot + lib.Downloads.Classifiers.NativesWindows.Path, librariesPath);
                        }
                    }
                }

                if (lib.Downloads.Artifact != null)
                {
                    if (!File.Exists(LibrariesRoot + lib.Downloads.Artifact.Path))
                    {
                        throw new MissingLibException(LibrariesRoot + lib.Downloads.Artifact.Path);
                    }

                    else
                    {
                        classPath += LibrariesRoot + (lib.Downloads.Artifact.Path).Replace('/', '\\') + ";";
                    }
                }
skip:; // breaking from nested loop
            }

            classPath += $"{VersionsRoot}{modpackJson.MinecraftVersion}/{modpackJson.MinecraftVersion}.jar;";
            info.Arguments = Settings.JVMArguments +
                " -Xmx" + MemoryAllocationBox.Text + "M" + //Maximum RAM
                " -Djava.library.path=\"" + librariesPath + "\"" + //Natives path
                classPath + "\"" + //classpath (all libs + client)
                " " + mainClass + //TODO minecraftArguments from forge/minecraft json
                " --username " + NickBox.Text + //nick
                " --version " + versionIdToUse + //version to use idk the purpose of this
                " --gameDir \"" + info.WorkingDirectory + "\"" + //game directory
                " --assetsDir \"" + AssetsRoot + "\"" + //assets dir
                " --assetIndex " + mcVersionJson.Assets +
                " --uuid " + uuid +
                " --accessToken 0" + //access token, planned to be working with 'online' accounts too
                " --userProperties {} " + //TODO properties such as resolution
                "--userType legacy" + //planned to be legacy/mojang
                " --tweakClass cpw.mods.fml.common.launcher.FMLTweaker"; //also will be variable set

            Log("Full java command line: " + info.FileName + " " + info.Arguments); // find a way to compress the log, this text is really long

            if (info.Arguments.Length > 8192)
            {
                MessageBox.Show("Path of installation is too long! Migrate your installation to another folder with shorter path!" + Environment.NewLine + "(Advanced Settings -> Migrate)");
                return;
            }

            proc.StartInfo = info;
            proc.Start();

            if (Settings.CloseAfterStart == true)
            {
                Application.Exit();
            }
        }

        private void MemoryAllocationBoxTextChanged(object sender, EventArgs e)
        {
            try
            {
                Settings.Ram = int.Parse((sender as TextBox).Text);
            }

            catch (FormatException)
            {
                MessageBox.Show("Illegal characters detected in the memory allocation box. Did you just try to crash the program with Ctrl+V?");
                (sender as TextBox).Text = Settings.Ram.ToString();
            }
        }

        private void OfficialPackButtonClick(object sender, EventArgs e)
        {
            if (Packs.ModpackList.ContainsKey("RabbitPack"))
            {
                MessageBox.Show("You have already downloaded official modpack");
                return;
            }

            else
            {
                try
                {
                    _ = Utils.NewDownload(DownloadType.ModpackFresh, downloadName: "RabbitPack");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to download RabbitPack - \n" + ex);
                }
            }
        }

        private void LauncherBodyClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                Cef.Shutdown();

                if (File.Exists(UpdateScriptPath))
                {
                    Process.Start(UpdateScriptPath);
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show("LauncherBodyClosing failed, how did this even happen? " + ex);
            }
        }

        private void LauncherBodyResize(object sender, EventArgs e) //resizing and relocating part, because dock is not enough for me
        {
            Browser.Size = new Size(Width - 16, Height - (ClickableStuff.Height + 38));
            OfficialModpackButton.Location = new Point(ClickableStuff.Width - 123, 6);
            AdvancedSettingsButton.Location = new Point(ClickableStuff.Width - 123, 46);
            CreditsButton.Location = new Point(ClickableStuff.Width - 123, 86);
            PlayButton.Location = new Point((ClickableStuff.Width - PlayButton.Size.Width) / 2, 34);
        }

        private void MemoryAllocationBoxKeyPressed(object sender, KeyPressEventArgs e) => e.Handled = !(char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar));

        private void AdvancedSettingsButtonClick(object sender, EventArgs e) => new AdvancedSettings(Settings).ShowDialog();

        private void PackListSelect(object sender, EventArgs e) => Settings.LatestModpack = PackList.SelectedItem.ToString();

        private void NickBoxTextChanged(object sender, EventArgs e) => Settings.Nick = NickBox.Text;

        public void PlayButtonClick(object sender, EventArgs e) => PlayButtonClick();
        #endregion

        private bool CheckDependencies()
        {
            int missingCount = FileIndex["Deps"].Where(x => !File.Exists(AppPath + "/" + x)).Count();

            bool downloadDeps()
            {
                InitializeStub();
                _ = Utils.NewDownload(downloadType: DownloadType.Deps);
                return false;
            }

            if (missingCount == FileIndex["Deps"].Length)
            {
                return downloadDeps();
            }

            else if (missingCount > 0)
            {
                var dr = MessageBox.Show("You are missing some dependencies, did you delete some by accident? Check your Recycle Bin" + Environment.NewLine + Environment.NewLine + "Or click OK to download dependency pack again, missing dependencies can cause Launcher malfunction.", "Dependency Check", MessageBoxButtons.OKCancel);

                if (dr == DialogResult.OK) //maybe it will be better to find those missing deps and download just the missing ones (I am too lazy), but few more MBs downloaded.. who cares
                {
                    return downloadDeps();
                }

                else
                {
                    return true;
                }
            }

            return true;
        }

        private void CheckLauncherVersion()
        {
            if (Utils.IsHigherThan(StaticElements.UpdateInfo.Version, LauncherVersion)) //if the version doesn't match, there will be new one to download, no prompt to prevent incompatibility
            {
                Utils.NewDownload(DownloadType.Other, uri: StaticElements.UpdateInfo.UpdateUrl, output: "/RabbiTeamLauncherNew.exe", completedAction: new DownloadAction(Utils.UpdateLauncher, DownloadActionArgs.Empty)).Start();
            }
        }

        private void InitializeBrowser()
        {
            var settings = new CefSettings { CachePath = CacheRoot.Substring(0, CacheRoot.Length - 1) };
            //settings.RegisterScheme(new CefCustomScheme
            //{
            //    SchemeName = "rtbrowser",
            //    SchemeHandlerFactory = new SchemeHandlerFactory()
            //});
            Cef.Initialize(settings);
            //handler = new RequestHandler(this);
            var webPage = new ChromiumWebBrowser(StaticElements.UpdateInfo.ChromeUrl);
            Browser.Controls.Add(webPage);
            //webPage.RequestHandler = handler;
            webPage.Dock = DockStyle.Fill;
        }

        private void InitializeGUI()
        {
            PackList.SelectedItem = Settings.LatestModpack;
            NickBox.Text = Settings.Nick;
            MemoryAllocationBox.Text = Settings.Ram.ToString();
            InitializeBrowser();
        }

        private ModPackLocalJson LoadModpacks()
        {
            ModPackLocalJson modPackLocalJson = null;

            if (File.Exists(ModpacksPath))
            {
                try
                {
                    modPackLocalJson = Utils.FromJsonString<ModPackLocalJson>(File.ReadAllText(ModpacksPath));
                }
                catch
                {
                    modPackLocalJson = null; // TODO backup
                }
            }
            
            if (modPackLocalJson is null)
            {
                modPackLocalJson = new ModPackLocalJson(ModpackJsonVersion);
                File.WriteAllText(ModpacksPath, Utils.ToJsonString(modPackLocalJson));
            }

            return modPackLocalJson;
        }

        private LauncherSettings LoadSettings()
        {
            LauncherSettings settings;
            string settingsString = "";

            try
            {
                settingsString = File.ReadAllText(SettingsPath);
                settings = Utils.FromJsonString<LauncherSettings>(settingsString);
            }

            catch
            {
                if (File.Exists(SettingsPath))
                {
                    File.WriteAllText($"{SettingsPath}-backup-{DateTime.Now:yyyyMMddHHmmss}{Path.GetExtension(SettingsPath)}", settingsString);
                    MessageBox.Show("Setting file cannot be loaded. You can find a backup of the old one in the root folder of your launcher.");
                }

                File.Delete(SettingsPath);

                settings = new LauncherSettings
                {
                    Nick = string.Empty,
                    Ram = DefaultRam,
                    ShowConsole = false,
                    CloseAfterStart = true,
                    JVMArguments = DefaultArgs,
                    LatestModpack = null,
                    LatestUUID = null
                };
            }

            return settings;
        }

        public static void Log(string message)
        {
            if (AppPath != null) //because nobody wants junk in drive's root folder
            {
                File.AppendAllText(AppPath + "/launcherlog.log", "[" + DateTimeOffset.Now.ToLocalTime() + "] " + message + Environment.NewLine);
            }
        }

        public void UpdatePackList()
        {
            foreach (var modpackEntry in Packs.ModpackList)
            {
                PackList.Items.Add(modpackEntry.Key);
            }

            if (PackList.Items.Count == 1)
            {
                Settings.LatestModpack = (string)PackList.Items[0];
                PackList.SelectedItem = PackList.Items[0];
            }
        }
    }
}
