using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DL = RabbiTeamLauncher.Downloader;
using LB = RabbiTeamLauncher.LauncherBody;
using FS = Microsoft.VisualBasic.FileIO.FileSystem;
using SharpCompress.Compressors.Xz;
using Microsoft.Win32;
using System.Diagnostics;

namespace RabbiTeamLauncher
{
    public static class Utils
    {
        public delegate Task DownloadCompletedAction(DownloadCompletedActionArgs args);
        public delegate Task DownloadBeforeStartAction(DownloadBeforeStartActionArgs args);

        public static async Task DownloaderConstructor(DownloadType downloadType = DownloadType.Other,
            string uri = null,
            string output = null,
            string downloadName = null,
            string fullForgeVersion = null,
            string forgeIdentifier = null,
            string mcVersion = null,
            DownloaderSettings settings = null,
            DownloadCompletedAction completedAction = null,
            DownloadCompletedActionArgs completedArgs = null,
            DownloadBeforeStartAction beforeAction = null,
            DownloadBeforeStartActionArgs beforeArgs = null)
        {
            switch (downloadType)
            {
                case (DownloadType.Deps):
                    {
                        settings = new DownloaderSettings
                        {
                            FilesToDownload = new List<FilesToDownload> {
                                new FilesToDownload
                                {
                                    Uri = LB.UpdateInfo.NuGetRoot + "cef.redist.x64." + LB.UpdateInfo.CefLibVersion + ".nupkg",
                                    OutputPath = "/Cache/temp/CefLib.zip"
                                },
                                new FilesToDownload
                                {
                                    Uri = LB.UpdateInfo.NuGetRoot + "cefsharp.common." + LB.UpdateInfo.CefSharpVersion + ".nupkg",
                                    OutputPath = "/Cache/temp/CefSharpCore.zip"
                                },
                                new FilesToDownload
                                {
                                    Uri = LB.UpdateInfo.NuGetRoot + "cefsharp.winforms." + LB.UpdateInfo.CefSharpVersion + ".nupkg",
                                    OutputPath = "/Cache/temp/CefSharpForms.zip"
                                },
                                new FilesToDownload
                                {
                                    Uri = LB.UpdateInfo.NuGetRoot + "windowsapicodepack." + LB.UpdateInfo.WinAPICodePackVersion + ".nupkg",
                                    OutputPath = "/Cache/temp/WinApiPack.zip"
                                },
                                new FilesToDownload
                                {
                                    Uri = LB.UpdateInfo.NuGetRoot + "newtonsoft.json." + LB.UpdateInfo.NewtonSoftJsonVersion + ".nupkg",
                                    OutputPath = "/Cache/temp/NewtonSoftJson.zip"
                                },
                                new FilesToDownload
                                {
                                    Uri = LB.UpdateInfo.NuGetRoot + "sharpcompress." + LB.UpdateInfo.SharpCompressVersion + ".nupkg",
                                    OutputPath = "/Cache/temp/SharpCompress.zip"
                                }
                            },
                            CompletedAction = ExtractDeps
                        };
                    }
                    break;
                case (DownloadType.ForgeVersion): //TODO - distinguish older forge version formats from the new one
                    {
                        DL.PendingForge.Add(fullForgeVersion);
                        settings = new DownloaderSettings
                        {
                            FilesToDownload = new List<FilesToDownload>
                            {
                                new FilesToDownload
                                {
                                    Uri = "https://files.minecraftforge.net/maven/net/minecraftforge/forge/" + fullForgeVersion + "/forge-" + fullForgeVersion + "-universal.jar", //needs testing and improvements
                                    OutputPath = "/resources/libraries/net/minecraftforge/forge/" + fullForgeVersion + "/forge-" + fullForgeVersion + ".jar"
                                }
                            },
                            CompletedAction = ExtractForgeVersionAndResolveLibs,
                            CompletedArgs = new DownloadCompletedActionArgs
                            {
                                ForgeVersion = fullForgeVersion,
                                ForgeIdentifier = forgeIdentifier
                            }
                        };
                    }
                    break;
                case (DownloadType.MinecraftVersion):
                    {
                        var relativeLibsPath = "/resources/libraries/";
                        DL.pendingMc.Add(mcVersion);
                        Directory.CreateDirectory(LB.AppPath + "/resources/versions/" + mcVersion);
                        var manifest = "https://launchermeta.mojang.com/mc/game/version_manifest.json".GetResponse().FromJsonString<McVersionsManifest>(); //download latest manifest version
                        string url = null;

                        settings = new DownloaderSettings
                        {
                            FilesToDownload = new List<FilesToDownload> { }
                        };

                        foreach (var version in manifest.Versions)
                            if (version.Id == mcVersion)
                            {
                                url = version.Url; //find mc version json
                                break;
                            }

                        var mcJsonResp = url.GetResponse();
                        var mcJson = mcJsonResp.FromJsonString<McVersionJson>();

                        using (FileStream fs = File.Create(LB.AppPath + "/resources/versions/" + mcVersion + "/" + mcVersion + ".json"))
                        {
                            Byte[] lines = new UTF8Encoding(true).GetBytes(mcJsonResp);
                            fs.Write(lines, 0, lines.Length);
                        }

                        foreach (var lib in mcJson.Libraries) //resolve libs
                        {
                            if (lib.Rules != null) //check if rules match our environment
                                foreach (var rule in lib.Rules)
                                {
                                    if (rule.Action == "allow")
                                        if (rule.Os != null)
                                            if (rule.Os.Name == "windows")
                                                goto continueDownload; //if so, continue

                                            else goto skipDownload; //if one rule doesn't match the environent needs, abort the whole download -- needs testing
                                }

                            continueDownload:

                            if (lib.Natives != null) //whether the lib contains natives
                                if (lib.Natives.Windows != null) //whether the lib contains also windows natives
                                    if (lib.Natives.Windows.Contains("${arch}")) //whether the natives are arch-specific
                                    {
                                        if (Environment.Is64BitOperatingSystem) //resolve architecture and add to download list
                                        {
                                            if (!File.Exists(LB.AppPath + relativeLibsPath + lib.Downloads.Classifiers.NativesWindows64.Path))
                                                settings.FilesToDownload.Add(new FilesToDownload
                                                {
                                                    Uri = lib.Downloads.Classifiers.NativesWindows64.Url,
                                                    OutputPath = relativeLibsPath + lib.Downloads.Classifiers.NativesWindows64.Path
                                                });
                                        }

                                        else
                                        {
                                            if (!File.Exists(LB.AppPath + relativeLibsPath + lib.Downloads.Classifiers.NativesWindows32.Path))
                                                settings.FilesToDownload.Add(new FilesToDownload
                                                {
                                                    Uri = lib.Downloads.Classifiers.NativesWindows32.Url,
                                                    OutputPath = relativeLibsPath + lib.Downloads.Classifiers.NativesWindows32.Path
                                                });
                                        }
                                    }

                                    else //if the lib doesn't contain arch-specific natives, just set the default ones for windows
                                        settings.FilesToDownload.Add(new FilesToDownload
                                        {
                                            Uri = lib.Downloads.Classifiers.NativesWindows.Url,
                                            OutputPath = relativeLibsPath + lib.Downloads.Classifiers.NativesWindows.Path
                                        });

                                else goto skipNatives; //if the lib doesn't contain windows natives, skipping natives download

                            skipNatives:

                            if (lib.Downloads.Artifact != null)
                            {
                                if (!File.Exists(LB.AppPath + relativeLibsPath + lib.Downloads.Artifact.Path))
                                    settings.FilesToDownload.Add(new FilesToDownload
                                    {
                                        Uri = lib.Downloads.Artifact.Url,
                                        OutputPath = relativeLibsPath + lib.Downloads.Artifact.Path
                                    });
                            }

                            skipDownload:;
                        }

                        settings.FilesToDownload.Add(new FilesToDownload
                        {
                            Uri = mcJson.Downloads.Client.Url,
                            OutputPath = "/resources/versions/" + mcJson.Id + "/" + mcJson.Id + ".jar"
                        });
                        string assetsIndexResp;
                        AssetIndex assetIndexJson;

                        if (!File.Exists(LB.AppPath + "/resources/assets/indexes/" + mcJson.Assets + ".json"))
                        {
                            assetsIndexResp = mcJson.AssetIndex.Url.GetResponse();
                            assetIndexJson = assetsIndexResp.FromJsonString<AssetIndex>();

                            if (!Directory.Exists(LB.AppPath + "/resources/assets/indexes"))
                                Directory.CreateDirectory(LB.AppPath + "/resources/assets/indexes");

                            using (FileStream fs = File.Create(LB.AppPath + "/resources/assets/indexes/" + mcJson.Assets + ".json"))
                            {
                                Byte[] lines = new UTF8Encoding(true).GetBytes(assetsIndexResp);
                                fs.Write(lines, 0, lines.Length);
                            }

                            foreach (var hash in assetIndexJson.Objects)
                                if (!File.Exists(LB.AppPath + "/resources/assets/objects/" + hash.Value.Hash.Substring(0, 2) + "/" + hash.Value.Hash))
                                    settings.FilesToDownload.Add(new FilesToDownload
                                    {
                                        Uri = "http://resources.download.minecraft.net/" + hash.Value.Hash.Substring(0, 2) + "/" + hash.Value.Hash,
                                        OutputPath = "/resources/assets/objects/" + hash.Value.Hash.Substring(0, 2) + "/" + hash.Value.Hash
                                    });
                        }
                    }
                    break;
                case (DownloadType.ModpackFresh):
                    {
                        var packOnlineJson = ((LB.ModpackUrl + downloadName + ".txt").GetResponse().RemoveNewLines() + "&path=/modpack.json").GetYadiskDownUrl().GetResponse().FromJsonString<ModPackOnlineJson>();
                        var _fullForgeVersion = packOnlineJson.ForgeAdditionalIdentifier != null ? packOnlineJson.MinecraftVersion + "-" + packOnlineJson.ForgeVersion + "-" + packOnlineJson.ForgeAdditionalIdentifier : packOnlineJson.MinecraftVersion + "-" + packOnlineJson.ForgeVersion;
                        
                        if (!Directory.Exists(LB.AppPath + "/resources/versions/" + packOnlineJson.MinecraftVersion) && !DL.pendingMc.Contains(packOnlineJson.MinecraftVersion))
                            await DownloaderConstructor(mcVersion: packOnlineJson.MinecraftVersion, downloadType: DownloadType.MinecraftVersion);
                        //TODO - versions prior to 1.6.1 doesn't have universal jar, oldest ones have client.zip files
                        if (!Directory.Exists(LB.AppPath + "/resources/libraries/net/minecraftforge/forge/" + _fullForgeVersion) && (packOnlineJson.ForgeVersion != null) && !DL.PendingForge.Contains(_fullForgeVersion))
                            await DownloaderConstructor(fullForgeVersion: _fullForgeVersion, forgeIdentifier: packOnlineJson.ForgeFullId, downloadType: DownloadType.ForgeVersion);
                        
                        settings = new DownloaderSettings
                        {
                            FilesToDownload = new List<FilesToDownload>
                            {
                                new FilesToDownload
                                {
                                    Uri = (LB.ModpackUrl + downloadName + ".txt").GetResponse().GetYadiskDownUrl(),
                                    OutputPath = "/Cache/temp-modpack-" + downloadName + ".zip"
                                }
                            },
                            CompletedAction = ModpackFresh,
                            CompletedArgs = new DownloadCompletedActionArgs
                            {
                                ModpackName = downloadName,
                                ExtractedModpackName = (LB.ModpackUrl + downloadName + ".txt").GetResponse().GetYadiskDirName()
                            }
                        };
                    }
                    break;
                case (DownloadType.ModpackUpdate):
                    {
                        //getting the changes from changelog
                        var separator = new string[] { Environment.NewLine };
                        var version = LB.Packs.ModpackList[downloadName].Version;
                        var yadiskUrl = (LB.ModpackUrl + downloadName + ".txt").GetResponse();
                        var fileLines = (yadiskUrl + "&path=/changelog.txt").GetYadiskDownUrl().GetResponse().Split(separator, StringSplitOptions.RemoveEmptyEntries);
                        var help1D = new string[2];
                        var newerVersion = false;
                        var changes = new Dictionary<string, string>();

                        //resolving changes
                        for (int i = 0; i < fileLines.Length; i++)
                        {
                            help1D = fileLines[i].Split(separator, StringSplitOptions.RemoveEmptyEntries);

                            if (fileLines[i].StartsWith("#"))
                                ;

                            if (fileLines[i].StartsWith("VERSION, "))
                                newerVersion = (help1D[1].IsHigherThan(version));

                            else if (fileLines[i].StartsWith("CHANGED, ") && newerVersion)
                            {
                                if (changes.ContainsKey(help1D[1]))
                                    changes[help1D[1]] = "CHANGED";

                                else
                                    changes.Add(help1D[1], "CHANGED");
                            }

                            else if (fileLines[i].StartsWith("ADDED, ") && newerVersion)
                            {
                                if (changes.ContainsKey(help1D[1]))
                                    changes[help1D[1]] = "ADDED";

                                else
                                    changes.Add(help1D[1], "ADDED");
                            }

                            else if (fileLines[i].StartsWith("REMOVED, ") && newerVersion)
                            {
                                if (changes.ContainsKey(help1D[1]))
                                    changes[help1D[1]] = "REMOVED";

                                else
                                    changes.Add(help1D[1], "REMOVED");
                            }
                        }

                        //making the bat
                        var delLines = "";
                        var downloadFiles = new List<FilesToDownload>();

                        foreach (var item in changes)
                        {
                            if (item.Value.Contains("REMOVED"))
                                delLines += ("del /f /q " + item.Key + Environment.NewLine);

                            else if (item.Value.Contains("CHANGED") || item.Value.Contains("ADDED"))
                                downloadFiles.Add(new FilesToDownload { OutputPath = item.Key, Uri = (yadiskUrl + "&path=" + item.Key).GetYadiskDownUrl() });
                        }

                        settings = new DownloaderSettings()
                        {
                            FilesToDownload = downloadFiles,
                            CompletedAction = UpdateModpack,
                            CompletedArgs = new DownloadCompletedActionArgs
                            {
                                ModpackName = downloadName,
                            }
                        };

                        delLines += "(goto) 2>nul & del \"%~f0\"" + Environment.
                          NewLine + "exit 0"; //these 2 lines will delete the bat script after executing

                        using (FileStream fs = File.Create(Application.StartupPath + "/update_script.bat"))
                        {
                            Byte[] lines = new UTF8Encoding(true).GetBytes(delLines);
                            fs.Write(lines, 0, lines.Length);
                        }
                    }
                    break;
                case (DownloadType.Other):
                    {
                        settings = new DownloaderSettings
                        {
                            FilesToDownload = new List<FilesToDownload>
                            {
                                new FilesToDownload
                                {
                                    Uri = uri,
                                    OutputPath = output,
                                    CompletedAction = completedAction
                                }
                            }
                        };
                    }
                    break;
            }

            DL.progressBar.Add(new ProgressBar
            {
                Size = new Size(776, 23),
                Location = new Point(12, 12 + DL.currentDownloads * 52),
            });
            DL.labels.Add(new Label
            {
                Size = new Size(776, 13),
                Location = new Point(12, 38 + DL.currentDownloads * 52),
                Text = "Waiting for another download to start downloading " + downloadName
            });
            DL.splitter.Add(new Panel
            {
                Size = new Size(800, 4),
                Location = new Point(1, 54 + DL.currentDownloads * 52),
                BorderStyle = BorderStyle.Fixed3D
            });

            if (LB.Downloader == null)
            {
                LB.Downloader = new DL();
                LB.Downloader.Show();
                LB.Downloader.BeginDownload(settings);
            }

            else if (DL.currentDownloads == 0)
                LB.Downloader.BeginDownload(settings);

            else
                DL.pendingDownloads.Add(settings);
            LB.Downloader.Controls.AddRange(new Control[] { DL.splitter.Last(), DL.labels.Last(), DL.progressBar.Last() });

            DL.currentDownloads++;
        }

        public static async Task ExtractNatives(string path, string versionID, string instanceIdentifier)
        {
            if (!Directory.Exists(LB.AppPath + "/resources/versions/" + versionID + "/" + instanceIdentifier))
                Directory.CreateDirectory(LB.AppPath + "/resources/versions/" + versionID + "/" + instanceIdentifier);

            using (ZipArchive nativesJar = ZipFile.OpenRead(path))
            {
                foreach (ZipArchiveEntry entry in nativesJar.Entries)
                    if (!entry.Name.Equals("MANIFEST.MF") && !entry.FullName.EndsWith("/"))
                    {
                        if (!File.Exists(LB.AppPath + "/resources/versions/" + versionID + "/" + instanceIdentifier + "/" + entry.Name))
                            entry.ExtractToFile(LB.AppPath + "/resources/versions/" + versionID + "/" + instanceIdentifier + "/" + entry.Name);
                    }
            }
        }

        public static async Task ExtractDeps(DownloadCompletedActionArgs args)
        {
            var index = new string[] { "CefLib", "CefSharpCore", "CefSharpForms", "WinApiPack", "NewtonSoftJson", "SharpCompress" };

            foreach (var entry in index)
            {
                DL.labels[0].Text = "Extracting " + entry + "...";
                Directory.CreateDirectory(LB.AppPath + "/Cache/temp_deps");
                ZipFile.ExtractToDirectory(LB.AppPath + "/Cache/temp/" + entry + ".zip", LB.AppPath + "/Cache/temp_deps");
                FS.MoveDirectory(LB.AppPath + "/Cache/temp_deps", LB.AppPath + "/Cache/temp_all", true);
            }

            DL.labels[0].Text = "Finishing...";
            FS.MoveDirectory(LB.AppPath + "/Cache/temp_all/CEF", LB.AppPath + "/Cache/temp_all", true);
            FS.MoveDirectory(LB.AppPath + "/Cache/temp_all/CefSharp/x64", LB.AppPath + "/Cache/temp_all", true);
            FS.MoveDirectory(LB.AppPath + "/Cache/temp_all/lib/net45", LB.AppPath + "/Cache/temp_all", true);
            FS.MoveDirectory(LB.AppPath + "/Cache/temp_all/lib", LB.AppPath + "/Cache/temp_all", true);
            DirectoryInfo root = new DirectoryInfo(LB.AppPath + "/Cache/temp_all");

            foreach (var subDirectory in root.EnumerateDirectories())
            {
                if (!(string.Join(", ", LB.FileIndex["Deps"]).Contains(subDirectory.Name)))
                    Directory.Delete(subDirectory.FullName, true);
            }

            foreach (var file in root.GetFiles())
            {
                if (!(string.Join(", ", LB.FileIndex["Deps"]).Contains(file.Name)))
                    file.Delete();
            }
            FS.MoveDirectory(LB.AppPath + "/Cache/temp_all", LB.AppPath, true);
            Directory.Delete(LB.AppPath + "/Cache/temp", true);
            Directory.Delete(LB.AppPath + "/CefSharp", true);
            Application.Restart();
        }

        public static async Task ModpackFresh(DownloadCompletedActionArgs args)
        {
            var path = LB.AppPath + "/Cache/temp-modpack-" + args.ModpackName + ".zip";
            DL.labels[0].Text = "Extracting " + args.ModpackName;

            if (!Directory.Exists(LB.AppPath + "/modpacks"))
                Directory.CreateDirectory(LB.AppPath + "/modpacks");

            ZipFile.ExtractToDirectory(path, LB.AppPath + "/modpacks");
            FS.RenameDirectory(LB.AppPath + "/modpacks/" + args.ExtractedModpackName, args.ModpackName);
            ((LB.ModpackUrl + args.ModpackName + ".txt").GetResponse() + "&path=/modpack.json").GetYadiskDownUrl().GetResponse().FromJsonString<ModPackOnlineJson>().AddToLocalJson();
        }

        public static async Task UpdateModpack(DownloadCompletedActionArgs args)
        {
            LB.Packs.ModpackList[args.ModpackName].Version = (LB.ModpackUrl + args.ModpackName + ".txt").GetResponse().GetYadiskDownUrl().GetResponse().FromJsonString<ModPackOnlineJson>().LatestVersion;
        }

        public static async Task ExtractForgeVersionAndResolveLibs(DownloadCompletedActionArgs args) //TODO - compatibility with all forge versions
        {
            using (ZipArchive forge = ZipFile.OpenRead(LB.AppPath + "/resources/libraries/net/minecraftforge/forge/" + args.ForgeVersion + "/forge-" + args.ForgeVersion + ".jar"))
            {
                foreach (ZipArchiveEntry entry in forge.Entries)
                    if (entry.FullName.Equals("version.json"))
                    {
                        if (!Directory.Exists(LB.AppPath + "/resources/versions/" + args.ForgeIdentifier))
                            Directory.CreateDirectory(LB.AppPath + "/resources/versions/" + args.ForgeIdentifier);

                        if (!File.Exists(LB.AppPath + "/resources/versions/" + args.ForgeIdentifier + "/" + args.ForgeIdentifier + ".json"))
                            entry.ExtractToFile(LB.AppPath + "/resources/versions/" + args.ForgeIdentifier + "/" + args.ForgeIdentifier + ".json");

                        break;
                    }
            }

            ForgeJson forgeJson;

            using (StreamReader stream = new StreamReader(LB.AppPath + "/resources/versions/" + args.ForgeIdentifier + "/" + args.ForgeIdentifier + ".json"))
            {
                forgeJson = stream.ReadToEnd().FromJsonString<ForgeJson>();
            }

            string pathToLib;
            string url;
            bool isForgeLib;
            var settings = new DownloaderSettings
            {
                FilesToDownload = new List<FilesToDownload> { }
            };

            foreach (var lib in forgeJson.Libraries)
            {
                isForgeLib = lib.Url != null;
                pathToLib = isForgeLib ? lib.Name.ToPath(false) + ".pack.xz" : lib.Name.ToPath(false);

                if (!File.Exists(LB.AppPath + "/" + lib.Name.ToPath()) && !lib.Name.Contains("minecraftforge"))
                {
                    url = isForgeLib ? lib.Url : "https://libraries.minecraft.net/";
                    settings.FilesToDownload.Add(new FilesToDownload
                    {
                        Uri = url + pathToLib,
                        OutputPath = "/resources/libraries/" + pathToLib,
                        CompletedAction = ExtractForgeXzLib,
                        CompletedArgs = new DownloadCompletedActionArgs
                        {
                            IsForgeLibBool = isForgeLib,
                            RelativePathToForgeLib = "/resources/libraries/" + pathToLib
                        }
                    });
                }
            }

            await LB.Downloader.DownloadFiles(settings);
            DL.PendingForge.Remove(args.ForgeVersion);
        }

        public static string GetJavaInstallationPath() //from stackoverflow
        {
            var javaKey = "SOFTWARE\\JavaSoft\\Java Runtime Environment";
            var arch = Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32;

            using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, arch).OpenSubKey(javaKey))
            {
                var currentVersion = baseKey.GetValue("CurrentVersion").ToString();

                using (var homeKey = baseKey.OpenSubKey(currentVersion))
                    return homeKey.GetValue("JavaHome").ToString();
            }
        }

        //extracts and unpacks the forge lib which uses the weird xz compression, needs checksums check as the all other libs
        public static async Task ExtractForgeXzLib(DownloadCompletedActionArgs args)
        {
            if (args.IsForgeLibBool)
            { //this code is mostly stolen from forge installer - all credits to the forge dev
                var path = LB.AppPath + args.RelativePathToForgeLib;
                Stream xzStream;
                byte[] decompressed = null;

                using (xzStream = new XZStream(File.OpenRead(path)))
                using (MemoryStream ms = new MemoryStream())
                {
                    xzStream.CopyTo(ms);
                    decompressed = ms.ToArray();
                    ms.Close();
                    xzStream.Close();
                }

                var end = Encoding.UTF8.GetString(decompressed);
                end = end.Substring(end.Length - 4, 4);

                if (!end.Equals("SIGN"))
                {
                    //we have failed
                    MessageBox.Show("Unpacking of " + args.RelativePathToForgeLib + "library failed!");
                    return;
                }

                var x = decompressed.Length; //I really don't understand the line below, but it's needed for successful extraction of 'forge' libs (it gets the length of checksums)
                var len = decompressed[(x - 8)] & 0xFF | (decompressed[(x - 7)] & 0xFF) << 8 | (decompressed[(x - 6)] & 0xFF) << 16 | (decompressed[(x - 5)] & 0xFF) << 24;
                var tempPackFile = path.Substring(0, path.Length - 3);
                var outputByte = decompressed.Take(decompressed.Length - len - 8);

                using (var output = File.Create(tempPackFile))
                using (MemoryStream ms = new MemoryStream(outputByte.ToArray()))
                {
                    ms.CopyTo(output);
                    output.Close();
                    ms.Close();
                }

                var unpacker = new Process();
                var unpackerArgs = new ProcessStartInfo
                {
                    FileName = GetJavaInstallationPath() + "\\bin\\unpack200.exe",
                    Arguments = "-r --remove-pack-file \"" + tempPackFile + "\" \"" + path.Substring(0, path.Length - 8) + "\"",
                    CreateNoWindow = true,
                    UseShellExecute = false
                };
                unpacker.StartInfo = unpackerArgs;
                unpacker.Start();
            }
        }

        public static async Task UpdateLauncher(DownloadCompletedActionArgs args)
        {
            var command = "@echo off" + Environment.
            NewLine + ":deleteloop" + Environment.
            NewLine + "tasklist /fi \"imagename eq RabbiTeamLauncher.exe\" |find \":\" > nul" + Environment.
            NewLine + "if errorlevel 1 taskkill /f /im \"RabbiTeamLauncher.exe\"&goto :deleteloop2" + Environment.
            NewLine + ":deleteloop2" + Environment.
            NewLine + "tasklist /fi \"imagename eq CefSharp.BrowserSubprocess.exe\" |find \":\" > nul" + Environment.
            NewLine + "if errorlevel 1 taskkill /f /im \"CefSharp.BrowserSubprocess.exe\"&goto :rest" + Environment.
            NewLine + ":rest" + Environment.
            NewLine + "del /f /q RabbiTeamLauncher.exe" + Environment.
            NewLine + "ren RabbiTeamLauncherNew.exe RabbiTeamLauncher.exe" + Environment.
            NewLine + "echo ################Launcher Updated!####################" + Environment.
            NewLine + "start RabbiTeamLauncher.exe" + Environment.
            NewLine + "(goto) 2>nul & del \"%~f0\"" + Environment.
            NewLine + "exit 0";

            using (FileStream fs = File.Create(Application.StartupPath + "/update_launcher.bat"))
            {
                Byte[] lines = new UTF8Encoding(true).GetBytes(command);
                fs.Write(lines, 0, lines.Length);
            }
            var dr = MessageBox.Show("An update for Launcher has been downloaded and is ready to replace this one, do you want to proceed now? Check if you don't have any downloads pending!" + Environment.NewLine +
                Environment.NewLine + "If you click on No, Launcher will update after closing.", "Updater", MessageBoxButtons.YesNo);

            if (dr == DialogResult.Yes)
                Process.Start(LB.AppPath + "/update_launcher.bat");
        }

        public static bool IsHigherThan(this string higher, string lower)
        {
            var lowerArr = lower.Split('.');
            var higherArr = higher.Split('.');

            for (int i = 0; i < Math.Max(lowerArr.Length, higherArr.Length); i++)
                try
                {
                    if (Convert.ToInt32(lowerArr[i]) < Convert.ToInt32(higherArr[i]))
                        return true;
                }

                catch
                {
                    return false;
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

        public static T FromJsonString<T>(this string source) => JsonConvert.DeserializeObject<T>(source, Converter.Settings);

        public static string ToJsonString<T>(this T source) => JsonConvert.SerializeObject(source, Converter.Settings);

        //returns path to directory from file path
        public static string DirectoryOf(this string filePath) => filePath.Substring(0, filePath.LastIndexOf('/'));

        //backups a file or a directory
        public static void Backup(this string path)
        {
            if (File.Exists(path) && !Directory.Exists(path))
                File.Copy(path, path + "-backup-" + DateTime.Now.ToString() + Path.GetExtension(path));

            else if (!File.Exists(path))
                Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory(path, path + "-backup-" + DateTime.Now.ToString());

        }

        public static string GetLine(this string fileName, int line)
        {
            using (var sr = new StreamReader(fileName))
            {
                for (var i = 1; i < line; i++)
                    sr.ReadLine();

                return sr.ReadLine();
            }

        }

        public static string GetResponse(this string rawURL)
        {
            HttpWebResponse response = null;
            var request = (HttpWebRequest)WebRequest.Create(rawURL);

            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }

            catch
            {
                return null;
            }

            return new StreamReader(response.GetResponseStream()).ReadToEnd();
        }

        //request is the public key optionally with path part
        public static string GetYadiskDownUrl(this string request)
        {
            return ("https://cloud-api.yandex.net/v1/disk/public/resources/download?public_key=" + request.RemoveNewLines()).GetResponse().FromJsonString<YandexDiskDownJson>().DownloadUrl;
        }

        public static string GetYadiskDirName(this string yadiskUrl)
        {
            return ("https://cloud-api.yandex.net/v1/disk/public/resources/?public_key=" + yadiskUrl.RemoveNewLines() + "&limit=0").GetResponse().FromJsonString<YandexDiskJson>().Name;
        }

        public static string RemoveNewLines(this string source)
        {
            var newLineIdentifiers = new string[] { Environment.NewLine, "\r\n", "\r", "\n" };
            return newLineIdentifiers.Aggregate(source, (current, toReplace) => current.Replace(toReplace, ""));

        }

        public static UpdateInfo ToUpdateInfo(this string source)
        {
            //var identifiers = new string[] { "VERSION = ", "CEF_LIB_VERSION = ", "CEF_SHARP_VERSION = ", "WIN_API_CODEPACK_VERSION = ", "NEWTONSOFT_JSON_VERSION = ", "CHROME_URL = ", "SPECIAL =" };
            //var updateInfo = new UpdateInfo();
            //var lines = source.Split(new string[] { Environment.NewLine, "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            //foreach (var line in lines)
            //{
            //    if (identifiers.Any(line.StartsWith))
            //    {
            //        updateInfo.Version = identifiers.Aggregate(line, (current, replacement) => current.Replace(replacement, ""));
            //    }
            //} //draft of better code, I don't have time to simplify it
            var updateInfo = new UpdateInfo();
            var lines = source.Split(new string[] { Environment.NewLine, "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                if (line.StartsWith("VERSION = "))
                    updateInfo.Version = line.Replace("VERSION = ", "");

                else if (line.StartsWith("CEF_LIB_VERSION = "))
                    updateInfo.CefLibVersion = line.Replace("CEF_LIB_VERSION = ", "");

                else if (line.StartsWith("CEF_SHARP_VERSION = "))
                    updateInfo.CefSharpVersion = line.Replace("CEF_SHARP_VERSION = ", "");

                else if (line.StartsWith("WIN_API_CODEPACK_VERSION = "))
                    updateInfo.WinAPICodePackVersion = line.Replace("WIN_API_CODEPACK_VERSION = ", "");

                else if (line.StartsWith("NEWTONSOFT_JSON_VERSION = "))
                    updateInfo.NewtonSoftJsonVersion = line.Replace("NEWTONSOFT_JSON_VERSION = ", "");

                else if (line.StartsWith("SHARP_COMPRESS_VERSION = "))
                    updateInfo.SharpCompressVersion = line.Replace("SHARP_COMPRESS_VERSION = ", "");

                else if (line.StartsWith("NUGET_ROOT = "))
                    updateInfo.NuGetRoot = line.Replace("NUGET_ROOT = ", "");

                else if (line.StartsWith("CHROME_URL = "))
                    updateInfo.ChromeUrl = line.Replace("CHROME_URL = ", "");

                else if (line.StartsWith("MODPACK_URL = "))
                    updateInfo.ModpackListRoot = line.Replace("MODPACK_URL = ", "");

                else if (line.StartsWith("UPDATE_URL = "))
                    updateInfo.UpdateUrl = line.Replace("UPDATE_URL = ", "");
            }

            return updateInfo;
        }

        public static void AddToLocalJson(this ModPackOnlineJson onlineJson)
        {
            if (LB.Packs.ModpackList.ContainsKey(onlineJson.Name))
            {
                MessageBox.Show("Something went wrong and modpack " + onlineJson.Name + " couldn't finish it's installation, most probably you alreadt have this modpack installed");
                return;
            }

            if (LB.Packs.ModpackJsonVersion < onlineJson.ModpackJsonVersion)
                LB.Packs.UpdatePrompt();

            else if (LB.Packs.ModpackJsonVersion > onlineJson.ModpackJsonVersion)
                onlineJson.UpdatePrompt();

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
                NumberOfRuns = 0,
                AutoUpdate = false
            };
            LB.Packs.ModpackList.Add(onlineJson.Name, newEntry);
            File.WriteAllText(LB.AppPath + "/modpacks.json", LB.Packs.ToJsonString());
            UpdatePackList();

        }

        public static bool IsWinTen()
        {
            return Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName", "").ToString().StartsWith("Windows 10", StringComparison.OrdinalIgnoreCase);
        }

        public static string ReadFile(this string path)
        {
            using (var sr = new StreamReader(path))
            {
                var @return = sr.ReadToEnd();
                sr.Close();
                return @return;
            }
        }

        public static McVersionJson ToMcJson(this string version)
        {
            return (LB.AppPath + "/resources/versions/" + version + "/" + version + ".json").ReadFile().FromJsonString<McVersionJson>();
        }

        public static void UpdatePackList()
        {
            foreach (var modpackEntry in LB.Packs.ModpackList)
                LB.PackList.Items.Add(modpackEntry.Key);

            if (LB.PackList.Items.Count == 1)
            {
                LB.Settings.LatestModpack = ((string)LB.PackList.Items[0]);
                LB.PackList.SelectedItem = LB.PackList.Items[0];
            }
        }

        public static string DefaultArgs()
        {
            return "-Dfml.ignoreInvalidMinecraftCertificates=true -Dfml.ignorePatchDiscrepancies=true -XX:HeapDumpPath=MojangTricksIntelDriversForPerformance_javaw.exe_minecraft.exe.heapdump -XX:+UseConcMarkSweepGC -XX:+CMSIncrementalMode -XX:-UseAdaptiveSizePolicy -Xmn512M";
        }

    }
}
