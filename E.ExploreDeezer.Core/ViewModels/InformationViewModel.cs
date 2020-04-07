using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

using System.ComponentModel;

using E.Deezer;
using E.ExploreDeezer.Core.Mvvm;

namespace E.ExploreDeezer.Core.ViewModels
{
    public interface IInformationViewModel : INotifyPropertyChanged,
                                             IDisposable
    {
        string Header { get; }
        string SubHeader { get; }
        string ArtworkUri { get; }

        IEnumerable<InformationEntry> Values { get; }
    }


    public enum EInformationType
    {
        Textual,
        Image,
    }

    public class InformationEntry
    {
        public InformationEntry(EInformationType type, string label, string value)
        {
            this.Type = type;
            this.Label = label;
            this.Value = value;
        }


        public EInformationType Type { get; }
        public string Label { get; }
        public string Value { get; }
    }




    internal class InformationViewModel : ViewModelBase, IInformationViewModel
    {
        private readonly IDeezerSession session;
        private readonly IAlbumViewModel albumViewModel;
        private readonly IPlaylistViewModel playlistViewModel;

        private string header;
        private string subHeader;
        private string artworkUri;
        private IEnumerable<InformationEntry> values;


        public InformationViewModel(IAlbumViewModel album,
                                    IDeezerSession session,
                                    IPlatformServices platformServices)
            : this(album, null, session, platformServices)
        { }

        public InformationViewModel(IPlaylistViewModel playlist,
                                    IDeezerSession session,
                                    IPlatformServices platformServices)
            : this(null, playlist, session, platformServices)
        { }


        public InformationViewModel(IAlbumViewModel album,
                                    IPlaylistViewModel playlist, 
                                    IDeezerSession session,
                                    IPlatformServices platformServices)
            : base(platformServices)
        {

            this.albumViewModel = album;
            this.playlistViewModel = playlist;
            this.session = session;

            PopulateInformation();
        }


        // IInformationViewModel
        public string Header
        {
            get => this.header;
            private set => SetProperty(ref this.header, value);
        }

        public string SubHeader
        {
            get => this.subHeader;
            private set => SetProperty(ref this.subHeader, value);
        }

        public string ArtworkUri
        {
            get => this.artworkUri;
            private set => SetProperty(ref this.artworkUri, value);
        }

        public IEnumerable<InformationEntry> Values
        {
            get => this.values;
            private set => SetProperty(ref this.values, value);
        }



        private void PopulateInformation()
        {
            if (this.albumViewModel != null)
            {
                PopulateAlbumInformation();
            }
            else if (this.playlistViewModel != null)
            {
                PopulatePlaylistInformation();
            }
            else
            {
                //TODO: What do we do here?
            }
        }


        private void PopulateAlbumInformation()
        {
            //Assert album aint null...

            this.Header = this.albumViewModel.Title;
            this.SubHeader = this.albumViewModel.ArtistName;

            var infos = new List<InformationEntry>();

            infos.Add(new InformationEntry(EInformationType.Image, "Album Art", this.albumViewModel.ArtworkUri));

            infos.Add(new InformationEntry(EInformationType.Textual, "Album Title", this.albumViewModel.Title));
            infos.Add(new InformationEntry(EInformationType.Textual, "Album Artist", this.albumViewModel.ArtistName));

            infos.Add(new InformationEntry(EInformationType.Textual, "Number of Tracks", this.albumViewModel.NumberOfTracks.ToString()));

            this.Values = infos;

            // Fetch additional items, so we can display more information!
            this.session.Albums.GetById(this.albumViewModel.ItemId, this.CancellationToken)
                               .ContinueWith(t =>
                               {
                                   if (t.IsFaulted)
                                       return; //TODO

                                   var album = t.Result;

                                   var updatedValues = new List<InformationEntry>(this.Values);

                                   if (album.Contributors.Any())
                                   {
                                       //TODO: Formatting
                                       updatedValues.Add(new InformationEntry(EInformationType.Textual, "Contributors", string.Join("\n", album.Contributors.Select(x => x.Name))));
                                   }


                                   if (album.Duration > 0)
                                   {
                                       //TODO: Formatting
                                       updatedValues.Add(new InformationEntry(EInformationType.Textual, "Duration", album.Duration.ToString())); 
                                   }


                                   if (album.Genre.Any())
                                   {
                                       updatedValues.Add(new InformationEntry(EInformationType.Textual, "Associated Genre", string.Join("\n", album.Genre.Select(x => x.Name))));
                                   }


                                   updatedValues.Add(new InformationEntry(EInformationType.Textual, "Explicit", album.HasExplicitLyrics ? "Yes" : "No"));

                                   updatedValues.Add(new InformationEntry(EInformationType.Textual, "Record Label", album.Label));

                                   
                                   if (album.ReleaseDate != null)
                                   {
                                       updatedValues.Add(new InformationEntry(EInformationType.Textual, "Release Date", album.ReleaseDate.Value.ToShortDateString()));
                                   }


                                   updatedValues.Add(new InformationEntry(EInformationType.Textual, "Share Link", album.ShareLink));

                                   updatedValues.Add(new InformationEntry(EInformationType.Textual, "UPC", album.UPC));


                                   this.Values = updatedValues;

                               }, this.CancellationToken, TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
            
        }

        private void PopulatePlaylistInformation()
        {
            //Assert playlist aint null...

            this.Header = this.playlistViewModel.Title;
            this.SubHeader = this.playlistViewModel.CreatorName;

            var infos = new List<InformationEntry>();

            infos.Add(new InformationEntry(EInformationType.Image, "Playlist Artwork", this.playlistViewModel.ArtworkUri));

            infos.Add(new InformationEntry(EInformationType.Textual, "Playlist Title", this.playlistViewModel.Title));
            infos.Add(new InformationEntry(EInformationType.Textual, "Creator", this.playlistViewModel.CreatorName));

            infos.Add(new InformationEntry(EInformationType.Textual, "Number of Tracks", this.playlistViewModel.NumberOfTracks.ToString()));


            this.Values = infos;
        }

       
    }
}
