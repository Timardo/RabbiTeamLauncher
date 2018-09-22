using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using Microsoft.VisualBasic.Devices;
using CefSharp;
using CefSharp.WinForms;
using System.Drawing;
using System.Collections.Generic;

namespace RabbiTeamLauncher
{
    public partial class LauncherBody : Form
    {
        public LauncherBody()
        {
            if (LauncherVersion != UpdateInfo.Version) //if the version doesn't match, there will be new one to download, no prompt to prevent incompatibility
                Utils.DownloaderConstructor(uri: UpdateInfo.UpdateUrl,
                    output: "/RabbiTeamLauncherNew.exe",
                    completedAction: new Utils.DownloadCompletedAction(Utils.UpdateLauncher));

            var i = 0; //checking dependencies 

            foreach (var dep in FileIndex["Deps"])
                if (!File.Exists(AppPath + "/" + dep))
                {
                    i++;
                }

            if (i == FileIndex["Deps"].Length)
            {
                InitializeStub();
                Utils.DownloaderConstructor(downloadType: DownloadType.Deps);
                goto stubLabel;
            }

            else if (i > 0)
            {
                var dr = MessageBox.Show("You are missing some dependencies, did you delete some by accident? Check your Recycle Bin" + Environment.NewLine + Environment.NewLine + "Or click OK to download dependency pack again, missing dependencies can cause Launcher malfunction.", "Dependency Check", MessageBoxButtons.OKCancel);

                if (dr == DialogResult.OK) //maybe it will be better to find those missing deps and download just the missing ones (I am too lazy), but few more MBs downloaded.. who cares
                {
                    InitializeStub();
                    Utils.DownloaderConstructor(downloadType: DownloadType.Deps);
                }

                else
                    goto tryToContinue;
            }

            tryToContinue:

            InitializeComponent();
            PackList = new ComboBox //PackList combobox - designer is not enough for static fields, so adding this programmatically
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                FormattingEnabled = true,
                Location = new Point(50, 73),
                Name = "PackList",
                Size = new Size(100, 23),
                TabIndex = 11
            };
            PackList.SelectedIndexChanged += new EventHandler(PackListSelect);
            clickableStuff.Controls.Add(PackList);

            try
            {
                Settings = new StreamReader(AppPath + "/settings.json").ReadToEnd().FromJsonString<LauncherSettings>();
            }

            catch
            {
                if (File.Exists(AppPath + "/settings.json"))
                {
                    (AppPath + "/settings.json").Backup();
                    MessageBox.Show("Setting file cannot be loaded. You can find backup of the old one in the root folder of Launcher.");
                }

                string nick = null;
                var ram = 3000;
                var consoleBool = false;
                var closeBool = true;

                try
                {
                    nick = (AppPath + "/settings.txt").GetLine(1);
                    ram = Convert.ToInt32((AppPath + "/settings.txt").GetLine(2));
                    consoleBool = Convert.ToBoolean((AppPath + "/settings.txt").GetLine(3));
                    closeBool = Convert.ToBoolean((AppPath + "/settings.txt").GetLine(4));
                    File.Delete(AppPath + "/settings.txt");
                }

                //update from plain text to better-handleable plain text
                catch
                {

                }

                Settings = new LauncherSettings
                {
                    Nick = nick,
                    Ram = ram,
                    ShowConsole = consoleBool,
                    CloseAfterStart = closeBool,
                    JVMArguments = Utils.DefaultArgs(),
                    LatestModpack = null,
                    LatestUuid = null
                };
            }

            if (!File.Exists(AppPath + "/modpacks.json"))
                File.WriteAllText(AppPath + "/modpacks.json", 
                    new ModPackLocalJson
                    {
                        ModpackJsonVersion = 1,
                        ModpackList = new Dictionary<string, ModpackEntry>()
                    }
                    .ToJsonString());

            using (var sr = new StreamReader(AppPath + "/modpacks.json"))
            {
                Packs = sr.ReadToEnd().FromJsonString<ModPackLocalJson>();
                sr.Close();
            }

            Utils.UpdatePackList();
            PackList.SelectedItem = Settings.LatestModpack;
            NickBox.Text = Settings.Nick;
            MemoryAllocationBox.Text = Settings.Ram.ToString();
            JVMArguments = Settings.JVMArguments;
            InitializeBrowser();
            ModpackUrl = UpdateInfo.ModpackListRoot;
            stubLabel:;
        }

        public static ComboBox PackList;
        public static string LauncherVersion = "1.0";
        public static Downloader Downloader = null;
        public static LauncherSettings Settings;
        public static string ModpackUrl;
        public static UpdateInfo UpdateInfo = "https://raw.githubusercontent.com/Timardo/RabbiTeamLauncher/Default/UpdateInfo.txt".GetResponse().ToUpdateInfo();
        public static string AppPath = Application.StartupPath;
        public static string JVMArguments;
        public static ModPackLocalJson Packs;
        public static Dictionary<string, string[]> FileIndex = new Dictionary<string, string[]>() //all files and folders that can launcher installation have
        {
            { "Deps", new string[] { "cef.pak", "cef_100_percent.pak", "cef_200_percent.pak", "cef_extensions.pak", "CefSharp.BrowserSubprocess.Core.dll", "CefSharp.BrowserSubprocess.exe", "CefSharp.Core.dll", "CefSharp.dll", "CefSharp.WinForms.dll", "chrome_elf.dll", "d3dcompiler_47.dll", "icudtl.dat", "libcef.dll", "natives_blob.bin", "snapshot_blob.bin", "Microsoft.WindowsAPICodePack.dll", "Microsoft.WindowsAPICodePack.Shell.dll", "Newtonsoft.Json.dll", "SharpCompress.dll" } },
            { "MCStuff", new string[] { "modpacks", "resources" } },
            { "LauncherStuff", new string[] { "debug.log", "settings.json", "RabbiTeamLauncher.exe", "locales", "Cache", "modpacks.json" } }
        };

        private void InitializeBrowser()
        {
            var settings = new CefSettings { CachePath = AppPath + "/Cache" };
            //settings.RegisterScheme(new CefCustomScheme
            //{
            //    SchemeName = "rtbrowser",
            //    SchemeHandlerFactory = new SchemeHandlerFactory()
            //});
            Cef.Initialize(settings);
            //handler = new RequestHandler(this);
            var webPage = new ChromiumWebBrowser(UpdateInfo.ChromeUrl);
            Browser.Controls.Add(webPage);
            //webPage.RequestHandler = handler;
            webPage.Dock = DockStyle.Fill;
        }

        public async System.Threading.Tasks.Task PlayButtonClick()
        {
            if (string.IsNullOrEmpty(NickBox.Text) || NickBox.Text.Contains(" ") || NickBox.Text.Contains("\\") || NickBox.Text.Contains("\""))
            {
                MessageBox.Show("Invalid nick format! Forbidden characters are spaces, backslashes and quotation marks. It also cannot be empty.");
                return;
            }

            var response = ("http://tools.glowingmines.eu/convertor/nick/" + NickBox.Text).GetResponse(); //will be offical mojang server soon and this one will be for offline UUIDs only
            var hasUuid = response != null ? true : false;
            string uuid = Settings.LatestUuid;

            if (hasUuid == true)
            {
                var JSONObj = response.FromJsonString<Dictionary<string, string>>();
                uuid = JSONObj["offlineuuid"];
                Settings.LatestUuid = uuid;
            }

            var proc = new Process();
            var info = new ProcessStartInfo
            {
                WorkingDirectory = AppPath + "\\modpacks\\" + Settings.LatestModpack,
                UseShellExecute = false
            };
            var memory = new ComputerInfo().TotalPhysicalMemory / (1024 ^ 2);
            var memorymb = Convert.ToInt32(memory);
            int allocatedMemory;
            info.FileName = Settings.ShowConsole ? Utils.GetJavaInstallationPath() + "\\bin\\java.exe" : Utils.GetJavaInstallationPath() + "\\bin\\javaw.exe";

            try
            {
                allocatedMemory = Convert.ToInt32(MemoryAllocationBox.Text);
            }

            catch
            {
                MessageBox.Show("Invalid memory allocation format! Type only numbers!");
                MemoryAllocationBox.Text = "3000";
                return;
            }

            if (memorymb < allocatedMemory)
            {
                MessageBox.Show("Not enough memory to allocate! You selected: " + allocatedMemory + "MB, and your maximum memory is " + memory + "MB.");
                return;
            }

            var relativeLibsPath = "\\resources\\libraries\\";
            List<string> noLibList = new List<string>();
            var classPath = " -cp \"";
            var modpackJson = Packs.ModpackList[Settings.LatestModpack];
            var versionIdToUse = modpackJson.ForgeVersion != null ? modpackJson.ForgeFullId : modpackJson.MinecraftVersion;
            var mcVersionJson = (AppPath + "\\resources/versions\\" + modpackJson.MinecraftVersion + "\\" + modpackJson.MinecraftVersion + ".json").ReadFile().FromJsonString<McVersionJson>();
            var instanceIdentifier = versionIdToUse + "-natives-" + (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - 3600000L); //official MC Launcher stuff
            var mainClass = (AppPath + "\\resources\\versions\\" + modpackJson.MinecraftVersion + "\\" + modpackJson.MinecraftVersion + ".json").ReadFile().FromJsonString<McVersionJson>().MainClass;

            if (!Directory.Exists(AppPath + "\\resources\\versions\\" + modpackJson.MinecraftVersion + "\\" + instanceIdentifier))
                Directory.CreateDirectory(AppPath + "\\resources\\versions\\" + modpackJson.MinecraftVersion + "\\" + instanceIdentifier);

            if (modpackJson.ForgeVersion != null)
            {
                var forgeJson = (AppPath + "\\resources\\versions\\" + modpackJson.ForgeFullId + "\\" + modpackJson.ForgeFullId + ".json").ReadFile().FromJsonString<ForgeJson>();
                mainClass = forgeJson.MainClass;
                string pathToLib;

                foreach (var lib in forgeJson.Libraries)
                {
                    pathToLib = lib.Name.ToPath();

                    if (!File.Exists(AppPath + relativeLibsPath + pathToLib))
                        MessageBox.Show(AppPath + relativeLibsPath + pathToLib);

                    else
                        classPath += AppPath + relativeLibsPath + pathToLib + ";";
                }
            }

            foreach (var lib in Packs.ModpackList[Settings.LatestModpack].MinecraftVersion.ToMcJson().Libraries)
            {
                if (lib.Rules != null) //check if rules match our environment
                    foreach (var rule in lib.Rules)
                    {
                        if (rule.Action == "allow")
                            if (rule.Os != null)
                                if (rule.Os.Name == "windows")
                                    goto @continue; //if so, continue

                                else goto skip; //if one rule doesn't match the environent needs, abort the whole download -- needs testing
                    }

                @continue:

                if (lib.Natives != null) //whether the lib contains natives
                    if (lib.Natives.Windows != null) //whether the lib contains also windows natives
                        if (lib.Natives.Windows.Contains("${arch}")) //whether the natives are arch-specific
                        {
                            if (Environment.Is64BitOperatingSystem) //resolve architecture
                            {
                                if (!File.Exists(AppPath + relativeLibsPath + lib.Downloads.Classifiers.NativesWindows64.Path))
                                    MessageBox.Show(AppPath + relativeLibsPath + lib.Downloads.Classifiers.NativesWindows64.Path);

                                else
                                    await Utils.ExtractNatives(AppPath + relativeLibsPath + lib.Downloads.Classifiers.NativesWindows64.Path, versionIdToUse, instanceIdentifier);
                            }

                            else
                            {
                                if (!File.Exists(AppPath + relativeLibsPath + lib.Downloads.Classifiers.NativesWindows32.Path))
                                    MessageBox.Show(AppPath + relativeLibsPath + lib.Downloads.Classifiers.NativesWindows32.Path);

                                else
                                    await Utils.ExtractNatives(AppPath + relativeLibsPath + lib.Downloads.Classifiers.NativesWindows64.Path, versionIdToUse, instanceIdentifier);
                            }
                        }

                        else if (!File.Exists(AppPath + relativeLibsPath + lib.Downloads.Classifiers.NativesWindows.Path))
                            MessageBox.Show(AppPath + relativeLibsPath + lib.Downloads.Classifiers.NativesWindows.Path); //if the lib doesn't contain arch-specific natives, just set the default ones for windows

                        else
                            await Utils.ExtractNatives(AppPath + relativeLibsPath + lib.Downloads.Classifiers.NativesWindows.Path, versionIdToUse, instanceIdentifier);

                    else goto skipNatives; //if the lib doesn't contain windows natives, skipping

                skipNatives:

                if (lib.Downloads.Artifact != null)
                {
                    if (!File.Exists(AppPath + relativeLibsPath + lib.Downloads.Artifact.Path))
                        MessageBox.Show(AppPath + relativeLibsPath + lib.Downloads.Artifact.Path);

                    else
                        classPath += AppPath + relativeLibsPath + (lib.Downloads.Artifact.Path).Replace('/', '\\') + ";";
                }

                skip:;
            }

            classPath += AppPath + "\\resources\\versions\\" + modpackJson.MinecraftVersion + "\\" + modpackJson.MinecraftVersion + ".jar" + ";";

            info.Arguments = JVMArguments + 
                " -Xmx" + MemoryAllocationBox.Text + "M" + //Maximum RAM
                " -Djava.library.path=\"" + AppPath + "\\resources\\versions\\" + versionIdToUse + "\\" + instanceIdentifier + "\"" + //Natives path
                classPath + "\"" + //classpath (all libs + client)
                " " + mainClass + //TODO minecraftArguments from forge/minecraft json
                " --username " + NickBox.Text + //nick
                " --version " + versionIdToUse + //version to use idk the purpose of this
                " --gameDir \"" + info.WorkingDirectory + "\"" + //game directory
                " --assetsDir \"" + AppPath + "\\resources\\assets\"" + //assets dir
                " --assetIndex " + mcVersionJson.Assets + 
                " --uuid " + uuid + 
                " --accessToken 0" + //access token, planned to be working with 'online' accounts too
                " --userProperties {} " + //TODO properties such as resolution
                "--userType legacy" + //planned to be legacy/mojang
                " --tweakClass cpw.mods.fml.common.launcher.FMLTweaker"; //also will be variable set

            if ((info.Arguments.Length) > 8192)
            {
                MessageBox.Show("Path of installation is too long! Migrate your installation to another folder with shorter path!" + Environment.NewLine + "(Advanced Settings -> Migrate)");
                return;
            }

            if (hasUuid == false)
            {
                MessageBox.Show("Not valid UUID, play multiplayer at your own risk!");
            }

            proc.StartInfo = info;
            proc.Start();

            if (Settings.CloseAfterStart == true)
                Application.Exit();

        }

        private void MemoryAllocationBoxTextChanged(object sender, EventArgs e)
        {
            try
            {
                Settings.Ram = Convert.ToInt32(MemoryAllocationBox.Text);
            }

            catch
            {

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
                Utils.DownloaderConstructor(DownloadType.ModpackFresh, downloadName: "RabbitPack");
            }
        }

        private void LauncherBodyClosing(object sender, FormClosingEventArgs e)
        {
            Cef.Shutdown();
            File.WriteAllText(AppPath + "/settings.json", Settings.ToJsonString());
        }

        private void LauncherBodyResize(object sender, EventArgs e) //resizing and relocating part, because dock is not enough for me
        {
            Browser.Size = new Size(Width - 16, Height - (clickableStuff.Height + 38));
            OfficialModpackButton.Location = new Point(clickableStuff.Width - 123, 6);
            AdvancedSettingsButton.Location = new Point(clickableStuff.Width - 123, 46);
            CreditsButton.Location = new Point(clickableStuff.Width - 123, 86);
            PlayButton.Location = new Point((clickableStuff.Width - PlayButton.Size.Width) / 2, 34);
        }

        private void AdvancedSettingsButtonClick(object sender, EventArgs e) => new AdvancedSettings().ShowDialog();

        private void PackListSelect(object sender, EventArgs e) => Settings.LatestModpack = PackList.SelectedItem.ToString();

        private void NickBoxTextChanged(object sender, EventArgs e) => Settings.Nick = NickBox.Text;

        public void PlayButtonClick(object sender, EventArgs e) => PlayButtonClick();
    }
    #region Code snippet of browser handler
    //Maybe I will implement this thing later.. it's still pretty buggy

    //private RequestHandler handler;
    //public static string cachePage = appPath + "/Cache/cannotConnect.html"; 
    //private void InitializeBrowser()
    //{
    //    var settings = new CefSettings { CachePath = AppPath + "/Cache" };
    //    //settings.RegisterScheme(new CefCustomScheme
    //    //{
    //    //    SchemeName = "rtbrowser",
    //    //    SchemeHandlerFactory = new SchemeHandlerFactory()
    //    //});
    //    Cef.Initialize(settings);
    //    //handler = new RequestHandler(this);
    //    var webPage = new ChromiumWebBrowser(UpdateInfo.ChromeUrl);
    //    browserStuff.Controls.Add(webPage);
    //    //webPage.RequestHandler = handler;
    //    webPage.Dock = DockStyle.Fill;
    //}
    //internal class RequestHandler : IRequestHandler //Source - https://github.com/sharpbrowser/SharpBrowser licensed under MIT license
    //{
    //    private launcherBody launcherBody;

    //    public RequestHandler(launcherBody launcherBody)
    //    {
    //        this.launcherBody = launcherBody;
    //    }

    //    public bool GetAuthCredentials(IWebBrowser browserControl, IBrowser browser, IFrame frame, bool isProxy, string host, int port, string realm, string scheme, IAuthCallback callback)
    //    {
    //        return false;
    //    }

    //    public IResponseFilter GetResourceResponseFilter(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response)
    //    {
    //        return null;
    //    }

    //    public bool OnBeforeBrowse(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, bool isRedirect)
    //    {
    //        return false;
    //    }

    //    public CefReturnValue OnBeforeResourceLoad(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IRequestCallback callback)
    //    {
    //        return CefReturnValue.Continue;
    //    }

    //    public bool OnCertificateError(IWebBrowser browserControl, IBrowser browser, CefErrorCode errorCode, string requestUrl, ISslInfo sslInfo, IRequestCallback callback)
    //    {
    //        return true;
    //    }

    //    public bool OnOpenUrlFromTab(IWebBrowser browserControl, IBrowser browser, IFrame frame, string targetUrl, WindowOpenDisposition targetDisposition, bool userGesture)
    //    {
    //        return false;
    //    }

    //    public void OnPluginCrashed(IWebBrowser browserControl, IBrowser browser, string pluginPath){}

    //    public bool OnProtocolExecution(IWebBrowser browserControl, IBrowser browser, string url)
    //    {
    //        return true;
    //    }

    //    public bool OnQuotaRequest(IWebBrowser browserControl, IBrowser browser, string originUrl, long newSize, IRequestCallback callback)
    //    {
    //        callback.Continue(true);
    //        return true;
    //    }

    //    public void OnRenderProcessTerminated(IWebBrowser browserControl, IBrowser browser, CefTerminationStatus status){}

    //    public void OnRenderViewReady(IWebBrowser browserControl, IBrowser browser){}

    //    public void OnResourceLoadComplete(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response, UrlRequestStatus status, long receivedContentLength){}

    //    public void OnResourceRedirect(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response, ref string newUrl){}

    //    public bool OnResourceResponse(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response)
    //    {
    //        int code = response.StatusCode;
    //        if (code == 404)
    //        {
    //            request.Url = launcherBody.cachePage;
    //            return true;
    //        }
    //        if (code == 0 && (!File.Exists(launcherBody.cachePage)))
    //        {
    //            MessageBox.Show("" + File.Exists(launcherBody.cachePage) + request.Url);
    //            request.Url = launcherBody.cachePage;
    //            return true;
    //        }
    //        else
    //        {
    //            if (code == 444 || (code >= 500 && code <= 599))
    //            {
    //                request.Url = launcherBody.cachePage;
    //                return true;
    //            }
    //        }

    //        return false;
    //    }

    //    public bool OnSelectClientCertificate(IWebBrowser browserControl, IBrowser browser, bool isProxy, string host, int port, X509Certificate2Collection certificates, ISelectClientCertificateCallback callback)
    //    {
    //        return false;
    //    }
    //}

    //internal class SchemeHandlerFactory : ISchemeHandlerFactory
    //{
    //    public IResourceHandler Create(IBrowser browser, IFrame frame, string schemeName, IRequest request)
    //    {
    //        return new SchemeHandler(launcherBody.body);
    //    }
    //}

    //internal class SchemeHandler : IResourceHandler
    //{
    //    private launcherBody body;
    //    private Stream stream;
    //    string mimeType;
    //    Uri uri;
    //    string fileName;

    //    public SchemeHandler(launcherBody body)
    //    {
    //        this.body = body;
    //    }

    //    public void Cancel(){}

    //    public bool CanGetCookie(CefSharp.Cookie cookie)
    //    {
    //        return true;
    //    }

    //    public bool CanSetCookie(CefSharp.Cookie cookie)
    //    {
    //        return true;
    //    }

    //    public void Dispose(){}

    //    public void GetResponseHeaders(IResponse response, out long responseLength, out string redirectUrl)
    //    {
    //        responseLength = stream.Length;
    //        redirectUrl = null;

    //        response.StatusCode = (int)HttpStatusCode.OK;
    //        response.StatusText = "OK";
    //        response.MimeType = mimeType;
    //    }

    //    public bool ProcessRequest(IRequest request, ICallback callback)
    //    {
    //        uri = new Uri(request.Url);
    //        fileName = uri.AbsolutePath;
    //        if (uri.Host == "Cache")
    //        {
    //            fileName = launcherBody.appPath + uri.Host + fileName;
    //            if (File.Exists(fileName))
    //            {
    //                Task.Factory.StartNew(() => {
    //                    using (callback)
    //                    {
    //                        FileStream fStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
    //                        mimeType = ResourceHandler.GetMimeType(Path.GetExtension(fileName));
    //                        stream = fStream;
    //                        callback.Continue();
    //                    }
    //                });

    //                return true;
    //            }
    //        }
    //        callback.Dispose();
    //        return false;
    //    }

    //    public bool ReadResponse(Stream dataOut, out int bytesRead, ICallback callback)
    //    {
    //        callback.Dispose();

    //        if (stream == null)
    //        {
    //            bytesRead = 0;
    //            return false;
    //        }

    //        var buffer = new byte[dataOut.Length];
    //        bytesRead = stream.Read(buffer, 0, buffer.Length);

    //        dataOut.Write(buffer, 0, buffer.Length);

    //        return bytesRead > 0;
    //    }
    //}
    #endregion
}