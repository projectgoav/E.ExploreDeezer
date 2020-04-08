using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using E.Deezer;
using E.Deezer.Api;

using E.ExploreDeezer.Core.ViewModels;
using E.ExploreDeezer.Core.Collections;

namespace E.ExploreDeezer.Core
{
    internal class InformationDataController : IDisposable
    {
        private readonly IDeezerSession session;
        private readonly FixedSizeObservableCollection<InformationEntry> values;


        private ulong itemId;
        private CancellationTokenSource cancellationTokenSource;


        public InformationDataController(IDeezerSession session)
        {
            this.session = session;
            this.values = new FixedSizeObservableCollection<InformationEntry>();

            this.itemId = 0;
        }


        public IObservableCollection<InformationEntry> InformationValues => this.values;



        public void FetchForAlbum(IAlbumViewModel album)
        {
            if (itemId == album.ItemId)
                return;

            this.itemId = album.ItemId;

            this.values.ClearContents();

            DisposeCancellationTokenSource();
            this.cancellationTokenSource = new CancellationTokenSource();

            this.session.Albums.GetById(this.itemId, this.cancellationTokenSource.Token)
                               .ContinueWith(t =>
                               {
                                   if (t.IsFaulted)
                                       return; //TODO

                                   PopulateCollectionForAlbum(t.Result);

                               }, this.cancellationTokenSource.Token, TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
        }

        public void FetchForPlaylist(IPlaylistViewModel playlist)
        {
            if (itemId == playlist.ItemId)
                return;

            this.itemId = playlist.ItemId;

            this.values.ClearContents();

            DisposeCancellationTokenSource();
            this.cancellationTokenSource = new CancellationTokenSource();

            this.session.Playlists.GetById(this.itemId, this.cancellationTokenSource.Token)
                                  .ContinueWith(t =>
                                  {
                                      if (t.IsFaulted)
                                          return; //TODO

                                      PopulateCollectionForPlaylist(t.Result);

                                  }, this.cancellationTokenSource.Token, TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
        }


        private void DisposeCancellationTokenSource()
        {
            if (this.cancellationTokenSource != null)
            {
                this.cancellationTokenSource.Cancel();
                this.cancellationTokenSource.Dispose();

                this.cancellationTokenSource = null;
            }
        }



        private void PopulateCollectionForAlbum(IAlbum album)
        {
            var infos = new List<InformationEntry>();

            infos.Add(new InformationEntry(EInformationType.Image, "Album Art", album.CoverArtwork.Large));

            infos.Add(new InformationEntry(EInformationType.Textual, "Album Title", album.Title));
            infos.Add(new InformationEntry(EInformationType.Textual, "Album Artist", album.ArtistName));

            infos.Add(new InformationEntry(EInformationType.Textual, "Number of Tracks", album.TrackCount.ToString()));

            //TODO : Number of discs??

            if (album.Contributors.Any())
            {
                //TODO: Formatting
                infos.Add(new InformationEntry(EInformationType.Textual, "Contributors", string.Join("\n", album.Contributors.Select(x => x.Name))));
            }


            if (album.Duration > 0)
            {
                //TODO: Formatting
                infos.Add(new InformationEntry(EInformationType.Textual, "Duration", album.Duration.ToString()));
            }


            if (album.Genre.Any())
            {
                infos.Add(new InformationEntry(EInformationType.Textual, "Associated Genre", string.Join("\n", album.Genre.Select(x => x.Name))));
            }


            infos.Add(new InformationEntry(EInformationType.Textual, "Explicit", album.HasExplicitLyrics ? "Yes" : "No"));

            infos.Add(new InformationEntry(EInformationType.Textual, "Record Label", album.Label));


            if (album.ReleaseDate != null)
            {
                infos.Add(new InformationEntry(EInformationType.Textual, "Release Date", album.ReleaseDate.Value.ToShortDateString()));
            }


            infos.Add(new InformationEntry(EInformationType.Textual, "Share Link", album.ShareLink));

            infos.Add(new InformationEntry(EInformationType.Textual, "UPC", album.UPC));


            // Update the collection! :)
            this.values.SetContents(infos);
        }


        private void PopulateCollectionForPlaylist(IPlaylist playlist)
        {
            var infos = new List<InformationEntry>();

            infos.Add(new InformationEntry(EInformationType.Image, "Artwork", playlist.Images.Large));

            infos.Add(new InformationEntry(EInformationType.Textual, "Playlist Title", playlist.Title));

            if (!string.IsNullOrEmpty(playlist.Description))
            {
                infos.Add(new InformationEntry(EInformationType.Textual, "Description", playlist.Description));
            }

            if (playlist.Creator != null)
            {
                infos.Add(new InformationEntry(EInformationType.Textual, "Creator", playlist.Creator.Username));

                if (playlist.Creator.ProfilePictures.HasPictureOfSize(PictureSize.Medium))
                {
                    infos.Add(new InformationEntry(EInformationType.Image, "Creator Profile Picture", playlist.Creator.ProfilePictures.Medium));
                }
            }


            infos.Add(new InformationEntry(EInformationType.Textual, "Duration", playlist.Duration.ToString())); //TODO: Formatting

            infos.Add(new InformationEntry(EInformationType.Textual, "Number of Tracks", playlist.NumberOfTracks.ToString()));

            infos.Add(new InformationEntry(EInformationType.Textual, "Share Link", playlist.ShareLink));


            this.values.SetContents(infos);
        }




        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.values.Clear();

                DisposeCancellationTokenSource();
            }
        }
    }
}
