namespace Authentication.DTO.Responses
{
    /// <summary>
    /// Authentication response contract.
    /// The JWT token produced by Authentication.Handler is returned here.
    /// </summary>
    public class AuthResponse
    {
        public string Token { get; set; } = string.Empty;
        public string TokenType { get; set; } = "Bearer";
        public int ExpiresInMinutes { get; set; }
    }
}
