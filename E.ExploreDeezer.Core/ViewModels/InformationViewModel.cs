using System;
using System.Collections.Generic;
using System.Text;

using System.ComponentModel;

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
        private readonly IAlbumViewModel albumViewModel;
        private readonly IPlaylistViewModel playlistViewModel;


        private string header;
        private string subHeader;
        private string artworkUri;
        private IEnumerable<InformationEntry> values;



        public InformationViewModel(IAlbumViewModel album,
                                    IPlatformServices platformServices)
            : this(album, null, platformServices)
        { }

        public InformationViewModel(IPlaylistViewModel playlist,
                                    IPlatformServices platformServices)
            : this(null, playlist, platformServices)
        { }


        public InformationViewModel(IAlbumViewModel album,
                                    IPlaylistViewModel playlist, 
                                    IPlatformServices platformServices)
            : base(platformServices)
        {
            this.albumViewModel = album;
            this.playlistViewModel = playlist;

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

            infos.Add(new InformationEntry(EInformationType.Textual, "Album Title", this.albumViewModel.Title));
            infos.Add(new InformationEntry(EInformationType.Textual, "Album Artist", this.albumViewModel.ArtistName));

            infos.Add(new InformationEntry(EInformationType.Textual, "Number of Tracks", this.albumViewModel.NumberOfTracks.ToString()));


            this.Values = infos;
        }

        private void PopulatePlaylistInformation()
        {
            //Assert playlist aint null...

            this.Header = this.playlistViewModel.Title;
            this.SubHeader = this.playlistViewModel.CreatorName;

            var infos = new List<InformationEntry>();

            infos.Add(new InformationEntry(EInformationType.Textual, "Playlist Title", this.playlistViewModel.Title));
            infos.Add(new InformationEntry(EInformationType.Textual, "Creator", this.playlistViewModel.CreatorName));

            infos.Add(new InformationEntry(EInformationType.Textual, "Number of Tracks", this.playlistViewModel.NumberOfTracks.ToString()));


            this.Values = infos;
        }

       
    }
}
