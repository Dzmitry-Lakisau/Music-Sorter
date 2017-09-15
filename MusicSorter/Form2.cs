using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MusicSorter
{
    public partial class Form2 : Form
    {
        public string source_path = null;
        string destination_path = "E:\\UNSORTED";
        const string SOURCE_NAME = "SONY 32 GB";
        string[] folders = null;
        private Form1 _Form1;
        public Form2(Form1 f)
        {
            _Form1 = f;
            InitializeComponent();
            Text = "Перенос с флешки";
            label2.Text = destination_path;
            UpdateFolders(destination_path, listBox2);
            DriveInfo[] allDrives = DriveInfo.GetDrives();

            foreach (DriveInfo d in allDrives)
            {
                try {
                    if (d.VolumeLabel.ToString() == SOURCE_NAME)
                    {
                        source_path = d.Name.ToString();
                        break;
                    }
                }
                catch (IOException err)
                {

                }
            }
            if (source_path != null)
            {
                label1.Text = source_path;
                UpdateFolders(source_path, listBox1);
            }
            else
            {
                MessageBox.Show(SOURCE_NAME + " не найдена", "Ошибка");
                Close();
            }
        }

        private void UpdateFolders(string path, ListBox listbox)
        {
            listbox.Items.Clear();
            folders = Directory.GetDirectories(path);
            foreach (string f in folders)
            {
                listbox.Items.Add(f.Substring(f.LastIndexOf("\\") + 1));
            }
            listbox.Sorted = true;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox combobox = (ComboBox)sender;
            UpdateFolders(combobox.GetItemText(combobox.SelectedItem), listBox1);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string[] folders = GetSelectedFolders();

            foreach (string s in folders)
            {
                CopyFolder(source_path + s, destination_path + "\\" + s);
                string[] files = Directory.GetFiles(source_path + s);
                foreach (string file in files)
                {
                    Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile(file, Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs, Microsoft.VisualBasic.FileIO.RecycleOption.SendToRecycleBin);
                }
                Directory.Delete(source_path + s, true);
            }
            UpdateFolders(destination_path, listBox2);
            UpdateFolders(source_path, listBox1);
        }

        static public void CopyFolder(string sourceFolder, string destFolder)
        {
            try
            {
                if (!Directory.Exists(destFolder))
                    Directory.CreateDirectory(destFolder);
                string[] files = Directory.GetFiles(sourceFolder);
                foreach (string file in files)
                {
                    string name = Path.GetFileName(file);
                    string dest = Path.Combine(destFolder, name);
                    File.Copy(file, dest);
                }
                string[] folders = Directory.GetDirectories(sourceFolder);
                foreach (string folder in folders)
                {
                    string name = Path.GetFileName(folder);
                    string dest = Path.Combine(destFolder, name);
                    CopyFolder(folder, dest);
                }
            }
            catch (IOException err) { MessageBox.Show(err.ToString(), "Ошибка"); }
        }

        private string[] GetSelectedFolders()
        {
            //string path = root;
            ListBox.SelectedObjectCollection temp = new ListBox.SelectedObjectCollection(listBox1);
            folders = new string[temp.Count];
            for (int i = 0; i < temp.Count; i++)
            {
                folders[i] = temp[i].ToString();
            }
            return folders;
        }

        private void label2_Click(object sender, EventArgs e)
        {
            listBox2.Items.Clear();
            UpdateFolders(destination_path, listBox2);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                string[] folders = GetSelectedFolders();
                foreach (string folder in folders)
                {
                        string apppath = "C:\\Program Files (x86)\\Audio Identifier\\AI.exe";
                        ProcessStartInfo processInfo = new ProcessStartInfo();
                        processInfo.FileName = apppath;
                        processInfo.WorkingDirectory = Path.GetDirectoryName(apppath);
                        processInfo.Arguments = "\"" + source_path + folder + "\"";
                        Process.Start(processInfo);
                }
            }
            catch (System.NullReferenceException err) { MessageBox.Show(err.ToString(), "Ошибка"); }
            catch (System.ComponentModel.Win32Exception err) { MessageBox.Show(err.ToString(), "Ошибка"); }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                string[] folders = GetSelectedFolders();
                foreach (string folder in folders)
                {
                    string[] files = Directory.GetFiles(source_path + folder);
                    files = FilterFiles(files);
                    foreach (string file in files)
                    {
                        //string folder = source_path + listBox1.SelectedItem.ToString();
                        string apppath = "C:\\Program Files\\Adobe\\Adobe Audition CC\\Adobe Audition CC.exe";
                        ProcessStartInfo processInfo = new ProcessStartInfo();
                        processInfo.FileName = apppath;
                        processInfo.WorkingDirectory = Path.GetDirectoryName(apppath);
                        processInfo.Arguments = "\"" + file + "\"";
                        Process.Start(processInfo);
                    }
                }
            }
            catch (System.NullReferenceException err) { MessageBox.Show(err.ToString(), "Ошибка"); }
            catch (System.ComponentModel.Win32Exception err) { MessageBox.Show(err.ToString(), "Ошибка"); }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string[] folders = GetSelectedFolders();

            foreach (string s in folders)
            {
                CopyFolder(source_path + s, destination_path + "\\UPCONVERT\\" + s);
                string[] files = Directory.GetFiles(source_path + s);
                foreach (string file in files)
                { 
                    Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile(file, Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs, Microsoft.VisualBasic.FileIO.RecycleOption.SendToRecycleBin);
                }
                Directory.Delete(source_path + s);
            }
            UpdateFolders(destination_path, listBox2);
            UpdateFolders(source_path, listBox1);
        }

        private string[] FilterFiles(string[] files)
        {

            List<string> sortedfiles = new List<string>();
           
            for (int i = 0; i < files.GetLength(0); i++)
                if (files[i].EndsWith(".mp3") || files[i].EndsWith(".flac"))
                {
                    sortedfiles.Add(files[i]);
                }

            return sortedfiles.ToArray<string>();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                string[] folders = GetSelectedFolders();
                foreach (string folder in folders)
                {
                    string temp = source_path + "\\" + folder;
                    Process.Start(@temp);
                }
            }
            catch (System.NullReferenceException err) { MessageBox.Show(err.ToString(), "Ошибка"); }
            catch (System.ComponentModel.Win32Exception err) { MessageBox.Show(err.ToString(), "Ошибка"); }
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            //    //Form1.label1_Click(sender, e);
            string s = destination_path;
            _Form1.UpdateFolders(s);
            _Form1.listBox2.Items.Clear();
        }
}
}
