using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json.Linq;

namespace E.ExploreDeezer.Core.OAuth
{
    public class OAuthResponse
    {
        private OAuthResponse(string accessToken, int expiry)
        {
            this.AccessToken = accessToken;
            this.Expires = expiry;
        }


        public string AccessToken { get; }
        public int Expires { get; } //In seconds


        public static OAuthResponse FromJson(JToken json)
        {
            JObject castJson = json as JObject;

            if (castJson == null)
                return null;

            string accessToken = castJson.Value<string>(Constants.ACCESSTOKEN_RESPONSE_KEY);
            int expiry = castJson.Value<int>(Constants.EXPIRY_RESPONSE_KEY);

            return new OAuthResponse(accessToken, expiry);
        }
    }
}
