namespace Frontend.Helpers
{
    public static class TokenHelper
    {
        private static string? _accessToken;

        public static string? AccessToken
        {
            get => _accessToken;
            set => _accessToken = value;
        }

        public static void SetToken(string token)
        {
            _accessToken = token;
        }

        public static string? GetToken()
        {
            return _accessToken;
        }

        public static void RemoveToken()
        {
            _accessToken = null;
        }
    }
}
