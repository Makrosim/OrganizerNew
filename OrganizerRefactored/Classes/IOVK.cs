using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using TagLib;

namespace OrganizerRefactored
{
    class IOVK : IIO
    {
        public IOVK(int appId, string login, string password)
        {

        }

        public void WritePlaylist(ObservableCollection<Composition> playlist)
        {

        }

        public ObservableCollection<Composition> ReadPlaylist(ObservableCollection<Composition> playlist)
        {

                return playlist;

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
            return playlist;
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
                if (pattern.IsMatch(path) && (System.IO.File.Exists(path)))
                {
                    try
                    {
                        Composition comp = new Composition();
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

        public void SaveComposition(Composition comp)
        {
            var Audiofile = TagLib.File.Create(comp.Path);
            Audiofile.Tag.Performers = comp.Performers.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
            Audiofile.Tag.Title = comp.Title;
            Audiofile.Tag.Album = comp.Album;
            Audiofile.Tag.Genres = comp.Genres.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
            Audiofile.Tag.Year = Convert.ToUInt32(comp.Year);
            Audiofile.Tag.MusicBrainzTrackId = comp.MusicBrainzID;
            Audiofile.Save();
            comp.Lb_Title = String.Join(", ", Audiofile.Tag.Performers) + " - " + Audiofile.Tag.Title;
            comp.OnPropertyChanged("Lb_Title");
        }

    }
}