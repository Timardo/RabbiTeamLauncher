using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace RabbiTeamLauncher
{
    public class StaticElements
    {
        public static readonly string AppPath = Application.StartupPath;
        public static readonly string ModpacksRoot = $"{AppPath}/modpacks/";
        public static readonly string SettingsPath = $"{AppPath}/settings.json";
        public static readonly string ModpacksPath = $"{AppPath}/modpacks.json";
        public static readonly string VersionsRoot = $"{AppPath}/resources/versions/";
        public static readonly string LibrariesRoot = $"{AppPath}/resources/libraries/";
        public static readonly string UpdateScriptPath = $"{AppPath}/update_launcher.bat";
        public static readonly string CacheRoot = $"{AppPath}/Cache/";
        public static readonly string TempRoot = $"{AppPath}/Cache/temp/";
        public static readonly string AssetsRoot = $"{AppPath}/resources/assets";
        public static readonly string AssetsObjectsRoot = $"{AssetsRoot}/objects/";
        public static readonly string AssetsIndexesRoot = $"{AssetsRoot}/indexes/";
        public static readonly string JavaInstallationPath = GetJavaInstallationPath();
        public static readonly UpdateInfo UpdateInfo = new UpdateInfo("https://raw.githubusercontent.com/Timardo/RabbiTeamLauncher/Default/UpdateInfo.txt");
        public static readonly Dictionary<string, string[]> FileIndex = new Dictionary<string, string[]>() //all files and folders that a launcher installation can have TODO: this is not a good way to store what files/folders the launcher needs
        {
            { "Deps", new string[] { "cef.pak", "cef_100_percent.pak", "cef_200_percent.pak", "cef_extensions.pak", "CefSharp.BrowserSubprocess.Core.dll", "CefSharp.BrowserSubprocess.exe", "CefSharp.Core.dll", "CefSharp.dll", "CefSharp.WinForms.dll", "chrome_elf.dll", "d3dcompiler_47.dll", "icudtl.dat", "libcef.dll", "natives_blob.bin", "snapshot_blob.bin", "Microsoft.WindowsAPICodePack.dll", "Microsoft.WindowsAPICodePack.Shell.dll", "Newtonsoft.Json.dll", "SharpCompress.dll" } },
            { "MCStuff", new string[] { "modpacks", "resources" } },
            { "LauncherStuff", new string[] { "debug.log", "settings.json", "RabbiTeamLauncher.exe", "locales", "Cache", "modpacks.json", "launcherlog.log" } }
        };

        // TODO: actually check for ćorrect Java version because 5 years ago latest Java was enough for everyone
        private static string GetJavaInstallationPath() //this is very bad, but one of the most reliable methods to get correct Java install path, if you know somehing better, let me know
        {
            try
            {
                string fileName = Environment.GetEnvironmentVariable("Path", EnvironmentVariableTarget.Machine).Split(';').Where(path => File.Exists($"{path}/java.exe")).FirstOrDefault();

                Process proc = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = $"{fileName}/java.exe",
                        Arguments = "-verbose",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true
                    }
                };

                proc.Start();
                string output = proc.StandardOutput.ReadToEnd();
                proc.WaitForExit();
                return output.Substring(8, output.IndexOf(']') - 19); // inconsistent magic - BAD
            }

            catch (Exception)
            {
                MessageBox.Show("The launcher was unable to locate Java installation. You may need to install or reinstall Java. The Program will exit now.", "Unable to locate Java", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                return null;
            }
        }
    }

    public class Constants
    {
        public const int DefaultRam = 3000;
        public const int ModpackJsonVersion = 1;
        public const string LauncherVersion = "1.0.3";
        public const string ForgeArtifactPath = "net/minecraftforge/forge/";
        public const string UUIDAPI = "http://tools.glowingmines.eu/convertor/nick/";
        public const string ForgeMavenUrl = "https://files.minecraftforge.net/maven/";
        public const string MinecraftLibrariesUrl = "https://libraries.minecraft.net/";
        public const string MinecraftResourcesUrl = "https://resources.download.minecraft.net/";
        public const string YandexAPIDirectory = "https://cloud-api.yandex.net/v1/disk/public/resources/?public_key=";
        public const string MinecraftVersionManifest = "https://launchermeta.mojang.com/mc/game/version_manifest.json";
        public const string YandexAPIDownload = "https://cloud-api.yandex.net/v1/disk/public/resources/download?public_key=";
        public const string DefaultArgs = "-Dfml.ignoreInvalidMinecraftCertificates=true -Dfml.ignorePatchDiscrepancies=true -XX:HeapDumpPath=MojangTricksIntelDriversForPerformance_javaw.exe_minecraft.exe.heapdump -XX:+UseConcMarkSweepGC -XX:+CMSIncrementalMode -XX:-UseAdaptiveSizePolicy -Xmn512M";
    }
}
