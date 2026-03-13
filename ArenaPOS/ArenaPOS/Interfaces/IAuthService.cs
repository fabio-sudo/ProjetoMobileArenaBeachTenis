using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ArenaPOS.Interfaces
{
    public interface IAuthService
    {
        Task<string> LoginAsync(string email, string password);
        void Logout();
    }
}
