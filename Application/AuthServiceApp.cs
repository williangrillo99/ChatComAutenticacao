using Application.DTOs.Request;
using Application.DTOs.Response;
using Application.Interface;
using Domain.Identity.Entity;
using Domain.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;


namespace Application
{
    public class AuthServiceApp : IAuthServiceApp
    {
        private readonly IConfiguration _config;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ITokenService _tokenService;

        public AuthServiceApp(ITokenService tokenService, RoleManager<IdentityRole> roleManager, UserManager<User> userManager, IConfiguration config)
        {
            _tokenService = tokenService;
            _roleManager = roleManager;
            _userManager = userManager;
            _config = config;
        }
        public async Task<AuthReponse> Logar(LoginRequest loginRequest)
        {
            var user = await _userManager.FindByNameAsync(loginRequest.UserName!);

            if (user is null)
            {
                throw new Exception("User name invalido");

            }
            if (user is not null && await _userManager.CheckPasswordAsync(user, loginRequest.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, user.UserName!),
                    new Claim(ClaimTypes.Email, user.Email!),
                    new Claim("id", user.UserName!),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),

                };

                foreach (var userRole in userRoles) {

                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }
                var token = _tokenService.GenerateAcessToken(authClaims, _config);

                var refreshToken = _tokenService.GenerateRefreshToken();

                _ = int.TryParse(_config["JWT:RefreshTokenValidityInMinutes"], out int refreshTokenValidityInMinutes);

                user.RefreshToken = refreshToken;

                user.RefreshTokenExpiryTime = DateTime.Now.AddMinutes(refreshTokenValidityInMinutes);

                await _userManager.UpdateAsync(user);

                return new AuthReponse
                {
                    Id = user.Id,
                    Email = user.Email,
                    UserName = user.UserName,
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    RefreshToken = refreshToken,
                    Expiration = token.ValidTo

                };
            }
            throw new Exception("Senha Incorreta");
        }
        public async Task<RegisterResponse> Register(RegisterRequest registerRequest)
        {
            var userExists = await _userManager.FindByNameAsync(registerRequest.UserName);
            if (userExists != null)
            {
                throw new Exception("Usuario existente");

            }
            User user = new()
            {
                Email = registerRequest.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = registerRequest.UserName,
            };

            var result = await _userManager.CreateAsync(user, registerRequest.Password);

            if (!result.Succeeded) {

                throw new ArgumentNullException("Error ao criar o usuario");

            }
            var response = new RegisterResponse() { Message = "Usuario Criado", Status = "Sucesso" };

            return response;
        }
        public async Task<TokenResponse> RefreshToken(TokenRequest request)
        {
            if (request is null)
            {
                throw new ArgumentNullException("Error ao criar o usuario");
            }

            string? accessToken = request.AcessToken ?? throw new ArgumentNullException(nameof(request));

            string? refreshToken = request.RefreshToken ?? throw new ArgumentNullException(nameof(request));

            //EXTRAIR AS CLIAMS
            var principal = _tokenService.GetClaimsPrincipalFromExpiredToken(accessToken!, _config);

            if (principal == null)
            {
                throw new ArgumentNullException("Invalid acess token/refresh token");
            }

            string username = principal.Identity.Name;

            var user = await _userManager.FindByNameAsync(username);

            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                throw new ArgumentNullException("Invalid acess token/refresh token");
            }

            var newAcessToken = _tokenService.GenerateAcessToken(principal.Claims.ToList(), _config);

            var newRefreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;

            await _userManager.UpdateAsync(user);

            return new TokenResponse() { accessToken = new JwtSecurityTokenHandler().WriteToken(newAcessToken), refreshToken = refreshToken };
        }
        public async void Revoke(string username) 
        { 
            var user = await _userManager.FindByNameAsync(username ?? throw new Exception("Invalid user name"));

            user.RefreshToken = null;

            await _userManager.UpdateAsync(user);
            
        }
        public async Task<RegisterResponse> CreateRole(string roleName)
        {
            var roleExist = await _roleManager.RoleExistsAsync(roleName);

            if(!roleExist)
            {
                var roleResult = await _roleManager.CreateAsync(new IdentityRole(roleName));

                if (roleResult.Succeeded)
                {
                    return new RegisterResponse() { Message = "Roles Added", Status = $"Role {roleName} added sucessfuly"};
                }
                else
                {
                    throw new Exception($"Error, Issue adding the new  {roleName} role\"");
                }
            }
            throw new Exception($"Role already exist,  {roleName} role\"");
        }
        public async Task<RegisterResponse> AddUserToRole(string email, string roleName)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if(user != null) 
            { 
                var result = await _userManager.AddToRoleAsync(user, roleName);

                if(result.Succeeded)
                {
                    return new RegisterResponse() { Message = $"User {user.Email} added to the {roleName} role", Status = "Sucess" };
                }
                else
                {
                    throw new Exception("Erro ao criar a role");
                }
            }
            
            throw new Exception("Erro ao criar a role");

        }
    }
}
