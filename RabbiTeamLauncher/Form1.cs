using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Net;
using System.IO;
using Microsoft.VisualBasic.Devices;

namespace RabbiTeamLauncher
{
    public partial class testLauncher : Form
    {
        public static string updateURL = "https://raw.githubusercontent.com/Timardo/RabbiTeamLauncher/Default/UpdateURL.txt";
        public static string appPath = Application.StartupPath;
        public static string settings = appPath + "//settings.txt";
        public testLauncher()
        {
            InitializeComponent();
            if (File.Exists(settings))
            {
                nick.Text = getLine(settings, 1);
                memoryAllocation.Text = getLine(settings, 2);
                if ((getLine(settings, 3)) == "true")
                    showConsole.CheckState = CheckState.Checked;
                else
                    showConsole.CheckState = CheckState.Unchecked;
                if ((getLine(settings, 4)) == "true")
                    closeAfterStart.CheckState = CheckState.Checked;
                else
                    closeAfterStart.CheckState = CheckState.Unchecked;
            }
            if (!File.Exists(settings))
            {
                string fileText = "" + Environment.NewLine + "3000" + Environment.NewLine + "true" + Environment.NewLine + "true" + Environment.NewLine + "none" + Environment.NewLine + "none";
                //                Nick                       RAM                            Show Console?                  Close after start?             Old link                       New link
                File.WriteAllText(settings, fileText);
            }
            string compareOld = getLine(settings, 5);
            try
            {
                lineChanger(getURL(updateURL), settings, 6);
            }
            catch
            {
                MessageBox.Show("Cannot check version..");
                return;
            }
            string compareNew = getLine(settings, 6);
            if (compareOld != compareNew)
            {
                Console.WriteLine(compareOld);
                Console.WriteLine(compareNew);
                DialogResult dr = MessageBox.Show("An update for RabbiTeam Client Modpack found, do you want to download and install it?", "Client Updater", MessageBoxButtons.YesNo);
                if (dr == DialogResult.Yes)
                {
                    new updater().Show();
                }
            }
        }
        public static string getURL(string rawURL)
        {
            HttpWebResponse response = null;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(rawURL);
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch
            {
                return "noResponse";
            }
            return new StreamReader(response.GetResponseStream()).ReadToEnd();
        }
        public static string getLine(string fileName, int line)
        {
            using (var sr = new StreamReader(fileName))
            {
                for (int i = 1; i < line; i++)
                    sr.ReadLine();
                return sr.ReadLine();
            }
        }
        public static void lineChanger(string newText, string fileName, int line_to_edit)
        {
            string[] arrLine = File.ReadAllLines(fileName);
            arrLine[line_to_edit - 1] = newText;
            File.WriteAllLines(fileName, arrLine);
        }
        private void testLauncher_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                playButt_Click(sender, e);
            }
        }
        public void playButt_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(nick.Text) || nick.Text.Contains(" ") || nick.Text.Contains("\\") || nick.Text.Contains("\""))
            {
                MessageBox.Show("Invalid nick format! Forbidden characters are spaces, backslashes and quotation marks.");
                return;
            }
            bool hasUuid = true;
            HttpWebResponse response = null;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://www.fabianwennink.nl/projects/OfflineUUID/api/" + nick.Text);
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch
            {
                hasUuid = false;
            }
            string uuid = "YouDontHaveVaildUUIDPleaseDontPlayUntilYouWillHaveOne";
            if (hasUuid == true)
            {
                uuid = new StreamReader(response.GetResponseStream()).ReadToEnd();
                uuid = uuid.Replace("{\"username\":\"" + nick.Text + "\",\"uuid\":\"", "").Replace("\",\"status\":true}", "").Replace("-", "");
            }
            Process proc = new Process();
            ProcessStartInfo info = new ProcessStartInfo();
            string dir = appPath + "\\";
            info.FileName = "javaw";
            if (showConsole.Checked == true)
            {
                info.FileName = "java";
            }
            info.Arguments = "-Dfml.ignoreInvalidMinecraftCertificates=true -Dfml.ignorePatchDiscrepancies=true -XX:HeapDumpPath=MojangTricksIntelDriversForPerformance_javaw.exe_minecraft.exe.heapdump -Xmx" + memoryAllocation.Text + "M -XX:+UseConcMarkSweepGC -XX:+CMSIncrementalMode -XX:-UseAdaptiveSizePolicy -Xmn128M -Djava.library.path=\"" + dir + "\\jars\\Natives\" -cp \"" + dir + "\\jars\\forge.jar;" + dir + "\\jars\\launchwrapper.jar;" + dir + "\\jars\\asm.jar;" + dir + "\\jars\\akka.jar;" + dir + "\\jars\\config.jar;" + dir + "\\jars\\scala-actors.jar;" + dir + "\\jars\\scala-compiler.jar;" + dir + "\\jars\\scala-continuations.jar;" + dir + "\\jars\\scala-library.jar;" + dir + "\\jars\\scala-parser.jar;" + dir + "\\jars\\scala-reflect.jar;" + dir + "\\jars\\scala-swing.jar;" + dir + "\\jars\\scala-xml.jar;" + dir + "\\jars\\lzma.jar;" + dir + "\\jars\\jopt.jar;" + dir + "\\jars\\guava.jar;" + dir + "\\jars\\commons.jar;" + dir + "\\jars\\realms.jar;" + dir + "\\jars\\commons-compress.jar;" + dir + "\\jars\\httpclient.jar;" + dir + "\\jars\\commons-logging.jar;" + dir + "\\jars\\httpcore.jar;" + dir + "\\jars\\vecmath.jar;" + dir + "\\jars\\trove4j.jar;" + dir + "\\jars\\core-mojang.jar;" + dir + "\\jars\\codecjorbis.jar;" + dir + "\\jars\\codecwav.jar;" + dir + "\\jars\\javasound.jar;" + dir + "\\jars\\lwjglopenal.jar;" + dir + "\\jars\\soundsystem.jar;" + dir + "\\jars\\netty.jar;" + dir + "\\jars\\commons-lang3.jar;" + dir + "\\jars\\commons-io.jar;" + dir + "\\jars\\commons-codec.jar;" + dir + "\\jars\\jinput.jar;" + dir + "\\jars\\jutils.jar;" + dir + "\\jars\\gson.jar;" + dir + "\\jars\\authlib.jar;" + dir + "\\jars\\log4j-api.jar;" + dir + "\\jars\\log4j-core.jar;" + dir + "\\jars\\lwjgl.jar;" + dir + "\\jars\\lwjgl-util.jar;" + dir + "\\jars\\twitch.jar;" + dir + "\\jars\\1.7.10.jar\" net.minecraft.launchwrapper.Launch --username " + nick.Text + " --version RabbiTeam --gameDir \"" + appPath + "\" --assetsDir \"" + dir + "\\assets\" --assetIndex 1.7.10 --uuid " + uuid + " --accessToken 0 --userProperties {} --userType legacy --tweakClass cpw.mods.fml.common.launcher.FMLTweaker";
            if ((info.Arguments.Length) > 8192)
            {
                MessageBox.Show("Path of installation is too long! Choose another, shorther installation path!");
                return;
            }
            ulong allocatedMemory = 3000;
            try
            {
                allocatedMemory = Convert.ToUInt64(memoryAllocation.Text);
            }
            catch
            {
                MessageBox.Show("Invalid memory allocation format! Type only numbers!");
                memoryAllocation.Text = "3000";
                return;
            }
            ulong memory = new ComputerInfo().TotalPhysicalMemory;
            ulong memorymb = memory/1024/1024-2000;
            memory = memorymb+2000;
            if (memory<allocatedMemory)
            {
                MessageBox.Show("Not enough memory to allocate! You selected: " + allocatedMemory + "MB, and your maximum memory is " + memory + "MB.");
                return;
            }
            if (hasUuid == false)
            {
                MessageBox.Show("Not valid UUID, don't play multiplayer until you have one!");
            }
            proc.StartInfo = info;
            proc.Start();
            if (closeAfterStart.CheckState == CheckState.Checked)
                Application.Exit();
        }
        private void nick_TextChanged(object sender, EventArgs e)
        {
            lineChanger(nick.Text, settings, 1);
        }
        private void memoryAllocation_TextChanged(object sender, EventArgs e)
        {
            lineChanger(memoryAllocation.Text, settings, 2);
        }
        private void showConsole_CheckedChanged(object sender, EventArgs e)
        {
            if (showConsole.CheckState == CheckState.Checked)
                lineChanger("true", settings, 3);
            else
                lineChanger("false", settings, 3);
        }
        private void closeAfterStart_CheckedChanged(object sender, EventArgs e)
        {
            if (closeAfterStart.CheckState == CheckState.Checked)
                lineChanger("true", settings, 4);
            else
                lineChanger("false", settings, 4);
        }
    }
}