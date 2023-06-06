using System;
using System.Collections.Generic;

namespace RabbiTeamLauncher
{
    public delegate void DownloadActionDelegate(DownloadActionArgs e);
    public class DownloadActionArgs
    {
        public static readonly DownloadActionArgs Empty = new DownloadActionArgs();
    }
    public class ForgeDownloadCompletedActionArgs : DownloadActionArgs
    {
        public ForgeDownloadCompletedActionArgs(string forgeVersion, string forgeIdentifier)
        {
            ForgeVersion = forgeVersion;
            ForgeIdentifier = forgeIdentifier;
        }

        public string ForgeVersion { get; }
        public string ForgeIdentifier { get; }
    }

    public class ModpackDownloadCompletedActionArgs : DownloadActionArgs
    {
        public ModpackDownloadCompletedActionArgs(string modpackName, string extractedModpackName = null)
        {
            ModpackName = modpackName;
            ExtractedModpackName = extractedModpackName;
        }

        public string ModpackName { get; }
        public string ExtractedModpackName { get; }
    }

    public class DownloadBeforeStartActionArgs
    {
        // TODO: currently not used
    }

    public enum DownloadType
    {
        Deps,
        ForgeVersion,
        MinecraftVersion,
        ModpackUpdate,
        ModpackFresh,
        Other
    }

    public class DownloadJob
    {
        public DownloadJob()
        {
            FilesToDownload = new List<FileToDownload>();
        }

        public DownloadJob(List<FileToDownload> filesToDownload, DownloadAction completedAction = null, DownloadAction beforeStartAction = null)
        {
            FilesToDownload = filesToDownload;
            CompletedAction = completedAction;
            BeforeStartAction = beforeStartAction;
        }

        public DownloadJob(FileToDownload file, DownloadAction completedAction = null, DownloadAction beforeStartAction = null)
        {
            FilesToDownload = new List<FileToDownload> { file };
            CompletedAction = completedAction;
            BeforeStartAction = beforeStartAction;
        }

        public List<FileToDownload> FilesToDownload { get; }
        public DownloadAction CompletedAction { get; }
        public DownloadAction BeforeStartAction { get; }
    }

    public class DownloadAction
    {
        public DownloadAction(DownloadActionDelegate action, DownloadActionArgs actionArgs)
        {
            Action = action;
            ActionArgs = actionArgs;
        }

        public DownloadActionDelegate Action { get; }
        public DownloadActionArgs ActionArgs { get; }
    }

    // TODO: after update change to record with other "storage/data-carrying" classes
    public class FileToDownload
    {
        public FileToDownload(
            string uri,
            string outputPath,
            DownloadAction completedAction = null,
            DownloadAction beforeStartAction = null)
        {
            Uri = uri;
            OutputPath = outputPath;
            CompletedAction = completedAction;
            BeforeStartAction = beforeStartAction;
        }

        public string Uri { get; set; }
        public string OutputPath { get; set; }
        public DownloadAction CompletedAction { get; set; }
        public DownloadAction BeforeStartAction { get; set; }
    }

    public class DownloadConstructorException : Exception
    {
        public DownloadConstructorException(Exception ex) : base("Download constructor failed", ex) { }
    }

    public class GetResponseException : Exception
    {
        public GetResponseException(Exception ex) : base("Could not get response", ex) { }
    }

    public class MissingLibException : Exception
    {
        public MissingLibException(string missingLibrary) : base($"There is a missing library in your installation \n{missingLibrary} \nYou will need to fix this before launching Minecraft again") { }
    }
}
