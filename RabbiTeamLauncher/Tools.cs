using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Security.Cryptography;
using Microsoft.VisualBasic;

namespace RabbiTeamLauncher
{
    public partial class Tools : Form
    {

        public Tools()
        {
            InitializeComponent();
            textBox1.Text = "F:\\Moje veci\\Projects\\GitHub\\RabbiTeamLauncher\\RabbiTeamLauncher\\bin";
            textBox3.Text = "F:\\Moje veci\\Projects\\GitHub\\RabbiTeamLauncher\\RabbiTeamLauncher\\bin";
        }

        bool firstDiff = true;
        string oldindex;
        string newindex;
        public static string indexPath = LauncherBody.AppPath + "\\changelog.txt";
        string[] stringSeparator1 = new string[] { Environment.NewLine };
        string[] stringSeparator2 = new string[] { ", " };

        public string hashCheck(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLower();
                }
            }
        }

        public void append(string action, string oldpath)
        {

            if (firstDiff) //we want version number before all the differencies
            {

                using (StreamWriter sw = File.AppendText(indexPath))
                {
                    sw.WriteLine("VERSION, " + Strings.Right(textBox3.Text, 3));
                }

                textBox2.AppendText("VERSION, " + Strings.Right(textBox3.Text, 3) + Environment.NewLine);
                firstDiff = false;
            }

            using (StreamWriter sw = File.AppendText(indexPath))
            {
                sw.WriteLine(action + ", " + oldpath);
            }

            textBox2.AppendText(action + ", " + oldpath + Environment.NewLine);
        }

        //pick the folder of old version of a modpack
        private void button1_Click(object sender, EventArgs e)
        {

            CommonOpenFileDialog oldVersion = new CommonOpenFileDialog
            {
                InitialDirectory = "F:\\Moje veci\\Projects\\GitHub\\RabbiTeamLauncher\\RabbiTeamLauncher\\bin",
                IsFolderPicker = true,
            };

            if (oldVersion.ShowDialog() == CommonFileDialogResult.Ok)
                textBox1.Text = oldVersion.FileName;

        }
        //making the index of files from old version
        private void button7_Click(object sender, EventArgs e)
        {
            string path = textBox1.Text; //path to old version
            string[] file;
            string[] folders = Directory.GetDirectories(path, "*", SearchOption.AllDirectories); //names of all folders including all subfolders of all subfolders (lol) from the root

            for (int i = 0; i < folders.Length; i++)
            {
                    file = Directory.GetFiles(folders[i]); //get all files in specific folder (folder names in string[] folders are paths not just names)

                    for (int j = 0; j < file.Length; j++)
                    {
                        file[j] = file[j] + ", " + hashCheck(file[j]);
                        //path from launcher/name.ext hash of file to check its uniqueness

                        try
                        {
                            file[j] = file[j].Replace(path + "\\", ""); //converting paths to desired and better-handleable format
                        }

                        catch
                        {

                        } //because PCs are not flawless

                    }

                    if (!(file == null || file.Length == 0))
                        oldindex += Environment.NewLine + string.Join(Environment.NewLine, file);

            }

        }
        //pick the folder of new version of a modpack
        private void button2_Click(object sender, EventArgs e)
        {
            CommonOpenFileDialog newVersion = new CommonOpenFileDialog
            {
                InitialDirectory = "F:\\Moje veci\\Projects\\GitHub\\RabbiTeamLauncher\\RabbiTeamLauncher\\bin",
                IsFolderPicker = true,
            };

            if (newVersion.ShowDialog() == CommonFileDialogResult.Ok)
                textBox3.Text = newVersion.FileName;

        }
        //getting the index of files from new version
        private void button3_Click(object sender, EventArgs e)
        {
            string path = textBox3.Text;
            string[] file;
            string[] folders = Directory.GetDirectories(path, "*", SearchOption.AllDirectories);

            for (int i = 0; i < folders.Length; i++)
            {
                file = Directory.GetFiles(folders[i]);

                for (int j = 0; j < file.Length; j++)
                {
                    file[j] = file[j] + ", " + hashCheck(file[j]);
                    //path from launcher/name.ext hash (to check "version" of file)

                    try
                    {
                        file[j] = file[j].Replace(path + "\\", "");
                    }

                    catch
                    {

                    }

                }

                if (!(file == null || file.Length == 0))
                    newindex += Environment.NewLine + string.Join(Environment.NewLine, file);

            }

        }

        //GetDiffs Button - getting diffs between new and old version
        private void button4_Click(object sender, EventArgs e)
        {
            string[] old = oldindex.Split(stringSeparator1, StringSplitOptions.RemoveEmptyEntries); //making an array from the index
            string[] new1 = newindex.Split(stringSeparator1, StringSplitOptions.RemoveEmptyEntries);
            string[][] oldarray = new string[old.Length][]; //idk what is the meaning of these things, but it's working
            string[][] newarray = new string[new1.Length][];
            int maxIndex = Math.Max(old.Length, new1.Length);

            for (int i = 0; i < maxIndex; i++)
            {
                if (i < old.Length)
                    oldarray[i] = old[i].Split(stringSeparator2, StringSplitOptions.RemoveEmptyEntries);

                if (i< new1.Length)
                    newarray[i] = new1[i].Split(stringSeparator2, StringSplitOptions.RemoveEmptyEntries);

            }

            string[] i1 = new string[newarray.Length]; //jeez I really don't remember what is this code for
            string[] i0 = new string[newarray.Length];

            for (int s = 0; s < newarray.Length; s++)
            {
                i0[s] = newarray[s][0];
                i1[s] = newarray[s][1];
            }

            string newarray1D = "";
            string oldarray1D = "";

            for (int i = 0; i < maxIndex; i++)
            {
                if (i < oldarray.Length)
                    oldarray1D += string.Join("|", oldarray[i]);

                if (i < newarray.Length)
                    newarray1D += string.Join("|", newarray[i]);

            }

            for (int i = 0; i < maxIndex; i++)
            {
                if (i < oldarray.Length)
                    if (newarray1D.Contains(oldarray[i][0]))
                        if (newarray[Array.IndexOf(i0, oldarray[i][0])][1].Equals(oldarray[i][1]))
                            ;

                        else
                            markAsChanged(oldarray[i][0]);

                    else if (newarray1D.Contains(oldarray[i][1]))
                        markAsRenamed(oldarray[i][0], newarray[Array.IndexOf(i1, oldarray[i][1])][0]);

                    else
                        markAsRemoved(oldarray[i][0]);

                if (i < newarray.Length)
                    if (!oldarray1D.Contains(newarray[i][1]) && !oldarray1D.Contains(newarray[i][0]))
                        markAsAdded(newarray[i][0]);

            }

        }

        private void markAsRemoved(string oldpath)
        {
            append("REMOVED", oldpath);
        }

        private void markAsRenamed(string oldPath, string newPath)
        {
            var renamedFile = MessageBox.Show("Renamed file found!" + Environment.NewLine + "OLD PATH: " + oldPath + Environment.NewLine + "NEW PATH: " + newPath + Environment.NewLine + "Do you want to mark this file as REMOVED/ADDED?", "Renamed File Found Exception", MessageBoxButtons.YesNo);

            if (renamedFile == DialogResult.Yes)
            {
                markAsRemoved(oldPath);
                markAsAdded(newPath);
            }

        }

        private void markAsAdded(string path)
        {
            append("ADDED", path);
        }

        private void markAsChanged(string path)
        {
            append("CHANGED", path);
        }

        private void button5_Click(object sender, EventArgs e) { }
    }
}
