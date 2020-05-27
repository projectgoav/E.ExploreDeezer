using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E.Deezer;
using E.ExploreDeezer.Core;

namespace E.ExploreDeezer.UWP
{
    
    internal class Secrets : ISecrets
    {
        public string AppId { get; } = string.Empty;

        public string AppSecret { get; } = string.Empty;

        public string OAuthRedirectUri { get; } = string.Empty;

        public DeezerPermissions DesiredPermissions { get; } = DeezerPermissions.BasicAccess        // Required
                                                                | DeezerPermissions.Email           // We can display the user's email address
                                                                | DeezerPermissions.ManageLibrary   // We can add favourites and add/edit playlists
                                                                | DeezerPermissions.DeleteLibrary   // We can remove favourites and delete playlists
                                                                | DeezerPermissions.OfflineAccess;  // HACK: We get a token that doesn't expire so we don't have to renew

    }
}
