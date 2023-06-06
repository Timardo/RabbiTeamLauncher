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
        private bool _firstDiff = true;
        private string _oldindex;
        private string _newindex;
        private static readonly string IndexPath = StaticElements.AppPath + "\\changelog.txt";
        private static readonly string[] StringSeparator1 = new string[] { Environment.NewLine };
        private static readonly string[] StringSeparator2 = new string[] { ", " };
        
        public Tools()
        {
            InitializeComponent();
            textBox1.Text = StaticElements.AppPath;
            textBox3.Text = StaticElements.AppPath;
        }

        public string HashCheck(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLower();
                }
            }
        }

        public void Append(string action, string oldpath)
        {
            if (_firstDiff) // version number before all the diffs
            {
                using (StreamWriter sw = File.AppendText(IndexPath))
                {
                    sw.WriteLine("VERSION, " + Strings.Right(textBox3.Text, 3));
                }

                textBox2.AppendText("VERSION, " + Strings.Right(textBox3.Text, 3) + Environment.NewLine);
                _firstDiff = false;
            }

            using (StreamWriter sw = File.AppendText(IndexPath))
            {
                sw.WriteLine(action + ", " + oldpath);
            }

            textBox2.AppendText(action + ", " + oldpath + Environment.NewLine);
        }

        //pick the folder of old version of a modpack
        private void OldVersionButtonClick(object sender, EventArgs e)
        {

            CommonOpenFileDialog oldVersion = new CommonOpenFileDialog
            {
                InitialDirectory = StaticElements.AppPath,
                IsFolderPicker = true,
            };

            if (oldVersion.ShowDialog() == CommonFileDialogResult.Ok)
            {
                textBox1.Text = oldVersion.FileName;
            }                

        }
        //making the index of files from old version
        private void OldVersionGetFilesButtonClick(object sender, EventArgs e)
        {
            string path = textBox1.Text; //path to old version
            string[] file;
            string[] folders = Directory.GetDirectories(path, "*", SearchOption.AllDirectories); //names of all folders including all subfolders of all subfolders (lol) from the root

            for (int i = 0; i < folders.Length; i++)
            {
                file = Directory.GetFiles(folders[i]); //get all files in specific folder (folder names in string[] folders are paths not just names)

                for (int j = 0; j < file.Length; j++)
                {
                    file[j] = file[j] + ", " + HashCheck(file[j]); //path from launcher/name.ext hash of file to check its uniqueness
                file[j] = file[j].Replace(path + "\\", ""); //converting paths to desired and better-handleable format
                }

                if (!(file == null || file.Length == 0))
                {
                    _oldindex += Environment.NewLine + string.Join(Environment.NewLine, file);
                }
            }
        }
        //pick the folder of new version of a modpack
        private void NewVersionButtonClick(object sender, EventArgs e)
        {
            CommonOpenFileDialog newVersion = new CommonOpenFileDialog
            {
                InitialDirectory = StaticElements.AppPath,
                IsFolderPicker = true,
            };

            if (newVersion.ShowDialog() == CommonFileDialogResult.Ok)
            {
                textBox3.Text = newVersion.FileName;
            }
        }
        //getting the index of files from new version
        private void NewVersionGetFilesButtonClick(object sender, EventArgs e)
        {
            string path = textBox3.Text;
            string[] file;
            string[] folders = Directory.GetDirectories(path, "*", SearchOption.AllDirectories);

            for (int i = 0; i < folders.Length; i++)
            {
                file = Directory.GetFiles(folders[i]);

                for (int j = 0; j < file.Length; j++)
                {
                    file[j] = file[j] + ", " + HashCheck(file[j]); //path from launcher/name.ext hash (to check "version" of file)
                    file[j] = file[j].Replace(path + "\\", "");
                }

                if (!(file == null || file.Length == 0))
                {
                    _newindex += Environment.NewLine + string.Join(Environment.NewLine, file);
                }
            }
        }

        //GetDiffs Button - getting diffs between new and old version
        private void GetDiffstButtonClick(object sender, EventArgs e)
        {
            string[] old = _oldindex.Split(StringSeparator1, StringSplitOptions.RemoveEmptyEntries); //making an array from the index
            string[] new1 = _newindex.Split(StringSeparator1, StringSplitOptions.RemoveEmptyEntries);
            string[][] oldarray = new string[old.Length][]; //idk what is the meaning of these things, but it's working
            string[][] newarray = new string[new1.Length][];
            int maxIndex = Math.Max(old.Length, new1.Length);

            for (int i = 0; i < maxIndex; i++)
            {
                if (i < old.Length)
                {
                    oldarray[i] = old[i].Split(StringSeparator2, StringSplitOptions.RemoveEmptyEntries);
                }

                if (i < new1.Length)
                {
                    newarray[i] = new1[i].Split(StringSeparator2, StringSplitOptions.RemoveEmptyEntries);
                }
            }

            string[] i1 = new string[newarray.Length]; // I really don't remember what this code is for
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
                {
                    oldarray1D += string.Join("|", oldarray[i]);
                }

                if (i < newarray.Length)
                {
                    newarray1D += string.Join("|", newarray[i]);
                }
            }

            for (int i = 0; i < maxIndex; i++)
            {
                if (i < oldarray.Length)
                {
                    if (newarray1D.Contains(oldarray[i][0]))
                    {
                        if (!newarray[Array.IndexOf(i0, oldarray[i][0])][1].Equals(oldarray[i][1]))
                        {
                            MarkAsChanged(oldarray[i][0]);
                        }
                    }

                    else if (newarray1D.Contains(oldarray[i][1]))
                    {
                        MarkAsRenamed(oldarray[i][0], newarray[Array.IndexOf(i1, oldarray[i][1])][0]);
                    }

                    else
                    {
                        MarkAsRemoved(oldarray[i][0]);
                    }
                }

                if (i < newarray.Length)
                {
                    if (!oldarray1D.Contains(newarray[i][1]) && !oldarray1D.Contains(newarray[i][0]))
                    {
                        MarkAsAdded(newarray[i][0]);
                    }
                }
            }
        }

        private void MarkAsRemoved(string oldpath)
        {
            Append("REMOVED", oldpath);
        }

        private void MarkAsRenamed(string oldPath, string newPath)
        {
            var renamedFile = MessageBox.Show("Renamed file found!" + Environment.NewLine + "OLD PATH: " + oldPath + Environment.NewLine + "NEW PATH: " + newPath + Environment.NewLine + "Do you want to mark this file as REMOVED/ADDED?", "Renamed File Found Exception", MessageBoxButtons.YesNo);

            if (renamedFile == DialogResult.Yes)
            {
                MarkAsRemoved(oldPath);
                MarkAsAdded(newPath);
            }
        }

        private void MarkAsAdded(string path)
        {
            Append("ADDED", path);
        }

        private void MarkAsChanged(string path)
        {
            Append("CHANGED", path);
        }

        private void MakeBatScriptButtonClick(object sender, EventArgs e) { }
    }
}
