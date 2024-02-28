using Application.DTOs.Request;
using Application.DTOs.Response;
using Application.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly IAuthServiceApp _authServiceApp;

        public AuthController(IAuthServiceApp authServiceApp)
        {
            _authServiceApp = authServiceApp;
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<AuthReponse>> Login([FromBody] LoginRequest request)
        {
            return Ok(await _authServiceApp.Logar(request));
        }
        [HttpPost]
        [Route("register")]
        public async Task<ActionResult<RegisterResponse>> Register([FromBody] RegisterRequest request)
        {
            return Ok(await _authServiceApp.Register(request));

        }
        [HttpPost]
        [Authorize]
        [Route("roveke/{username}")]
        public ActionResult Revoke(string username)
        {
            _authServiceApp.Revoke(username);
            return NoContent();
        }

        [HttpPost]
        [Route("refresh-token")]
        public async Task<ActionResult<TokenResponse>> RefreshToken(TokenRequest tokenRequest)
        {
            return Ok(await _authServiceApp.RefreshToken(tokenRequest));
        }

        [HttpGet]
        [Route("User")]
        [Authorize(Policy = "UserOnly")]
        public string GetUser()
        {
            return "Ok";
        }
        [HttpGet]
        [Route("Admin")]
        [Authorize(Policy = "AdminOnly")]
        public string GetAdmin()
        {
            return "Ok";
        }

        [HttpPost]
        [Route("CreateRole")]

        public async Task<ActionResult<RegisterResponse>> CreateRole (string roleName)
        {
            return Ok(await _authServiceApp.CreateRole(roleName));
        }

        [HttpPost]
        [Route("AddUserToRole")]
        public async Task<IActionResult> AddUserToRole(string email, string roleName)
        {
            return Ok(await _authServiceApp.AddUserToRole(email, roleName));
        }
    }
}
