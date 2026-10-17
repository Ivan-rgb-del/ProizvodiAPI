using Microsoft.AspNetCore.Mvc;
using ProizvodiAPI.Application.Interfaces;
using static ProizvodiAPI.Application.DTOs.AuthDTO;

namespace ProizvodiAPI.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthServices _authServices;

        public AuthController(IAuthServices authServices)
        {
            _authServices = authServices;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDTO dto)
        {
            var token = _authServices.Login(dto);

            if (token == null)
                return Unauthorized("Neispravno korisničko ime ili lozinka!");

            return Ok(token);
        }
    }
}
