using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using Microsoft.VisualBasic.FileIO;

using static RabbiTeamLauncher.StaticElements;
using static RabbiTeamLauncher.Constants;
using RabbiTeamLauncher.Properties;
using System.Linq.Expressions;

namespace RabbiTeamLauncher
{
    public static class Utils
    {
        public static T FromJsonString<T>(string source) => JsonConvert.DeserializeObject<T>(source, Converter.Settings);

        public static string ToJsonString<T>(T source) => JsonConvert.SerializeObject(source, Converter.Settings);

        // TODO: this is a mess, that works, yes, but still needs a lot of love so it won't be pain to work with in the future
        // replace switc with multiple methods/objects that have preferably one specific function
        public static async Task NewDownload(
            DownloadType downloadType,
            string uri = null,
            string output = null,
            string downloadName = null,
            string fullForgeVersion = null,
            string forgeIdentifier = null,
            string mcVersion = null,
            DownloadAction completedAction = null)
        {
            try
            {
                switch (downloadType)
                {
                    case (DownloadType.Deps):
                        {
                            List<FileToDownload> filesToDownload = new List<FileToDownload>
                            {
                                new FileToDownload($"{StaticElements.UpdateInfo.NuGetRoot}cef.redist.x64.{StaticElements.UpdateInfo.CefLibVersion}.nupkg", $"{TempRoot}CefLib.zip"),
                                new FileToDownload($"{StaticElements.UpdateInfo.NuGetRoot}cefsharp.common.{StaticElements.UpdateInfo.CefSharpVersion}.nupkg", $"{TempRoot}CefSharpCore.zip"),
                                new FileToDownload($"{StaticElements.UpdateInfo.NuGetRoot}cefsharp.winforms.{StaticElements.UpdateInfo.CefSharpVersion}.nupkg", $"{TempRoot}CefSharpForms.zip"),
                                new FileToDownload($"{StaticElements.UpdateInfo.NuGetRoot}windowsapicodepack.{StaticElements.UpdateInfo.WinAPICodePackVersion}.nupkg", $"{TempRoot}WinApiPack.zip"),
                                new FileToDownload($"{StaticElements.UpdateInfo.NuGetRoot}newtonsoft.json.{StaticElements.UpdateInfo.NewtonSoftJsonVersion}.nupkg", $"{TempRoot}NewtonSoftJson.zip"),
                                new FileToDownload($"{StaticElements.UpdateInfo.NuGetRoot}sharpcompress.{StaticElements.UpdateInfo.SharpCompressVersion}.nupkg", $"{TempRoot}SharpCompress.zip")
                            };

                            DownloadJob job = new DownloadJob(filesToDownload, new DownloadAction(ExtractDeps, DownloadActionArgs.Empty));
                            Launcher.Instance.Downloader.AddDownload(job, downloadName);
                        }
                        break;
                    case (DownloadType.ForgeVersion): //TODO - distinguish older forge version formats from the new one
                        {
                            //Downloader.PendingForge.Add(fullForgeVersion); // TODO: ADD AGAIN - used to avoid scheduling downloading the same thing twice + the URI needs testing and improvements
                            string forgeFileOnlinePath = GetForgeFilePath(fullForgeVersion, ForgeMavenUrl, true);
                            string forgeFileOfflinePath = GetForgeFilePath(fullForgeVersion, LibrariesRoot, false);
                            DownloadJob job = new DownloadJob(new FileToDownload(forgeFileOnlinePath, forgeFileOfflinePath), new DownloadAction(ExtractForgeVersionAndResolveLibs, new ForgeDownloadCompletedActionArgs(fullForgeVersion, forgeIdentifier)));
                            Launcher.Instance.Downloader.AddDownload(job, downloadName);
                        }
                        break;
                    case (DownloadType.MinecraftVersion):
                        {
                            //Downloader.PendingMc.Add(mcVersion); // TODO: ADD AGAIN - used to avoid scheduling downloading the same thing twice
                            McVersionsManifest manifest = FromJsonString<McVersionsManifest>(GetResponse(MinecraftVersionManifest)); //download latest manifest version
                            string url = null;

                            DownloadJob job = new DownloadJob();
                            
                            foreach (Versions version in manifest.Versions)
                            {
                                if (version.Id == mcVersion)
                                {
                                    url = version.Url; //find mc version json
                                    break;
                                }
                            }

                            string mcJsonResp = GetResponse(url);
                            McVersionJson mcJson = FromJsonString<McVersionJson>(mcJsonResp);

                            string mcJsonPath = VersionsRoot + mcVersion + "/" + mcVersion + ".json";
                            if (!Directory.Exists(DirectoryOf(mcJsonPath)))
                            {
                                Directory.CreateDirectory(DirectoryOf(mcJsonPath));
                            }

                            File.WriteAllText(mcJsonPath, mcJsonResp);

                            foreach (Library lib in mcJson.Libraries) //resolve libs
                            {
                                if (lib.Rules != null) //check if rules match our environment
                                {
                                    foreach (Rules rule in lib.Rules)
                                    {
                                        if (rule.Action == "allow") // there are multiple if statements NOT merged into one with AND for future expansion of rule handling and readability
                                        {
                                            if (rule.Os != null)
                                            {
                                                if (rule.Os.Name == "windows")
                                                {
                                                    break; //if so, continue
                                                }

                                                else
                                                {
                                                    goto skipDownload; //if one rule doesn't match the environent needs, abort the whole download -- needs testing
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
                                            if (Environment.Is64BitOperatingSystem) //resolve architecture and add to download list
                                            {
                                                if (!File.Exists(LibrariesRoot + lib.Downloads.Classifiers.NativesWindows64.Path))
                                                {
                                                    job.FilesToDownload.Add(new FileToDownload(lib.Downloads.Classifiers.NativesWindows64.Url, LibrariesRoot + lib.Downloads.Classifiers.NativesWindows64.Path));
                                                }
                                            }

                                            else
                                            {
                                                if (!File.Exists(LibrariesRoot + lib.Downloads.Classifiers.NativesWindows32.Path))
                                                {
                                                    job.FilesToDownload.Add(new FileToDownload(lib.Downloads.Classifiers.NativesWindows32.Url, LibrariesRoot + lib.Downloads.Classifiers.NativesWindows32.Path));
                                                }
                                            }
                                        }

                                        else //if the lib doesn't contain arch-specific natives, just set the default ones for windows
                                        {
                                            job.FilesToDownload.Add(new FileToDownload(lib.Downloads.Classifiers.NativesWindows.Url, LibrariesRoot + lib.Downloads.Classifiers.NativesWindows.Path));
                                        }
                                    }
                                }

                                if (lib.Downloads.Artifact != null)
                                {
                                    if (!File.Exists(LibrariesRoot + lib.Downloads.Artifact.Path))
                                    {
                                        job.FilesToDownload.Add(new FileToDownload(lib.Downloads.Artifact.Url, LibrariesRoot + lib.Downloads.Artifact.Path));
                                    }
                                }
skipDownload:; // using labels is the easiest way of breaking from nested for loops
                            }

                            job.FilesToDownload.Add(new FileToDownload(mcJson.Downloads.Client.Url, VersionsRoot + mcJson.Id + "/" + mcJson.Id + ".jar"));

                            if (!File.Exists(LibrariesRoot + mcJson.Assets + ".json"))
                            {
                                string assetsIndexResp = GetResponse(mcJson.AssetIndex.Url);
                                AssetIndex assetIndexJson = FromJsonString<AssetIndex>(assetsIndexResp);

                                if (!Directory.Exists(AssetsIndexesRoot))
                                {
                                    Directory.CreateDirectory(AssetsIndexesRoot);
                                }

                                File.WriteAllText(AssetsIndexesRoot + mcJson.Assets + ".json", assetsIndexResp);

                                foreach (var hash in assetIndexJson.Objects)
                                {
                                    if (!File.Exists(AssetsObjectsRoot + hash.Value.Hash.Substring(0, 2) + "/" + hash.Value.Hash))
                                    {
                                        job.FilesToDownload.Add(new FileToDownload(MinecraftResourcesUrl + hash.Value.Hash.Substring(0, 2) + "/" + hash.Value.Hash, AssetsObjectsRoot + hash.Value.Hash.Substring(0, 2) + "/" + hash.Value.Hash));
                                    }
                                }
                            }

                            Launcher.Instance.Downloader.AddDownload(job, downloadName);
                        }
                        break;
                    case (DownloadType.ModpackFresh):
                        {
                            string modpackJsonYadiskUrl = RemoveNewLines(GetResponse((StaticElements.UpdateInfo.ModpackListRoot + downloadName + ".txt"))) + "&path=/modpack.json";
                            string modpackJsonContent = GetResponse(GetYadiskDownUrl(modpackJsonYadiskUrl));
                            ModPackOnlineJson packOnlineJson = FromJsonString<ModPackOnlineJson>(modpackJsonContent);
                            string _fullForgeVersion = packOnlineJson.ForgeAdditionalIdentifier != null ? packOnlineJson.MinecraftVersion + "-" + packOnlineJson.ForgeVersion + "-" + packOnlineJson.ForgeAdditionalIdentifier : packOnlineJson.MinecraftVersion + "-" + packOnlineJson.ForgeVersion;

                            if (!Directory.Exists(VersionsRoot + packOnlineJson.MinecraftVersion)/* && !Downloader.PendingMc.Contains(packOnlineJson.MinecraftVersion)*/)
                            {
                                await NewDownload(DownloadType.MinecraftVersion, mcVersion: packOnlineJson.MinecraftVersion);
                            }

                            //TODO - versions prior to 1.6.1 don't have universal jar, oldest ones have client.zip files
                            if (!Directory.Exists(LibrariesRoot + ForgeArtifactPath + _fullForgeVersion) && (packOnlineJson.ForgeVersion != null)/* && !Downloader.PendingForge.Contains(_fullForgeVersion)*/)
                            {
                                await NewDownload(DownloadType.ForgeVersion, fullForgeVersion: _fullForgeVersion, forgeIdentifier: packOnlineJson.ForgeFullId);
                            }

                            string modpackUrl = GetYadiskDownUrl(GetResponse((StaticElements.UpdateInfo.ModpackListRoot + downloadName + ".txt")));
                            string outputPath = "/Cache/temp-modpack-" + downloadName + ".zip";
                            string extractedModpackName = GetYadiskDirName(GetResponse((StaticElements.UpdateInfo.ModpackListRoot + downloadName + ".txt")));
                            DownloadJob job = new DownloadJob(new FileToDownload(modpackUrl, outputPath), new DownloadAction(ModpackFresh, new ModpackDownloadCompletedActionArgs(downloadName, extractedModpackName)));
                            Launcher.Instance.Downloader.AddDownload(job, downloadName);
                        }
                        break;
                    case (DownloadType.ModpackUpdate):
                        {
                            //getting the changes from changelog
                            string[] separator = new string[] { Environment.NewLine };
                            string version = Launcher.Instance.Packs.ModpackList[downloadName].Version;
                            string yadiskUrl = GetResponse((StaticElements.UpdateInfo.ModpackListRoot + downloadName + ".txt"));
                            string[] fileLines = GetResponse(GetYadiskDownUrl(yadiskUrl + "&path=/changelog.txt")).Split(separator, StringSplitOptions.RemoveEmptyEntries);
                            string[] split = new string[2];
                            bool newerVersion = false;
                            Dictionary<string, string> changes = new Dictionary<string, string>();

                            //resolving changes
                            for (int i = 0; i < fileLines.Length; i++)
                            {
                                split = fileLines[i].Split(separator, StringSplitOptions.RemoveEmptyEntries);

                                if (fileLines[i].StartsWith("VERSION, "))
                                {
                                    newerVersion = IsHigherThan(split[1], version);
                                }

                                else if (fileLines[i].StartsWith("CHANGED, ") && newerVersion)
                                {
                                    if (changes.ContainsKey(split[1]))
                                    {
                                        changes[split[1]] = "CHANGED";
                                    }

                                    else
                                    {
                                        changes.Add(split[1], "CHANGED");
                                    }
                                }

                                else if (fileLines[i].StartsWith("ADDED, ") && newerVersion)
                                {
                                    if (changes.ContainsKey(split[1]))
                                    {
                                        changes[split[1]] = "ADDED";
                                    }

                                    else
                                    {
                                        changes.Add(split[1], "ADDED");
                                    }
                                }

                                else if (fileLines[i].StartsWith("REMOVED, ") && newerVersion)
                                {
                                    if (changes.ContainsKey(split[1]))
                                    {
                                        changes[split[1]] = "REMOVED";
                                    }

                                    else
                                    {
                                        changes.Add(split[1], "REMOVED");
                                    }
                                }
                            }

                            //making the bat
                            var delLines = "";
                            var downloadFiles = new List<FileToDownload>();

                            foreach (var item in changes)
                            {
                                if (item.Value.Contains("REMOVED"))
                                {
                                    delLines += ("del /f /q " + item.Key + Environment.NewLine);
                                }

                                else if (item.Value.Contains("CHANGED") || item.Value.Contains("ADDED"))
                                {
                                    downloadFiles.Add(new FileToDownload(GetYadiskDownUrl(yadiskUrl + "&path=" + item.Key), item.Key));
                                }
                            }
                            
                            delLines += "(goto) 2>nul & del \"%~f0\"" + Environment.NewLine +
                                "exit 0"; //these 2 lines will delete the bat script after executing

                            File.WriteAllText($"{AppPath}/update_script.bat", delLines);
                            DownloadJob job = new DownloadJob(downloadFiles, new DownloadAction(UpdateModpack, new ModpackDownloadCompletedActionArgs(downloadName)));
                            Launcher.Instance.Downloader.AddDownload(job, downloadName);
                        }
                        break;
                    case (DownloadType.Other):
                        {
                            DownloadJob job = new DownloadJob(new FileToDownload(uri, output, completedAction));
                            Launcher.Instance.Downloader.AddDownload(job, downloadName);
                        }
                        break;
                }
            }

            catch (Exception ex)
            {
                throw new DownloadConstructorException(ex);
            }
        }

        public static void ExtractNatives(string pathToFIle, string libraryPath)
        {
            try
            {
                if (!Directory.Exists(libraryPath))
                {
                    Directory.CreateDirectory(libraryPath);
                }

                using (ZipArchive nativesJar = ZipFile.OpenRead(pathToFIle))
                {
                    foreach (ZipArchiveEntry entry in nativesJar.Entries)
                    {
                        if (!entry.Name.Equals("MANIFEST.MF") && !entry.FullName.EndsWith("/"))
                        {
                            if (!File.Exists($"{libraryPath}/{entry.Name}"))
                            {
                                entry.ExtractToFile($"{libraryPath}/{entry.Name}");
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show("Extract Natives failed - " + ex);
            }
        }

        public static void ExtractDeps(DownloadActionArgs _)
        {
            try
            {
                var index = new string[] { "CefLib", "CefSharpCore", "CefSharpForms", "WinApiPack", "NewtonSoftJson", "SharpCompress" };
                string tempDepsDir = CacheRoot + "temp_deps";
                string tempDepsDirAll = CacheRoot + "temp_all";

                foreach (var entry in index)
                {
                    Launcher.Instance.Downloader.SetLabel("Extracting " + entry + "...");
                    Directory.CreateDirectory(tempDepsDir);
                    ZipFile.ExtractToDirectory(TempRoot + entry + ".zip", tempDepsDir);
                    FileSystem.MoveDirectory(tempDepsDir, tempDepsDirAll, true);
                }

                Launcher.Instance.Downloader.SetLabel("Finishing...");
                FileSystem.MoveDirectory(tempDepsDirAll + "/CEF", tempDepsDirAll, true);
                FileSystem.MoveDirectory(tempDepsDirAll + "/CefSharp/x64", tempDepsDirAll, true);
                FileSystem.MoveDirectory(tempDepsDirAll + "/lib/net45", tempDepsDirAll, true);
                FileSystem.MoveDirectory(tempDepsDirAll + "/lib", tempDepsDirAll, true);
                DirectoryInfo root = new DirectoryInfo(tempDepsDirAll);

                foreach (var subDirectory in root.EnumerateDirectories())
                {
                    if (!string.Join(", ", FileIndex["Deps"]).Contains(subDirectory.Name))
                    {
                        Directory.Delete(subDirectory.FullName, true);
                    }
                }

                foreach (var file in root.GetFiles())
                {
                    if (!string.Join(", ", FileIndex["Deps"]).Contains(file.Name))
                    {
                        file.Delete();
                    }
                }

                FileSystem.MoveDirectory(tempDepsDirAll, AppPath, true);
                Directory.Delete(TempRoot.Substring(0, TempRoot.Length - 1), true);
                Directory.Delete(AppPath + "/CefSharp", true);
                Application.Restart();
            }

            catch (Exception ex)
            {
                MessageBox.Show("ExtractDeps failed - " + ex);
            }
        }

        public static void ModpackFresh(DownloadActionArgs args)
        {
            ModpackDownloadCompletedActionArgs castedArgs = args as ModpackDownloadCompletedActionArgs;

            try
            {
                var path = CacheRoot + "temp-modpack-" + castedArgs.ModpackName + ".zip";
                Launcher.Instance.Downloader.SetLabel("Extracting " + castedArgs.ModpackName);

                if (!Directory.Exists(ModpacksRoot.Substring(0, ModpacksRoot.Length - 1)))
                {
                    Directory.CreateDirectory(ModpacksRoot.Substring(0, ModpacksRoot.Length - 1));
                }

                ZipFile.ExtractToDirectory(path, ModpacksRoot.Substring(0, ModpacksRoot.Length - 1));
                FileSystem.RenameDirectory(ModpacksRoot + castedArgs.ExtractedModpackName, castedArgs.ModpackName);
                ModPackOnlineJson onlineJson = FromJsonString<ModPackOnlineJson>(GetResponse(GetYadiskDownUrl(GetResponse((StaticElements.UpdateInfo.ModpackListRoot + castedArgs.ModpackName + ".txt")) + "&path=/modpack.json")));

                if (Launcher.Instance.Packs.ModpackList.ContainsKey(onlineJson.Name))
                {
                    MessageBox.Show("Something went wrong and modpack " + onlineJson.Name + " couldn't finish it's installation, you probably already have this modpack installed");
                    return;
                }

                var newEntry = new ModpackEntry
                {
                    Version = onlineJson.LatestVersion,
                    MinecraftVersion = onlineJson.MinecraftVersion,
                    ForgeVersion = onlineJson.ForgeVersion,
                    ForgeAdditionalIdentifier = onlineJson.ForgeAdditionalIdentifier,
                    ForgeFullId = onlineJson.ForgeFullId,
                    Id = onlineJson.Id,
                    ModList = onlineJson.ModList,
                    AdditionalLibs = onlineJson.AdditionalLibs,
                    CustomLibs = onlineJson.CustomLibs,
                    DefaultJavaArgs = onlineJson.DefaultJavaArgs,
                    Description = onlineJson.Description,
                    ModpackType = "RTLauncher",
                    NumberOfRuns = 0, // not implemented yet
                    AutoUpdate = false
                };

                Launcher.Instance.Packs.ModpackList.Add(onlineJson.Name, newEntry);
                File.WriteAllText(ModpacksPath, ToJsonString(Launcher.Instance.Packs));
                Launcher.Instance.UpdatePackList();
            }

            catch (Exception ex)
            {
                MessageBox.Show("ModpackFresh failed - " + ex);
            }
        }

        public static void UpdateModpack(DownloadActionArgs args)
        {
            ModpackDownloadCompletedActionArgs castedArgs = args as ModpackDownloadCompletedActionArgs;
            Launcher.Instance.Packs.ModpackList[castedArgs.ModpackName].Version = FromJsonString<ModPackOnlineJson>(GetResponse(GetYadiskDownUrl(GetResponse((StaticElements.UpdateInfo.ModpackListRoot + castedArgs.ModpackName + ".txt"))))).LatestVersion;
        }

        public static void ExtractForgeVersionAndResolveLibs(DownloadActionArgs args) //TODO - compatibility with all forge versions
        {
            ForgeDownloadCompletedActionArgs castedArgs = args as ForgeDownloadCompletedActionArgs;
            string pathhh = GetForgeFilePath(castedArgs.ForgeVersion, LibrariesRoot, false);
            using (ZipArchive forge = ZipFile.OpenRead(pathhh))
            {
                foreach (ZipArchiveEntry entry in forge.Entries)
                {
                    if (entry.FullName.Equals("version.json"))
                    {
                        if (!Directory.Exists(VersionsRoot + castedArgs.ForgeIdentifier))
                        {
                            Directory.CreateDirectory(VersionsRoot + castedArgs.ForgeIdentifier);
                        }

                        if (!File.Exists(VersionsRoot + castedArgs.ForgeIdentifier + "/" + castedArgs.ForgeIdentifier + ".json"))
                        {
                            entry.ExtractToFile(VersionsRoot + castedArgs.ForgeIdentifier + "/" + castedArgs.ForgeIdentifier + ".json");
                        }

                        break;
                    }
                }
            }

            ForgeJson forgeJson = FromJsonString<ForgeJson>(File.ReadAllText(VersionsRoot + castedArgs.ForgeIdentifier + "/" + castedArgs.ForgeIdentifier + ".json"));
            string pathToLib;
            string url;
            bool isForgeLib;
            DownloadJob job = new DownloadJob();

            foreach (var lib in forgeJson.Libraries)
            {
                isForgeLib = lib.Url != null;
                //pathToLib = isForgeLib ? lib.Name.ToPath(false) + ".pack.xz" : lib.Name.ToPath(false);
                pathToLib = lib.Name.ToPath(false);

                if (!File.Exists(AppPath + "/" + lib.Name.ToPath()) && !lib.Name.Contains("minecraftforge"))
                {
                    url = isForgeLib ? lib.Url : MinecraftLibrariesUrl;
                    job.FilesToDownload.Add(new FileToDownload(url + pathToLib, LibrariesRoot + pathToLib));
                }
            }

            Launcher.Instance.Downloader.AddDownload(job, "Forge libraries");
            //Downloader.PendingForge.Remove(args.ForgeVersion);
        }

        public static void UpdateLauncher(DownloadActionArgs _)
        {
            var command = "@echo off" + Environment.NewLine +
                ":deleteloop" + Environment.NewLine +
                "tasklist /fi \"imagename eq RabbiTeamLauncher.exe\" |find \":\" > nul" + Environment.NewLine +
                "if errorlevel 1 taskkill /f /im \"RabbiTeamLauncher.exe\"&goto :deleteloop2" + Environment.NewLine +
                ":deleteloop2" + Environment.NewLine +
                "tasklist /fi \"imagename eq CefSharp.BrowserSubprocess.exe\" |find \":\" > nul" + Environment.NewLine +
                "if errorlevel 1 taskkill /f /im \"CefSharp.BrowserSubprocess.exe\"&goto :rest" + Environment.NewLine +
                ":rest" + Environment.NewLine +
                "del /f /q RabbiTeamLauncher.exe" + Environment.NewLine +
                "ren RabbiTeamLauncherNew.exe RabbiTeamLauncher.exe" + Environment.NewLine +
                "echo ################Launcher Updated!####################" + Environment.NewLine +
                "start RabbiTeamLauncher.exe" + Environment.NewLine +
                "(goto) 2>nul & del \"%~f0\"" + Environment.NewLine +
                "exit 0";

            File.WriteAllText(UpdateScriptPath, command);

            if (MessageBox.Show("An update for Launcher has been downloaded and is ready to replace this one, do you want to proceed now? Check if you don't have any downloads pending!" + Environment.NewLine + Environment.NewLine + "If you click on No, Launcher will update after closing.", "Updater", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Process.Start(UpdateScriptPath);
            }
        }

        public static bool IsHigherThan(string higher, string lower)
        {
            var lowerArr = lower.Split('.');
            var higherArr = higher.Split('.');

            for (int i = 0; i < Math.Max(lowerArr.Length, higherArr.Length); i++)
            {
                try
                {
                    if (Convert.ToInt32(lowerArr[i]) < Convert.ToInt32(higherArr[i]))
                    {
                        return true;
                    }
                }

                catch
                {
                    return false;
                }
            }

            return false;
        }

        public static string ToPath(this string artifactName, bool filesystem = true)
        {
            var splitStart = artifactName.Substring(0, artifactName.IndexOf(':'));
            var splitEnd = artifactName.Remove(0, splitStart.Length).Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
            splitStart = filesystem ? splitStart.Replace('.', '\\') : splitStart.Replace('.', '/');

            return filesystem ? splitStart + "\\" + splitEnd[0] + "\\" + splitEnd[1] + "\\" + splitEnd[0] + "-" + splitEnd[1] + ".jar" :
                splitStart + "/" + splitEnd[0] + "/" + splitEnd[1] + "/" + splitEnd[0] + "-" + splitEnd[1] + ".jar";
        }

        //returns path to directory from file path
        public static string DirectoryOf(string filePath) => filePath.Substring(0, filePath.LastIndexOf('/'));

        public static string GetResponse(string rawUrl)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(rawUrl);
            HttpWebResponse response;

            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }

            catch (Exception ex)
            {
                throw new GetResponseException(ex);
            }

            string output;

            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                output = reader.ReadToEnd();
            }

            return output;
        }

        //request is the public key optionally with path part
        public static string GetYadiskDownUrl(string request)
        {
            return FromJsonString<YandexDiskDownJson>(GetResponse((YandexAPIDownload + RemoveNewLines(request)))).DownloadUrl;
        }

        public static string GetYadiskDirName(string yadiskUrl)
        {
            return FromJsonString<YandexDiskJson>(GetResponse((YandexAPIDirectory + RemoveNewLines(yadiskUrl) + "&limit=0"))).Name;
        }

        public static string RemoveNewLines(string source)
        {
            var newLineIdentifiers = new string[] { Environment.NewLine, "\r\n", "\r", "\n" };
            return newLineIdentifiers.Aggregate(source, (current, toReplace) => current.Replace(toReplace, ""));
        }

        public static string GetUUID(string nick)
        {
            string response = "";
            bool hasUuid = true;

            try
            {
                response = GetResponse(UUIDAPI + nick); //will be offical mojang server *soon* and this one will be for offline UUIDs only
            }

            catch (GetResponseException)
            {
                hasUuid = false;
            }

            string uuid = Launcher.Instance.Settings.LatestUUID;

            if (hasUuid == true)
            {
                var JSONObj = FromJsonString<Dictionary<string, string>>(response);
                uuid = JSONObj["offlineuuid"];
                Launcher.Instance.Settings.LatestUUID = uuid;
            }

            return uuid;
        }

        private static string GetForgeFilePath(string fullForgeVersion, string startingPath, bool online)
        {
            return $"{startingPath}/{ForgeArtifactPath}{fullForgeVersion}/forge-{fullForgeVersion}{(online ? "-universal" : "")}.jar";
        }
    }
}
