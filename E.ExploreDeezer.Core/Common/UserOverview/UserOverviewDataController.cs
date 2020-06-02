using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using E.Deezer;
using E.Deezer.Api;

using E.ExploreDeezer.Core.Util;
using E.ExploreDeezer.Core.ViewModels;
using E.ExploreDeezer.Core.Extensions;
using E.ExploreDeezer.Core.Collections;
using Microsoft.Win32.SafeHandles;

namespace E.ExploreDeezer.Core.Common
{
    internal interface IUserOverviewDataController
    {
        IUserProfileViewModel CompleteProfile { get; }
        event FetchStateChangedEventHandler OnCompleteProfileFetchStateChanged;

        IObservableCollection<ITrackViewModel> Flow { get; }
        event FetchStateChangedEventHandler OnFlowFetchStateChanged;

        IObservableCollection<IPlaylistViewModel> Playlists { get; }
        event FetchStateChangedEventHandler OnPlaylistFetchStateChanged;

        ulong CurrentUserId { get; }

        void FetchUserProfileAsync(ulong userId);
    }

    internal class UserOverviewDataController : IUserOverviewDataController,
                                                IDisposable
    {
        private readonly IDeezerSession session;
        private readonly UpdatableFetchState flowFetchState;
        private readonly UpdatableFetchState playlistsFetchState;
        private readonly UpdatableFetchState completeProfileFetchState;
        private readonly ResetableCancellationTokenSource tokenSource;
        private readonly PagedObservableCollection<ITrackViewModel> flow;
        private readonly FixedSizeObservableCollection<IPlaylistViewModel> playlists;


        public UserOverviewDataController(IDeezerSession session)
        {
            this.session = session;

            this.CurrentUserId = 0;
            this.tokenSource = new ResetableCancellationTokenSource();

            this.flowFetchState = new UpdatableFetchState();
            this.playlistsFetchState = new UpdatableFetchState();
            this.completeProfileFetchState = new UpdatableFetchState();

            this.flow = new PagedObservableCollection<ITrackViewModel>();
            this.playlists = new FixedSizeObservableCollection<IPlaylistViewModel>();
        }



        
        // IUserOverviewDataController
        public ulong CurrentUserId { get; private set; }


        public IUserProfileViewModel CompleteProfile { get; private set; }
        
        public event FetchStateChangedEventHandler OnCompleteProfileFetchStateChanged
        {
            add => this.completeProfileFetchState.OnFetchStateChanged += value;
            remove => this.completeProfileFetchState.OnFetchStateChanged -= value;
        }


        public IObservableCollection<ITrackViewModel> Flow => this.flow;

        public event FetchStateChangedEventHandler OnFlowFetchStateChanged
        {
            add => this.flowFetchState.OnFetchStateChanged += value;
            remove => this.flowFetchState.OnFetchStateChanged -= value;
        }


        public IObservableCollection<IPlaylistViewModel> Playlists => this.playlists;

        public event FetchStateChangedEventHandler OnPlaylistFetchStateChanged
        {
            add => this.playlistsFetchState.OnFetchStateChanged += value;
            remove => this.playlistsFetchState.OnFetchStateChanged -= value;
        }


        public void FetchUserProfileAsync(ulong userId)
        {
            if (this.CurrentUserId == userId)
                return;

            this.tokenSource.Reset();

            this.CurrentUserId = userId;

            this.CompleteProfile = null;

            this.flowFetchState.SetLoading();
            this.playlistsFetchState.SetLoading();
            this.completeProfileFetchState.SetLoading();

            this.session.User.GetById(this.CurrentUserId, this.tokenSource.Token)
                             .ContinueWhenNotCancelled(t =>
                             {
                                 (bool faulted, Exception ex) = t.CheckIfFailed();
                                 if (faulted)
                                 {
                                     System.Diagnostics.Debug.WriteLine($"Failed to fetch complete user profile. {ex}");
                                     this.completeProfileFetchState.SetError();
                                     return;
                                 }

                                 this.CompleteProfile = new UserProfileViewModel(t.Result);
                                 this.completeProfileFetchState.SetAvailable();
                             }, this.tokenSource.Token);


            this.flow.SetFetcher((start, count, ct) => this.session.User.GetFlow(this.CurrentUserId, ct, (uint)start, (uint)count)
                                                                        .ContinueWhenNotCancelled<IEnumerable<ITrack>, IEnumerable<ITrackViewModel>>(t =>
                                                                        {
                                                                            (bool faulted, Exception ex) = t.CheckIfFailed();
                                                                            if (faulted)
                                                                            {
                                                                                System.Diagnostics.Debug.WriteLine($"Failed to fetch user flow. {ex}");
                                                                                this.flowFetchState.SetError();
                                                                                return null;
                                                                            }

                                                                            var items = t.Result.Select(x => new TrackViewModel(x, 
                                                                                                                                ETrackLHSMode.Artwork, 
                                                                                                                                ETrackArtistMode.NameWithLink));

                                                                            bool hasContents = this.flow.Count > 0 || items.Any();
                                                                            if (hasContents)
                                                                            {
                                                                                this.flowFetchState.SetAvailable();
                                                                            }
                                                                            else
                                                                            {
                                                                                this.flowFetchState.SetEmpty();
                                                                            }

                                                                            return items;

                                                                        }, ct));


            FetchPlaylists(this.CurrentUserId, this.tokenSource.Token);                                                                        
        }



        private void FetchPlaylists(ulong userId, CancellationToken token)
        {
            /* Playlists for a user both favourites and personal playlists in a combined list.
             * This function aims to download the entire list of playlists and populate the
             * collection with only those whom the specified userId has created.
             * 
             * We might need to extend this to include contributors? */

            Task.Factory.StartNew(() =>
            {
                const uint CHUNK_SIZE = 100;

                bool keepFetching = true;
                bool fetchingFailed = false;
                var userPlaylists = new List<IPlaylistViewModel>();

                uint currentOffset = 0;

                while(keepFetching)
                {
                    var fetchedPlaylists = this.session.User.GetFavouritePlaylists(userId, token, currentOffset, CHUNK_SIZE)
                                                            .ContinueWhenNotCancelled<IEnumerable<IPlaylist>, IEnumerable<IPlaylistViewModel>>(t =>
                                                            {
                                                                (bool faulted, Exception ex) = t.CheckIfFailed();
                                                                if (faulted)
                                                                {
                                                                    System.Diagnostics.Debug.WriteLine($"Failed to fetch user playlists. {ex}");

                                                                    fetchingFailed = true;
                                                                    keepFetching = false;
                                                                    return Array.Empty<IPlaylistViewModel>();
                                                                }


                                                                return t.Result.Where(x => x != null
                                                                                            && x.Creator != null
                                                                                            && x.Creator.Id == userId
                                                                                            && !x.IsLovedTrack)
                                                                               .Select(x => new PlaylistViewModel(x));
                                                                              
                                                            }, token)
                                                            .Result; //Blocks a threadpool thread, so construct the parent task as 'LongRunning' to prevent this from causing issues

                    if (fetchedPlaylists.Any())
                    {
                        userPlaylists.AddRange(fetchedPlaylists);
                        currentOffset += CHUNK_SIZE;
                    }
                    else
                    {
                        break;
                    }
                }


                if (fetchingFailed)
                {
                    this.playlistsFetchState.SetError();
                }
                else if (userPlaylists.Count == 0)
                {
                    this.playlists.SetContents(Array.Empty<IPlaylistViewModel>());
                    this.playlistsFetchState.SetEmpty();
                }
                else
                {
                    this.playlists.SetContents(userPlaylists);
                    this.playlistsFetchState.SetAvailable();
                }

            },token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
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

                this.flow.Dispose();
                this.playlists.Dispose();

                this.flowFetchState.Dispose();
                this.playlistsFetchState.Dispose();
                this.completeProfileFetchState.Dispose();
            }
        }
    }
}
