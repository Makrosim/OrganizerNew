﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Fingerprinter;

namespace OrganizerRefactored
{
    public partial class TagEditor : Page, INotifyPropertyChanged
    {
        public ActionCommand cmd_Save { get; set; }
        public ActionCommand cmd_Recognize { get; set; }
        public ActionCommand cmd_ToUTF16 { get; set; }
        public ActionCommand cmd_TagToTheName { get; set; }
        public Composition OldComposition { get; set; }
        public Composition Composition { get; set; }
        public bool IsMultiselect { get; set; }

        private List<Composition> CompList;
        private IPlaylist ICompList;
        private bool Saved = true;

        public event PropertyChangedEventHandler PropertyChanged;

        public TagEditor(IPlaylist complist)
        {
            InitializeComponent();
            DataContext = this;
            cmd_Save = new ActionCommand(SaveTags) { IsExecutable = true };
            cmd_Recognize = new ActionCommand(RecognizeComposition) { IsExecutable = true };
            cmd_ToUTF16 = new ActionCommand(FixEncoding) { IsExecutable = true };
            cmd_TagToTheName = new ActionCommand(TagToTheName) { IsExecutable = true };
            ICompList = complist;
            ICompList.SelectionChangedEvent += SelectionChanged;
        }

        protected void OnPropertyChanged(string property)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        private void SelectionChanged(object sender, EventArgs e)
        {
            if(ICompList.GetSelectedCompositions().Count() > 0)
                CompList = ICompList.GetSelectedCompositions();

            if (CompList != null)
            {
                Composition = CompList.First();

                if (Composition.Path.StartsWith("http"))
                {
                    btn_Recognize.IsEnabled = false;
                    btn_FixEncoding.IsEnabled = false;
                    btn_TagToTheName.IsEnabled = false;
                    tbx_ID.IsEnabled = false;
                    tbx_Year.IsEnabled = false;
                }
            }

            if (CompList.Count > 1)
                IsMultiselect = true;
            else
                IsMultiselect = false;

            OldComposition = null;
            Saved = true;
            OnPropertyChanged("Composition");
            OnPropertyChanged("IsMultiselect");
        }

        public void SaveTags()
        {
            if (IsMultiselect)
            {
                foreach (Composition comp in CompList)
                {
                    if (chb_Performers.IsChecked == true)
                        comp.Performers = Composition.Performers;
                    if (chb_Album.IsChecked == true)
                        comp.Album = Composition.Album;
                    if (chb_Genre.IsChecked == true)
                        comp.Genre = Composition.Genre;
                    if (chb_Year.IsChecked == true)
                        comp.Year = Composition.Year;
                    if (!ICompList.SaveComposition(comp))
                        MessageBox.Show("Вконтакте позволяет задавать следующие жанры аудиозаписей:\nRock, Pop, Rap & Hip - Hop, Easy Listening, Dance & House, Instrumental, Metal, Alternative, Dubstep, Jazz & Blues, Drum & Bass, Trance, Chanson, Ethnic, Acoustic & Vocal, Reggae, Classical, Indie Pop, Speech, Electropop & Disco, Other\nПожалуйста, введите корректное значение.");
                }
            }
            else
            {
                if (!ICompList.SaveComposition(Composition))
                {
                    MessageBox.Show("Вконтакте позволяет задавать следующие жанры аудиозаписей:\nRock, Pop, Rap & Hip - Hop, Easy Listening, Dance & House, Instrumental, Metal, Alternative, Dubstep, Jazz & Blues, Drum & Bass, Trance, Chanson, Ethnic, Acoustic & Vocal, Reggae, Classical, Indie Pop, Speech, Electropop & Disco, Other\nПожалуйста, введите корректное значение.");
                }
            }
            ICompList.RefreshCollection();
        }

        public void RecognizeComposition()
        {
            var Recognizer = new Recognizer();
            string[] tags = Recognizer.Recognize(Composition.Path);

            if (tags[0].Equals("Error"))
                MessageBox.Show(tags[1] + ": " + tags[2], tags[0]);
            else
            {
                tbx_Title.Text = tags[0];
                tbx_Performers.Text = tags[1];
                tbx_ID.Text = tags[2];
            }
        }
         
        private void TagManuallyEdited(object sender, KeyEventArgs e)
        {
            TagEdited();
        }

        private void TagEdited()
        {
            if (Saved)
            {
                OldComposition = (Composition)Composition.Clone();
                Saved = false;
            }
        }
        
        private void Switch(object sender, MouseEventArgs e)
        {
            if (!Saved)
            {
                var tmp = (Composition)Composition.Clone();
                Composition = OldComposition.Copy(Composition);
                OldComposition = tmp.Copy(OldComposition);
                OnPropertyChanged("Composition");
            }
        }

        private void btn_Previous_Click(object sender, RoutedEventArgs e)
        {
            OldComposition = null;
            Saved = true;
        }

        private void FixEncoding()
        {
            TagEdited();
            if (!IsMultiselect)
            {
                Composition.ToUTF16();
            }
            else
            {
                foreach (Composition comp in CompList)
                {
                    comp.ToUTF16();
                }
            }
        }

        private void TagToTheName()
        {
            if (!IsMultiselect)
            {
                Composition.TagToTheName();
            }
            else
            {
                foreach (Composition comp in CompList)
                {
                    comp.TagToTheName();
                }
            }
            OnPropertyChanged("Composition");
        }
    }
}