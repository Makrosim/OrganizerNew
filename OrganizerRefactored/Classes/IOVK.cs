using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using TagLib;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model;
using VkNet.Model.RequestParams;
using VkNet.Enums;

namespace OrganizerRefactored
{
    class IOVK : IIO
    {
        VkApi vk;
        User user;

        public IOVK(ulong appId, string login, string password)
        {
            vk = new VkApi();
            vk.Authorize(new ApiAuthParams {ApplicationId = appId, Login = login, Password = password, Settings = Settings.All});
        }

        public void WritePlaylist(ObservableCollection<Composition> playlist)
        {

        }

        public ObservableCollection<Composition> ReadPlaylist(ObservableCollection<Composition> playlist)
        {
            var audios = vk.Audio.Get(out user, new AudioGetParams {Count = 6000, NeedUser = true });
            FillCollection(playlist, audios);
                return playlist;
        }

        public ObservableCollection<Composition> OpenFiles(ObservableCollection<Composition> playlist)
        {
            return playlist;
        }

        public ObservableCollection<Composition> OpenFolder(ObservableCollection<Composition> playlist)
        {
            return playlist;
        }

        private void FillCollection(ObservableCollection<Composition> playlist, ReadOnlyCollection<VkNet.Model.Attachments.Audio> opened)
        {
            foreach (var audio in opened)
            {
                Composition comp = new Composition();

                comp.Title = audio.Title;
                comp.Performers = audio.Artist;
                comp.UpperLine = audio.Artist + " - " + audio.Title;
                comp.Genre = audio.Genre.ToString();

                var tmp1 = TimeSpan.FromSeconds(audio.Duration);
                comp.Duration = tmp1.ToString("mm\\:ss");

                string tmp = audio.Url.ToString();
                comp.Path = tmp.Substring(0, tmp.IndexOf(@"?"));
                comp.FileName = audio.Id.ToString();
                comp.Album = null;
                comp.Year = null;
                comp.LowerLine = null;
                comp.TagType = null;

                playlist.Add(comp);

                comp.Number = (UInt32)playlist.IndexOf(comp) + 1;
            }
        }

        public bool SaveComposition(Composition comp) //Add text
        {
            var audio = vk.Audio.GetById(comp.FileName);
            string text;

            if (audio.First().LyricsId != null)
                text = vk.Audio.GetLyrics((long)audio.First().LyricsId).ToString();
            else
                text = "";
            try
            {
                vk.Audio.Edit(new AudioEditParams
                {
                    OwnerId = user.Id,
                    AudioId = Convert.ToInt32(comp.FileName),
                    Title = comp.Title,
                    Artist = comp.Performers,
                    GenreId = (AudioGenre)Enum.Parse(typeof(AudioGenre), comp.Genre),
                    Text = text
                });
            }
            catch (ArgumentException ex)
            {
                return false;
            }
            return true;
        }

    }
}