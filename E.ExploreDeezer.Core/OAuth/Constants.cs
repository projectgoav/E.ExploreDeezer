using System;
using System.Collections.Generic;
using System.Text;

namespace E.ExploreDeezer.Core.OAuth
{
    public static class Constants
    {

        public const string APPID_QUERY_KEY = "app_id";
        public const string REDIRECTURI_QUERY_KEY = "redirect_uri";
        public const string PERMS_QUERY_KEY = "perms";
        public const string SECRET_QUERY_KEY = "secret";
        public const string OUTPUT_QUERY_KEY = "output";
        public const string CODE_QUERY_KEY = "code";

        public const string JSON_OUTPUT_QUERY_VALUE = "json";

        public const string ERROR_RESPONSE_QUERY_KEY = "error_reason";
        public const string CODE_RESPONSE_QUERY_KEY = "code";

        public const string ACCESSTOKEN_RESPONSE_KEY = "access_token";
        public const string EXPIRY_RESPONSE_KEY = "expires";
    }
}
