namespace Frontend.Helpers
{
    public static class TokenHelper
    {
        // Variabile statica per il token
        private static string? _accessToken;

        // Proprietà pubblica per ottenere e impostare il token
        public static string? AccessToken
        {
            get => _accessToken;
            set => _accessToken = value;
        }

        // Metodo per settare il token
        public static void SetToken(string token)
        {
            _accessToken = token;
        }

        // Metodo per ottenere il token
        public static string? GetToken()
        {
            return _accessToken;
        }

        // Metodo per rimuovere il token
        public static void RemoveToken()
        {
            _accessToken = null;
        }
    }
}
