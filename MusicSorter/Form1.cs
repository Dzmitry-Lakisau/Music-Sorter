using System;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Net;
using System.Collections.Generic;
using System.Drawing;
using TagLib;

namespace MusicSorter
{

    public partial class Form1 : Form
    {
        public string[] folders = null;
        string root = "E:\\UNSORTED";
        
        public Form1()
        {
            InitializeComponent();
            Text = "Music Tool";
            label1.Text = root;
            UpdateFolders(root);
        }

        //private string GetInfo(string[] files)
        //{

        //        string parent = root; //Directory.GetParent(s).ToString();
        //        Directory.CreateDirectory(parent + "\\" + foldername);
        //        files = Directory.GetFiles(files[0].Substring(0, files[0].LastIndexOf("\\")));
        //        foreach (string f in files)
        //        {
        //            string destpath = parent + "\\" + foldername + "\\" + f.Substring(f.LastIndexOf("\\") + 1);
        //            try
        //            {
        //                File.Move(f, destpath);
        //            }
        //            catch (IOException err)
        //            {
        //                MessageBox.Show(err.ToString());
        //            }
        //        }
        //        return searchquery;

        //}



        private void button1_Click(object sender, EventArgs e)
        {
            string[] folders = GetSelectedFolders();  

            foreach (string s in folders)
            {
                string[] files = Directory.GetFiles(root + "\\" + s);
                if (FilterFiles(files).Length != 0)
                {
                    MoveFiles(files);
                    //Directory.Delete(root + "\\" + s);
                }
                else
                {
                    MessageBox.Show("Музыкальных файлов в данной папке нет!", "Ошибка");
                //return null;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string[] folders = GetSelectedFolders();

            foreach (string s in folders)
            {
                string[] files = Directory.GetFiles(root + "\\" + s);
                 if (FilterFiles(files).Length != 0)
                {
                    SearchCover(files);
                }
                else
                {
                    MessageBox.Show("Музыкальных файлов в данной папке нет!", "Ошибка");
                }
            }
        }

        public void label1_Click(object sender, EventArgs e)
        {
            UpdateFolders(root);
            listBox2.Items.Clear();
        }

        public void UpdateFolders(string path)
        {
            listBox1.Items.Clear();
            folders = Directory.GetDirectories(path);
            foreach (string f in folders)
            {
                listBox1.Items.Add(f.Substring(f.LastIndexOf("\\") + 1));
            }
        }

        private string[] FilterFiles(string[] files){
            string[] sortedfiles = null;
            int n=0;
            for (int i = 0; i < files.GetLength(0); i++)         
                if (files[i].EndsWith(".mp3") || files[i].EndsWith(".flac"))
                {
                    n++;
                }
            
            sortedfiles = new string[n];

            int j = 0;
            for (int i = 0; i < files.GetLength(0); i++)
                if (files[i].EndsWith(".mp3") || files[i].EndsWith(".flac"))
                {
                    sortedfiles[j] = files[i];
                    j++;
                }
            return sortedfiles;
        }

        private string[] GetSelectedFolders(){
            //string path = root;
            ListBox.SelectedObjectCollection temp = new ListBox.SelectedObjectCollection(listBox1);
            folders = new string[temp.Count];
            for (int i = 0; i < temp.Count; i++)
            {
                folders[i] = temp[i].ToString();
            }
            return folders;
        }

        private void MoveFiles(string[] files){
            string foldername = null;
            try {
                string[] sortedfiles = FilterFiles(files);
                List<string[]> tags = GetTags(sortedfiles);
                if (isCompilation(tags[0], tags[2], tags[4])) RenameFilesFromCompilation(sortedfiles, tags[0], tags[1], tags[3]);
                else RenameFilesFromAlbum(sortedfiles, tags[1], tags[3]);
                foldername = GenerateNewFolderName(tags[0], tags[2], tags[4]);
                string parent = root; //Directory.GetParent(s).ToString();
                Directory.CreateDirectory(parent + "\\" + foldername);
                files = Directory.GetFiles(files[0].Substring(0, files[0].LastIndexOf("\\")));
                foreach (string f in files)
                {
                    string destpath = parent + "\\" + foldername + "\\" + f.Substring(f.LastIndexOf("\\") + 1);
                    System.IO.File.Move(f, destpath);
                }
                if ((files[0].Substring(0, files[0].LastIndexOf("\\"))) != (parent + "\\" + foldername))
                    Directory.Delete(files[0].Substring(0, files[0].LastIndexOf("\\")));
            }
            catch (IOException err){ MessageBox.Show(err.ToString(), "Ошибка"); }

            UpdateFolders(root);
            int i = 0;
            foreach (var item in listBox1.Items)
            {
                if (item.ToString().Equals(foldername)) {
                    i = listBox1.Items.IndexOf(item);
                    break;
                }
            }
            listBox1.SelectedIndex = i;
        }

        private List<string[]> GetTags(string[] files){
            int n = files.GetLength(0);
            string[] artists = new string[n];
            string[] albums = new string[n];
            string[] years = new string[n];
            string[] titles = new string[n];
            string[] tracks = new string[n];

            for (int i = 0; i < n; i++)
                {
                    TagLib.File file = TagLib.File.Create(files[i]);
                    //TagLib.Id3v2.Tag tag = new TagLib.Id3v2.Tag(file, 0);
                    //artist = tag.Performers;
                    //album = tag.Album;
                    //year = tag.Year;                   

                    string[] artist = file.Tag.Performers;
                    string album = file.Tag.Album;
                    uint year = file.Tag.Year;
                    string title = file.Tag.Title;
                    uint track = file.Tag.Track;

                    artists[i] = artist[0];
                    albums[i] = album;
                    years[i] = year.ToString();
                    titles[i] = title;
                    tracks[i] = track.ToString();
                }
                List<string[]> tags = new List<string[]>(){artists, titles, albums, tracks, years};
                return tags;
        }

        private bool isCompilation(string[] artists, string[] albums, string[] years){
                int same_year = 0;
                for (int i = 0; i < years.GetLength(0) - 1; i++)
                    if (years[i] == years[i + 1]) same_year++;
                int same_album = 0;
                for (int i = 0; i < albums.GetLength(0) - 1; i++)
                    if (albums[i] == albums[i + 1]) same_album++;
                int same_artist = 0;
                for (int i = 0; i < artists.GetLength(0) - 1; i++)
                    if (artists[i][0] == artists[i + 1][0]) same_artist++;
                float percent = (float) ++same_artist / (float) artists.GetLength(0);
                bool compilation = !(++same_album == albums.GetLength(0) && ++same_year == years.GetLength(0) && percent > 0.9);
                return compilation;                
        }

        private void RenameFilesFromCompilation(string[] sortedfiles, string[] artists, string[] titles, string[] tracks){
            string formated_track = 0.ToString();
            for (int i = 0; i < sortedfiles.GetLength(0); i++)
                    {
                        if (int.Parse(tracks[i]) < 10) formated_track = "0" + tracks[i].ToString();
                            else formated_track = tracks[i].ToString();
                System.IO.File.Move(sortedfiles[i], sortedfiles[i].Substring(0, sortedfiles[i].LastIndexOf("\\") + 1) + formated_track + ". "+ artists[i] + " - " + titles[i] + sortedfiles[i].Substring(sortedfiles[i].LastIndexOf('.')));
                    }
                    //searchquery = album;
                    //if (album.EndsWith(")")) foldername = searchquery + "(" + year + ")";
                    //else foldername = searchquery + " (" + year + ")";
        }

        private void RenameFilesFromAlbum(string[] sortedfiles, string[] titles, string[] tracks){
            string formated_track = 0.ToString();
            for (int i=0; i < sortedfiles.GetLength(0); i++)
                {
                    if (int.Parse(tracks[i]) < 10) formated_track = "0" + tracks[i].ToString();
                        else formated_track = tracks[i].ToString();
                System.IO.File.Move(sortedfiles[i], sortedfiles[i].Substring(0, sortedfiles[i].LastIndexOf("\\") + 1)+formated_track+". "+titles[i]+sortedfiles[i].Substring(sortedfiles[i].LastIndexOf('.')));
                }
                    //searchquery = artist[0] + " - " + album;
                    //if (album.EndsWith(")")) foldername = searchquery + "(" + year + ")";
                    //else foldername = searchquery + " (" + year + ")";
        }

        private string GenerateNewFolderName(string[] artists, string[] albums, string[] years){
            string foldername = null;
            if (!isCompilation(artists, albums, years)) {
                if (albums[0].EndsWith(")")) foldername = artists[0] + " - " + albums[0] + "(" + years[0] + ")";
                    else foldername = artists[0] + " - " + albums[0] + " (" + years[0] + ")";
            }
            else {
                if (albums[0].EndsWith(")")) foldername = albums[0] + "(" + years[0] + ")";
                    else foldername = albums[0] + " (" + years[0] + ")";
            }
            foldername = foldername.Replace(" / ", ", ");
            return foldername;
        }

        private void SearchCover(string[] files){

            string[] sortedfiles = FilterFiles(files);
            List<string[]> tags = GetTags(sortedfiles);
            string s = null;
            if (isCompilation(tags[0], tags[2], tags[4])) s = tags[2][0];
                else s = tags[0][0] + " - " + tags[2][0];

            String link = "https://www.google.com/search?num=20&q=" + WebUtility.UrlEncode(s) + "&tbm=isch&tbs=isz:l";
            Process.Start(link);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string[] folders = GetSelectedFolders();

            foreach (string s in folders)
            {
                string[] files = Directory.GetFiles(root + "\\" + s);
                string[] sortedfiles = FilterFiles(files);
                if (sortedfiles.GetLength(0) != 0)
                {
                    ExtractImageFromFiles(sortedfiles);
                    RefreshList();
                }
                else
                {
                    MessageBox.Show("Музыкальных файлов в данной папке нет!", "Ошибка");
                    //return null;
                }
            }
        }

        private void ExtractImageFromFiles(string[] files)
        {
            try
            {
                List<string[]> tags = GetTags(files);
                string foldername = GenerateNewFolderName(tags[0], tags[2], tags[4]);

                foreach (string s in files)
                {
                    TagLib.File tag = TagLib.File.Create(s);

                    TagLib.IPicture[] pic = tag.Tag.Pictures;
                    if (pic.GetLength(0) > 0)
                    {
                        MemoryStream stream = new MemoryStream(pic[0].Data.Data);
                        Image im = Image.FromStream(stream);
                        im.Save(s.Substring(0, s.LastIndexOf("\\")) + "//" + foldername + ".jpg");
                    }
                }
            }
            catch (IOException err) { MessageBox.Show(err.ToString(), "Ошибка"); }
        }

        private void DeleteImagesFromFiles(string[] files) {
            try {
                List<string[]> tags = GetTags(files);
                //string foldername = GenerateNewFolderName(tags[0], tags[2], tags[4]);

                foreach (string s in files)
                {
                    TagLib.File tag = TagLib.File.Create(s);

                    TagLib.IPicture[] pic = tag.Tag.Pictures;
                    if (pic.GetLength(0) > 0)
                    {
                        TagLib.Tag tempTag = null;
                        tempTag = new TagLib.Id3v2.Tag();
                        tag.Tag.CopyTo(tempTag, true);
                        tag.RemoveTags(TagLib.TagTypes.AllTags);
                        System.Threading.Thread.Sleep(50);
                        tag.Save();
                        tag.Dispose();

                        TagLib.File tag2 = TagLib.File.Create(s);
                        tempTag.CopyTo(tag2.Tag, true);
                        tag2.Save();
                        tag2.Dispose();
                    }
                }
            }
            catch (IOException err) { MessageBox.Show(err.ToString(), "Ошибка"); }
            catch (UnauthorizedAccessException err) { MessageBox.Show(err.ToString(), "Ошибка"); }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string[] folders = GetSelectedFolders();

            foreach (string s in folders)
            {
                string[] files = Directory.GetFiles(root + "\\" + s);
                string[] sortedfiles = FilterFiles(files);
                if (sortedfiles.GetLength(0) != 0)
                {
                    List<string[]> tags = GetTags(sortedfiles);
                    string foldername = GenerateNewFolderName(tags[0], tags[2], tags[4]);
                    InsertImagesInFiles(sortedfiles, root + "\\" + s + "\\" + foldername + ".jpg");
                }
                else
                {
                    MessageBox.Show("Музыкальных файлов в данной папке нет!", "Ошибка");
                    //return null;
                }
            }
        }

        private void InsertImagesInFiles(string[] files, string imagename)
        {
            try {
                foreach (string s in files)
                {
                    TagLib.File TagLibFile = TagLib.File.Create(s);
                    TagLib.Picture picture = new TagLib.Picture(imagename);
                    picture.Description = "";
                    TagLib.Id3v2.AttachedPictureFrame albumCoverPictFrame = new TagLib.Id3v2.AttachedPictureFrame(picture);
                    //albumCoverPictFrame.MimeType = System.Net.Mime.MediaTypeNames.Image.Jpeg;
                    //albumCoverPictFrame.Type = TagLib.PictureType.FrontCover;
                    TagLib.IPicture[] pictFrames = new IPicture[1];
                    pictFrames[0] = (IPicture)albumCoverPictFrame;
                    TagLibFile.Tag.Pictures = pictFrames;
                    TagLibFile.Save();
                }
            }
            catch (IOException err) { MessageBox.Show(err.ToString(), "Ошибка"); }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string[] files = Directory.GetFiles(root + "\\" + listBox1.SelectedItem);
            listBox2.Items.Clear();
            foreach (string f in files) {
                listBox2.Items.Add(f);
            }
        }

        private void listBox2_DoubleClick(object sender, EventArgs e)
        {
            if (listBox2.SelectedItem != null)
            {
                Process.Start(listBox2.SelectedItem.ToString());
            }
        }

        private void listBox2_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Delete)
                {
                    DeleteFile(listBox2.SelectedItem.ToString());
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    Process.Start(listBox2.SelectedItem.ToString());
                }
                else if (e.KeyCode == Keys.F5)
                {
                    RefreshList();
                }
            }
            catch (FileNotFoundException err) { MessageBox.Show(err.ToString(), "Ошибка"); }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            button5.Enabled = false;
            ListBox.SelectedObjectCollection temp = new ListBox.SelectedObjectCollection(listBox2);
            string[] files = new string[temp.Count];
            for (int i = 0; i < temp.Count; i++)
            {
                files[i] = temp[i].ToString();
            }
            foreach (string file in files)
            {
                DeleteFile(file);
            }
            RefreshList();
            button5.Enabled = true;
        }

        private void DeleteFile(string file)
        {
            int x = listBox2.SelectedIndex;
            try
            {
                Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile(file, Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs, Microsoft.VisualBasic.FileIO.RecycleOption.SendToRecycleBin);
                RefreshList();
            }
            catch (FileNotFoundException err) { MessageBox.Show(err.ToString(), "Ошибка"); }
            if (listBox2.Items.Count > 0)
                listBox2.SelectedIndex = 0;
            listBox2.SelectedIndex = listBox2.Items.Count - 1;

            try
            {
                // Set SelectedIndex to what it was
                listBox2.SelectedIndex = x;
            }

            catch
            {
                // Set SelectedIndex to one below if item was last in list
                listBox2.SelectedIndex = x - 1;
            }           
        }

        private void listBox2_Leave(object sender, EventArgs e)
        {
            //button5.Enabled = false;
        }

        private void listBox2_Enter(object sender, EventArgs e)
        {
            button5.Enabled = true;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            RefreshList();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            string[] folders = GetSelectedFolders();

            foreach (string s in folders)
            {
                string[] files = Directory.GetFiles(root + "\\" + s);
                string[] sortedfiles = FilterFiles(files);
                if (sortedfiles.GetLength(0) != 0)
                {
                    List<string[]> tags = GetTags(sortedfiles);
                    string foldername = GenerateNewFolderName(tags[0], tags[2], tags[4]);
                    InsertImagesInFiles(sortedfiles, listBox2.SelectedItem.ToString());
                }
                else
                {
                    MessageBox.Show("Музыкальных файлов в данной папке нет!", "Ошибка");
                    //return null;
                }
            }
        }

        private void RefreshList()
        {
            string[] files = Directory.GetFiles(root + "\\" + listBox1.SelectedItem);
            listBox2.Items.Clear();
            foreach (string f in files)
            {
                listBox2.Items.Add(f);
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            string[] folders = GetSelectedFolders();

            foreach (string s in folders)
            {
                string[] files = Directory.GetFiles(root + "\\" + s);
                string[] sortedfiles = FilterFiles(files);
                if (sortedfiles.GetLength(0) != 0)
                {
                    DeleteImagesFromFiles(sortedfiles);
                }
                else
                {
                    MessageBox.Show("Музыкальных файлов в данной папке нет!", "Ошибка");
                    //return null;
                }
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            try {
                string[] folders = GetSelectedFolders();
                foreach (string folder in folders)
                {
                    string temp = root + "\\" + folder;
                    Process.Start(@temp);
                }
            }
            catch (System.NullReferenceException err) { MessageBox.Show(err.ToString(), "Ошибка");}
            catch (System.ComponentModel.Win32Exception err) { MessageBox.Show(err.ToString(), "Ошибка");}
        }

      

        private void button12_Click(object sender, EventArgs e)
        {
            Form2 f = new Form2(this);
            if (f.source_path != null) f.Show();
                else f.Dispose();
        }
    }
}
