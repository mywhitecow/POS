using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using POS.Client.Helpers;
using System.Security.Claims;
using System.Text.Json;

namespace POS.Client.Providers
{
    public class CustomAuthProvider(IJSRuntime js, HttpClient httpClient) : AuthenticationStateProvider
    {
        private readonly IJSRuntime _js = js;
        private readonly HttpClient _httpClient = httpClient;
        public static readonly string TOKENKEY = "TOKENKEY";
        private static AuthenticationState Anonimo => new(new ClaimsPrincipal(new ClaimsIdentity(TOKENKEY)));
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _js.GetFromLocalStorage(TOKENKEY);
            if (string.IsNullOrEmpty(token))
            {
                return Anonimo;
            }

            return BuildAuthenticationState(token);
        }
        // Metodo para iniciar sesion y almacenar el token en el localStorage
        public AuthenticationState BuildAuthenticationState(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", token);
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJWT(token))));
        }
        // Metodo para decodificar el JWT y extraer los claims
        private static List<Claim>? ParseClaimsFromJWT(string jwt)
        {
            var claims = new List<Claim>();
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
            claims.AddRange(keyValuePairs!.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()!)));
            if (keyValuePairs!.TryGetValue("role", out var roleValue))
            {
                claims.Add(new Claim(ClaimTypes.Role, roleValue.ToString()!));
            }
            return claims;
        }
        // Metodo para decodificar la cadena Base64 sin relleno
        public static byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
        public async Task Login(string token)
        {
            await _js.SetInLocalStorage(TOKENKEY, token);
            var authState = BuildAuthenticationState(token);
            NotifyAuthenticationStateChanged(Task.FromResult(authState));
        }
        public async Task Logout()
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
            await _js.RemoveItem(TOKENKEY);
            NotifyAuthenticationStateChanged(Task.FromResult(Anonimo));
        }
    }
}
