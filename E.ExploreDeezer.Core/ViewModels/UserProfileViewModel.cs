using System;
using System.Collections.Generic;
using System.Text;

using E.Deezer.Api;

namespace E.ExploreDeezer.Core.ViewModels
{
    public interface IUserProfileViewModel
    {
        ulong Id { get; }
        string Username { get; }
        string ProfilePicture { get; }
    }


    internal class UserProfileViewModel : IUserProfileViewModel
    {
        public UserProfileViewModel(IUserProfile profile)
        {
            this.Id = profile?.Id ?? 0u;
            this.Username = profile?.Username ?? string.Empty;
            this.ProfilePicture = profile?.ProfilePictures?.Medium; //TODO: Fallback image...
        }

        // IUserProfile
        public ulong Id { get; }
        public string Username { get; }
        public string ProfilePicture { get; }
    }
}
