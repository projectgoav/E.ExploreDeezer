using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using E.Deezer;

namespace E.ExploreDeezer.Core.OAuth
{
    public enum OAuthLoginState
    {
        Success,
        UserCancelled,
        UnknownError
    }

    public class OAuthLoginResult
    {
        public OAuthLoginResult(OAuthLoginState state,
                                OAuthResponse tokenResponse)
        {
            this.State = state;
            this.TokenResponse = tokenResponse;
        }

        public OAuthLoginState State { get; }
        public OAuthResponse TokenResponse { get; }
    }

    public interface IOAuthClient
    {
        string LoginUri { get; }

        bool CanParseRedirect(Uri uri);
        Task<OAuthLoginResult> TryParseRedirect(Uri uri);
    }


    internal class OAuthClient : IOAuthClient
    {
        private const string BASE_OAUTH_URI = "https://connect.deezer.com/oauth";

        private const string LOGIN_ENDPOINT = "auth.php";
        private const string TOKEN_ENDPOINT = "access_token.php";


        private readonly string appId;
        private readonly string appSecret;
        private readonly string redirectUri;

        public OAuthClient(string appId, 
                           string appSecret,
                           string redirectUri,
                           DeezerPermissions requestedPermissions)
        {
            this.appId = appId;
            this.appSecret = appSecret;
            this.redirectUri = redirectUri;

            this.LoginUri = GenerateLoginUri(requestedPermissions);

        }


        // IOAuthClient
        public string LoginUri { get; }

        public bool CanParseRedirect(Uri uri)
        {
            return uri.AbsoluteUri
                      .StartsWith(this.redirectUri);
        }

        public Task<OAuthLoginResult> TryParseRedirect(Uri uri)
        {
            var query = uri.Query.Trim(new char[] { '?', ' ' });

            if (query.Contains(Constants.ERROR_RESPONSE_QUERY_KEY))
                return Task.FromResult(new OAuthLoginResult(OAuthLoginState.UserCancelled, null));

            if (!query.Contains(Constants.CODE_RESPONSE_QUERY_KEY))
                return Task.FromResult(new OAuthLoginResult(OAuthLoginState.UnknownError, null));

            int equalsIndex = query.IndexOf('=');
            string code = query.Substring(equalsIndex + 1);

            string tokenUri = GenerateTokenUri(code);

            //TODO: Need to make the call to to actually get the token!
            
            return Task.FromResult<OAuthLoginResult>(new OAuthLoginResult(OAuthLoginState.UnknownError, null));
        }



        private string GenerateLoginUri(DeezerPermissions requestedPermissions)
        {
            StringBuilder sb = new StringBuilder(1024);

            sb.Append(BASE_OAUTH_URI);
            sb.Append('/');
            sb.Append(LOGIN_ENDPOINT);
            sb.Append('?');
            sb.Append(Constants.APPID_QUERY_KEY);
            sb.Append('=');
            sb.Append(Uri.EscapeUriString(this.appId));
            sb.Append('&');
            sb.Append(Constants.REDIRECTURI_QUERY_KEY);
            sb.Append('=');
            sb.Append(Uri.EscapeUriString(this.redirectUri));
            sb.Append('&');
            sb.Append(Constants.PERMS_QUERY_KEY);
            sb.Append('=');
            sb.Append(Uri.EscapeUriString(requestedPermissions.PermissionToString()));

            return sb.ToString();
        }

        private string GenerateTokenUri(string tokenCode)
        {
            StringBuilder sb = new StringBuilder(1024);

            sb.Append(BASE_OAUTH_URI);
            sb.Append('/');
            sb.Append(TOKEN_ENDPOINT);
            sb.Append('?');
            sb.Append(Constants.APPID_QUERY_KEY);
            sb.Append('=');
            sb.Append(Uri.EscapeDataString(this.appId));
            sb.Append('&');
            sb.Append(Constants.SECRET_QUERY_KEY);
            sb.Append('=');
            sb.Append(Uri.EscapeDataString(this.appSecret));
            sb.Append('&');
            sb.Append(Constants.CODE_QUERY_KEY);
            sb.Append('=');
            sb.Append(Uri.EscapeDataString(tokenCode));
            sb.Append('&');
            sb.Append(Constants.OUTPUT_QUERY_KEY);
            sb.Append('=');
            sb.Append(Constants.JSON_OUTPUT_QUERY_VALUE);

            return sb.ToString();
        }
    }
}
