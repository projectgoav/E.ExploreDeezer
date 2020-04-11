using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

using E.Deezer;

using E.ExploreDeezer.Core.ViewModels;
using E.ExploreDeezer.Core.Collections;

namespace E.ExploreDeezer.Core
{
    /*
    internal class ChartsDataController
    {
        private readonly IDeezerSession session;
        private readonly PagedObservableCollection<IAlbumViewModel> albums;
        private readonly PagedObservableCollection<IArtistViewModel> artists;
        private readonly PagedObservableCollection<ITrackViewModel> tracks;
        private readonly PagedObservableCollection<IPlaylistViewModel> playlists;

        private ulong genreId;


        public ChartsDataController(IDeezerSession session)
        {
            this.session = session;

            this.genreId = ulong.MaxValue;

            this.albums = new PagedObservableCollection<IAlbumViewModel>(pageSize: 100, nextPageThreashold: 100);
            this.artists = new PagedObservableCollection<IArtistViewModel>(pageSize: 100, nextPageThreashold: 100);
            this.tracks = new PagedObservableCollection<ITrackViewModel>(pageSize: 100, nextPageThreashold: 100);
            this.playlists = new PagedObservableCollection<IPlaylistViewModel>(pageSize: 100, nextPageThreashold: 100);
        }


        public IObservableCollection<IAlbumViewModel> Albums => this.albums;
        public IObservableCollection<IArtistViewModel> Artists => this.artists;
        public IObservableCollection<ITrackViewModel> Tracks => this.tracks;
        public IObservableCollection<IPlaylistViewModel> Playlists => this.playlists;


        public void SetGenreId(ulong genreId)
        {
            if (this.genreId == genreId)
                return;

            this.genreId = genreId;

            SetItemFetchers();
        }


        private void SetItemFetchers()
        {
            //TODO: It might be nice to take advantage of the fact we can fetch all charts at once
            //      then parse out the individual parts to the collections?

            var albumFetcher = new ItemFetcher<IAlbumViewModel>((start, count, token) =>
            {
                return this.session.Charts.GetAlbumChartForGenre(this.genreId, token, (uint)start, (uint)count)
                                          .ContinueWith<IEnumerable<IAlbumViewModel>>(t =>
                                          {
                                              if (t.IsFaulted)
                                                  throw t.Exception.GetBaseException();

                                              return t.Result.Select(x => new AlbumViewModel(x));

                                          }, token, TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
            });

            this.albums.SetFetcher(albumFetcher);


            var artistFetcher = new ItemFetcher<IArtistViewModel>((start, count, token) =>
            {
                return this.session.Charts.GetArtistChartForGenre(this.genreId, token, (uint)start, (uint)count)
                                          .ContinueWith<IEnumerable<IArtistViewModel>>(t =>
                                          {
                                              if (t.IsFaulted)
                                                  throw t.Exception.GetBaseException();

                                              return t.Result.Select(x => new ArtistViewModel(x));

                                          }, token, TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
            });

            this.artists.SetFetcher(artistFetcher);



            var trackFetcher = new ItemFetcher<ITrackViewModel>((start, count, token) =>
            {
                return this.session.Charts.GetTrackChartForGenre(this.genreId, token, (uint)start, (uint)count)
                                          .ContinueWith<IEnumerable<ITrackViewModel>>(t =>
                                          {
                                              if (t.IsFaulted)
                                                  throw t.Exception.GetBaseException();

                                              return t.Result.Select(x => new TrackViewModel(x));

                                          }, token, TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
            });

            this.tracks.SetFetcher(trackFetcher);



            var playlistFetcher = new ItemFetcher<IPlaylistViewModel>((start, count, token) =>
            {
                return this.session.Charts.GetPlaylistChartForGenre(this.genreId, token, (uint)start, (uint)count)
                                          .ContinueWith<IEnumerable<IPlaylistViewModel>>(t =>
                                          {
                                              if (t.IsFaulted)
                                                  throw t.Exception.GetBaseException();

                                              return t.Result.Select(x => new PlaylistViewModel(x));

                                          }, token, TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
            });

            this.playlists.SetFetcher(playlistFetcher);
        }
    }
    */
}
