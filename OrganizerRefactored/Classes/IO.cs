using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.IO;
using Microsoft.Win32;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace OrganizerRefactored
{
    class IO : IIO
    {
        public void WritePlaylist(ObservableCollection<Composition> playlist)
        {
            using (StreamWriter file = new StreamWriter(@"Playlist.txt", false))
            {
                foreach (Composition comp in playlist)
                {
                    file.WriteLine(comp.Path);
                }
            }
        }

        public ObservableCollection<Composition> ReadPlaylist(ObservableCollection<Composition> playlist)
        {
            var opened = new List<string>();

            if (File.Exists("Playlist.txt"))
            {
                using (StreamReader sr = new StreamReader("Playlist.txt"))
                {
                    string line;
                    for (int i = 0; (line = sr.ReadLine()) != null; i++)
                    {
                        opened.Add(line);
                    }
                }
                FillCollection(playlist, opened);
                return playlist;
            }
            else
            {
                return playlist;
            }
        }

        public ObservableCollection<Composition> OpenFiles(ObservableCollection<Composition> playlist)
        {
            var opened = new List<string>();
            var openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Multiselect = true;

            if (openFileDialog.ShowDialog() == true)
            {
                opened = openFileDialog.FileNames.ToList();
            }

            FillCollection(playlist, opened);
            return playlist;
        }

        public ObservableCollection<Composition> OpenFolder(ObservableCollection<Composition> playlist)
        {
            var opened = new List<string>();
            string folderName;
            var fbd = new FolderBrowserDialog();
            DialogResult result = fbd.ShowDialog();
            if (!(result == DialogResult.OK))
            {

            }
            folderName = fbd.SelectedPath;
            GetFilesFromDirectory(folderName, ref opened);
            FillCollection(playlist, opened);
            return playlist;
        }

        private void GetFilesFromDirectory(string folderName, ref List<string> files)
        {
            files.AddRange(Directory.GetFiles(folderName));
            foreach(string s in Directory.GetDirectories(folderName))
            {
                GetFilesFromDirectory(s, ref files);
            }
        }

        private void FillCollection(ObservableCollection<Composition> playlist, List<string> opened)
        {
            foreach (Composition comp in playlist)
            {
                foreach (string _path in opened)
                {
                    if (comp.Path.Equals(_path))
                    {
                        opened.Remove(_path);
                        break;
                    }
                }
            }

            var pattern = new Regex(@"mp3$"); //Добавить форматы
            foreach (string path in opened)
            {
                if (pattern.IsMatch(path) && (File.Exists(path)))
                {
                    try
                    {
                        Composition comp = new Composition(path);
                        playlist.Add(comp);
                        comp.Number = (UInt32)playlist.IndexOf(comp) + 1;
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show("Error reading the file path:" + ex.Message);
                    }
                }
            }
        }

    }
}