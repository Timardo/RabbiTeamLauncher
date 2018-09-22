using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;
using LB = RabbiTeamLauncher.LauncherBody;

namespace RabbiTeamLauncher
{
    public partial class AdvancedSettings : Form
    {
        public AdvancedSettings()
        {
            InitializeComponent();

            if (LB.Settings.ShowConsole)
                ShowConsoleCheck.CheckState = CheckState.Checked;

            else
                ShowConsoleCheck.CheckState = CheckState.Unchecked;

            if (LB.Settings.CloseAfterStart)
                CloseAfterStart.CheckState = CheckState.Checked;

            else
                CloseAfterStart.CheckState = CheckState.Unchecked;

            JavaArgs.Text = LB.JVMArguments;
            LauncherPath.Text = LB.AppPath;
        }

        private void MigrateButtonClick(object sender, EventArgs e)
        {
            MessageBox.Show("This button will help you to migrate your RabbiTeam Launcher installation to other directory" + Environment.
                NewLine + "Be sure to close all programs which can use this installation! (Except launcher)");
            var newFolder = new CommonOpenFileDialog
            {
                InitialDirectory = LB.AppPath,
                IsFolderPicker = true,
            };

            if (newFolder.ShowDialog() == CommonFileDialogResult.Ok)
            {
                var dialog = MessageBox.Show("Your Launcher installation (all files and folders) will be migrated to this folder: " + Environment.NewLine + Environment.NewLine + newFolder.FileName,
                    "Client Migration",
                    MessageBoxButtons.OKCancel);

                if (dialog == DialogResult.OK)
                {
                    string filesString = null;

                    foreach (string file in LB.FileIndex["Deps"].Concat(LB.FileIndex["MCStuff"].Concat(LB.FileIndex["LauncherStuff"])))
                        filesString += "move /-Y "  + file + " \"" + newFolder.FileName + "\""  + Environment.NewLine;


                    var command = "@echo off" + Environment.
                        NewLine + "chcp 65001" + Environment. //needs testing on Win7
                        NewLine + ":deleteloop" + Environment.
                        NewLine + "tasklist /fi \"imagename eq RabbiTeamLauncher.exe\" |find \":\" > nul" + Environment.
                        NewLine + "if errorlevel 1 taskkill /f /im \"RabbiTeamLauncher.exe\"&goto :deleteloop2" + Environment.
                        NewLine + ":deleteloop2" + Environment.
                        NewLine + "tasklist /fi \"imagename eq CefSharp.BrowserSubprocess.exe\" |find \":\" > nul" + Environment.
                        NewLine + "if errorlevel 1 taskkill /f /im \"CefSharp.BrowserSubprocess.exe\"&goto :rest" + Environment.
                        NewLine + ":rest" + Environment.
                        NewLine + filesString + "echo ################Client Migrated!####################" + Environment.
                        NewLine + "start /D \"" + newFolder.FileName + "\" RabbiTeamLauncher.exe" + Environment.
                        NewLine + "(goto) 2>nul & del \"%~f0\"" + Environment.
                        NewLine + "exit 0";

                    using (FileStream fs = File.Create(Application.StartupPath + "/migrate_script.bat"))
                    {
                        Byte[] lines = new UTF8Encoding(true).GetBytes(command);
                        fs.Write(lines, 0, lines.Length);
                    }

                    System.Diagnostics.Process.Start(LB.AppPath + "/migrate_script.bat");
                }
            }
        }

        private void ResetArgsButtonClick(object sender, EventArgs e)
        {
            LB.Settings.JVMArguments = Utils.DefaultArgs();
            JavaArgs.Text = Utils.DefaultArgs();
            LB.JVMArguments = Utils.DefaultArgs();
        }

        private void JavaArgsTextChanged(object sender, EventArgs e)
        {
            LB.JVMArguments = JavaArgs.Text;
            LB.Settings.JVMArguments = JavaArgs.Text;
        }

        private void DevButtonClick(object sender, EventArgs e)
        {
            var dr = MessageBox.Show("This button helps me (the dev of this program) to prepare new versions of modpack, it will be useful for others in future ( ͡° ͜ʖ ͡°)" + Environment.NewLine + 
                Environment.NewLine + "USE AT YOUR OWN RISK! IT CAN DAMAGE YOUR COMPUTER IN UNPREDICTABLE WAYS! Kappa", "", MessageBoxButtons.OKCancel);

            if (dr == DialogResult.OK)
                new Tools().ShowDialog();
        }

        private void ShowConsoleCheckChanged(object sender, EventArgs e)
        {
            if (ShowConsoleCheck.CheckState == CheckState.Checked)
                LB.Settings.ShowConsole = true;
            else
                LB.Settings.ShowConsole = false;
        }

        private void CloseAfterStartCheckChanged(object sender, EventArgs e)
        {
            if (CloseAfterStart.CheckState == CheckState.Checked)
                LB.Settings.CloseAfterStart = true;
            else
                LB.Settings.CloseAfterStart = false;
        }
    }
}
