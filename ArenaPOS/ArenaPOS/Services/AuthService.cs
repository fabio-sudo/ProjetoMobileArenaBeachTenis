using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ArenaPOS.Interfaces;
using Microsoft.Maui.Storage;

namespace ArenaPOS.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> LoginAsync(string email, string password)
        {
            // For Demo/Mock purposes until API is connected:
            if (email == "admin" && password == "admin123")
            {
                var fakeToken = "eyJhbGciOiJIUzI1NiIsInR5cCI...fake_token_data";
                await SecureStorage.SetAsync("auth_token", fakeToken);
                return fakeToken;
            }

            try
            {
                var loginData = new { Email = email, Password = password };
                var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginData);
                
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                    if (result != null && !string.IsNullOrEmpty(result.Token))
                    {
                        await SecureStorage.SetAsync("auth_token", result.Token);
                        return result.Token;
                    }
                }
                
                return null;
            }
            catch
            {
                return null;
            }
        }

        public void Logout()
        {
            SecureStorage.Remove("auth_token");
            // App navigation logic usually handles returning to the login screen
        }
        
        private class LoginResponse 
        {
            public string Token { get; set; }
        }
    }
}
