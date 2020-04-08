using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

using System.ComponentModel;

using E.Deezer;

using E.ExploreDeezer.Core.Mvvm;
using E.ExploreDeezer.Core.Collections;

namespace E.ExploreDeezer.Core.ViewModels
{
    public interface IInformationViewModel : INotifyPropertyChanged,
                                             IDisposable
    {
        string Header { get; }
        string SubHeader { get; }
        string ArtworkUri { get; }

        IObservableCollection<InformationEntry> Values { get; }
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
        private readonly IAlbumViewModel albumViewModel;
        private readonly IPlaylistViewModel playlistViewModel;
        private readonly MainThreadObservableCollectionAdapter<InformationEntry> informationValues;

        private string header;
        private string subHeader;
        private string artworkUri;


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

            var dataController = ServiceRegistry.GetService<InformationDataController>();

            this.informationValues = new MainThreadObservableCollectionAdapter<InformationEntry>(dataController.InformationValues,
                                                                                                 this.PlatformServices.MainThreadDispatcher);

            if (this.albumViewModel != null)
            {
                this.Header = this.albumViewModel.Title;
                this.SubHeader = this.albumViewModel.ArtistName;

                dataController.FetchForAlbum(this.albumViewModel);
            }
            else if (this.playlistViewModel != null)
            {
                this.Header = this.playlistViewModel.Title;
                this.SubHeader = this.playlistViewModel.CreatorName;

                dataController.FetchForPlaylist(this.playlistViewModel);
            }
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

        public IObservableCollection<InformationEntry> Values => this.informationValues;



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.informationValues.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
