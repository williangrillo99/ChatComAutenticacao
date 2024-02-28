using Application.DTOs.Request;
using Application.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface
{
    public interface IAuthServiceApp
    {
        Task<AuthReponse> Logar(LoginRequest loginRequest);
        Task<RegisterResponse> Register(RegisterRequest registerRequest);
        void Revoke(string username);
        Task<TokenResponse> RefreshToken(TokenRequest request);
        Task<RegisterResponse> CreateRole(string roleName);
        Task<RegisterResponse> AddUserToRole(string email, string roleName);
    }
}
