using System;
using System.Collections.Generic;

namespace RabbiTeamLauncher
{
    public class DownloadCompletedActionArgs
    {
        public string ForgeVersion { get; set; }
        public string ForgeIdentifier { get; set; }
        public string ModpackName { get; set; }
        public string RelativePathToForgeLib { get; set; }
        public string ExtractedModpackName { get; set; }
        public bool IsForgeLibBool { get; set; }
    }

    public class DownloadBeforeStartActionArgs
    {

    }

    public enum DownloadType { Deps, ForgeVersion, MinecraftVersion, ModpackUpdate, ModpackFresh, Other}

    public class DownloaderSettings
    {
        public List<FilesToDownload> FilesToDownload { get; set; }
        public Utils.DownloadCompletedAction CompletedAction { get; set; }
        public Utils.DownloadBeforeStartAction BeforeStartAction { get; set; }
        public DownloadCompletedActionArgs CompletedArgs { get; set; }
        public DownloadBeforeStartActionArgs BeforeStartArgs { get; set; }
    }
    public class FilesToDownload
    {
        public string Uri { get; set; }
        public string OutputPath { get; set; }
        public Utils.DownloadCompletedAction CompletedAction { get; set; }
        public Utils.DownloadBeforeStartAction BeforeStartAction { get; set; }
        public DownloadCompletedActionArgs CompletedArgs { get; set; }
        public DownloadBeforeStartActionArgs BeforeStartArgs { get; set; }
    }

    public class DownloadJob
    {
        public List<FileToDownload> FilesToDownload { get; set; }
        public string Title { get; set; }
    }

    public class FileToDownload
    {
        public string Uri { get; set; }
        public string Output { get; set; }
    }
}