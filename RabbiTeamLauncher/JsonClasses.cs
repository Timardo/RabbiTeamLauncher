using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using J = Newtonsoft.Json.JsonPropertyAttribute;
using N = Newtonsoft.Json.NullValueHandling;

namespace RabbiTeamLauncher
{
    //all the information about downloaded modpacks, not fully implemented
    public class ModPackLocalJson
    {
        public int ModpackJsonVersion { get; set; }
        public Dictionary<string, ModpackEntry> ModpackList { get; set; }

        public void UpdatePrompt()
        {
            System.Windows.Forms.MessageBox.Show("It looks like your Launcher is outdated, this is not good." + Environment.NewLine + Environment.NewLine + "How did this even happen?");
        }
    }

    public class ModpackEntry
    {
        [J("version")] public string Version { get; set; } //version of the modpack in classic format (1.2.5, 1.12.1, 1.0, 1.3.6.452.18) use any amount of "." you want, different amount of "." can be used throughout versions
        [J("desc")] public string Description { get; set; }
        [J("id", NullValueHandling = N.Ignore)] public int Id { get; set; } //ID of modpack increases with amount of registered modpacks, only RT modpacks have this
        [J("number_of_runs")] public int NumberOfRuns { get; set; }
        [J("mc_version")] public string MinecraftVersion { get; set; }
        [J("forge_version", NullValueHandling = N.Ignore)] public string ForgeVersion { get; set; }
        [J("forge_full_id")] public string ForgeFullId { get; set; }
        [J("forge_additional_identifier")] public string ForgeAdditionalIdentifier { get; set; }
        [J("modpack_type")] public string ModpackType { get; set; } //Planned Feature - ATLauncher, FTB, Curse, insert-other-launcher-platform-here modpacks
        [J("custom_libs", NullValueHandling = N.Ignore)] public List<string> CustomLibs { get; set; } //Planned Feature - RTLauncher modpacks will have the ability to use custom libs - security check on download (overriding default ones)
        [J("additional_libs", NullValueHandling = N.Ignore)] public List<string> AdditionalLibs { get; set; } //Planned Feature - RTLauncher modpacks could also have additional libs - security check (adding new ones)
        [J("default_jvm_args", NullValueHandling = N.Ignore)] public string DefaultJavaArgs { get; set; } //Planned Feature - some modpacks need special JVM args, so let's give the modpack creators this ability
        [J("auto_update")] public bool AutoUpdate { get; set; }
        [J("mod_list", NullValueHandling = N.Ignore)] public ModList ModList { get; set; } //the modlist is not required, however, it is good to have one specified
    }

    public class ModPackOnlineJson
    {
        [J("modpack_json_version")] public int ModpackJsonVersion { get; set; }
        [J("name")] public string Name { get; set; }
        [J("latest_version")] public string LatestVersion { get; set; }
        [J("desc")] public string Description { get; set; }
        [J("id")] public int Id { get; set; } //ID of modpack increases with amount of registered modpacks, only RT modpacks have this
        [J("mc_version")] public string MinecraftVersion { get; set; }
        [J("forge_version")] public string ForgeVersion { get; set; }
        [J("forge_full_id")] public string ForgeFullId { get; set; } //full forge version described in the version.json file in the downloaded jar, only present in builds higher than 1.6.2-9.10.0.796
        [J("forge_additional_identifier")] public string ForgeAdditionalIdentifier { get; set; } //additional identifier to use while downloading the forge version (example: 1710ls, new ...)
        [J("custom_libs", NullValueHandling = N.Ignore)] public List<string> CustomLibs { get; set; }
        [J("additional_libs", NullValueHandling = N.Ignore)] public List<string> AdditionalLibs { get; set; }
        [J("default_jvm_args", NullValueHandling = N.Ignore)] public string DefaultJavaArgs { get; set; }
        [J("mod_list")] public ModList ModList { get; set; }

        public void UpdatePrompt()
        {
            throw new NotImplementedException(); //since there is only one version of Modpack JSON, this doesn't have any function, yet
        }
    }

    public class ModList
    {
        [J("mod_name")] public string ModName { get; set; } //full name of the mod, used by curse API - api.cfwidget.com/$path_from_root_of_curseforge to get info
        [J("mod_id")] public string ModId { get; set; } //mod id, also used to get info through curse API
        [J("mod_mc_version")] public string ModMcVersion { get; set; }
        [J("mod_size")] public long ModSize { get; set; } //mod size in bytes, used by curse API to differ between releases for same mc versions
        [J("mod_link", NullValueHandling = N.Ignore)] public string ModLink { get; set; } //direct link to mod download in case it is not on curse
        [J("mod_author")] public string ModAuthor { get; set; }
    }

    public class UpdateInfo
    {
        public string Version { get; set; }
        public string CefLibVersion { get; set; }
        public string CefSharpVersion { get; set; }
        public string WinAPICodePackVersion { get; set; }
        public string NewtonSoftJsonVersion { get; set; }
        public string ChromeUrl { get; set; }
        public string SharpCompressVersion { get; set; }
        public string Special { get; set; } //TODO
        public string NuGetRoot { get; set; }
        public string ModpackListRoot { get; set; }
        public string UpdateUrl { get; set; }
    }

    public class LauncherSettings
    {
        [J("nick")] public string Nick { get; set; }
        [J("ram")] public int Ram { get; set; }
        [J("show_console")] public bool ShowConsole { get; set; }
        [J("close_after_start")] public bool CloseAfterStart { get; set; }
        [J("jvm_arguments")] public string JVMArguments { get; set; }
        [J("latest_uuid")] public string LatestUuid { get; set; }
        [J("latest_modpack")] public string LatestModpack { get; set; }
    }

    public class YandexDiskDownJson
    {
        [J("href")] public string DownloadUrl { get; set; }
        [J("method")] public string Method { get; set; }
        [J("templated")] public bool Templated { get; set; }
    }


    #region Assets Index
    public class AssetIndex
    {
        [J("objects")] public Dictionary<string, Objects> Objects { get; set; }
    }

    public class Objects
    {
        [J("hash")] public string Hash { get; set; }
        [J("size")] public long Size { get; set; }
    }

    #endregion

    #region YandexDiskJson
    public class YandexDiskJson
    {
        [J("public_key")] public string PublicKey { get; set; }
        [J("_embedded")] public Embedded Embedded { get; set; }
        [J("name")] public string Name { get; set; }
        [J("created")] public DateTimeOffset Created { get; set; }
        [J("resource_id")] public string ResourceId { get; set; }
        [J("public_url")] public string PublicUrl { get; set; }
        [J("modified")] public DateTimeOffset Modified { get; set; }
        [J("views_count")] public long ViewsCount { get; set; }
        [J("owner")] public Owner Owner { get; set; }
        [J("path")] public string Path { get; set; }
        [J("comment_ids")] public CommentIds CommentIds { get; set; }
        [J("type")] public string Type { get; set; }
        [J("revision")] public long Revision { get; set; }
    }

    public class CommentIds
    {
        [J("private_resource")] public string PrivateResource { get; set; }
        [J("public_resource")] public string PublicResource { get; set; }
    }

    public class Embedded
    {
        [J("sort")] public string Sort { get; set; }
        [J("public_key")] public string PublicKey { get; set; }
        [J("items")] public List<object> Items { get; set; }
        [J("limit")] public long Limit { get; set; }
        [J("offset")] public long Offset { get; set; }
        [J("path")] public string Path { get; set; }
        [J("total")] public long Total { get; set; }
    }
    public class Owner
    {
        [J("login")] public string Login { get; set; }
        [J("display_name")] public string DisplayName { get; set; }
        [J("uid")] public string Uid { get; set; }
    }
    #endregion

    #region ForgeVersionJson
    public class ForgeJson
    {
        [J("id")] public string Id { get; set; }
        [J("time")] public string Time { get; set; }
        [J("releaseTime")] public string ReleaseTime { get; set; }
        [J("type")] public string Type { get; set; }
        [J("minecraftArguments")] public string MinecraftArguments { get; set; }
        [J("mainClass")] public string MainClass { get; set; }
        [J("minimumLauncherVersion", NullValueHandling = N.Ignore)] public long MinimumLauncherVersion { get; set; }
        [J("assets")] public string Assets { get; set; }
        [J("inheritsFrom")] public string InheritsFrom { get; set; }
        [J("jar")] public string Jar { get; set; }
        [J("libraries")] public List<ForgeLibrary> Libraries { get; set; }
    }

    public class ForgeLibrary
    {
        [J("name")] public string Name { get; set; }
        [J("url", NullValueHandling = N.Ignore)] public string Url { get; set; }
        [J("serverreq", NullValueHandling = N.Ignore)] public bool? Serverreq { get; set; }
        [J("checksums", NullValueHandling = N.Ignore)] public List<string> Checksums { get; set; }
        [J("clientreq", NullValueHandling = N.Ignore)] public bool? Clientreq { get; set; }
    }
    #endregion

    #region McVersionsManifest 
    public class McVersionsManifest
    {
        [J("latest")] public Latest Latest { get; set; }
        [J("versions")] public List<Versions> Versions { get; set; }
    }

    public class Latest
    {
        [J("snapshot")] public string Snapshot { get; set; }
        [J("release")] public string Release { get; set; }
    }

    public class Versions
    {
        [J("id")] public string Id { get; set; }
        [J("type")] public TypeEnum Type { get; set; }
        [J("time")] public DateTimeOffset Time { get; set; }
        [J("releaseTime")] public DateTimeOffset ReleaseTime { get; set; }
        [J("url")] public string Url { get; set; }
    }

    public enum TypeEnum { OldAlpha, OldBeta, Release, Snapshot };
    #endregion

    #region McVersionJson, mostly generated with quicktype's json2sharp app - https://app.quicktype.io/
    public class McVersionJson
    {
        [J("arguments", NullValueHandling = N.Ignore)] public Arguments Arguments { get; set; } //command line args to run mc versions > 1.12.2
        [J("assetIndex")] public DownloadInfo AssetIndex { get; set; } //all the info about used assets in this version
        [J("assets")] public string Assets { get; set; } //version of assets, mostly equals mc version
        [J("downloads")] public Downloads Downloads { get; set; } //downloads of client, server and windows server
        [J("id")] public string Id { get; set; } //id of the mc version, mostly equals mc version, older alpha/beta versions have special format
        [J("libraries")] public List<Library> Libraries { get; set; } //all the libraries info, urls, hashes, paths..
        [J("logging", NullValueHandling = N.Ignore)] public Logging Logging { get; set; } //format used to log stuff to console, not all versions have this
        [J("mainClass")] public string MainClass { get; set; } //main class of minecraft to be called on the command line
        [J("minecraftArguments", NullValueHandling = N.Ignore)] public string MinecraftArguments { get; set; } //command line args for mc versions prior 1.13
        [J("minimumLauncherVersion")] public long MinimumLauncherVersion { get; set; } //useless
        [J("releaseTime")] public DateTimeOffset ReleaseTime { get; set; } //release time of the mc version
        [J("time")] public DateTimeOffset Time { get; set; } //this time represents the date when this version was added to the manifest, maybe, idk, useless
        [J("type")] public TypeEnum ReleaseType { get; set; } //old alpha/beta release or snapshot
    }

    public class Arguments
    {
        [J("game")] public List<GameElement> Game { get; set; } //game arguments
        [J("jvm")] public List<JvmElement> Jvm { get; set; } //jvm arguments
    }

    public struct GameElement
    {
        public GameArgs ConditionalValue; //value which is conditional
        public string Value; //string value of the argument, can be a variable property (username, uuid...)

        public static implicit operator GameElement(GameArgs ConditionalValue) => new GameElement { ConditionalValue = ConditionalValue };
        public static implicit operator GameElement(string Value) => new GameElement { Value = Value };
    }

    public struct JvmElement
    {
        public JvmArgs ConditionalValue; //value which is conditional
        public string Value; //string value of the argument mostly containing a variable property (classpath, natives dir...)

        public static implicit operator JvmElement(JvmArgs ConditionalValue) => new JvmElement { ConditionalValue = ConditionalValue };
        public static implicit operator JvmElement(string Value) => new JvmElement { Value = Value };
    }

    public class GameArgs
    {
        [J("rules")] public List<Rules> Rules { get; set; } //list of rules, actually a single value, but whatever
        [J("value")] public Value Values { get; set; } //string values, sometimes an array of strings, sometimes a string and sometimes even an array consisting of one string, weird
    }

    public class JvmArgs
    {
        [J("rules")] public List<Rules> Rules { get; set; }
        [J("value")] public Value Values { get; set; }
    }

    public class Rules
    {
        [J("action")] public string Action { get; set; } //bool string which determines if the value/library should be passed or not depending on the result of condition, can be allow/disallow
        [J("features", NullValueHandling = N.Ignore)] public Conditions Conditions { get; set; } //conditions (mostly one) which must be met in order to pass the value/library
        [J("os", NullValueHandling = N.Ignore)] public Os Os { get; set; } //conditions (mostly one) which must be met in order to pass the value/library
    }

    public class Os
    {
        [J("name")] public string Name { get; set; } //name of the Os
    }

    public class Conditions
    {
        [J("is_demo_user", NullValueHandling = N.Ignore)] public bool? IsDemoUser { get; set; } //game args rule, useless atm
        [J("has_custom_resolution", NullValueHandling = N.Ignore)] public bool? HasCustomResolution { get; set; } //if the player has set custom resoulution in their Launcher settings
        [J("name", NullValueHandling = N.Ignore)] public OsEnum Name { get; set; } //os name, known names in enum
        [J("version", NullValueHandling = N.Ignore)] public string Version { get; set; } //os version, seen only with windows
        [J("arch", NullValueHandling = N.Ignore)] public string Arch { get; set; } //architecture of the os - x86/x64
    }

    public enum OsEnum { Osx, Windows }

    public struct Value
    {
        public string StringValue; //final string value which would be passed or not
        public List<string> ValuesList; //final string value list

        public static implicit operator Value(string StringValue) => new Value { StringValue = StringValue };
        public static implicit operator Value(List<string> ValuesList) => new Value { ValuesList = ValuesList };
    }
    public class Downloads
    {
        [J("client")] public DownloadInfo Client { get; set; } //client entry of the version
        [J("server", NullValueHandling = N.Ignore)] public DownloadInfo Server { get; set; } //server
        [J("windows_server", NullValueHandling = N.Ignore)] public DownloadInfo WindowsServer { get; set; } //win server (.exe)
    }

    public class DownloadInfo
    {
        [J("id", NullValueHandling = N.Ignore)] public string Id { get; set; } //id of the logging file/assets, mostly useless
        [J("sha1")] public string Sha1 { get; set; } //hash of the file below
        [J("size")] public long Size { get; set; } //size of file in bytes
        [J("path", NullValueHandling = N.Ignore)] public string Path { get; set; } //path to the library, mc downloads don't have this entry
        [J("url")] public string Url { get; set; } //url to file
    }

    public class Library
    {
        [J("name")] public string Name { get; set; } //name of the library in maven-like format groupid:artifact:version
        [J("downloads")] public LibraryDownloads Downloads { get; set; } //all the info needed to download and parse the lib
        [J("natives", NullValueHandling = N.Ignore)] public Natives Natives { get; set; } //if the library contains natives, this entry is present
        [J("extract", NullValueHandling = N.Ignore)] public Extract Extract { get; set; } //natives should have this property, contains excluded files/folders
        [J("rules", NullValueHandling = N.Ignore)] public List<Rules> Rules { get; set; } //rules which determines if the lib should be passed or not, disallow has priority
    }

    public class LibraryDownloads
    {
        [J("artifact", NullValueHandling = N.Ignore)] public DownloadInfo Artifact { get; set; } //actual lib entry, url, hash.. if null Classiefiers property is used 
        [J("classifiers", NullValueHandling = N.Ignore)] public Classifiers Classifiers { get; set; } //I have no idea why there is Natives and Extract property when there is this, contains natives and test libs
    }
    public class Classifiers //self-explaining class, also, why is there MacOs entry and Osx entry
    {
        [J("tests", NullValueHandling = N.Ignore)] public DownloadInfo Tests { get; set; }
        [J("natives-linux", NullValueHandling = N.Ignore)] public DownloadInfo NativesLinux { get; set; }
        [J("natives-macos", NullValueHandling = N.Ignore)] public DownloadInfo NativesMacos { get; set; }
        [J("natives-windows", NullValueHandling = N.Ignore)] public DownloadInfo NativesWindows { get; set; }
        [J("natives-windows-32", NullValueHandling = N.Ignore)] public DownloadInfo NativesWindows32 { get; set; }
        [J("natives-windows-64", NullValueHandling = N.Ignore)] public DownloadInfo NativesWindows64 { get; set; }
        [J("natives-osx", NullValueHandling = N.Ignore)] public DownloadInfo NativesOsx { get; set; }
    }

    public class Extract
    {
        [J("exclude")] public List<string> Exclude { get; set; } //list of files/folders to be excluded from natives extraction
    }

    public class Natives //self-explaining
    {
        [J("linux", NullValueHandling = N.Ignore)] public string Linux { get; set; }
        [J("osx", NullValueHandling = N.Ignore)] public string Osx { get; set; }
        [J("windows", NullValueHandling = N.Ignore)] public string Windows { get; set; }
    }

    public class Logging
    {
        [J("client")] public LoggingClient Client { get; set; } //client logging
    }

    public class LoggingClient
    {
        [J("file")] public DownloadInfo File { get; set; }
        [J("argument")] public string Argument { get; set; } //argument to be added to the command line
        [J("type")] public string Type { get; set; } //??
    }
    #endregion

    #region Converter compatibility stuff
    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = {
                GameElementConverter.Singleton,
                ValueConverter.Singleton,
                JvmElementConverter.Singleton,
                TypeEnumConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class GameElementConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(GameElement) || t == typeof(GameElement?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
                case JsonToken.String:
                case JsonToken.Date:
                    var stringValue = serializer.Deserialize<string>(reader);
                    return new GameElement { Value = stringValue };
                case JsonToken.StartObject:
                    var objectValue = serializer.Deserialize<GameArgs>(reader);
                    return new GameElement { ConditionalValue = objectValue };
            }
            throw new Exception("Cannot unmarshal type GameElement");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (GameElement)untypedValue;
            if (value.Value != null)
            {
                serializer.Serialize(writer, value.Value);
                return;
            }
            if (value.ConditionalValue != null)
            {
                serializer.Serialize(writer, value.ConditionalValue);
                return;
            }
            throw new Exception("Cannot marshal type GameElement");
        }

        public static readonly GameElementConverter Singleton = new GameElementConverter();
    }

    internal class ValueConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(Value) || t == typeof(Value?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
                case JsonToken.String:
                case JsonToken.Date:
                    var stringValue = serializer.Deserialize<string>(reader);
                    return new Value { StringValue = stringValue };
                case JsonToken.StartArray:
                    var arrayValue = serializer.Deserialize<List<string>>(reader);
                    return new Value { ValuesList = arrayValue };
            }
            throw new Exception("Cannot unmarshal type Value");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (Value)untypedValue;
            if (value.StringValue != null)
            {
                serializer.Serialize(writer, value.StringValue);
                return;
            }
            if (value.ValuesList != null)
            {
                serializer.Serialize(writer, value.ValuesList);
                return;
            }
            throw new Exception("Cannot marshal type Value");
        }

        public static readonly ValueConverter Singleton = new ValueConverter();
    }

    internal class JvmElementConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(JvmElement) || t == typeof(JvmElement?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
                case JsonToken.String:
                case JsonToken.Date:
                    var stringValue = serializer.Deserialize<string>(reader);
                    return new JvmElement { Value = stringValue };
                case JsonToken.StartObject:
                    var objectValue = serializer.Deserialize<JvmArgs>(reader);
                    return new JvmElement { ConditionalValue = objectValue };
            }
            throw new Exception("Cannot unmarshal type JvmElement");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (JvmElement)untypedValue;
            if (value.Value != null)
            {
                serializer.Serialize(writer, value.Value);
                return;
            }
            if (value.ConditionalValue != null)
            {
                serializer.Serialize(writer, value.ConditionalValue);
                return;
            }
            throw new Exception("Cannot marshal type JvmElement");
        }

        public static readonly JvmElementConverter Singleton = new JvmElementConverter();
    }

    internal class TypeEnumConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(TypeEnum) || t == typeof(TypeEnum?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "old_alpha":
                    return TypeEnum.OldAlpha;
                case "old_beta":
                    return TypeEnum.OldBeta;
                case "release":
                    return TypeEnum.Release;
                case "snapshot":
                    return TypeEnum.Snapshot;
            }
            throw new Exception("Cannot unmarshal type TypeEnum");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (TypeEnum)untypedValue;
            switch (value)
            {
                case TypeEnum.OldAlpha:
                    serializer.Serialize(writer, "old_alpha");
                    return;
                case TypeEnum.OldBeta:
                    serializer.Serialize(writer, "old_beta");
                    return;
                case TypeEnum.Release:
                    serializer.Serialize(writer, "release");
                    return;
                case TypeEnum.Snapshot:
                    serializer.Serialize(writer, "snapshot");
                    return;
            }
            throw new Exception("Cannot marshal type TypeEnum");
        }

        public static readonly TypeEnumConverter Singleton = new TypeEnumConverter();
    }
    #endregion

}
