using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Response
{
    public class AuthReponse
    {
        public string Id { get; set; }
        public string Token { get; set; }
        public string Email { get; set; }
        public string UserName {  get; set; }
        public string RefreshToken { get; set; }
        public DateTime Expiration { get; set; }
    }
    public class RegisterResponse
    {
        public string Message { get; set; }
        public string Status { get; set; }
    }
    public class TokenResponse
    {
        public string accessToken { get; set; }
        public string refreshToken { get; set; }
    }
}
