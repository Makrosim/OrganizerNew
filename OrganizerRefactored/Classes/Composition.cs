using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TagLib;

namespace OrganizerRefactored
{
    public class Composition
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
            Title = String.Join(", ", Audiofile.Tag.Performers) + " - " + Audiofile.Tag.Title;
        }

        public Composition Clone(Composition copy)
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
    }
}
