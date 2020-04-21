using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

using E.Deezer;

using E.ExploreDeezer.Core.Util;
using E.ExploreDeezer.Core.ViewModels;
using E.ExploreDeezer.Core.Collections;

namespace E.ExploreDeezer.Core.Search
{
    internal interface ISearchDataController
    {
        EFetchState AlbumResultsFetchState { get; }
        IObservableCollection<IAlbumViewModel> AlbumResults { get; }
        event FetchStateChangedEventHandler OnAlbumResultsFetchStateChanged;

        EFetchState ArtistResultsFetchState { get; }
        IObservableCollection<IArtistViewModel> ArtistResults { get; }
        event FetchStateChangedEventHandler OnArtistResultsFetchStateChanged;

        EFetchState TrackResultsFetchState { get; }
        IObservableCollection<ITrackViewModel> TrackResults { get; }
        event FetchStateChangedEventHandler OnTrackResultsFetchStateChanged;

        EFetchState PlaylistResultFetchState { get; }
        IObservableCollection<IPlaylistViewModel> PlaylistResults { get; }
        event FetchStateChangedEventHandler OnPlaylistResultsFetchStateChanged;


        string CurrentQuery { get; }

        void SearchAsync(string query);
        void ClearQuery();

    }


    internal class SearchDataController : ISearchDataController,
                                          IDisposable
    {
        private readonly IDeezerSession session;

        private readonly UpdatableFetchState albumsFetchState;
        private readonly UpdatableFetchState tracksFetchState;
        private readonly UpdatableFetchState artistsFetchState;
        private readonly UpdatableFetchState playlistsFetchState;
        private readonly ResetableCancellationTokenSource tokenSource;
        private readonly PagedObservableCollection<IAlbumViewModel> albumResults;
        private readonly PagedObservableCollection<ITrackViewModel> trackResults;
        private readonly PagedObservableCollection<IArtistViewModel> artistResults;
        private readonly PagedObservableCollection<IPlaylistViewModel> playlistResults;


        public SearchDataController(IDeezerSession session)
        {
            this.session = session;

            this.tokenSource = new ResetableCancellationTokenSource();

            this.albumsFetchState = new UpdatableFetchState();
            this.tracksFetchState = new UpdatableFetchState();
            this.artistsFetchState = new UpdatableFetchState();
            this.playlistsFetchState = new UpdatableFetchState();

            this.albumResults = new PagedObservableCollection<IAlbumViewModel>();
            this.trackResults = new PagedObservableCollection<ITrackViewModel>();
            this.artistResults = new PagedObservableCollection<IArtistViewModel>();
            this.playlistResults = new PagedObservableCollection<IPlaylistViewModel>();
        }


        //ISearchDataController
        public EFetchState AlbumResultsFetchState => this.albumsFetchState.CurrentState;

        public IObservableCollection<IAlbumViewModel> AlbumResults => this.albumResults;

        public event FetchStateChangedEventHandler OnAlbumResultsFetchStateChanged
        {
            add => this.albumsFetchState.OnFetchStateChanged += value;
            remove => this.albumsFetchState.OnFetchStateChanged -= value;
        }


        public EFetchState ArtistResultsFetchState => this.artistsFetchState.CurrentState;

        public IObservableCollection<IArtistViewModel> ArtistResults => this.artistResults;

        public event FetchStateChangedEventHandler OnArtistResultsFetchStateChanged
        {
            add => this.artistsFetchState.OnFetchStateChanged += value;
            remove => this.artistsFetchState.OnFetchStateChanged -= value;
        }


        public EFetchState TrackResultsFetchState => this.tracksFetchState.CurrentState;

        public IObservableCollection<ITrackViewModel> TrackResults => this.trackResults;

        public event FetchStateChangedEventHandler OnTrackResultsFetchStateChanged
        {
            add => this.tracksFetchState.OnFetchStateChanged += value;
            remove => this.tracksFetchState.OnFetchStateChanged -= value;
        }


        public EFetchState PlaylistResultFetchState => this.playlistsFetchState.CurrentState;

        public IObservableCollection<IPlaylistViewModel> PlaylistResults => this.playlistResults;

        public event FetchStateChangedEventHandler OnPlaylistResultsFetchStateChanged
        {
            add => this.playlistsFetchState.OnFetchStateChanged += value;
            remove => this.playlistsFetchState.OnFetchStateChanged -= value;
        }


        public string CurrentQuery { get; private set; }

        public void SearchAsync(string newQuery)
        {
            if (this.CurrentQuery == newQuery)
                return;

            if (string.IsNullOrEmpty(newQuery))
            {
                ClearQuery();
                return;
            }

            BeginSearch(newQuery);
        }


        public void ClearQuery()
        {
            this.tokenSource.Reset();

            this.CurrentQuery = string.Empty;

            this.albumResults.Clear();
            this.trackResults.Clear();
            this.artistResults.Clear();
            this.playlistResults.Clear();
        }



        private void BeginSearch(string newQuery)
        {
            this.tokenSource.Reset();

            this.CurrentQuery = newQuery;

            this.albumsFetchState.SetLoading();
            this.tracksFetchState.SetLoading();
            this.artistsFetchState.SetLoading();
            this.playlistsFetchState.SetLoading();

            this.albumResults.SetFetcher((start, count, ct) => this.session.Search.FindAlbums(this.CurrentQuery, ct, (uint)start, (uint)count)
                                                                                  .ContinueWith<IEnumerable<IAlbumViewModel>>(t =>
                                                                                  {
                                                                                      if (t.IsFaulted)
                                                                                      {
                                                                                          this.albumsFetchState.SetError();

                                                                                          var ex = t.Exception.GetBaseException();

                                                                                          System.Diagnostics.Debug.WriteLine($"Album search failed. {ex}");
                                                                                          throw ex;
                                                                                      }

                                                                                      var items = t.Result.Select(x => new AlbumViewModel(x));

                                                                                      bool hasContents = this.albumResults.Count > 0 || items.Any();
                                                                                      if (hasContents)
                                                                                      {
                                                                                          this.albumsFetchState.SetAvailable();
                                                                                      }
                                                                                      else
                                                                                      {
                                                                                          this.albumsFetchState.SetEmpty();
                                                                                      }

                                                                                      return items;

                                                                                  }, ct, TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default));


            this.trackResults.SetFetcher((start, count, ct) => this.session.Search.FindTracks(this.CurrentQuery, ct, (uint)start, (uint)count)
                                                                      .ContinueWith<IEnumerable<ITrackViewModel>>(t =>
                                                                      {
                                                                          if (t.IsFaulted)
                                                                          {
                                                                              this.tracksFetchState.SetError();

                                                                              var ex = t.Exception.GetBaseException();

                                                                              System.Diagnostics.Debug.WriteLine($"Track search failed. {ex}");
                                                                              throw ex;
                                                                          }

                                                                          var items = t.Result.Select(x => new TrackViewModel(x, 
                                                                                                                              ETrackLHSMode.Artwork, 
                                                                                                                              ETrackArtistMode.NameWithLink));

                                                                          bool hasContents = this.trackResults.Count > 0 || items.Any();
                                                                          if (hasContents)
                                                                          {
                                                                              this.tracksFetchState.SetAvailable();
                                                                          }
                                                                          else
                                                                          {
                                                                              this.tracksFetchState.SetEmpty();
                                                                          }

                                                                          return items;

                                                                      }, ct, TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default));


            this.artistResults.SetFetcher((start, count, ct) => this.session.Search.FindArtists(this.CurrentQuery, ct, (uint)start, (uint)count)
                                                                      .ContinueWith<IEnumerable<IArtistViewModel>>(t =>
                                                                      {
                                                                          if (t.IsFaulted)
                                                                          {
                                                                              this.artistsFetchState.SetError();

                                                                              var ex = t.Exception.GetBaseException();

                                                                              System.Diagnostics.Debug.WriteLine($"Artist search failed. {ex}");
                                                                              throw ex;
                                                                          }

                                                                          var items = t.Result.Select(x => new ArtistViewModel(x));

                                                                          bool hasContents = this.artistResults.Count > 0 || items.Any();
                                                                          if (hasContents)
                                                                          {
                                                                              this.artistsFetchState.SetAvailable();
                                                                          }
                                                                          else
                                                                          {
                                                                              this.artistsFetchState.SetEmpty();
                                                                          }

                                                                          return items;

                                                                      }, ct, TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default));


            this.playlistResults.SetFetcher((start, count, ct) => this.session.Search.FindPlaylists(this.CurrentQuery, ct, (uint)start, (uint)count)
                                                          .ContinueWith<IEnumerable<IPlaylistViewModel>>(t =>
                                                          {
                                                              if (t.IsFaulted)
                                                              {
                                                                  this.playlistsFetchState.SetError();

                                                                  var ex = t.Exception.GetBaseException();

                                                                  System.Diagnostics.Debug.WriteLine($"Playlist search failed. {ex}");
                                                                  throw ex;
                                                              }

                                                              var items = t.Result.Select(x => new PlaylistViewModel(x));

                                                              bool hasContents = this.playlistResults.Count > 0 || items.Any();
                                                              if (hasContents)
                                                              {
                                                                  this.playlistsFetchState.SetAvailable();
                                                              }
                                                              else
                                                              {
                                                                  this.playlistsFetchState.SetEmpty();
                                                              }

                                                              return items;

                                                          }, ct, TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default));

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
                this.tokenSource.Dispose();

                this.albumsFetchState.Dispose();
                this.tracksFetchState.Dispose();
                this.artistsFetchState.Dispose();
                this.playlistsFetchState.Dispose();
            }
        }
    }
}
