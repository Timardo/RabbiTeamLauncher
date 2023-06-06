using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;
using static RabbiTeamLauncher.StaticElements;
using static RabbiTeamLauncher.Constants;

namespace RabbiTeamLauncher
{
    public partial class AdvancedSettings : Form
    {
        private static readonly string MigrateScriptPath = $"{AppPath}/migrate_script.bat";
        private readonly LauncherSettings _settings;
        public AdvancedSettings(LauncherSettings settings)
        {
            _settings = settings;
            InitializeComponent();
            SetValuesFromSettings();
        }

        private void SetValuesFromSettings()
        {
            ShowConsoleCheck.CheckState = _settings.ShowConsole ? CheckState.Checked : CheckState.Unchecked;
            CloseAfterStart.CheckState = _settings.CloseAfterStart ? CheckState.Checked : CheckState.Unchecked;
            JavaArgs.Text = _settings.JVMArguments;
            LauncherPath.Text = AppPath;
        }
        // migrating is done with a bat script currently, TODO: change to code
        private void MigrateButtonClick(object sender, EventArgs e)
        {
            MessageBox.Show("This button will help you migrate your RabbiTeam Launcher installation to a different directory" + Environment.NewLine + "Be sure to close all programs which can use this installation! (Except launcher)");

            CommonOpenFileDialog newFolder = new CommonOpenFileDialog
            {
                InitialDirectory = AppPath,
                IsFolderPicker = true,
            };

            if (newFolder.ShowDialog() == CommonFileDialogResult.Ok)
            {
                if (MessageBox.Show("Your Launcher installation (all files and folders) will be migrated to this folder: " + Environment.NewLine + Environment.NewLine + newFolder.FileName, "Client Migration", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    string filesString = "";

                    foreach (string file in FileIndex["Deps"].Concat(FileIndex["MCStuff"].Concat(FileIndex["LauncherStuff"])))
                    {
                        filesString += "move /-Y " + file + " \"" + newFolder.FileName + "\"" + Environment.NewLine;
                    }

                    string command = "@echo off" + Environment.NewLine +
                        "chcp 65001" + Environment.NewLine + //needs testing on Win7
                        ":deleteloop" + Environment.NewLine +
                        "tasklist /fi \"imagename eq RabbiTeamLauncher.exe\" |find \":\" > nul" + Environment.NewLine + // assumes the Launcher has not been renamed - not good
                        "if errorlevel 1 taskkill /f /im \"RabbiTeamLauncher.exe\"&goto :deleteloop2" + Environment.NewLine +
                        ":deleteloop2" + Environment.NewLine +
                        "tasklist /fi \"imagename eq CefSharp.BrowserSubprocess.exe\" |find \":\" > nul" + Environment.NewLine +
                        "if errorlevel 1 taskkill /f /im \"CefSharp.BrowserSubprocess.exe\"&goto :rest" + Environment.NewLine +
                        ":rest" + Environment.NewLine +
                        filesString + "echo ################Client Migrated!####################" + Environment.NewLine +
                        "start /D \"" + newFolder.FileName + "\" RabbiTeamLauncher.exe" + Environment.NewLine +
                        "(goto) 2>nul & del \"%~f0\"" + Environment.NewLine +
                        "exit 0";

                    File.WriteAllText(MigrateScriptPath, command);
                    Process.Start(MigrateScriptPath);
                }
            }
        }

        private void ResetArgsButtonClick(object sender, EventArgs e)
        {
            JavaArgs.Text = DefaultArgs;
        }

        private void JavaArgsTextChanged(object sender, EventArgs e)
        {
            _settings.JVMArguments = JavaArgs.Text;
        }

        private void DevButtonClick(object sender, EventArgs e)
        {
            if (MessageBox.Show("This button helps me to prepare new versions of modpack, it will be useful for others in the future" + Environment.NewLine + Environment.NewLine + "USE AT YOUR OWN RISK!", "", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                new Tools().ShowDialog();
            }
        }

        private void ShowConsoleCheckChanged(object sender, EventArgs e)
        {
            _settings.ShowConsole = ShowConsoleCheck.CheckState == CheckState.Checked;
        }

        private void CloseAfterStartCheckChanged(object sender, EventArgs e)
        {
            _settings.CloseAfterStart = CloseAfterStart.CheckState == CheckState.Checked;
        }

        private void OpenFolderButtClick(object sender, EventArgs e)
        {
            Process.Start(AppPath);
        }
    }
}
