using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TagLib;
using System.IO;
using System.ComponentModel;

namespace OrganizerRefactored
{
    public class Composition : INotifyPropertyChanged, ICloneable
    {
        public UInt32 Number { get; set; }
        public string Path { get; set; }
        public string FileName { get; set; }
        public string Performers { get; set; }
        public string Title { get; set; }
        public string Lb_Title { get; set; }
        public string Album { get; set; }
        public string Genres { get; set; }
        public string Year { get; set; }
        public string MusicBrainzID { get; set; }
        public string Bitrate { get; set; }
        public string Duration { get; set; }
        public string TagType { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public Composition()
        {

        }

        public Composition(string path)
        {
            var Audiofile = TagLib.File.Create(path);
            Path = path;
            FileName = Audiofile.Name.Substring(Audiofile.Name.LastIndexOf(@"\") + 1);
            Performers = String.Join(", ", Audiofile.Tag.Performers);
            Title = Audiofile.Tag.Title;
            Lb_Title = String.Join(", ", Audiofile.Tag.Performers) + " - " + Audiofile.Tag.Title;
            Album = Audiofile.Tag.Album;
            Genres = String.Join(", ", Audiofile.Tag.Genres);
            Year = Audiofile.Tag.Year.ToString();
            Bitrate = Audiofile.Properties.AudioBitrate.ToString() + "kbps  " + Convert.ToString(Audiofile.Length / 1048576);
            Duration = Audiofile.Properties.Duration.ToString("mm\\:ss");
            TagType = Audiofile.Tag.TagTypes.ToString();
        }

        protected void OnPropertyChanged(string property)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        public void SaveChanges()
        {
            var Audiofile = TagLib.File.Create(Path);
            Audiofile.Tag.Performers = Performers.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
            Audiofile.Tag.Title = Title;
            Audiofile.Tag.Album = Album;
            Audiofile.Tag.Genres = Genres.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
            Audiofile.Tag.Year = Convert.ToUInt32(Year);
            Audiofile.Tag.MusicBrainzTrackId = MusicBrainzID;
            Audiofile.Save();
            Lb_Title = String.Join(", ", Audiofile.Tag.Performers) + " - " + Audiofile.Tag.Title;
            OnPropertyChanged("Lb_Title");
        }

        public object Clone()
        {
            Composition clone = new Composition();
            clone.Number = this.Number;
            clone.Path = this.Path;
            clone.FileName = this.FileName;
            clone.Performers = this.Performers;
            clone.Title = this.Title;
            clone.Lb_Title = this.Lb_Title;
            clone.Album = this.Album;
            clone.Genres = this.Genres;
            clone.Year = this.Year;
            clone.MusicBrainzID = this.MusicBrainzID;
            clone.Bitrate = this.Bitrate;
            clone.Duration = this.Duration;
            return clone;
        }

        public Composition Copy(Composition copy)
        {
            copy.Number = this.Number;
            copy.Path = this.Path;
            copy.FileName = this.FileName;
            copy.Performers = this.Performers;
            copy.Title = this.Title;
            copy.Lb_Title = this.Lb_Title;
            copy.Album = this.Album;
            copy.Genres = this.Genres;
            copy.Year = this.Year;
            copy.MusicBrainzID = this.MusicBrainzID;
            copy.Bitrate = this.Bitrate;
            copy.Duration = this.Duration;
            return copy;
        }

        public void TagToTheName()
        {
            var ext = Path.Substring(Path.LastIndexOf("."));
            System.IO.File.Move(Path, Path.Substring(0, Path.LastIndexOf(@"\") + 1) + Lb_Title + ext);
            Path = Path.Substring(0, Path.LastIndexOf(@"\") + 1) + Lb_Title + ext;
            OnPropertyChanged("Path");
            FileName = Path.Substring(Path.LastIndexOf(@"\") + 1);
            OnPropertyChanged("FileName");
        }
    }
}
