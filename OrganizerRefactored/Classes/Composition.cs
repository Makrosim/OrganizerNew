using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.ComponentModel;
using System.Windows.Media.Imaging;

namespace OrganizerRefactored
{
    public class Composition : INotifyPropertyChanged, ICloneable
    {
        public UInt32 Number { get; set; }
        public string Path { get; set; }
        public string FileName { get; set; }
        public string Performers { get; set; }
        public string Album { get; set; }
        public string Genre { get; set; }
        public string Year { get; set; }
        public string Text { get; set; }
        public string MusicBrainzID { get; set; }
        public string Duration { get; set; }
        public string TagType { get; set; }
        public BitmapImage Image { get; set; }

        public string Title { get; set; }
        public string UpperLine { get; set; }
        public string LowerLine { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string property)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        public object Clone()
        {
            Composition clone = new Composition();
            clone.Number = this.Number;
            clone.Path = this.Path;
            clone.FileName = this.FileName;
            clone.Performers = this.Performers;
            clone.Title = this.Title;
            clone.UpperLine = this.UpperLine;
            clone.Album = this.Album;
            clone.Genre = this.Genre;
            clone.Year = this.Year;
            clone.MusicBrainzID = this.MusicBrainzID;
            clone.LowerLine = this.LowerLine;
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
            copy.UpperLine = this.UpperLine;
            copy.Album = this.Album;
            copy.Genre = this.Genre;
            copy.Year = this.Year;
            copy.MusicBrainzID = this.MusicBrainzID;
            copy.LowerLine = this.LowerLine;
            copy.Duration = this.Duration;
            return copy;
        }

        public void TagToTheName()
        {
            var ext = Path.Substring(Path.LastIndexOf("."));
            System.IO.File.Move(Path, Path.Substring(0, Path.LastIndexOf(@"\") + 1) + UpperLine + ext);
            Path = Path.Substring(0, Path.LastIndexOf(@"\") + 1) + UpperLine + ext;
            OnPropertyChanged("Path");
            FileName = Path.Substring(Path.LastIndexOf(@"\") + 1);
            OnPropertyChanged("FileName");
        }

        public void ToUTF16()
        {
            Performers = StringToUTF16(Performers);
            OnPropertyChanged("Performers");
            Title = StringToUTF16(Title);
            OnPropertyChanged("Title");
            UpperLine = StringToUTF16(UpperLine);
            OnPropertyChanged("Lb_Title");
            Album = StringToUTF16(Album);
            OnPropertyChanged("Album");
        }

        private string StringToUTF16(string str)
        {
            var win1251 = Encoding.GetEncoding(1251);
            var utf16 = Encoding.GetEncoding(1200);

            var oldbyte = utf16.GetBytes(str);
            var newbyte = new byte[oldbyte.Length / 2];
            for (int i = 0, j = 0; i < oldbyte.Length; i = i + 2, j++)
            {
                newbyte[j] = oldbyte[i];
            }
            return str = win1251.GetString(newbyte);
        }
    }
}
